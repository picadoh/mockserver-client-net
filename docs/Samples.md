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
                        .WithMethod(HttpMethod.Post)
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

### Using HTTPs (One-way/Two-way TLS)

**Requirements**

Two-way TLS has the following requirements, not needed for One-way TLS:

- `mockserver.tlsMutualAuthenticationRequired` must be set to `true` on the server configuration.
- A client certificate must be provided to the server upon TLS handshake, signed with the CA certificate available at the [Mock-Server repository](https://github.com/mock-server/mockserver/blob/master/mockserver-core/src/main/resources/org/mockserver/socket/CertificateAuthorityCertificate.pem).

It's also possible to configure Mock-Server to generate new CA certificate and private key for added security, to avoid using the publicly available ones. Refer to the official Mock-Server documentation about [Configuration](https://www.mock-server.com/mock_server/configuration_properties.html) and [HTTPS & TLS](https://www.mock-server.com/mock_server/HTTPS_TLS.html) for more details.

**Code Sample**

    using System;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using MockServerClientNet;
    using MockServerClientNet.Model;
    using static MockServerClientNet.Model.HttpRequest;
    using static MockServerClientNet.Model.HttpResponse;

    public class Program
    {
        public static async Task Main()
        {
            // Custom HTTP Client Handler
            var handler = new HttpClientHandler
            {
                // Client Certificate (Only required for Two-way TLS)
                ClientCertificates = { GetClientCertificate() },

                // Server Certificate Validation
                ServerCertificateCustomValidationCallback = ValidateServerCertificate
            };

            // Create Mock-Server client with HTTPs scheme and custom handler
            var mockServerClient = new MockServerClient("127.0.0.1", 1080,
                httpScheme: HttpScheme.Https, httpHandler: handler);

            // Setup expectation
            await mockServerClient
                .When(Request()
                        .WithSecure(true)
                        .WithPath("/"),
                    Times.Unlimited())
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithBody("hello")
                    .WithDelay(TimeSpan.FromSeconds(0)));

            // Send request
            using var client = new HttpClient(handler);
            var response = await client.GetAsync("https://localhost:1080/");
            await response.Content.ReadAsStringAsync();
            // ...
        }

        private static X509Certificate2 GetClientCertificate()
        {
            // get/generate and return valid client certificate
        }

        private static bool ValidateServerCertificate(
            HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors spe)
        {
            // validate the server certificate and return true if valid, false otherwise
        }
    }
