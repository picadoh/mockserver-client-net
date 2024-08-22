using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet.Extensions;
using MockServerClientNet.Model;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;

namespace MockServerClientNet.Tests
{
    public class ExpectationsTest(MockServerFixture fixture) : MockServerClientTest(fixture: fixture)
    {
        [Fact]
        public async Task ShouldRespondAccordingToExpectation()
        {
            // arrange
            await SetupPostExpectation();

            // act
            var response = await SendRequestAsync(BuildPostRequest());

            // assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("{ \"id\": \"123\" }", response.Content.ReadAsStringAsync().AwaitResult());
            Assert.Equal(HttpStatusCode.Created.ToString(), response.ReasonPhrase);
        }

        [Fact]
        public async Task ShouldRespondWithCustomReasonPhrase()
        {
            // arrange
            await SetupPostExpectation(reasonPhrase: "custom reason");

            // act
            var response = await SendRequestAsync(BuildPostRequest());

            // assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("{ \"id\": \"123\" }", response.Content.ReadAsStringAsync().AwaitResult());
            Assert.Equal("custom reason", response.ReasonPhrase);
        }

        [Fact]
        public async Task ShouldRespondAccordingToExpectationOnly2Times()
        {
            // arrange
            await SetupPostExpectation(false, 2);

            // act 1
            var response1 = await SendRequestAsync(BuildPostRequest());

            // assert
            Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
            Assert.Equal("{ \"id\": \"123\" }", await response1.Content.ReadAsStringAsync());

            // act 2
            var response2 = await SendRequestAsync(BuildPostRequest());

            // assert
            Assert.Equal(HttpStatusCode.Created, response2.StatusCode);
            Assert.Equal("{ \"id\": \"123\" }", await response2.Content.ReadAsStringAsync());

            // act 3
            var response3 = await SendRequestAsync(BuildPostRequest());

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response3.StatusCode);
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
            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal("hello", responseBody);

            // act 2
            Assert.NotNull(MockServerClient.Clear(request));

            SendRequest(BuildGetRequest("/hello"), out responseBody, out statusCode);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, statusCode);
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
            Assert.Equal(3, result[0].Headers.Count);
            Assert.True(result[0].Headers.Exists(h => h.Name == "Host"));
        }

        private async Task SetupPostExpectation(bool unlimited = true, int times = 0, string reasonPhrase = null)
        {
            const string body = "{\"name\": \"foo\"}";

            await MockServerClient
                .When(Request()
                        .WithMethod("POST")
                        .WithPath("/customers")
                        .WithHeaders(
                            new Header("Content-Type", "application/json; charset=utf-8"),
                            new Header("Content-Length", body.Length.ToString()))
                        .WithHeader("Host", HostHeader)
                        .WithKeepAlive(true)
                        .WithQueryStringParameter("param", "value")
                        .WithBody(body),
                    unlimited ? Times.Unlimited() : Times.Exactly(times))
                .RespondAsync(Response()
                    .WithStatusCode(HttpStatusCode.Created)
                    .WithReasonPhrase(reasonPhrase)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{ \"id\": \"123\" }"));
        }

        private HttpRequestMessage BuildPostRequest()
        {
            return BuildRequest(HttpMethod.Post, "/customers?param=value", "{\"name\": \"foo\"}");
        }
    }
}