namespace MockServerClientCSharp
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using MockServerClientCSharp.Extensions;
  using MockServerClientCSharp.Model;
  using MockServerClientCSharp.Serializer;

  public class MockServerClient : IDisposable
  {
    readonly ExpectationSerializer ExpectationSerializer = new ExpectationSerializer();
    readonly string Host;
    readonly int Port;
    readonly string ContextPath;

    public MockServerClient(string host, int port, string contextPath = "")
    {
      this.Host = host;
      this.Port = port;
      this.ContextPath = contextPath;
    }

    public ForwardChainExpectation When(HttpRequest httpRequest, Times times)
    {
      return new ForwardChainExpectation(this, new Expectation(httpRequest, times, TimeToLive.Unlimited()));
    }

    public ForwardChainExpectation When(HttpRequest httpRequest, Times times, TimeToLive timeToLive)
    {
      return new ForwardChainExpectation(this, new Expectation(httpRequest, times, timeToLive));
    }

    public MockServerClient Reset()
    {
      SendRequest(new HttpRequestMessage().WithMethod("PUT").WithPath(CalculatePath("reset")));
      return this;
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

    public HttpResponseMessage SendRequest(HttpRequestMessage mockServerRequest)
    {
      return SendRequestAsync(mockServerRequest).Result;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequest)
    {
      using (HttpClient client = new HttpClient())
      {
        using (HttpResponseMessage res = await client.SendAsync(
          httpRequest.WithHeader(HttpRequestHeader.Host.ToString(), $"{this.Host}:{this.Port}")))
        {
          if (res != null && res.StatusCode == HttpStatusCode.BadRequest)
          {
            using (HttpContent content = res.Content)
            {
              throw new ArgumentException(await content.ReadAsStringAsync());
            }
          }

          return res;
        }
      }
    }

    public void Dispose()
    {
      Stop();
    }

    public MockServerClient Stop()
    {
      return Stop(false);
    }

    public MockServerClient Stop(bool ignoreFailure)
    {
      try
      {
        SendRequest(new HttpRequestMessage().WithMethod("PUT").WithPath(CalculatePath("stop")));

        int attemps = 0;
        while (IsRunning() && attemps++ < 50)
        {
          Thread.Sleep(5000);
        }
      }
      catch (Exception e)
      {
        if (!ignoreFailure)
        {
          throw new ClientException("Failed to send stop request to MockServer", e);
        }
      }

      return this;
    }

    public bool IsRunning()
    {
      return IsRunning(10, 500);
    }

    public bool IsRunning(int attempts, int timeoutMillis)
    {
      try
      {
        while (attempts-- > 0) {
          HttpResponseMessage httpResponse = SendRequest(new HttpRequestMessage().WithMethod("PUT").WithPath(CalculatePath("status")));

          if (httpResponse.StatusCode == HttpStatusCode.OK)
          {
            return true;
          }

          Thread.Sleep(timeoutMillis);
        }

        return false;
      }
      catch (Exception e)
      {
        return false;
      }
    }

    string CalculatePath(string path)
    {
      var cleanedPath = path;

      if (String.IsNullOrEmpty(this.ContextPath))
      {
        cleanedPath = this.ContextPath.PrefixWith("/").SuffixWith("/") + cleanedPath.RemovePrefix("/");
      }

      return $"{this.Host}:{this.Port}{cleanedPath.PrefixWith("/")}";
    }
  }
}