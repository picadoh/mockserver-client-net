// -----------------------------------------------------------------------
// <copyright file="MsClientNetMockTests.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;
    using DotNetMockServerClient.DataContracts.Serializer;
    using Moq;
    using Moq.Protected;
    using Xunit;
    using HttpRequest = DotNetMockServerClient.DataContracts.HttpRequest;

    /// <summary>
    /// The implementation of MockServerClientNetTests tests.
    /// </summary>
    public class MsClientNetMockTests : IDisposable
    {
        private readonly Mock<IMockServerClient> mockMockServerClient;

        private readonly string mockServerHost = Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "localhost";

        private readonly int mockServerPort = int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "1080", CultureInfo.InvariantCulture);

        private MockServerClient mockServerClient;

        private HttpResponseMessage httpResponseMessage;

        private Mock<HttpMessageHandler> mockHttpMessageHandler;

        private HttpClient httpClient;

        private bool canDispose;

        private bool canReset;

        /// <summary>
        /// Initialises a new instance of the <see cref="MsClientNetMockTests"/> class.
        /// </summary>
        public MsClientNetMockTests()
        {
            this.canDispose = true;
            this.canReset = true;

            var result1 = new List<Expectation>();
            this.mockMockServerClient = new Mock<IMockServerClient>();
            this.mockMockServerClient.Setup(x => x.RetrieveActiveExpectationsAsync(It.IsAny<HttpRequest>())).ReturnsAsync(result1.ToArray()).Verifiable();
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_ActualSendExpectationAsync()
        {
            // arrange
            var sendRequest = new Expectation();

            // Always get a 'Cannot access a disposed object in SendRequest' just for this test.
            this.canDispose = false;

            this.CreateClient(string.Empty, HttpStatusCode.Created);

            // act
            await this.mockServerClient.SendExpectationAsync(sendRequest).ConfigureAwait(false);

            // assert
            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test RetrieveActiveExpectationsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_MockRetrieveActiveExpectationsAsync()
        {
            this.CreateClient("Unused");

            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockMockServerClient.Object.RetrieveActiveExpectationsAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test RetrieveActiveExpectationsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_MockSendRequestAsync_Fails()
        {
            // arrange
            // Reset fails if CreateClient is HttpStatusCode.BadRequest
            this.canReset = false;

            this.CreateClient(string.Empty, HttpStatusCode.BadRequest);

            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            try
            {
                await this.mockServerClient.RetrieveLogMessagesAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // assert
                Assert.NotNull(ex);
            }
        }

        /// <summary>
        /// Test RetrieveActiveExpectationsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_RetrieveActiveExpectationsAsync()
        {
            // arrange
            var fp = Path.Combine("ExpectationFiles", "TestExpectations.json");
            var fb = File.ReadAllBytes(fp);
            var fs = Encoding.UTF8.GetString(fb, 0, fb.Length);

            this.CreateClient(fs);

            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockServerClient.RetrieveActiveExpectationsAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_RetrieveLogMessagesAsync()
        {
            // arrange
            var expectedResult = "[{'id':1,'value':'1'}]";

            this.CreateClient(expectedResult);

            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockServerClient.RetrieveLogMessagesAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Test RetrieveRecordedRequestsAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_RetrieveRecordedRequestsAsync()
        {
            // arrange
            JsonSerializer<HttpRequest> s = new JsonSerializer<HttpRequest>();

            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");
            HttpRequest testRequest = HttpRequest.Request().WithMethod("TEST");

            var jsonRequest = "[" + s.Serialize(testRequest) + "]";

            this.CreateClient(jsonRequest);

            // act
            var result = await this.mockServerClient.RetrieveRecordedRequestsAsync(httpRequest).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            Assert.Equal("TEST", result[0].Method);
        }

        /// <summary>
        /// Test ClearAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_ClearAsync()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.ClearAsync(httpRequest).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test ClearAsync.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_Clear()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.Clear(httpRequest);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test VerifyAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_VerifyAsync()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result1 = await this.mockServerClient.VerifyAsync(httpRequest).ConfigureAwait(false);

            var result2 = await this.mockServerClient.VerifyAsync(httpRequest, VerificationTimes.Once()).ConfigureAwait(false);

            // assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test VerifyZeroInteractions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_VerifyZeroInteractionsAsync()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.VerifyZeroInteractionsAsync().ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test VerifyZeroInteractions.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_VerifyZeroInteractions()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.VerifyZeroInteractions();

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_IsRunningAsync_Okay()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.IsRunningAsync(1).ConfigureAwait(false);

            // assert
            Assert.True(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test ShouldCheck_IsRunningAsync_Fails.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_IsRunningAsync_Fails()
        {
            // arrange
            this.CreateClient(string.Empty, HttpStatusCode.NotFound);

            // act
            var result = await this.mockServerClient.IsRunningAsync(2).ConfigureAwait(false);

            // assert
            Assert.False(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test RetrieveLogMessagesAsync.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_IsRunning()
        {
            // arrange
            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.IsRunning(1);

            // assert
            Assert.True(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test StopAsync.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "MockTests")]
        public async Task ShouldCheck_StopAsync()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = await this.mockServerClient.ClearAsync(httpRequest).ConfigureAwait(false);

            this.CreateClient(string.Empty, HttpStatusCode.Accepted);

            await this.mockServerClient.StopAsync(true).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Test Stop.
        /// </summary>
        [Fact]
        [Trait("Category", "MockTests")]
        public void ShouldCheck_Stop()
        {
            // arrange
            HttpRequest httpRequest = HttpRequest.Request().WithMethod("POST");

            this.CreateClient(string.Empty);

            // act
            var result = this.mockServerClient.Clear(httpRequest);

            this.CreateClient(string.Empty, HttpStatusCode.Accepted);

            this.mockServerClient.Stop(true);

            // assert
            Assert.NotNull(result);

            this.mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Moq.Times.AtLeast(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Dispose operation.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose operation.
        /// </summary>
        /// <param name="disposing">The run identifier.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.canReset && this.canDispose)
            {
                this.mockServerClient.Reset();
            }

            if (this.canDispose)
            {
                this.mockServerClient.Dispose();
                this.httpResponseMessage.Dispose();
                this.httpClient.Dispose();
            }
        }

        private void CreateClient(string body, HttpStatusCode sc = HttpStatusCode.OK)
        {
            this.httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = sc,
                Content = new StringContent(body),
            };

            this.mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            this.mockHttpMessageHandler
               .Protected()

               // Setup the PROTECTED method to mock SendAsync call.
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())

               // Prepare the expected response of the mocked http call.
               .ReturnsAsync(this.httpResponseMessage)
               .Verifiable();

            this.mockHttpMessageHandler
               .Protected()

               // Setup the PROTECTED method to allow httpClient to call dispose.
               .Setup(
                  "Dispose",
                  ItExpr.IsAny<bool>());

            // Use real http client with mocked handler
            this.httpClient = new HttpClient(this.mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            this.mockServerClient = new MockServerClient(this.mockServerHost, this.mockServerPort, string.Empty, this.httpClient);
        }
    }
}
