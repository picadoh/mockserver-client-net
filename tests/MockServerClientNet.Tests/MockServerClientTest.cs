using System;
using System.Net;
using System.Net.Http;
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
            using (var client = new HttpClient())
            using (var res = client.SendAsync(request).Result)
            using (var content = res.Content)
            {
                statusCode = res.StatusCode;
                responseBody = content.ReadAsStringAsync().Result;
            }
        }

        protected HttpRequestMessage BuildRequest(HttpMethod method, string path, string body = "", bool ssl = false)
        {
            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"{GetMockServerBaseUri(ssl)}/{path}"),
                Content = new StringContent(body)
            };
        }

        protected HttpRequestMessage BuildGetRequest(string path)
        {
            return BuildRequest(HttpMethod.Get, path);
        }

        private string GetMockServerBaseUri(bool ssl)
        {
            var scheme = ssl ? "https" : "http";
            return scheme + "://" + _mockServerHost + ":" + _mockServerPort;
        }
    }
}