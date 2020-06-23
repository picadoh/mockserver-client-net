using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MockServerClientNet.Extensions;
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

        protected MockServerClientTest()
        {
            MockServerClient = new MockServerClient(_mockServerHost, _mockServerPort);
        }

        public void Dispose()
        {
            MockServerClient.Reset();
        }

        protected static void SendRequest(HttpRequestMessage request, out string responseBody,
            out HttpStatusCode? statusCode)
        {
            (responseBody, statusCode) = SendRequestAsync(request).AwaitResult();
        }

        protected static async Task<Tuple<string, HttpStatusCode>> SendRequestAsync(HttpRequestMessage request)
        {
            using (var client = new HttpClient())
            using (var res = await client.SendAsync(request))
            using (var content = res.Content)
            {
                var statusCode = res.StatusCode;
                var responseBody = await content.ReadAsStringAsync();
                return new Tuple<string, HttpStatusCode>(responseBody, statusCode);
            }
        }

        protected HttpRequestMessage BuildRequest(HttpMethod method, string path, string body)
        {
            return new HttpRequestMessage()
                .WithMethod(method)
                .WithUri(MockServerClient.ServerAddressWithPath(path))
                .WithBody(body);
        }

        protected HttpRequestMessage BuildGetRequest(string path)
        {
            return BuildRequest(HttpMethod.Get, path, string.Empty);
        }
    }
}