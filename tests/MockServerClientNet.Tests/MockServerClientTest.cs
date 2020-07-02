using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet.Extensions;
using MockServerClientNet.Model;
using Xunit;

namespace MockServerClientNet.Tests
{
    [Collection("Sequential")]
    public class MockServerClientTest : IDisposable
    {
        private readonly string _mockServerHost =
            Environment.GetEnvironmentVariable("MOCKSERVER_TEST_HOST") ?? "localhost";

        private readonly int _mockServerPort =
            int.Parse(Environment.GetEnvironmentVariable("MOCKSERVER_TEST_PORT") ?? "1080");

        protected readonly MockServerClient MockServerClient;

        protected string HostHeader => $"{_mockServerHost}:{_mockServerPort}";

        protected MockServerClientTest(HttpScheme scheme = HttpScheme.Http, HttpClientHandler handler = null)
        {
            MockServerClient = new MockServerClient(_mockServerHost, _mockServerPort,
                httpScheme: scheme, httpHandler: handler);
            Assert.True(MockServerClient.IsRunning(), "Server is not running");
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
            using (var client = new HttpClient())
            {
                return await client.SendAsync(request);
            }
        }

        protected HttpRequestMessage BuildRequest(HttpMethod method, string path, string body)
        {
            return new HttpRequestMessage()
                .WithMethod(method)
                .WithUri(MockServerClient.ServerAddress(path))
                .WithBody(body);
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