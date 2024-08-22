using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet.Model;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;

namespace MockServerClientNet.Tests
{
    public class HttpSchemeTest(MockServerFixture fixture) : MockServerClientTest(fixture, HttpScheme.Https, Handler)
    {
        private static readonly HttpClientHandler Handler = GetHttpClientHandler();

        [Fact]
        public async Task ShouldRespondToHttpsRequest()
        {
            // arrange
            await SetupExpectation(MockServerClient, true);

            // act
            var response = await SendHello(HttpScheme.Https);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("{\"message\": \"hello\"}", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ShouldNotMatchHttpsRequestWhenSecureIsFalse()
        {
            // arrange
            await SetupExpectation(MockServerClient, false);

            // act
            var response = await SendHello(HttpScheme.Https);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ShouldNotMatchHttpRequestWhenSecureIsTrue()
        {
            // arrange
            await SetupExpectation(MockServerClient, true);

            // act
            var response = await SendHello(HttpScheme.Http);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private static HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>
                {
                    Assert.Contains("O=MockServer", cert.Issuer);
                    return true;
                }
            };
        }

        private static async Task SetupExpectation(MockServerClient mockServerClient, bool secure)
        {
            await mockServerClient.ResetAsync();

            await mockServerClient.When(Request()
                    .WithSecure(secure)
                    .WithMethod(HttpMethod.Get)
                    .WithPath("/hello"),
                Times.Unlimited()
            ).RespondAsync(Response()
                .WithDelay(TimeSpan.FromSeconds(0))
                .WithStatusCode(200)
                .WithBody("{\"message\": \"hello\"}"));
        }

        private async Task<HttpResponseMessage> SendHello(HttpScheme scheme)
        {
            using (var client = new HttpClient(Handler, false))
            {
                var host = MockServerClient.ServerAddress().Host;
                var port = MockServerClient.ServerAddress().Port;
                return await client.GetAsync(new Uri($"{scheme}://{host}:{port}/hello"));
            }
        }
    }
}