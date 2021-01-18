// -----------------------------------------------------------------------
// <copyright file="MsClientNetIntegrationTests.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;
    using DotNetMockServerClient.Extensions;
    using Xunit;
    using HttpRequest = DotNetMockServerClient.DataContracts.HttpRequest;
    using HttpResponse = DotNetMockServerClient.DataContracts.HttpResponse;

    /// <summary>
    /// The implementation of MockServerClientNetTests tests.
    /// </summary>
    public class MsClientNetIntegrationTests : IDisposable
    {
        private readonly MockServerClient mockServerClient;

        private readonly string mockServerHost = Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "localhost";

        private readonly int mockServerPort = int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "1080", CultureInfo.InvariantCulture);

        /// <summary>
        /// Initialises a new instance of the <see cref="MsClientNetIntegrationTests"/> class.
        /// </summary>
        public MsClientNetIntegrationTests()
        {
            this.mockServerClient = new MockServerClient(this.mockServerHost, this.mockServerPort);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldCheckIfThereAreAnyExpectationAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = await this.mockServerClient.RetrieveActiveExpectationsAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldCheckIfThereAreAnyExpectation()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("POST").WithPath("/hello");

            // act
            var result = this.mockServerClient.RetrieveActiveExpectations(request);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldClearExpectationAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            // act 1
            var result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            string responseBody = result.Item1;
            HttpStatusCode? statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.OK, statusCode.Value);
            Assert.Equal("hello", responseBody);

            // act 2
            await this.mockServerClient.ClearAsync(request).ConfigureAwait(false);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");

            result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldClearExpectation()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            // act 1
            SendRequest(getRequest, out var responseBody, out var statusCode);

            // assert
            Assert.Equal(HttpStatusCode.OK, statusCode.Value);
            Assert.Equal("hello", responseBody);

            // act 2
            this.mockServerClient.Clear(request);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            SendRequest(getRequest, out _, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRespondAccordingToExpectationAsync()
        {
            // arrange
            this.SetupLoginResponse(unlimited: true);
            var getRequest = this.BuildLoginRequest();

            // act
            var result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            string responseBody = result.Item1;
            HttpStatusCode? statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRespondAccordingToExpectation()
        {
            // arrange
            this.SetupLoginResponse(unlimited: true);
            var getRequest = this.BuildLoginRequest();

            // act
            SendRequest(getRequest, out string responseBody, out HttpStatusCode? statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRespondAccordingToExpectationOnly2TimesAsync()
        {
            // arrange
            this.SetupLoginResponse(unlimited: false, times: 2);
            var getRequest = this.BuildLoginRequest();
            var getRequest2 = this.BuildLoginRequest();
            var getRequest3 = this.BuildLoginRequest();

            // act 1
            var result = await SendRequestAsync(getRequest).ConfigureAwait(false);

            string responseBody = result.Item1;
            HttpStatusCode? statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 2
            result = await SendRequestAsync(getRequest2).ConfigureAwait(false);

            responseBody = result.Item1;
            statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 3
            result = await SendRequestAsync(getRequest3).ConfigureAwait(false);

            statusCode = result.Item2;

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
            getRequest2.Dispose();
            getRequest3.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRespondAccordingToExpectationOnly2Times()
        {
            // arrange
            this.SetupLoginResponse(unlimited: false, times: 2);
            var getRequest = this.BuildLoginRequest();
            var getRequest2 = this.BuildLoginRequest();
            var getRequest3 = this.BuildLoginRequest();

            // act 1
            SendRequest(getRequest, out string responseBody, out HttpStatusCode? statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 2
            SendRequest(getRequest2, out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 3
            SendRequest(getRequest3, out _, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
            getRequest.Dispose();
            getRequest2.Dispose();
            getRequest3.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRetrieveLogsAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildLoginRequest();

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            await SendRequestAsync(getRequest).ConfigureAwait(false);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            await SendRequestAsync(getRequest).ConfigureAwait(false);

            var result = await this.mockServerClient.RetrieveLogMessagesAsync(request).ConfigureAwait(false);

            // assert
            Assert.NotEqual(0, result.Length);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRetrieveLogs()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildLoginRequest();

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            SendRequest(getRequest, out _, out _);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            SendRequest(getRequest, out _, out _);

            var result = this.mockServerClient.RetrieveLogMessages(request);

            // assert
            Assert.NotEqual(0, result.Length);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldRetrieveRecordedRequestsAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/hello");

            this.mockServerClient
            .When(request, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            await SendRequestAsync(getRequest).ConfigureAwait(false);
            await SendRequestAsync(getRequest2).ConfigureAwait(false);

            var result = await this.mockServerClient.RetrieveRecordedRequestsAsync(request).ConfigureAwait(false);

            // assert
            Assert.Equal(2, result.Length);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldRetrieveRecordedRequests()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/hello");

            this.mockServerClient
            .When(request, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            SendRequest(getRequest, out _, out _);
            SendRequest(getRequest2, out _, out _);

            var result = this.mockServerClient.RetrieveRecordedRequests(request);

            // assert
            Assert.Equal(2, result.Length);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyAsync()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            await SendRequestAsync(getRequest).ConfigureAwait(false);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            await SendRequestAsync(getRequest).ConfigureAwait(false);

            // act
            var result = await this.mockServerClient.VerifyAsync(
                HttpRequest.Request()
                                                .WithMethod("GET")
                                                .WithPath("/hello"),
                VerificationTimes.Exactly(2)).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerify()
        {
            // arrange
            HttpRequest request = HttpRequest.Request().WithMethod("GET").WithPath("/hello");

            this.mockServerClient
            .When(request, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));
            var getRequest = this.BuildGetRequest("/hello");

            SendRequest(getRequest, out _, out _);
            getRequest.Dispose();
            getRequest = this.BuildGetRequest("/hello");
            SendRequest(getRequest, out _, out _);

            // act
            var result = this.mockServerClient.Verify(
                HttpRequest.Request()
                                                .WithMethod("GET")
                                                .WithPath("/hello"),
                VerificationTimes.Exactly(2));

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyMultipleAsync()
        {
            // arrange
            HttpRequest request1 = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            HttpRequest request2 = HttpRequest.Request().WithMethod("GET").WithPath("/world");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/world");

            this.mockServerClient
            .When(request1, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            this.mockServerClient
            .When(request2, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("world").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);
            await SendRequestAsync(getRequest2).ConfigureAwait(false);

            // act
            var result = await this.mockServerClient.VerifyAsync(request1, request2).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyMultiple()
        {
            // arrange
            HttpRequest request1 = HttpRequest.Request().WithMethod("GET").WithPath("/hello");
            HttpRequest request2 = HttpRequest.Request().WithMethod("GET").WithPath("/world");
            var getRequest = this.BuildGetRequest("/hello");
            var getRequest2 = this.BuildGetRequest("/world");

            this.mockServerClient
            .When(request1, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            this.mockServerClient
            .When(request2, Times.Unlimited())
            .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("world").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out _, out _);
            SendRequest(getRequest2, out _, out _);

            // act
            var result = this.mockServerClient.Verify(request1, request2);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyXPathAsync()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>111</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);

            // act
            var result = await this.mockServerClient.VerifyAsync(request, VerificationTimes.Exactly(1)).ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyXPath()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>111</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out var _, out var _);

            // act
            var result = this.mockServerClient.Verify(request, VerificationTimes.Exactly(1));

            // assert
            Assert.NotNull(result);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyXPath_And_Not_ResponseAsync()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[not(text() != '111')]"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>112</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);

            async Task Act() => await this.mockServerClient.VerifyAsync(request, VerificationTimes.Exactly(1)).ConfigureAwait(false);

            // assert
            await Assert.ThrowsAsync<AssertionException>(Act).ConfigureAwait(false);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyXPath_And_Not_Response()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[not(text() != '111')]"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>112</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out var _, out var _);

            void Act() => this.mockServerClient.Verify(request, VerificationTimes.Exactly(1));

            // assert
            Assert.Throws<AssertionException>(Act);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyXPathFailsAsync()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>222</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            await SendRequestAsync(getRequest).ConfigureAwait(false);

            // act
            async Task Act() => await this.mockServerClient.VerifyAsync(request, VerificationTimes.Exactly(1)).ConfigureAwait(false);

            // assert
            await Assert.ThrowsAsync<AssertionException>(Act).ConfigureAwait(false);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyXPathFails()
        {
            // arrange
            var request = HttpRequest.Request().WithMethod("POST").WithPath("/hello").WithBody(BodyCheck.WithXPath("//id[text() = '111']"));
            var getRequest = this.BuildPostRequest("/hello", "<elem><id>222</id></elem>", "application/xml");

            this.mockServerClient
                .When(request, Times.Unlimited())
                .Respond(HttpResponse.Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(getRequest, out var _, out var _);

            // act
            void Act() => this.mockServerClient.Verify(request, VerificationTimes.Exactly(1));

            // assert
            Assert.Throws<AssertionException>(Act);
            getRequest.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task ShouldVerifyZeroInteractionsAsync()
        {
            // act
            var result = await this.mockServerClient.VerifyZeroInteractionsAsync().ConfigureAwait(false);

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void ShouldVerifyZeroInteractions()
        {
            // act
            var result = this.mockServerClient.VerifyZeroInteractions();

            // assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Some test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public async Task WhenExpectationsAreLoadedFromFile_ShoulRespondFromTheConfiguredRoutesAsync()
        {
            // arrange
            var filePath = Path.Combine("ExpectationFiles", "TestExpectations.json");
            var getRequest = this.BuildGetRequest("/hello?id=1");
            var getRequest2 = this.BuildGetRequest("/hello2");

            // act
            await this.mockServerClient.LoadExpectationsFromFileAsync(filePath).ConfigureAwait(false);

            var result = SendRequestAsync(getRequest).ConfigureAwait(false);

            var otherResult = SendRequestAsync(getRequest2).ConfigureAwait(false);

            // assert
            var result1 = await result;
            var result2 = await otherResult;
            Assert.NotNull(result1.Item1);
            Assert.NotNull(result2.Item1);

            getRequest.Dispose();
            getRequest2.Dispose();
        }

        /// <summary>
        /// Some test.
        /// </summary>
        [Fact]
        [Trait("Category", "DockerIntegrationTests")]
        public void WhenExpectationsAreLoadedFromFile_ShoulRespondFromTheConfiguredRoutes()
        {
            // arrange
            var filePath = Path.Combine("ExpectationFiles", "TestExpectations.json");
            var getRequest = this.BuildGetRequest("/hello?id=1");
            var getRequest2 = this.BuildGetRequest("/hello2");

            // act
            this.mockServerClient.LoadExpectationsFromFile(filePath);
            SendRequest(getRequest, out var responseBody1, out _);
            SendRequest(getRequest2, out var responseBody2, out _);

            // assert
            Assert.NotNull(responseBody1);
            Assert.NotNull(responseBody2);
            getRequest.Dispose();
            getRequest2.Dispose();
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
            this.mockServerClient.Reset();
            this.mockServerClient.Dispose();
        }

        private static async Task<Tuple<string, HttpStatusCode?>> SendRequestAsync(HttpRequestMessage request)
        {
            string responseBody;
            HttpStatusCode? statusCode;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.SendAsync(request).ConfigureAwait(false))
            using (HttpContent content = res.Content)
            {
                statusCode = res.StatusCode;
                responseBody = await content.ReadAsStringAsync().ConfigureAwait(false);
            }

            return new Tuple<string, HttpStatusCode?>(responseBody, statusCode);
        }

        private static void SendRequest(HttpRequestMessage request, out string responseBody, out HttpStatusCode? statusCode)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = client.SendAsync(request).Result)
            using (HttpContent content = res.Content)
            {
                statusCode = res.StatusCode;
                responseBody = content.ReadAsStringAsync().Result;
            }
        }

        private HttpRequestMessage BuildGetRequest(string path)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://" + this.mockServerHost + ":" + this.mockServerPort + path),
            };
            return request;
        }

        private HttpRequestMessage BuildLoginRequest(bool ssl = false)
        {
            string scheme = ssl ? "https" : "http";
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(scheme + "://" + this.mockServerHost + ":" + this.mockServerPort + "/login?returnUrl=/account"),
                Content = new StringContent("{\"username\": \"foo\", \"password\": \"bar\"}"),
            };

            return request;
        }

        private HttpRequestMessage BuildPostRequest(string path, string body, string mediaType)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://" + this.mockServerHost + ":" + this.mockServerPort + path),
                Content = new StringContent(body, Encoding.UTF8, mediaType),
            };
            return request;
        }

        private void SetupLoginResponse(bool ssl = false, bool unlimited = true, int times = 0)
        {
            this.mockServerClient
            .When(
                HttpRequest.Request()
                    .WithMethod("POST")
                    .WithPath("/login")
                    .WithQueryStringParameters(
                    new Parameter("returnUrl", "/account"))
                    .WithBody("{\"username\": \"foo\", \"password\": \"bar\"}")
                    .WithSecure(ssl),
                unlimited ? Times.Unlimited() : Times.Exactly(times))
            .Respond(HttpResponse.Response()
                        .WithStatusCode(401)
                        .WithHeaders(
                        new Header("Content-Type", "application/json; charset=utf-8"),
                        new Header("Cache-Control", "public, max-age=86400"))
                        .WithBody("{ \"message\": \"incorrect username and password combination\" }")
                        .WithDelay(TimeSpan.FromSeconds(0)));
        }
    }
}
