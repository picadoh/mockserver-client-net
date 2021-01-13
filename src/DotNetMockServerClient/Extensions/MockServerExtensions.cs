// -----------------------------------------------------------------------
// <copyright file="MockServerExtensions.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.Extensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;
    using DotNetMockServerClient.Serializer;

    /// <summary>
    /// The mock server extensions class.
    /// </summary>
    public static class MockServerExtensions
    {
        /// <summary>
        /// Load expectations from the file.
        /// </summary>
        /// <param name="mockServerClient">The mock server client.</param>
        /// <param name="expectationsFilePath">The expectations file path.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static Task LoadExpectationsFromFileAsync(this MockServerClient mockServerClient, string expectationsFilePath)
        {
            if (!File.Exists(expectationsFilePath))
            {
                throw new FileNotFoundException($"File: {expectationsFilePath} not found!");
            }

            if (mockServerClient == null)
            {
                throw new ArgumentNullException(nameof(mockServerClient));
            }

            return LoadExpectationsFromFileInternalAsync(mockServerClient, expectationsFilePath);
        }

        /// <summary>Loads the expectations from file.</summary>
        /// <param name="mockServerClient">The mock server client.</param>
        /// <param name="expectationsFilePath">The expectations file path.</param>
        public static void LoadExpectationsFromFile(this MockServerClient mockServerClient, string expectationsFilePath)
        {
            if (!File.Exists(expectationsFilePath))
            {
                throw new FileNotFoundException($"File: {expectationsFilePath} not found!");
            }

            if (mockServerClient == null)
            {
                throw new ArgumentNullException(nameof(mockServerClient));
            }

            var fileContent = File.ReadAllText(expectationsFilePath).Replace("\r", string.Empty, StringComparison.InvariantCulture).Replace("\n", string.Empty, StringComparison.InvariantCulture);
            var serializer = new JsonSerializer<Expectation>();
            var expectations = serializer.DeserializeList(fileContent);

            if (expectations.Any())
            {
                foreach (var expectation in expectations)
                {
                    var httpRequest = HttpRequest.Request();
                    var httpResponse = HttpResponse.Response();

                    SetupMockServerRequest(mockServerClient, expectation, httpRequest, httpResponse);
                }
            }
        }

        private static async Task LoadExpectationsFromFileInternalAsync(MockServerClient mockServerClient, string expectationsFilePath)
        {
            using (var source = new FileStream(expectationsFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the source file into a byte array.
                byte[] bytes = new byte[source.Length];
                int numBytesToRead = (int)source.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = await source.ReadAsync(bytes.AsMemory(numBytesRead, numBytesToRead), cancellationToken: new CancellationToken(false)).ConfigureAwait(false);

                    // Break when the end of the file is reached.
                    if (n == 0)
                    {
                        break;
                    }

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                source.Position = 0;
                var serializer = new JsonSerializer<Expectation>();
                var expectations = await serializer.DeserializeListAsync(source).ConfigureAwait(false);

                if (expectations.Any())
                {
                    foreach (var expectation in expectations)
                    {
                        var httpRequest = HttpRequest.Request();
                        var httpResponse = HttpResponse.Response();

                        SetupMockServerRequest(mockServerClient, expectation, httpRequest, httpResponse);
                    }
                }
            }
        }

        private static TimeSpan GetTimeSpanDelay(Delay delay)
        {
            if (delay == null)
            {
                return TimeSpan.FromSeconds(0);
            }

            switch (delay.TimeUnit)
            {
                case "MILLISECONDS":
                    return TimeSpan.FromMilliseconds(delay.Value);

                case "SECONDS":
                    return TimeSpan.FromSeconds(delay.Value);

                case "MINUTES":
                    return TimeSpan.FromMinutes(delay.Value);

                case "HOURS":
                    return TimeSpan.FromHours(delay.Value);

                case "DAYS":
                    return TimeSpan.FromDays(delay.Value);

                case "TICKS":
                    return TimeSpan.FromTicks(delay.Value);

                default:
                    return TimeSpan.FromSeconds(0);
            }
        }

        private static void SetupMockServerRequest(MockServerClient mockServerClient, Expectation expectation, HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var isSecure = expectation.HttpRequest.IsSecure.HasValue && expectation.HttpRequest.IsSecure.Value;
            var unlimitedTimes = expectation.Times == null || expectation.Times.IsUnlimited;

            mockServerClient
                .When(
                    httpRequest
                        .WithMethod(expectation.HttpRequest.Method)
                        .WithPath(expectation.HttpRequest.Path)
                        .WithQueryStringParameters(expectation.HttpRequest.ParametersList.ToArray())
                        .WithBody(expectation.HttpRequest.Body)
                        .WithSecure(isSecure),
                    unlimitedTimes ? Times.Unlimited() : Times.Exactly(expectation.Times.Count))
                .Respond(httpResponse
                         .WithStatusCode(expectation.HttpResponse.StatusCode)
                         .WithHeaders(expectation.HttpResponse.Headers)
                         .WithBody(expectation.HttpResponse.Body ?? string.Empty)
                         .WithDelay(GetTimeSpanDelay(expectation.HttpResponse.Delay)));
        }
    }
}
