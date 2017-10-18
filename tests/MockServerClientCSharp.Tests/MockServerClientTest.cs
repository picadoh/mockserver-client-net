namespace MockServerClientCSharp.Tests
{
  using System;
  using System.Net;
  using System.Net.Http;
  using MockServerClientCSharp.Model;
  using Xunit;
  using static MockServerClientCSharp.Model.HttpRequest;
  using static MockServerClientCSharp.Model.HttpResponse;

  public class MockServerClientTest
  {
    private const string MockServerHost = "mockserver";
    private const int MockServerPort = 1080;

    [Fact]
    public void ShouldRespondAccordingToExpectation()
    {
      // arrange
      SetupResponseExpectation(unlimited: true);

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
      SetupResponseExpectation(unlimited: false, times: 2);

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

    void SetupResponseExpectation(bool ssl = false, bool unlimited = true, int times = 0)
    {
      new MockServerClient(MockServerHost, MockServerPort)
        .Reset()
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
  }
}
