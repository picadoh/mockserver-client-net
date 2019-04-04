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
        private readonly string MockServerHost = Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "mockserver.westeurope.cloudapp.azure.com";
        private readonly int MockServerPort = int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "1080");

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
            request.RequestUri = new Uri("http://" + MockServerHost + ":" + MockServerPort + path);
            //"path" : "/JACEK10/When_the_user_created_a_new_helpdesk_request/helpdesk-requests/create",
            request.Headers.Add("Accept", "application/vnd.agility.api.v2+json");
            request.Headers.Add("X-Device-Id", "cNhGJk1gZr8:APA91bFSeht1XgcmSl6UNKHHfotSBqcwPNXaSmC-rx2XBqijyyI6bvnbx35GkkrfDByZ2dd9qTOUzO9mDLG5cjK0_3lBEvy4GpVQChsYuHU-B-mTg6K800brW3GWuGPc1eAbZOQml2m7");
            request.Headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkU3ODU0RDdBOTZBMzU3RjIyRjNCQzk0QkY3Q0M0RUU4MDU3NDFFMUIiLCJ0eXAiOiJKV1QiLCJ4NXQiOiI1NFZOZXBhalZfSXZPOGxMOTh4TzZBVjBIaHMifQ.eyJuYmYiOjE1NTM4NzEwNjcsImV4cCI6MTU1Mzg3NDY2NywiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnNzZ2luc2lnaHQuY29tIiwiYXVkIjpbImh0dHBzOi8vYWNjb3VudC5zc2dpbnNpZ2h0LmNvbS9yZXNvdXJjZXMiLCJzc2ctYWdpbGl0eS1hcGkiXSwiY2xpZW50X2lkIjoic3NnLWFnaWxpdHktZW5naW5lZXIiLCJzdWIiOiIyNzczMDA1OC1iYmIyLTQ2MTEtYmU3My0wOGQ1ZTY1Y2UxOWYiLCJhdXRoX3RpbWUiOjE1NTM4NzEwNjcsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsic3NnLWFnaWxpdHktYXBpIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.K6QZIl5pJdM585QiyzVh_V3GQuTBTnKvrQxWU5tY6JEDPQuO809iv_NvDIJMRPh1iTDImmza0P7exQBzLBPr2gkDJ9hUlYrDnp0KS0B2-GKsJAZkqTPaaufyZ-eIeQyBwKCMo06KcY2Lp3i9bu8LCpWasrHWJ-ZD613FODNrqJHVT6IFcR06Aj7TyjUHiZVIIuT8UTucyH2Eetjumkmcf9si_nu6TVuGFDwPqYIQhcVCHkOtEkEECntPgz8R6YmTopFywLXtLWxRWcI1KTGMk0nZ10ZrxjzSO4a579PP9w3bzOBopIjaaz22w1uT5QnfZECoomB5UOgcAu9Wq-9Ibw");
            request.Headers.Add("Host", "mockserver.westeurope.cloudapp.azure.com:1080");
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
            //"path" : "/JACEK10/When_the_user_created_a_new_helpdesk_request/helpdesk-requests/create",
            request.Headers.Add("Accept", "application/vnd.agility.api.v2+json");
            request.Headers.Add("X-Device-Id", "cNhGJk1gZr8:APA91bFSeht1XgcmSl6UNKHHfotSBqcwPNXaSmC-rx2XBqijyyI6bvnbx35GkkrfDByZ2dd9qTOUzO9mDLG5cjK0_3lBEvy4GpVQChsYuHU-B-mTg6K800brW3GWuGPc1eAbZOQml2m7");
            request.Headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkU3ODU0RDdBOTZBMzU3RjIyRjNCQzk0QkY3Q0M0RUU4MDU3NDFFMUIiLCJ0eXAiOiJKV1QiLCJ4NXQiOiI1NFZOZXBhalZfSXZPOGxMOTh4TzZBVjBIaHMifQ.eyJuYmYiOjE1NTM4NzEwNjcsImV4cCI6MTU1Mzg3NDY2NywiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnNzZ2luc2lnaHQuY29tIiwiYXVkIjpbImh0dHBzOi8vYWNjb3VudC5zc2dpbnNpZ2h0LmNvbS9yZXNvdXJjZXMiLCJzc2ctYWdpbGl0eS1hcGkiXSwiY2xpZW50X2lkIjoic3NnLWFnaWxpdHktZW5naW5lZXIiLCJzdWIiOiIyNzczMDA1OC1iYmIyLTQ2MTEtYmU3My0wOGQ1ZTY1Y2UxOWYiLCJhdXRoX3RpbWUiOjE1NTM4NzEwNjcsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsic3NnLWFnaWxpdHktYXBpIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.K6QZIl5pJdM585QiyzVh_V3GQuTBTnKvrQxWU5tY6JEDPQuO809iv_NvDIJMRPh1iTDImmza0P7exQBzLBPr2gkDJ9hUlYrDnp0KS0B2-GKsJAZkqTPaaufyZ-eIeQyBwKCMo06KcY2Lp3i9bu8LCpWasrHWJ-ZD613FODNrqJHVT6IFcR06Aj7TyjUHiZVIIuT8UTucyH2Eetjumkmcf9si_nu6TVuGFDwPqYIQhcVCHkOtEkEECntPgz8R6YmTopFywLXtLWxRWcI1KTGMk0nZ10ZrxjzSO4a579PP9w3bzOBopIjaaz22w1uT5QnfZECoomB5UOgcAu9Wq-9Ibw");
            request.Headers.Add("Host", "mockserver.westeurope.cloudapp.azure.com:1080");

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
