namespace MockServerClientNet.Tests;

using System;
using Model;
using Xunit;
using Xunit.Abstractions;
using static Model.HttpRequest;
using static Model.HttpForward;

public class ForwardTest(MockServerFixture fixture, ITestOutputHelper testOutputHelper)
    : MockServerClientTest(fixture: fixture)
{
    [Fact]
    public void ShouldForwardRequestWithStringScheme()
    {
        // arrange
        var request = Request().WithMethod("GET").WithPath("/hello");

        var host = MockServerClient.ServerAddress().Host;

        MockServerClient
            .When(request, Times.Exactly(1))
            .Forward(Forward()
                .WithScheme("HTTP")
                .WithHost(host)
                .WithPort(1080));

        // act
        SendRequest(BuildGetRequest("/hello"), out _, out _);

        var result = MockServerClient.RetrieveRecordedRequests(request);

        testOutputHelper.WriteLine(result.ToString());

        // assert
        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void ShouldForwardRequestWithDelay()
    {
        // arrange
        var request = Request().WithMethod("GET").WithPath("/hello");

        var host = MockServerClient.ServerAddress().Host;

        MockServerClient
            .When(request, Times.Exactly(1))
            .Forward(Forward()
                .WithScheme("HTTP")
                .WithHost(host)
                .WithPort(1080)
                .WithDelay(TimeSpan.FromMilliseconds(100)));

        // act
        SendRequest(BuildGetRequest("/hello"), out _, out _);

        var result = MockServerClient.RetrieveRecordedRequests(request);

        testOutputHelper.WriteLine(result.ToString());

        // assert
        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void ShouldForwardRequestUsingHttpSchemeEnum()
    {
        // arrange
        var request = Request().WithMethod("GET").WithPath("/hello");

        var host = MockServerClient.ServerAddress().Host;

        MockServerClient
            .When(request, Times.Exactly(1))
            .Forward(Forward()
                .WithScheme(HttpScheme.Https)
                .WithHost(host)
                .WithPort(1080));

        // act
        SendRequest(BuildGetRequest("/hello"), out _, out _);

        var result = MockServerClient.RetrieveRecordedRequests(request);

        // assert
        Assert.Equal(2, result.Length);
    }
}
