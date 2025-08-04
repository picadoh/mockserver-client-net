using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MockServerClientNet.Extensions;
using MockServerClientNet.Model;
using MockServerClientNet.Serializer;
using MockServerClientNet.Verify;

namespace MockServerClientNet
{
    public class MockServerClient : IDisposable
    {
        private const string MockServerBasePath = "/mockserver";

        private const string ResetEndpoint = "/reset";
        private const string StopEndpoint = "/stop";
        private const string StatusEndpoint = "/status";
        private const string ClearEndpoint = "/clear";
        private const string VerifyEndpoint = "/verify";
        private const string VerifySequenceEndpoint = "/verifySequence";
        private const string ExpectationEndpoint = "/expectation";
        private const string RetrieveEndpoint = "/retrieve";

        private const int DefaultPriority = 0;

        private readonly JsonSerializer<Expectation> _expectationSerializer = new JsonSerializer<Expectation>();
        private readonly JsonSerializer<HttpRequest> _httpRequestSerializer = new JsonSerializer<HttpRequest>();

        private readonly JsonSerializer<VerificationSequence> _verificationSequenceSerializer =
            new JsonSerializer<VerificationSequence>();

        private readonly JsonSerializer<Verification> _verificationSerializer = new JsonSerializer<Verification>();

        private readonly string _host;
        private readonly int _port;
        private readonly string _contextPath;
        private readonly HttpScheme _httpScheme;
        private readonly HttpClient _httpClient;

        public MockServerClient(string host, int port, string contextPath = "",
            HttpScheme httpScheme = HttpScheme.Http, HttpClientHandler httpHandler = null)
        {
            _host = host;
            _port = port;
            _contextPath = contextPath;
            _httpScheme = httpScheme;
            _httpClient = new HttpClient(httpHandler ?? new HttpClientHandler());
        }

