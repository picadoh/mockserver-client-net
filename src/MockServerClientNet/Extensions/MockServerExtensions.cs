using System;
using System.Collections.Generic;
using System.IO;
using MockServerClientNet.Model;
using Newtonsoft.Json;

namespace MockServerClientNet.Extensions
{
    public static class MockServerExtensions
    {
        public static void LoadExpectationsFromFile(this MockServerClient mockServerClient, string expectationsFilePath)
        {
            if (!File.Exists(expectationsFilePath))
            {
                throw new FileNotFoundException($"File: {expectationsFilePath} not found!");
            }

            var fileContent = File.ReadAllText(expectationsFilePath);
            var expectations = JsonConvert.DeserializeObject<IEnumerable<Expectation>>(fileContent);

            foreach (var expectation in expectations)
            {
                var httpRequest = HttpRequest.Request();
                var httpResponse = HttpResponse.Response();

                SetupMockServerRequest(mockServerClient, expectation, httpRequest, httpResponse);
            }
        }

        private static void SetupMockServerRequest(MockServerClient mockServerClient, Expectation expectation,
            HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var isSecure = expectation.HttpRequest.IsSecure.HasValue && expectation.HttpRequest.IsSecure.Value;
            var unlimitedTimes = expectation.Times == null || expectation.Times.IsUnlimited;

            mockServerClient
                .When(httpRequest
                        .WithMethod(expectation.HttpRequest.Method)
                        .WithPath(expectation.HttpRequest.Path)
                        .WithQueryStringParameters(expectation.HttpRequest.Parameters.ToArray())
                        .WithBody(expectation.HttpRequest.Body)
                        .WithSecure(isSecure),
                    unlimitedTimes ? Times.Unlimited() : Times.Exactly(expectation.Times.Count))
                .Respond(httpResponse
                    .WithStatusCode(expectation.HttpResponse.StatusCode)
                    .WithHeaders(expectation.HttpResponse.Headers.ToArray())
                    .WithBody(expectation.HttpResponse.Body ?? string.Empty)
                    .WithDelay(GetTimeSpanDelay(expectation.HttpResponse.Delay)));
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
    }
}