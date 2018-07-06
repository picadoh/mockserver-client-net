namespace MockServerClientNet
{
  using MockServerClientNet.Model;

  public class ForwardChainExpectation
  {
    private readonly MockServerClient mockServerClient;
    private readonly Expectation expectation;

    public ForwardChainExpectation(MockServerClient mockServerClient, Expectation expectation)
    {
      this.mockServerClient = mockServerClient;
      this.expectation = expectation;
    }

    public void Respond(HttpResponse httpResponse)
    {
      this.expectation.ThenRespond(httpResponse);
      this.mockServerClient.SendExpectation(this.expectation);
    }

    public void Forward(HttpForward httpForward)
    {
      this.expectation.ThenForward(httpForward);
      this.mockServerClient.SendExpectation(this.expectation);
    }
  }
}
