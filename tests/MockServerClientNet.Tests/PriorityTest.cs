using System;
using System.Threading.Tasks;
using MockServerClientNet.Model;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;

namespace MockServerClientNet.Tests
{
    public class PriorityTest : MockServerClientTest
    {
        [Fact]
        public async Task ShouldMatchSamePriorityUsingOrder()
        {
            await MockServerClient
                .When(Request().WithPath("/hello"), Times.Once(), 5)
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody("{\"msg\": \"first\"}")
                    .WithDelay(TimeSpan.Zero));

            await MockServerClient
                .When(Request().WithPath("/hello"), Times.Once(), 5)
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody("{\"msg\": \"second\"}")
                    .WithDelay(TimeSpan.Zero));

            var (responseBody, statusCode) = await SendRequestAsync(BuildGetRequest("/hello"));

            Assert.Equal("{\"msg\": \"first\"}", responseBody);
        }

        [Fact]
        public async Task ShouldMatchHighPriorityFirst()
        {
            await MockServerClient
                .When(Request().WithPath("/hello"), Times.Once(), 5)
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody("{\"msg\": \"first\"}")
                    .WithDelay(TimeSpan.Zero));

            await MockServerClient
                .When(Request().WithPath("/hello"), Times.Once(), 10)
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody("{\"msg\": \"second\"}")
                    .WithDelay(TimeSpan.Zero));

            var (responseBody, _) = await SendRequestAsync(BuildGetRequest("/hello"));

            Assert.Equal("{\"msg\": \"second\"}", responseBody);
        }
    }
}