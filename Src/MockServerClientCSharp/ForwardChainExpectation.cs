namespace MockServerClientCSharp
{
  using MockServerClientCSharp.Model;

  public class ForwardChainExpectation
  {
    readonly MockServerClient MockServerClient;
    readonly Expectation Expectation;

    public ForwardChainExpectation(MockServerClient mockServerClient, Expectation expectation)
    {
      this.MockServerClient = mockServerClient;
      this.Expectation = expectation;
    }

    public void Respond(HttpResponse httpResponse)
    {
      Expectation.ThenRespond(httpResponse);
      MockServerClient.SendExpectation(this.Expectation);
    }

    public void Forward(HttpForward httpForward) {
      Expectation.ThenForward(httpForward);
      MockServerClient.SendExpectation(this.Expectation);
    }
  }
}
