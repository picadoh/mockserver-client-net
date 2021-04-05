using System.Threading.Tasks;
using MockServerClientNet.Model;

namespace MockServerClientNet
{
    public class ForwardChainExpectation
    {
        private readonly MockServerClient _mockServerClient;
        private readonly Expectation _expectation;

        public ForwardChainExpectation(MockServerClient mockServerClient, Expectation expectation)
        {
            _mockServerClient = mockServerClient;
            _expectation = expectation;
        }

        public void Respond(HttpResponse httpResponse)
        {
            RespondAsync(httpResponse).GetAwaiter().GetResult();
        }

        public void Respond(HttpResponseTemplate httpResponseTemplate)
        {
            RespondAsync(httpResponseTemplate).GetAwaiter().GetResult();
        }

        public async Task RespondAsync(HttpResponse httpResponse)
        {
            _expectation.ThenRespond(httpResponse);
            await _mockServerClient.SendExpectationAsync(_expectation);
        }

        public async Task RespondAsync(HttpResponseTemplate httpResponseTemplate)
        {
            _expectation.ThenRespond(httpResponseTemplate);
            await _mockServerClient.SendExpectationAsync(_expectation);
        }

        public void Forward(HttpForward httpForward)
        {
            ForwardAsync(httpForward).GetAwaiter().GetResult();
        }

        public async Task ForwardAsync(HttpForward httpForward)
        {
            _expectation.ThenForward(httpForward);
            await _mockServerClient.SendExpectationAsync(_expectation);
        }
    }
}