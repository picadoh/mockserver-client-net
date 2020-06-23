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
            _expectation.ThenRespond(httpResponse);
            _mockServerClient.SendExpectation(_expectation);
        }

        public void Forward(HttpForward httpForward)
        {
            _expectation.ThenForward(httpForward);
            _mockServerClient.SendExpectation(_expectation);
        }
    }
}