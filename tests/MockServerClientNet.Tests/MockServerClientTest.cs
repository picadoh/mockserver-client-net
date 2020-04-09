using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace MockServerClientNet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using MockServerClientNet.Extensions;
    using MockServerClientNet.Model;
    using MockServerClientNet.Verify;
    using Newtonsoft.Json;
    using Xunit;
    using static MockServerClientNet.Model.HttpRequest;
    using static MockServerClientNet.Model.HttpResponse;

    public class MockServerClientTest : IDisposable
    {
        private readonly string MockServerHost = Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "ssg-mockserver.azurewebsites.net";
        private readonly int MockServerPort = int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "80");

        private MockServerClient mockServerClient;

        public MockServerClientTest()
        {
            mockServerClient = new MockServerClient(MockServerHost, MockServerPort);
            mockServerClient.Reset();
        }

        public void Dispose()
        {
            mockServerClient.Reset();
        }

        [Fact]
        public void ShouldRespondAccordingToExpectation()
        {
            // arrange

            SetupLoginResponse(unlimited: true);

            // act
            string responseBody = null;
            HttpStatusCode? statusCode = null;

            SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);
        }

        [Fact]
        public void ShouldRespondAccordingToExpectationOnly2Times()
        {
            // arrange
            SetupLoginResponse(unlimited: false, times: 2);

            // act 1
            string responseBody = null;
            HttpStatusCode? statusCode = null;
            SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 2
            responseBody = null;
            statusCode = null;
            SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
            Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

            // act 3
            responseBody = null;
            statusCode = null;
            SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
        }

        [Fact]
        public void ShouldClearExpectation()
        {
            // arrange

            HttpRequest request = Request().WithMethod("GET").WithPath("/hello");

            mockServerClient
            .When(request, Times.Unlimited())
            .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act 1
            string responseBody = null;
            HttpStatusCode? statusCode = null;

            SendRequest(BuildGetRequest("/hello"), out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.OK, statusCode.Value);
            Assert.Equal("hello", responseBody);

            // act 2
            mockServerClient.Clear(request);

            SendRequest(BuildGetRequest("/hello"), out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
        }

        [Fact]
        public void ShouldVerify()
        {
            // arrange

            HttpRequest request = Request().WithMethod("GET").WithPath("/hello");

            mockServerClient
            .When(request, Times.Unlimited())
            .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(BuildGetRequest("/hello"), out _, out _);
            SendRequest(BuildGetRequest("/hello"), out _, out _);

            // act
            var result = mockServerClient.Verify(Request()
                                                .WithMethod("GET")
                                                .WithPath("/hello"), VerificationTimes.Exactly(2));

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldVerifyMultiple()
        {
            // arrange

            HttpRequest request1 = Request().WithMethod("GET").WithPath("/hello");
            HttpRequest request2 = Request().WithMethod("GET").WithPath("/world");

            mockServerClient
            .When(request1, Times.Unlimited())
            .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            mockServerClient
            .When(request2, Times.Unlimited())
            .Respond(Response().WithStatusCode(200).WithBody("world").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(BuildGetRequest("/hello"), out _, out _);
            SendRequest(BuildGetRequest("/world"), out _, out _);

            // act
            var result = mockServerClient.Verify(request1, request2);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldVerifyZeroInteractions()
        {

            // act
            var result = mockServerClient.VerifyZeroInteractions();

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldRetrieveRecordedRequests()
        {
            // arrange

            const string REQUEST_URL = "/ShouldRetrieveRecordedRequests";
            HttpRequest request = Request().WithMethod(".*").WithPath(REQUEST_URL);

            mockServerClient
            .When(request, Times.Unlimited())
            .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            string responseBody1 = null;
            string responseBody2 = null;

            SendRequest(BuildGetRequest(REQUEST_URL), out responseBody1, out _);
            SendRequest(BuildGetRequest(REQUEST_URL), out responseBody2, out _);
            SendRequest(BuildPostRequest(REQUEST_URL), out responseBody2, out _);
            SendRequest(BuildPostRequestMultipart(REQUEST_URL), out responseBody2, out _);

            var result = mockServerClient.RetrieveRecordedRequests(request);

            // assert
            Assert.Equal(4, result.Length);
        }

        [Fact]
        public void WhenExpectationsAreLoadedFromFile_ShoulRespondFromTheConfiguredRoutes()
        {
            // arrange
            mockServerClient.Reset();
            var filePath = Path.Combine("ExpectationFiles", "TestExpectations.json");
            mockServerClient.LoadExpectationsFromFile(filePath);

            // act
            string responseBody1 = null;
            string responseBody2 = null;

            SendRequest(BuildGetRequest("/hello?id=1"), out responseBody1, out _);
            SendRequest(BuildGetRequest("/hello2"), out responseBody2, out _);

            // assert
            Assert.NotNull(responseBody1);
            Assert.NotNull(responseBody2);
        }

        [Fact]
        public void Then_Websocket()
        {

            mockServerClient.When(Request().WithMethod("GET").WithPath("/test"), Times.Unlimited());

        }

        [Fact]
        public void WhenExpectationsAreReturningBodyBinary_ShoulRespondFromTheConfiguredRoutes()
        {
            byte[] binary = new byte[1000];
            for (int i = 0; i < 1000; i++) binary[i] = (byte)(i % 0xFF);
            mockServerClient.When(
               Request()
               .WithMethod("GET")
               .WithPath("/helloBinaryTest")
               ,
               Times.Unlimited())
               .Respond(Response()
                   .WithStatusCode(200)
                   .WithHeaders(new Header("Content-Type", "text/plain"))
                   .WithBody(Body.BinaryBody(binary, "image/jpg"))
                   .WithDelay(TimeSpan.FromMilliseconds(100))
           );
            // act
            byte[] responseBody1 = null;

            SendRequestBinary(BuildGetRequest("/helloBinaryTest"), out responseBody1, out _);

            // assert
            Assert.Equal(responseBody1, binary);
        }


        void SendRequest(HttpRequestMessage request, out string responseBody, out HttpStatusCode? statusCode)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = client.SendAsync(request).Result)
            using (HttpContent content = res.Content)
            {
                statusCode = res.StatusCode;
                responseBody = content.ReadAsStringAsync().Result;
            }
        }
        void SendRequestBinary(HttpRequestMessage request, out byte[] responseBody, out HttpStatusCode? statusCode)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = client.SendAsync(request).Result)
            using (HttpContent content = res.Content)
            {
                statusCode = res.StatusCode;
                responseBody = content.ReadAsByteArrayAsync().Result;
            }
        }
        void SetupLoginResponse(bool ssl = false, bool unlimited = true, int times = 0)
        {
            mockServerClient
            .When(Request()
                    .WithMethod("POST")
                    .WithPath("/login")
                    .WithQueryStringParameters(
                    new Parameter("returnUrl", "/account"))
                    .WithBody("{\"username\": \"foo\", \"password\": \"bar\"}")
                    .WithSecure(ssl),
                    unlimited ? Times.Unlimited() : Times.Exactly(times))
            .Respond(Response()
                        .WithStatusCode(401)
                        .WithHeaders(
                        new Header("Content-Type", "application/json; charset=utf-8"),
                        new Header("Cache-Control", "public, max-age=86400"))
                        .WithBody("{ \"message\": \"incorrect username and password combination\" }")
                        .WithDelay(TimeSpan.FromSeconds(0)));
        }

        HttpRequestMessage BuildLoginRequest(bool ssl = false)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;

            string scheme = ssl ? "https" : "http";
            request.RequestUri = new Uri(scheme + "://" + MockServerHost + ":" + MockServerPort + "/login?returnUrl=/account");
            request.Content = new StringContent("{\"username\": \"foo\", \"password\": \"bar\"}");
            return request;
        }

        HttpRequestMessage BuildGetRequest(string path)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri("http://" + MockServerHost + ":" + MockServerPort + path);
            request.Content = new StringContent("{ \"param\" : \"1234\" }");
            return request;
        }

        HttpRequestMessage BuildPostRequest(string path)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("https://" + MockServerHost + path);

            request.Content = new StringContent(
            @"{
                ""referenceId"":""20137705-bb8a-4215-bd39-f18dbb78ef7a"",""assetCode"":""Dishwasher"",""description"":""dummy description\\n"",""priorityId"":""5012"",""costCodeId"":null,""createDate"":""2019-03-29 14:51:47Z""
            }");
            //request.Content.Headers.Add("Content-Type", "application/json; charset=utf-8");
            //request.Content.Headers.Add("Content-Length", "189");
            return request;
        }

        HttpRequestMessage BuildPostRequestMultipart(string path)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("http://" + MockServerHost + ":" + MockServerPort + path);
            var msg = @"{
                ""referenceId"":""20137705-bb8a-4215-bd39-f18dbb78ef7a"",""assetCode"":""Dishwasher"",""description"":""dummy description\\n"",""priorityId"":""5012"",""costCodeId"":null,""createDate"":""2019-03-29 14:51:47Z""
            }";
            var mc = new MultipartFormDataContent("BOUNDARY1234");
            mc.Add(new StringContent(msg), "somename", "somefilename");
            request.Content = mc;
            //request.Content.Headers.Add("Content-Type", "application/json; charset=utf-8");
            //request.Content.Headers.Add("Content-Length", "189");
            return request;
        }
    }
}
