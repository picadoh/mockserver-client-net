// -----------------------------------------------------------------------
// <copyright file="IAbstractClient.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;

    /// <summary>
    /// The abstract client interface.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public interface IAbstractClient<T>
        where T : IAbstractClient<T>
    {
        /// <summary>
        /// Gets or sets the add suffix.
        /// </summary>
        /// <value>
        /// The add suffix.
        /// </value>
        string AddSuffix { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        int Port { get; set; }

        /// <summary>
        /// Clears the asynchronous.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>Task of type.</returns>
        Task<T> ClearAsync(HttpRequest httpRequest);

        /// <summary>
        /// Clears the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The task.</returns>
        T Clear(HttpRequest httpRequest);

#pragma warning disable S2953 // 5007 Methods named "Dispose" should implement "IDisposable.Dispose"
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();
#pragma warning restore S2953 // Methods named "Dispose" should implement "IDisposable.Dispose"

        /// <summary>
        /// Determines whether [is running asynchronous] [the specified attempts].
        /// </summary>
        /// <param name="attempts">The attempts.</param>
        /// <param name="timeoutMillis">The timeout millis.</param>
        /// <returns>Task of bool.</returns>
        Task<bool> IsRunningAsync(int attempts = 10, int timeoutMillis = 500);

        /// <summary>
        /// Is running.
        /// </summary>
        /// <param name="attempts">The attempts.</param>
        /// <param name="timeoutMillis">The time out milliseconds.</param>
        /// <returns>Whether mock server is running or not.</returns>
        bool IsRunning(int attempts = 10, int timeoutMillis = 500);

        /// <summary>
        /// Resets the asynchronous.
        /// </summary>
        /// <returns>Task of type.</returns>
        Task<T> ResetAsync();

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <returns>The type.</returns>
        T Reset();

        /// <summary>
        /// Sends the request asynchronous.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>Task of response.</returns>
        Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequest);

        /// <summary>
        /// The send request.
        /// </summary>
        /// <param name="mockServerRequest">The mock server request.</param>
        /// <returns>The mock server response.</returns>
        HttpResponseMessage SendRequest(HttpRequestMessage mockServerRequest);

        /// <summary>
        /// Stops the asynchronous.
        /// </summary>
        /// <param name="ignoreFailure">if set to <c>true</c> [ignore failure].</param>
        /// <returns>Task of type.</returns>
        Task<T> StopAsync(bool ignoreFailure);

        /// <summary>
        /// Stops the specified ignore failure.
        /// </summary>
        /// <param name="ignoreFailure">if set to <c>true</c> [ignore failure].</param>
        /// <returns>The type.</returns>
        T Stop(bool ignoreFailure);

        /// <summary>
        /// Verifies the asynchronous.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="times">The times.</param>
        /// <returns>Task of type.</returns>
        Task<T> VerifyAsync(HttpRequest httpRequest, VerificationTimes times);

        /// <summary>
        /// Verifies the asynchronous.
        /// </summary>
        /// <param name="httpRequests">The HTTP requests.</param>
        /// <returns>Task of type.</returns>
        Task<T> VerifyAsync(params HttpRequest[] httpRequests);

        /// <summary>
        /// Verifies the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="times">The times.</param>
        /// <returns>The type.</returns>
        T Verify(HttpRequest httpRequest, VerificationTimes times);

        /// <summary>
        /// Verifies the specified HTTP requests.
        /// </summary>
        /// <param name="httpRequests">The HTTP requests.</param>
        /// <returns>The type.</returns>
        T Verify(params HttpRequest[] httpRequests);

        /// <summary>
        /// Verifies the zero interactions asynchronous.
        /// </summary>
        /// <returns>The type.</returns>
        Task<T> VerifyZeroInteractionsAsync();

        /// <summary>
        /// Verifies the zero interactions.
        /// </summary>
        /// <returns>The type.</returns>
        T VerifyZeroInteractions();
    }
}
