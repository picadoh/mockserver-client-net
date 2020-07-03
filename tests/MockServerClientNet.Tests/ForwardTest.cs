using MockServerClientNet.Extensions;
using MockServerClientNet.Model;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpForward;

namespace MockServerClientNet.Tests
{
    public class ForwardTest : MockServerClientTest
    {
        [Fact]
        public void ShouldForwardRequestWithStringScheme()
        {
            // arrange
            var request = Request().WithMethod("GET").WithPath("/hello");

            var host = MockServerClient.ServerAddress().Host;
            var port = MockServerClient.ServerAddress().Port;

            MockServerClient
                .When(request, Times.Exactly(1))
                .Forward(Forward()
                    .WithScheme("HTTP")
                    .WithHost(host)
                    .WithPort(port));

            // act
            SendRequest(BuildGetRequest("/hello"), out _, out _);

            var result = MockServerClient.RetrieveRecordedRequests(request);

            // assert
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void ShouldForwardRequestUsingHttpSchemeEnum()
        {
            // arrange
            var request = Request().WithMethod("GET").WithPath("/hello");

            var host = MockServerClient.ServerAddress().Host;
            var port = MockServerClient.ServerAddress().Port;

            MockServerClient
                .When(request, Times.Exactly(1))
                .Forward(Forward()
                    .WithScheme(HttpScheme.Https)
                    .WithHost(host)
                    .WithPort(port));

            // act
            SendRequest(BuildGetRequest("/hello"), out _, out _);

            var result = MockServerClient.RetrieveRecordedRequests(request);

            // assert
            Assert.Equal(2, result.Length);
        }
    }
}