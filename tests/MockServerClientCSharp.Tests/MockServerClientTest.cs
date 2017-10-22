namespace MockServerClientCSharp.Tests
{
  using System;
  using System.Net;
  using System.Net.Http;
  using MockServerClientCSharp.Model;
  using Xunit;
  using static MockServerClientCSharp.Model.HttpRequest;
  using static MockServerClientCSharp.Model.HttpResponse;

  public class MockServerClientTest: IDisposable
  {
    private string MockServerHost = Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "localhost";
    private int MockServerPort = int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "1080");

    private MockServerClient mockServerClient;

    public MockServerClientTest()
    {
      mockServerClient = new MockServerClient(MockServerHost, MockServerPort);
    }

    public void Dispose()
    {
      mockServerClient.Reset();
    }

    [Fact]
    public void ShouldRespondAccordingToExpectation()
    {
      // arrange
      SetupLoginResponse(unlimited: true);

      // act
      string responseBody = null;
      HttpStatusCode? statusCode = null;

      SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

      // assert
      Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
      Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);
    }

    [Fact]
    public void ShouldRespondAccordingToExpectationOnly2Times()
    {
      // arrange
      SetupLoginResponse(unlimited: false, times: 2);

      // act 1
      string responseBody = null;
      HttpStatusCode? statusCode = null;
      SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

      // assert
      Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
      Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

      // act 2
      responseBody = null;
      statusCode = null;
      SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

      // assert
      Assert.Equal(HttpStatusCode.Unauthorized, statusCode.Value);
      Assert.Equal("{ \"message\": \"incorrect username and password combination\" }", responseBody);

      // act 3
      responseBody = null;
      statusCode = null;
      SendRequest(BuildLoginRequest(), out responseBody, out statusCode);

      // assert
      Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
    }

    [Fact]
    public void ShouldClearExpectation()
    {
      // arrange
      HttpRequest request = Request().WithMethod("GET").WithPath("/hello");

      mockServerClient
        .When(request, Times.Unlimited())
        .Respond(Response().WithStatusCode(200).WithBody("hello").WithDelay(TimeSpan.FromSeconds(0)));

      // act 1
      string responseBody = null;
      HttpStatusCode? statusCode = null;

      SendRequest(BuildHelloRequest(), out responseBody, out statusCode);

      // assert
      Assert.Equal(HttpStatusCode.OK, statusCode.Value);
      Assert.Equal("hello", responseBody);

      // act 2
      mockServerClient.Clear(request);

      SendRequest(BuildHelloRequest(), out responseBody, out statusCode);

      // assert
      Assert.Equal(HttpStatusCode.NotFound, statusCode.Value);
    }

    void SendRequest(HttpRequestMessage request, out string responseBody, out HttpStatusCode? statusCode)
    {
      using (HttpClient client = new HttpClient())
      using (HttpResponseMessage res = client.SendAsync(request).Result)
      using (HttpContent content = res.Content)
      {
        statusCode = res.StatusCode;
        responseBody = content.ReadAsStringAsync().Result;
      }
    }

    void SetupLoginResponse(bool ssl = false, bool unlimited = true, int times = 0)
    {
      mockServerClient
        .When(Request()
              .WithMethod("POST")
              .WithPath("/login")
              .WithQueryStringParameters(
                new Parameter("returnUrl", "/account"))
              .WithBody("{\"username\": \"foo\", \"password\": \"bar\"}")
              .WithSecure(ssl),
              unlimited ? Times.Unlimited() : Times.Exactly(times))
        .Respond(Response()
                 .WithStatusCode(401)
                 .WithHeaders(
                   new Header("Content-Type", "application/json; charset=utf-8"),
                   new Header("Cache-Control", "public, max-age=86400"))
                 .WithBody("{ \"message\": \"incorrect username and password combination\" }")
                 .WithDelay(TimeSpan.FromSeconds(0)));
    }

    HttpRequestMessage BuildLoginRequest(bool ssl = false)
    {
      HttpRequestMessage request = new HttpRequestMessage();
      request.Method = HttpMethod.Post;

      string scheme = ssl ? "https" : "http";
      request.RequestUri = new Uri(scheme + "://" + MockServerHost + ":" + MockServerPort + "/login?returnUrl=/account");
      request.Content = new StringContent("{\"username\": \"foo\", \"password\": \"bar\"}");
      return request;
    }

    HttpRequestMessage BuildHelloRequest()
    {
      HttpRequestMessage request = new HttpRequestMessage();
      request.Method = HttpMethod.Get;
      request.RequestUri = new Uri("http://" + MockServerHost + ":" + MockServerPort + "/hello");
      return request;
    }
  }
}
