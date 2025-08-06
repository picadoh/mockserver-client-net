using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet.Extensions;
using MockServerClientNet.Model;
using Xunit;

namespace MockServerClientNet.Tests
{
    [Collection(nameof(MockServerCollection))]
    public class MockServerClientTest : IClassFixture<MockServerFixture>, IDisposable
    {
        protected readonly MockServerClient MockServerClient;
        protected readonly string HostHeader;

        protected MockServerClientTest(
            MockServerFixture fixture,
            HttpScheme scheme = HttpScheme.Http,
            HttpClientHandler handler = null
        )
        {
            MockServerClient = new(
                host: fixture.Host,
                port: fixture.Port,
                httpScheme: scheme,
                httpHandler: handler
            );
            HostHeader = fixture.HostHeader;
        }

        protected MockServerClientTest(
            MockServerFixture fixture,
            HttpClient httpClient,
            HttpScheme scheme = HttpScheme.Http
        )
        {
            MockServerClient = new(
                host: fixture.Host,
                port: fixture.Port,
                httpScheme: scheme,
                httpClient: httpClient
            );
            HostHeader = fixture.HostHeader;
        }

        public void Dispose()
        {
            Assert.NotNull(MockServerClient.Reset());
        }

        protected static void SendRequest(HttpRequestMessage request, out string responseBody,
            out HttpStatusCode? statusCode)
        {
            var response = SendRequestAsync(request).AwaitResult();
            statusCode = response.StatusCode;
            responseBody = response.Content.ReadAsStringAsync().AwaitResult();
        }

        protected static async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            using var client = new HttpClient();
            return await client.SendAsync(request);
        }

        protected HttpRequestMessage BuildRequest(HttpMethod method, string path, string body)
        {
            return new HttpRequestMessage()
                .WithMethod(method)
                .WithUri(MockServerClient.ServerAddress(path))
                .WithBody(body);
        }
        
        protected HttpRequestMessage BuildRequest(HttpMethod method, string path, HttpContent content)
        {
            return new HttpRequestMessage()
                .WithMethod(method)
                .WithUri(MockServerClient.ServerAddress(path))
                .WithBody(content);
        }

        protected HttpRequestMessage BuildRequest(HttpMethod method, string path, byte[] bytes)
        {
            var content = new ByteArrayContent(bytes);

            return new HttpRequestMessage()
                .WithMethod(method)
                .WithUri(MockServerClient.ServerAddress(path))
                .WithBody(content);
        }

        protected HttpRequestMessage BuildGetRequest(string path)
        {
            return BuildRequest(HttpMethod.Get, path, string.Empty);
        }
    }
}
