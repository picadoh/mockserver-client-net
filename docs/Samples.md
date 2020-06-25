### Adding an expectation

    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using MockServerClientNet;
    using MockServerClientNet.Model;
    using static MockServerClientNet.Model.HttpRequest;
    using static MockServerClientNet.Model.HttpResponse;

    public class Program
    {
        public static async Task Main()
        {
            var mockServerClient = new MockServerClient("127.0.0.1", 1080);

            // set expectation
            await mockServerClient.When(Request()
                        .WithMethod("POST")
                        .WithPath("/customers")
                        .WithQueryStringParameters(
                            new Parameter("param", "value")
                        )
                        .WithBody("{\"name\": \"foo\"}"),
                    Times.Unlimited())
                .RespondAsync(Response()
                    .WithStatusCode(201)
                    .WithHeaders(
                        new Header("Content-Type", "application/json; charset=utf-8"))
                    .WithBody("{ \"message\": \"example response message\" }")
                    .WithDelay(TimeSpan.FromSeconds(0))
                );

            // send request
            await new HttpClient().PostAsync(
                "http://localhost:1080/customers?param=value",
                new StringContent("{\"name\": \"foo\"}"));

        }
    }

### Verifying Requests

    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using MockServerClientNet;
    using MockServerClientNet.Model;
    using MockServerClientNet.Verify;
    using static MockServerClientNet.Model.HttpRequest;
    using static MockServerClientNet.Model.HttpResponse;

    public class Program
    {
        public static async Task Main()
        {
            var mockServerClient = new MockServerClient("127.0.0.1", 1080);

            // set expectation
            await mockServerClient.When(Request()
                        .WithPath("/hello.*"),
                    Times.Unlimited())
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody("{ \"message\": \"hello,world!\" }")
                    .WithDelay(TimeSpan.FromSeconds(0))
                );

            // send request
            var httpClient = new HttpClient();
            Task req1 = httpClient.GetAsync("http://localhost:1080/hello_foo");
            Task req2 = httpClient.GetAsync("http://localhost:1080/hello_bar");
            await Task.WhenAll(req1, req2);

            // verify request
            await mockServerClient.VerifyAsync(
                Request().WithPath("/hello.*"),
                VerificationTimes.AtLeast(2));
        }
    }

### Forwarding Requests

    using System.Net.Http;
    using System.Threading.Tasks;
    using MockServerClientNet;
    using MockServerClientNet.Model;
    using MockServerClientNet.Verify;
    using static MockServerClientNet.Model.HttpRequest;
    using static MockServerClientNet.Model.HttpForward;

    public class Program
    {
        public static async Task Main()
        {
            var mockServerClient = new MockServerClient("127.0.0.1", 1080);

            await mockServerClient.ResetAsync();

            // set forward
            await mockServerClient.When(Request()
                        .WithPath("/hello"),
                    Times.Exactly(1))
                .ForwardAsync(Forward()
                    .WithHost("127.0.0.1")
                    .WithPort(1080));

            // send 1 request
            var httpClient = new HttpClient();
            await httpClient.GetAsync("http://localhost:1080/hello");

            // verify 2 requests
            await mockServerClient.VerifyAsync(
                Request().WithPath("/hello"),
                VerificationTimes.Exactly(2));
        }
    }