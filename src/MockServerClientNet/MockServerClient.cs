namespace MockServerClientNet
{
  using System.Net;
  using System.Net.Http;
  using System.Text;
  using MockServerClientNet.Extensions;
  using MockServerClientNet.Model;

  public class MockServerClient : AbstractClient<MockServerClient>
  {
    public MockServerClient(string host, int port, string contextPath = ""): base(host, port, contextPath)
    {
    }

    public ForwardChainExpectation When(HttpRequest httpRequest, Times times)
    {
      return new ForwardChainExpectation(this, new Expectation(httpRequest, times, TimeToLive.Unlimited()));
    }

    public ForwardChainExpectation When(HttpRequest httpRequest, Times times, TimeToLive timeToLive)
    {
      return new ForwardChainExpectation(this, new Expectation(httpRequest, times, timeToLive));
    }

    public void SendExpectation(Expectation expectation)
    {
      var expectationBody = expectation != null ? ExpectationSerializer.Serialize(expectation) : "";

      using (HttpResponseMessage httpResponse = SendRequest(
        new HttpRequestMessage()
        .WithMethod(HttpMethod.Put)
        .WithPath(CalculatePath("expectation"))
        .WithBody(expectationBody)))
      {
        if (httpResponse != null && httpResponse.StatusCode != HttpStatusCode.Created)
        {
          throw new ClientException($"\n\nerror: {httpResponse}\n{expectationBody}\n");
        }
      }
    }

    public HttpRequest[] RetrieveRecordedRequests(HttpRequest httpRequest)
    {
      var res = SendRequest(new HttpRequestMessage()
                            .WithMethod("PUT")
                            .WithPath(CalculatePath("retrieve"))
                            .WithBody(httpRequest != null ? HttpRequestSerializer.Serialize(httpRequest) : string.Empty, Encoding.UTF8));

      var body = res?.Content.ReadAsStringAsync().Result;

      if (!string.IsNullOrEmpty(body))
      {
        return HttpRequestSerializer.DeserializeArray(body);
      }

      return new HttpRequest[0];
    }
  }
}