        public MockServerClient(string host, int port, HttpClient httpClient, string contextPath = "",
            HttpScheme httpScheme = HttpScheme.Http)
        {
            _host = host;
            _port = port;
            _contextPath = contextPath;
            _httpScheme = httpScheme;
            _httpClient = httpClient;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public Uri ServerAddress(string path = "")
        {
            return new Uri($"{_httpScheme.Value()}://{_host}:{_port}{path.PrefixWith("/")}");
        }

        public ForwardChainExpectation When(
            HttpRequest httpRequest, int priority = DefaultPriority)
        {
            return When(httpRequest, Times.Unlimited(), TimeToLive.Unlimited(), priority);
        }

        public ForwardChainExpectation When(
            HttpRequest httpRequest, Times times, int priority = DefaultPriority)
        {
            return When(httpRequest, times, TimeToLive.Unlimited(), priority);
        }

        public ForwardChainExpectation When(
            HttpRequest httpRequest, Times times, TimeToLive timeToLive, int priority = DefaultPriority)
        {
            return new ForwardChainExpectation(
                this,
                new Expectation(httpRequest, times, timeToLive, priority));
        }

        public MockServerClient Clear(HttpRequest httpRequest)
        {
            return ClearAsync(httpRequest).AwaitResult();
        }

        public async Task<MockServerClient> ClearAsync(HttpRequest httpRequest)
        {
            await SendRequestAsync(new HttpRequestMessage()
                .WithMethod(HttpMethod.Put)
                .WithUri(ServerAddress(FullPath(ClearEndpoint)))
                .WithBody(_httpRequestSerializer.Serialize(httpRequest, string.Empty)));
            return this;
        }

        public MockServerClient Verify(params HttpRequest[] httpRequests)
        {
            return VerifyAsync(httpRequests).AwaitResult();
        }

        public async Task<MockServerClient> VerifyAsync(params HttpRequest[] httpRequests)
        {
            if (httpRequests.IsNullOrEmpty())
            {
                throw new ArgumentException("Required: Non-Null and Non-Empty array of requests");
            }

            var sequence = new VerificationSequence().WithRequests(httpRequests);
            var response = await SendRequestAsync(new HttpRequestMessage()
                .WithMethod(HttpMethod.Put)
                .WithUri(ServerAddress(FullPath(VerifySequenceEndpoint)))
                .WithBody(_verificationSequenceSerializer.Serialize(sequence), Encoding.UTF8));

            var body = await response.Content.ReadAsStringAsync();

            if (!body.IsNullOrEmpty())
            {
                throw new AssertionException(body);
            }

            return this;
        }

        public MockServerClient Verify(HttpRequest httpRequest, VerificationTimes times)
        {
            return VerifyAsync(httpRequest, times).AwaitResult();
        }

        public async Task<MockServerClient> VerifyAsync(HttpRequest httpRequest, VerificationTimes times)
        {
            if (httpRequest == null)
            {
                throw new ArgumentException("Required: Non-Null request");
            }

            if (times == null)
            {
                throw new ArgumentException("Required: Non-Null verification times");
            }

            var verification = new Verification().WithRequest(httpRequest).WithTimes(times);

            var response = await SendRequestAsync(new HttpRequestMessage()
                .WithMethod(HttpMethod.Put)
                .WithUri(ServerAddress(FullPath(VerifyEndpoint)))
                .WithBody(_verificationSerializer.Serialize(verification), Encoding.UTF8));

            var body = await response.Content.ReadAsStringAsync();

            if (!body.IsNullOrEmpty())
            {
                throw new AssertionException(body);
            }

            return this;
        }

        public MockServerClient VerifyZeroInteractions()
        {
            return VerifyZeroInteractionsAsync().AwaitResult();
        }

        public async Task<MockServerClient> VerifyZeroInteractionsAsync()
        {
            return await VerifyAsync(new HttpRequest(), VerificationTimes.Exactly(0));
        }

        public HttpRequest[] RetrieveRecordedRequests(HttpRequest httpRequest)
        {
            return RetrieveRecordedRequestsAsync(httpRequest).AwaitResult();
        }

        public async Task<HttpRequest[]> RetrieveRecordedRequestsAsync(HttpRequest httpRequest)
        {
            var response = await SendRequestAsync(new HttpRequestMessage()
                .WithMethod(HttpMethod.Put)
                .WithUri(ServerAddress(FullPath(RetrieveEndpoint)))
                .WithBody(_httpRequestSerializer.Serialize(httpRequest, string.Empty),
                    Encoding.UTF8));

            var body = await response.Content.ReadAsStringAsync();

            return _httpRequestSerializer.DeserializeArray(body, new HttpRequest[0]);
        }

        public async Task SendExpectationAsync(Expectation expectation)
        {
            var expectationBody = expectation != null ? _expectationSerializer.Serialize(expectation) : string.Empty;

            using (var response = await SendRequestAsync(
                new HttpRequestMessage()
                    .WithMethod(HttpMethod.Put)
                    .WithUri(ServerAddress(FullPath(ExpectationEndpoint)))
                    .WithBody(expectationBody)))
            {
                if (response?.StatusCode != HttpStatusCode.Created)
                {
                    throw new ClientException($"\n\nerror: {response}\n{expectationBody}\n");
                }
            }
        }

        public MockServerClient Reset()
        {
            return ResetAsync().AwaitResult();
        }

        public async Task<MockServerClient> ResetAsync()
        {
            await SendRequestAsync(new HttpRequestMessage()
                .WithMethod(HttpMethod.Put)
                .WithUri(ServerAddress(FullPath(ResetEndpoint))));
            return this;
        }

        public MockServerClient Stop(bool ignoreFailure = false)
        {
            return StopAsync(ignoreFailure).AwaitResult();
        }

        public async Task<MockServerClient> StopAsync(bool ignoreFailure = false)
        {
            try
            {
                await SendRequestAsync(new HttpRequestMessage()
                    .WithMethod(HttpMethod.Put)
                    .WithUri(ServerAddress(FullPath(StopEndpoint))));

                foreach (var unused in Enumerable.Range(0, 50))
                {
                    if (await IsRunningAsync())
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception e)
            {
                if (!ignoreFailure)
                {
                    throw new ClientException("Failed to send stop request to MockServer", e);
                }
            }

            return this;
        }

        public bool IsRunning(int attempts = 10, int timeoutMillis = 500)
        {
            return IsRunningAsync(attempts, timeoutMillis).AwaitResult();
        }

        public async Task<bool> IsRunningAsync(int attempts = 10, int timeoutMillis = 500)
        {
            try
            {
                foreach (var unused in Enumerable.Range(0, attempts))
                {
                    var response =
                        await SendRequestAsync(new HttpRequestMessage()
                            .WithMethod(HttpMethod.Put)
                            .WithUri(ServerAddress(FullPath(StatusEndpoint))));

                    if (response?.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }

                    Thread.Sleep(timeoutMillis);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequest)
        {
            var response = await _httpClient.SendAsync(
                httpRequest.WithHeader(HttpRequestHeader.Host.ToString(), $"{_host}:{_port}"));

            if (response?.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException(await response.Content.ReadAsStringAsync());
            }

            return response;
        }

        private string FullPath(string endpoint)
        {
            return endpoint
                .PrefixWith("/")
                .PrefixWith(MockServerBasePath)
                .PrefixWith("/")
                .PrefixWith(_contextPath)
                .PrefixWith("/");
        }
    }
}
