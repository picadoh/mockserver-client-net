### Adding an expectation

    using System;
    using MockServerClientCSharp;
    using MockServerClientCSharp.Model;
    using static MockServerClientCSharp.Model.HttpRequest;
    using static MockServerClientCSharp.Model.HttpResponse;

    public class Program 
    {
        public static void Main()
        {
          new MockServerClient("127.0.0.1", 1080)
            .When(Request()
                .WithMethod("POST")
                .WithPath("/login")
                .WithQueryStringParameters(
                  new Parameter("returnUrl", "/account")
                )
                .WithBody("{\"username\": \"foo\", \"password\": \"bar\"}"),
                Times.Unlimited())
            .Respond(Response()
                 .WithStatusCode(401)
                 .WithHeaders(
                   new Header("Content-Type", "application/json; charset=utf-8"),
                   new Header("Cache-Control", "public, max-age=86400"))
                 .WithBody("{ \"message\": \"incorrect username and password combination\" }")
                 .WithDelay(TimeSpan.FromSeconds(1))
            );
        }
    }
