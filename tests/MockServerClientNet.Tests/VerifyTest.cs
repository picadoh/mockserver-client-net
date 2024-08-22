using System;
using System.Net.Http;
using MockServerClientNet.Model;
using MockServerClientNet.Verify;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;

namespace MockServerClientNet.Tests
{
    public class VerifyTest(MockServerFixture fixture) : MockServerClientTest(fixture: fixture)
    {
        [Fact]
        public void ShouldVerifyOnceSuccess()
        {
            SendHello(1);

            Assert.NotNull(MockServerClient.Verify(Request()
                .WithMethod(HttpMethod.Get)
                .WithPath("/hello"), VerificationTimes.Once()));
        }

        [Fact]
        public void ShouldVerifyOnceFailed()
        {
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod(HttpMethod.Get)
                    .WithPath("/hello"), VerificationTimes.Once());
            });

            Assert.StartsWith("Request not found exactly once", ex.Message);
        }

        [Fact]
        public void ShouldVerifyExactlySuccess()
        {
            SendHello(2);

            Assert.NotNull(MockServerClient.Verify(Request()
                .WithMethod(HttpMethod.Get)
                .WithPath("/hello"), VerificationTimes.Exactly(2)));
        }

        [Fact]
        public void ShouldVerifyExactlyFailed()
        {
            // arrange
            var request = Request().WithMethod(HttpMethod.Get).WithPath("/hello");

            MockServerClient
                .When(request, Times.Unlimited())
                .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            SendRequest(BuildGetRequest("/hello"), out _, out _);

            // assert
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod(HttpMethod.Get)
                    .WithPath("/hello"), VerificationTimes.Exactly(2));
            });

            Assert.StartsWith("Request not found exactly 2 times", ex.Message);
        }

        [Fact]
        public void ShouldVerifyAtLeastSuccess()
        {
            SendHello(3);

            Assert.NotNull(MockServerClient.Verify(Request()
                .WithMethod(HttpMethod.Get)
                .WithPath("/hello"), VerificationTimes.AtLeast(2)));
        }

        [Fact]
        public void ShouldVerifyAtLeastFailed()
        {
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod(HttpMethod.Get)
                    .WithPath("/hello"), VerificationTimes.AtLeast(2));
            });

            Assert.StartsWith("Request not found at least 2 times", ex.Message);
        }

        [Fact]
        public void ShouldVerifyAtMostSuccess()
        {
            SendHello(1);

            Assert.NotNull(MockServerClient.Verify(Request()
                .WithMethod(HttpMethod.Get)
                .WithPath("/hello"), VerificationTimes.AtMost(2)));
        }

        [Fact]
        public void ShouldVerifyAtMostFailed()
        {
            SendHello(3);

            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod(HttpMethod.Get)
                    .WithPath("/hello"), VerificationTimes.AtMost(2));
            });

            Assert.StartsWith("Request not found at most 2 times", ex.Message);
        }

        [Fact]
        public void ShouldVerifyBetweenSuccess()
        {
            SendHello(2);

            Assert.NotNull(MockServerClient.Verify(Request()
                .WithMethod(HttpMethod.Get)
                .WithPath("/hello"), VerificationTimes.Between(1, 3)));
        }

        [Fact]
        public void ShouldVerifyBetweenFailed()
        {
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod(HttpMethod.Get)
                    .WithPath("/hello"), VerificationTimes.Between(1, 2));
            });

            Assert.StartsWith("Request not found between 1 and 2", ex.Message);
        }

        [Fact]
        public void ShouldVerifyMultipleRequests()
        {
            MockServerClient
                .When(Request().WithPath("/hello"), Times.Exactly(1))
                .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            MockServerClient
                .When(Request().WithPath("/world"), Times.Exactly(1))
                .Respond(Response().WithStatusCode(200).WithBody("world").WithDelay(TimeSpan.FromSeconds(0)));

            SendRequest(BuildGetRequest("/hello"), out var helloResponse, out _);
            SendRequest(BuildGetRequest("/world"), out var worldResponse, out _);

            Assert.Equal("hello", helloResponse);
            Assert.Equal("world", worldResponse);

            Assert.NotNull(MockServerClient.Verify(Request().WithPath("/hello"), Request().WithPath("/world")));
        }

        [Fact]
        public void ShouldVerifyMultipleRequestsFailed()
        {
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request().WithPath("/hello"), Request().WithPath("/world"));
            });

            Assert.StartsWith("Request sequence not found", ex.Message);
        }

        [Fact]
        public void ShouldVerifyZeroInteractionsSuccess()
        {
            Assert.NotNull(MockServerClient.VerifyZeroInteractions());
        }

        [Fact]
        public void ShouldVerifyZeroInteractionsFailed()
        {
            SendHello(1);

            var ex = Assert.Throws<AssertionException>(() => { MockServerClient.VerifyZeroInteractions(); });

            Assert.StartsWith("Request not found exactly 0 times", ex.Message);
        }

        private void SendHello(int times)
        {
            var request = Request().WithMethod(HttpMethod.Get).WithPath("/hello");

            MockServerClient
                .When(request, Times.Unlimited())
                .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            for (var i = 0; i < times; i++)
            {
                SendRequest(BuildGetRequest("/hello"), out _, out _);
            }
        }
    }
}