using System;
using MockServerClientNet.Model;
using MockServerClientNet.Verify;
using Xunit;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;

namespace MockServerClientNet.Tests
{
    public class VerifyTest : MockServerClientTest
    {
        [Fact]
        public void ShouldVerifyOnceSuccess()
        {
            SendHello(1);

            MockServerClient.Verify(Request()
                .WithMethod("GET")
                .WithPath("/hello"), VerificationTimes.Once());
        }

        [Fact]
        public void ShouldVerifyOnceFailed()
        {
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod("GET")
                    .WithPath("/hello"), VerificationTimes.Once());
            });

            Assert.StartsWith("Request not found exactly once", ex.Message);
        }

        [Fact]
        public void ShouldVerifyExactlySuccess()
        {
            SendHello(2);

            MockServerClient.Verify(Request()
                .WithMethod("GET")
                .WithPath("/hello"), VerificationTimes.Exactly(2));
        }

        [Fact]
        public void ShouldVerifyExactlyFailed()
        {
            // arrange
            var request = Request().WithMethod("GET").WithPath("/hello");

            MockServerClient
                .When(request, Times.Unlimited())
                .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            SendRequest(BuildGetRequest("/hello"), out _, out _);

            // assert
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod("GET")
                    .WithPath("/hello"), VerificationTimes.Exactly(2));
            });

            Assert.StartsWith("Request not found exactly 2 times", ex.Message);
        }

        [Fact]
        public void ShouldVerifyAtLeastSuccess()
        {
            SendHello(3);

            MockServerClient.Verify(Request()
                .WithMethod("GET")
                .WithPath("/hello"), VerificationTimes.AtLeast(2));
        }

        [Fact]
        public void ShouldVerifyAtLeastFailed()
        {
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod("GET")
                    .WithPath("/hello"), VerificationTimes.AtLeast(2));
            });

            Assert.StartsWith("Request not found at least 2 times", ex.Message);
        }

        [Fact]
        public void ShouldVerifyAtMostSuccess()
        {
            SendHello(1);

            MockServerClient.Verify(Request()
                .WithMethod("GET")
                .WithPath("/hello"), VerificationTimes.AtMost(2));
        }

        [Fact]
        public void ShouldVerifyAtMostFailed()
        {
            SendHello(3);

            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod("GET")
                    .WithPath("/hello"), VerificationTimes.AtMost(2));
            });

            Assert.StartsWith("Request not found at most 2 times", ex.Message);
        }

        [Fact]
        public void ShouldVerifyBetweenSuccess()
        {
            SendHello(2);

            MockServerClient.Verify(Request()
                .WithMethod("GET")
                .WithPath("/hello"), VerificationTimes.Between(1, 3));
        }

        [Fact]
        public void ShouldVerifyBetweenFailed()
        {
            var ex = Assert.Throws<AssertionException>(() =>
            {
                MockServerClient.Verify(Request()
                    .WithMethod("GET")
                    .WithPath("/hello"), VerificationTimes.Between(1, 2));
            });

            Assert.StartsWith("Request not found between 1 and 2", ex.Message);
        }

        [Fact]
        public void ShouldVerifyMultipleRequests()
        {
            // arrange
            var request1 = Request().WithMethod("GET").WithPath("/hello");
            var request2 = Request().WithMethod("GET").WithPath("/world");

            MockServerClient
                .When(request1, Times.Unlimited())
                .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

            MockServerClient
                .When(request2, Times.Unlimited())
                .Respond(Response().WithStatusCode(200).WithBody("world").WithDelay(TimeSpan.FromSeconds(0)));

            // act
            SendRequest(BuildGetRequest("/hello"), out _, out _);
            SendRequest(BuildGetRequest("/world"), out _, out _);

            var result = MockServerClient.Verify(request1, request2);

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldVerifyZeroInteractions()
        {
            // assert
            var result = MockServerClient.VerifyZeroInteractions();
            Assert.NotNull(result);
        }

        private void SendHello(int times)
        {
            var request = Request().WithMethod("GET").WithPath("/hello");

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