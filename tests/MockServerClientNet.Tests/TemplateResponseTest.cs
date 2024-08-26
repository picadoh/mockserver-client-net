namespace MockServerClientNet.Tests;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Extensions;
using Model;
using Xunit;
using static Model.HttpRequest;
using static Model.HttpTemplate;

public class TemplateResponseTest(MockServerFixture fixture) : MockServerClientTest(fixture: fixture)
{
    [Fact]
    public async Task ShouldRespondWithResolvedVelocityTemplate()
    {
        // arrange
        await SetupPostExpectation(
            TemplateType.Velocity,
            """
            { 'statusCode': 201, body: "{ \"reason\": \"$request.queryStringParameters.param[0]\" }" }
            """);

        // act
        var response = await SendRequestAsync(BuildPostRequest("test value"));

        // assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{ \"reason\": \"test value\" }", response.Content.ReadAsStringAsync().AwaitResult());
    }

    [Fact]
    public async Task ShouldRespondWithResolvedMustacheTemplate()
    {
        // arrange
        await SetupPostExpectation(
            TemplateType.Mustache,
            """
            { 'statusCode': 201, body: "{ \"reason\": \"{{ request.queryStringParameters.param.0 }}\" }" }
            """);

        // act
        var response = await SendRequestAsync(BuildPostRequest("test value"));

        // assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{ \"reason\": \"test value\" }", response.Content.ReadAsStringAsync().AwaitResult());
    }

    private async Task SetupPostExpectation(
        TemplateType templateType,
        string templateValue,
        bool unlimited = true,
        int times = 0
    )
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
                    .WithBody(body),
                unlimited ? Times.Unlimited() : Times.Exactly(times))
            .RespondAsync(Template(templateType).WithTemplate(templateValue));
    }

    private HttpRequestMessage BuildPostRequest(string param)
    {
        return BuildRequest(HttpMethod.Post, $"/customers?param={param}", "{\"name\": \"foo\"}");
    }
}
