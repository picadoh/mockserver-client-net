namespace MockServerClientNet;

using System.Threading.Tasks;
using Model;

public class ForwardChainExpectation(MockServerClient mockServerClient, Expectation expectation)
{
    public void Respond(HttpResponse httpResponse)
    {
        RespondAsync(httpResponse).GetAwaiter().GetResult();
    }

    public void Respond(HttpTemplate httpTemplate)
    {
        RespondAsync(httpTemplate).GetAwaiter().GetResult();
    }

    public async Task RespondAsync(HttpResponse httpResponse)
    {
        expectation.ThenRespond(httpResponse);
        await mockServerClient.SendExpectationAsync(expectation);
    }

    public async Task RespondAsync(HttpTemplate httpTemplate)
    {
        expectation.ThenRespond(httpTemplate);
        await mockServerClient.SendExpectationAsync(expectation);
    }

    public void Forward(HttpForward httpForward)
    {
        ForwardAsync(httpForward).GetAwaiter().GetResult();
    }

    public void Forward(HttpTemplate httpTemplate)
    {
        ForwardAsync(httpTemplate).GetAwaiter().GetResult();
    }

    public async Task ForwardAsync(HttpForward httpForward)
    {
        expectation.ThenForward(httpForward);
        await mockServerClient.SendExpectationAsync(expectation);
    }

    public async Task ForwardAsync(HttpTemplate httpTemplate)
    {
        expectation.ThenForward(httpTemplate);
        await mockServerClient.SendExpectationAsync(expectation);
    }
}
