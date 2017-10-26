namespace MockServerClientCSharp
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using MockServerClientCSharp.Extensions;
  using MockServerClientCSharp.Model;
  using MockServerClientCSharp.Serializer;
  using MockServerClientCSharp.Verify;

  public abstract class AbstractClient<T>: IDisposable where T : AbstractClient<T>
  {
    protected readonly JsonSerializer<Expectation> ExpectationSerializer = new JsonSerializer<Expectation>();
    protected readonly JsonSerializer<HttpRequest> HttpRequestSerializer = new JsonSerializer<HttpRequest>();
    protected readonly JsonSerializer<VerificationSequence> VerificationSequenceSerializer = new JsonSerializer<VerificationSequence>();
    protected readonly JsonSerializer<Verification> VerificationSerializer = new JsonSerializer<Verification>();

    protected HttpClient httpClient = new HttpClient();

    protected readonly string Host;
    protected readonly int Port;
    protected readonly string ContextPath;

    protected AbstractClient(string host, int port, string contextPath = "")
    {
      this.Host = host;
      this.Port = port;
      this.ContextPath = contextPath;
    }

    public T Reset()
    {
      SendRequest(new HttpRequestMessage().WithMethod("PUT").WithPath(CalculatePath("reset")));
      return (T)this;
    }

    public T Clear(HttpRequest httpRequest)
    {
      SendRequest(new HttpRequestMessage()
                  .WithMethod("PUT")
                  .WithPath(CalculatePath("clear"))
                  .WithBody(httpRequest != null ? HttpRequestSerializer.Serialize(httpRequest) : ""));
      return (T)this;
    }

    public T Verify(params HttpRequest[] httpRequests)
    {
      if (httpRequests == null || httpRequests.Length == 0 || httpRequests[0] == null)
      {
        throw new ArgumentException("Required: Non-Null and Non-Empty array of requests");
      }

      var sequence = new VerificationSequence().WithRequests(httpRequests);
      var res = SendRequest(new HttpRequestMessage()
                            .WithMethod("PUT")
                            .WithPath(CalculatePath("verifySequence"))
                            .WithBody(VerificationSequenceSerializer.Serialize(sequence), Encoding.UTF8));

      var body = res?.Content.ReadAsStringAsync().Result;

      if (!string.IsNullOrEmpty(body))
      {
        throw new AssertionException(body);
      }

      return (T)this;
    }

    public T Verify(HttpRequest httpRequest, VerificationTimes times)
    {
      if (httpRequest == null)
      {
        throw new ArgumentException("Required: Non-Null request");
      }

      if (times == null)
      {
        throw new ArgumentException("Required: Non-Null verification times");
      }

      var verification = new Verification().WithRequest(httpRequest).WithTimes(times);

      var res = SendRequest(new HttpRequestMessage()
                            .WithMethod("PUT")
                            .WithPath(CalculatePath("verify"))
                            .WithBody(VerificationSerializer.Serialize(verification), Encoding.UTF8));

      var body = res?.Content.ReadAsStringAsync().Result;

      if (!string.IsNullOrEmpty(body))
      {
        throw new AssertionException(body);
      }

      return (T)this;
    }

    public T VerifyZeroInteractions()
    {
      var verification = new Verification().WithRequest(new HttpRequest()).WithTimes(VerificationTimes.Exactly(0));

      var res = SendRequest(new HttpRequestMessage()
                            .WithMethod("PUT")
                            .WithPath(CalculatePath("verify"))
                            .WithBody(VerificationSerializer.Serialize(verification), Encoding.UTF8));

      var body = res?.Content.ReadAsStringAsync().Result;

      if (!string.IsNullOrEmpty(body))
      {
        throw new AssertionException(body);
      }

      return (T)this;
    }

    public void Dispose()
    {
      Stop();
    }

    public T Stop()
    {
      return Stop(false);
    }

    public T Stop(bool ignoreFailure)
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

      return (T)this;
    }

    public bool IsRunning()
    {
      return IsRunning(10, 500);
    }

    public bool IsRunning(int attempts, int timeoutMillis)
    {
      try
      {
        while (attempts-- > 0)
        {
          HttpResponseMessage httpResponse = SendRequest(new HttpRequestMessage().WithMethod("PUT").WithPath(CalculatePath("status")));

          if (httpResponse.StatusCode == HttpStatusCode.OK)
          {
            return true;
          }

          Thread.Sleep(timeoutMillis);
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    public HttpResponseMessage SendRequest(HttpRequestMessage mockServerRequest)
    {
      return SendRequestAsync(mockServerRequest).Result;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequest)
    {
      var response = await httpClient.SendAsync(
        httpRequest.WithHeader(HttpRequestHeader.Host.ToString(), $"{this.Host}:{this.Port}"));

      if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
      {
        throw new ArgumentException(await response.Content.ReadAsStringAsync());
      }

      return response;
    }

    protected string CalculatePath(string path)
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
