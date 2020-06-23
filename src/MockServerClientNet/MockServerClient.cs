using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MockServerClientNet.Extensions;
using MockServerClientNet.Model;

namespace MockServerClientNet
{
    public class MockServerClient : AbstractClient<MockServerClient>
    {
        public MockServerClient(string host, int port, string contextPath = "") : base(host, port, contextPath)
        {
        }

        public ForwardChainExpectation When(HttpRequest httpRequest, Times times)
        {
            return new ForwardChainExpectation(
                this,
                new Expectation(httpRequest, times, TimeToLive.Unlimited()));
        }

        public ForwardChainExpectation When(HttpRequest httpRequest, Times times, TimeToLive timeToLive)
        {
            return new ForwardChainExpectation(
                this,
                new Expectation(httpRequest, times, timeToLive));
        }

        public void SendExpectation(Expectation expectation)
        {
            SendExpectationAsync(expectation).GetAwaiter().GetResult();
        }

        public async Task SendExpectationAsync(Expectation expectation)
        {
            var expectationBody = expectation != null ? ExpectationSerializer.Serialize(expectation) : string.Empty;

            using (var httpResponse = await SendRequestAsync(
                new HttpRequestMessage()
                    .WithMethod(HttpMethod.Put)
                    .WithUri(ServerAddressWithPath(CalculatePath("expectation")))
                    .WithBody(expectationBody)))
            {
                if (httpResponse != null && httpResponse.StatusCode != HttpStatusCode.Created)
                {
                    throw new ClientException($"\n\nerror: {httpResponse}\n{expectationBody}\n");
                }
            }
        }

        public HttpRequest[] RetrieveRecordedRequests(HttpRequest httpRequest)
        {
            return RetrieveRecordedRequestsAsync(httpRequest).AwaitResult();
        }

        public async Task<HttpRequest[]> RetrieveRecordedRequestsAsync(HttpRequest httpRequest)
        {
            var res = await SendRequestAsync(new HttpRequestMessage()
                .WithMethod("PUT")
                .WithUri(ServerAddressWithPath(CalculatePath("retrieve")))
                .WithBody(httpRequest != null ? HttpRequestSerializer.Serialize(httpRequest) : string.Empty,
                    Encoding.UTF8));

            var body = await res.Content.ReadAsStringAsync();

            return !string.IsNullOrEmpty(body) ? HttpRequestSerializer.DeserializeArray(body) : new HttpRequest[0];
        }
    }
}