namespace MockServerClientNet.Tests;

using System;
using Model;
using Xunit;
using static Model.HttpRequest;
using static Model.HttpTemplate;

public class TemplateForwardTest(MockServerFixture fixture) : MockServerClientTest(fixture: fixture)
{
    [Fact]
    public void ShouldForwardRequestWithResolvedVelocityTemplate()
    {
        // arrange
        var host = MockServerClient.ServerAddress().Host;
        var port = MockServerClient.ServerAddress().Port;

        var request = Request().WithMethod("GET").WithPath($"/hello");

        MockServerClient
            .When(request, Times.Exactly(1))
            .Forward(Template(TemplateType.Velocity).WithTemplate(
                    """
                    {
                      "headers": {
                          "Host": [ "$request.queryStringParameters.fw[0]:1080" ]
                      },
                      "method" : "GET",
                      "path": "/hello",
                    }
                    """)
                .WithDelay(TimeSpan.FromSeconds(0))
            );

        // act
        SendRequest(BuildGetRequest($"/hello?fw={host}&fw={port}"), out _, out _);

        var result = MockServerClient.RetrieveRecordedRequests(request);

        // assert
        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void ShouldForwardRequestWithResolvedMustacheTemplate()
    {
        // arrange
        var host = MockServerClient.ServerAddress().Host;
        var port = MockServerClient.ServerAddress().Port;

        var request = Request().WithMethod("GET").WithPath($"/hello");

        MockServerClient
            .When(request, Times.Exactly(1))
            .Forward(Template(TemplateType.Mustache).WithTemplate(
                    """
                    {
                      "headers": {
                          "Host": [ "{{ request.queryStringParameters.fw.0 }}:1080" ]
                      },
                      "method" : "GET",
                      "path": "/hello",
                    }
                    """)
                .WithDelay(TimeSpan.FromSeconds(0))
            );

        // act
        SendRequest(BuildGetRequest($"/hello?fw={host}&fw={port}"), out _, out _);

        var result = MockServerClient.RetrieveRecordedRequests(request);

        // assert
        Assert.Equal(2, result.Length);
    }
}
