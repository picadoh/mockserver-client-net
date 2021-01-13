// -----------------------------------------------------------------------
// <copyright file="ForwardChainExpectation.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient
{
    using DotNetMockServerClient.DataContracts;

    /// <summary>
    /// The forward chain expectation.
    /// </summary>
    public class ForwardChainExpectation
    {
        private readonly Expectation expectation;

        private readonly MockServerClient mockServerClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="ForwardChainExpectation"/> class.
        /// </summary>
        /// <param name="mockServerClient">The mock server client.</param>
        /// <param name="expectation">The expectation.</param>
        public ForwardChainExpectation(MockServerClient mockServerClient, Expectation expectation)
        {
            this.mockServerClient = mockServerClient;
            this.expectation = expectation;
        }

        /// <summary>
        /// Forwards the specified HTTP forward.
        /// </summary>
        /// <param name="httpForward">The HTTP forward.</param>
        public void Forward(HttpForward httpForward)
        {
            this.expectation.ThenForward(httpForward);
            this.mockServerClient.SendExpectation(this.expectation);
        }

        /// <summary>
        /// Responds the specified HTTP response.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        public void Respond(HttpResponse httpResponse)
        {
            this.expectation.ThenRespond(httpResponse);
            this.mockServerClient.SendExpectation(this.expectation);
        }
    }
}
