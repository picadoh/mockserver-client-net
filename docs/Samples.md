### Adding an Expectation

Creating expectations allows to setup what Mock-Server will return when a specific request is made. Fluently, this is done in the form "When **This** Respond **That**".

> :information_source: Refer to the official Mock-Server documentation about [Expectations](https://www.mock-server.com/mock_server/creating_expectations.html) for more details. Bear in mind that this is not an official client and does not support all the client-side features of Mock-Server.

<details>
<summary>Expand/Collapse Code Sample</summary>

```c#
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
```

</details>

### Verifying Requests

Verifying requests allows to determine whether a request was made to the server or not, as well as how many times that happened. Fluently, this is done in the form "Verify **This** occured exactly/at least/at most **N** times or between **M** and **N** times".

> :information_source: Refer to the official Mock-Server documentation about [Verification](https://www.mock-server.com/mock_server/verification.html) for more details Bear in mind that this is not an official client and does not support all the client-side features of Mock-Server.

<details>
<summary>Expand/Collapse Code Sample</summary>

```c#
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
```

</details>

### Forwarding Requests

Forwarding is the ability to tell Mock-Server to forward requests to another server when they arrive at Mock-Server. Fluently, this is done in the form "When **This** Forward **There**".

> :information_source: Refer to the official Mock-Server documentation about [Forward Actions](https://www.mock-server.com/mock_server/getting_started.html#forward_action) for more details. Bear in mind that this is not an official client and does not support all the client-side features of Mock-Server.

<details>
<summary>Expand/Collapse Code Sample</summary>

```c#
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
```

</details>

### Using HTTPs (One-way/Two-way TLS)

Two-way TLS has the following requirements, not needed for One-way TLS:

- `mockserver.tlsMutualAuthenticationRequired` must be set to `true` on the server configuration.
- A client certificate must be provided to the server upon TLS handshake, signed with the CA certificate available at the [Mock-Server repository](https://github.com/mock-server/mockserver/blob/master/mockserver-core/src/main/resources/org/mockserver/socket/CertificateAuthorityCertificate.pem).

It's also possible to configure Mock-Server to generate new CA certificate and private key for added security, to avoid using the publicly available ones.

> :information_source: Refer to the official Mock-Server documentation about [Configuration](https://www.mock-server.com/mock_server/configuration_properties.html) and [HTTPS & TLS](https://www.mock-server.com/mock_server/HTTPS_TLS.html) for more details. Bear in mind that this is not an official client and does not support all the client-side features of Mock-Server.

<details>
<summary>Expand/Collapse Code Sample</summary>

```c#
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
```
</details>

### Request Body Matchers

Instead of matching a request body with a simple string, more advanced body matcher constructs can be used:

- Matchers.MatchingEmptyString
- Matchers.MatchingExactString
- Matchers.MatchingSubString
- Matchers.MatchingBinary
- Matchers.MatchingXml
- Matchers.MatchingJson
- Matchers.MatchingStrictJson
- Matchers.MatchingXPath
- Matchers.MatchingJsonPath
- Matchers.MatchingXmlSchema
- Matchers.MatchingJsonSchema
- Matchers.MatchingRegex

> :information_source: Refer to the official Mock-Server documentation about [Request Matchers](https://www.mock-server.com/mock_server/creating_expectations.html#request_matchers) for more details. Bear in mind that this is not an official client and does not support all the client-side features of Mock-Server.

<details>
<summary>Expand/Collapse Code Sample</summary>

```c#
using System;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet;
using MockServerClientNet.Model;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;
using static MockServerClientNet.Model.Body.Matchers;

public class Program
{
    public static async Task Main()
    {
        await new MockServerClient("127.0.0.1", 1080)
            .When(Request()
                    .WithMethod(HttpMethod.Post)
                    .WithBody(MatchingJsonPath("$.people[?(@.age > 35)]"))
                    .WithPath("/"),
                Times.Unlimited())
            .RespondAsync(Response()
                .WithStatusCode(200)
                .WithBody("response")
                .WithDelay(TimeSpan.FromSeconds(0)));

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.PostAsync("http://localhost:1080",
                new StringContent(@"
                    { ""people"": [
                            { ""name"": ""John"", ""age"": 40 },
                            { ""name"": ""Jane"", ""age"": 30 }
                        ]
                    }"));
            // ...
        }
    }
}
```

</details>

### Response Body Contents

Instead of expecting a response body containing a simple string, more advanced body content constructs can be used:

- Contents.Text
- Contents.Binary
- Contents.Xml
- Contents.Json

> :information_source: Refer to the official Mock-Server documentation about [Response Actions](https://www.mock-server.com/mock_server/creating_expectations.html#response_action) for more details. Bear in mind that this is not an official client and does not support all the client-side features of Mock-Server.

<details>
<summary>Expand/Collapse Code Sample</summary>

```c#
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet;
using MockServerClientNet.Model;
using static MockServerClientNet.Model.HttpRequest;
using static MockServerClientNet.Model.HttpResponse;
using static MockServerClientNet.Model.Body.Contents;

public class Program
{
    public static async Task Main()
    {
        using (var fileStream = File.OpenRead("/path/to/image.jpg"))
        {
            await new MockServerClient("127.0.0.1", 1080)
                .When(Request().WithPath("/"), Times.Unlimited())
                .RespondAsync(Response()
                    .WithStatusCode(200)
                    .WithHeaders(
                        new Header("Content-Disposition", "inline"),
                        new Header("Content-Type", "image/jpg"))
                    .WithBody(Binary(fileStream))
                    .WithDelay(TimeSpan.FromSeconds(0)));
        }

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync("http://localhost:1080");
            // ...
        }
    }
}
```

</details>
