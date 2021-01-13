// -----------------------------------------------------------------------
// <copyright file="IMockServerClient.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient
{
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;

    /// <summary>
    /// The mock server client interface.
    /// </summary>
    public interface IMockServerClient : IAbstractClient<IMockServerClient>
    {
        /// <summary>
        /// Retrieves the active expectations asynchronous.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The active expectations.</returns>
        Task<Expectation[]> RetrieveActiveExpectationsAsync(HttpRequest httpRequest);

        /// <summary>
        /// Retrieves the active expectations.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The active expectations.</returns>
        Expectation[] RetrieveActiveExpectations(HttpRequest httpRequest);

        /// <summary>
        /// Retrieves the log messages asynchronous.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The log messages.</returns>
        Task<string> RetrieveLogMessagesAsync(HttpRequest httpRequest = null);

        /// <summary>
        /// Retrieves the log messages.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The log messages.</returns>
        string RetrieveLogMessages(HttpRequest httpRequest = null);

        /// <summary>
        /// Retrieves the recorded requests asynchronous.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The recorded requests.</returns>
        Task<HttpRequest[]> RetrieveRecordedRequestsAsync(HttpRequest httpRequest = null);

        /// <summary>
        /// Retrieves the recorded requests.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The recorded requests.</returns>
        HttpRequest[] RetrieveRecordedRequests(HttpRequest httpRequest = null);

        /// <summary>
        /// Sends the expectation asynchronous.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <returns>A task.</returns>
        Task SendExpectationAsync(Expectation expectation);

        /// <summary>
        /// Sends the expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        void SendExpectation(Expectation expectation);

        /// <summary>
        /// Whens the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="times">The times.</param>
        /// <returns>Forward chain expectation.</returns>
        ForwardChainExpectation When(HttpRequest httpRequest, Times times);

        /// <summary>
        /// Whens the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="times">The times.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <returns>Forward chain expectation.</returns>
        ForwardChainExpectation When(HttpRequest httpRequest, Times times, TimeToLive timeToLive);
    }
}
