using System.Net;
using System.Net.Http;
using MockServerClientNet.Model;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;

namespace MockServerClientNet.Tests
{
    public class ExpectationsTest : MockServerClientTest
    {
        [Fact]
        public void ShouldRespondAccordingToExpectation()
        {
            // arrange
            SetupPostExpectation();

            // act
            SendRequest(BuildPostRequest(), out var responseBody, out var statusCode);

            // assert
            Assert.NotNull(statusCode);
            Assert.Equal(HttpStatusCode.Created, statusCode.Value);
            Assert.Equal("{ \"id\": \"123\" }", responseBody);
        }

        [Fact]
        public void ShouldRespondAccordingToExpectationOnly2Times()
        {
            // arrange
            SetupPostExpectation(false, 2);

            // act 1
            SendRequest(BuildPostRequest(), out var responseBody, out var statusCode);

            // assert
            Assert.NotNull(statusCode);
            Assert.Equal(HttpStatusCode.Created, statusCode.Value);
            Assert.Equal("{ \"id\": \"123\" }", responseBody);

            // act 2
            SendRequest(BuildPostRequest(), out responseBody, out statusCode);

            // assert
            Assert.NotNull(statusCode);
            Assert.Equal(HttpStatusCode.Created, statusCode.Value);
            Assert.Equal("{ \"id\": \"123\" }", responseBody);

            // act 3
            SendRequest(BuildPostRequest(), out responseBody, out statusCode);

            // assert
            Assert.NotNull(statusCode);
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
        }

        [Fact]
        public void ShouldClearExpectation()
        {
            // arrange
            var request = Request().WithMethod(HttpMethod.Get).WithPath("/hello");

            MockServerClient
                .When(request, Times.Unlimited())
                .Respond(Response().WithStatusCode(200).WithBody("hello"));

            // act 1
            SendRequest(BuildGetRequest("/hello"), out var responseBody, out var statusCode);

            // assert
            Assert.NotNull(statusCode);
            Assert.Equal(HttpStatusCode.OK, statusCode.Value);
            Assert.Equal("hello", responseBody);

            // act 2
            Assert.NotNull(MockServerClient.Clear(request));

            SendRequest(BuildGetRequest("/hello"), out responseBody, out statusCode);

            // assert
            Assert.NotNull(statusCode);
            Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
        }

        [Fact]
        public void ShouldRetrieveRecordedRequests()
        {
            // arrange
            var request = Request().WithMethod(HttpMethod.Get).WithPath("/hello");

            MockServerClient
                .When(request)
                .Respond(Response().WithBody("hello"));

            // act
            SendRequest(BuildGetRequest("/hello"), out _, out var statusCode1);
            SendRequest(BuildGetRequest("/hello"), out _, out var statusCode2);

            var result = MockServerClient.RetrieveRecordedRequests(request);

            // assert
            Assert.Equal(2, result.Length);
            Assert.Equal(HttpStatusCode.OK, statusCode1);
            Assert.Equal(HttpStatusCode.OK, statusCode2);
            Assert.True(result[0].Headers.Exists(h => h.Name == "Host"));
        }

        private void SetupPostExpectation(bool unlimited = true, int times = 0)
        {
            const string body = "{\"name\": \"foo\"}";

            MockServerClient
                .When(Request()
                        .WithMethod("POST")
                        .WithPath("/customers")
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithHeader("Content-Length", body.Length.ToString())
                        .WithHeader("Host", HostHeader)
                        .WithKeepAlive(true)
                        .WithQueryStringParameters(
                            new Parameter("param", "value"))
                        .WithBody(body),
                    unlimited ? Times.Unlimited() : Times.Exactly(times))
                .Respond(Response()
                    .WithStatusCode(201)
                    .WithHeaders(new Header("Content-Type", "application/json"))
                    .WithBody("{ \"id\": \"123\" }"));
        }

        private HttpRequestMessage BuildPostRequest()
        {
            return BuildRequest(HttpMethod.Post, "/customers?param=value", "{\"name\": \"foo\"}");
        }
    }
}