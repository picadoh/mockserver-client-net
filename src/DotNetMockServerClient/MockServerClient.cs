// -----------------------------------------------------------------------
// <copyright file="MockServerClient.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;
    using DotNetMockServerClient.Extensions;

    /// <summary>
    /// The MockServerClient.
    /// </summary>
    public class MockServerClient : AbstractClient<MockServerClient>, IMockServerClient
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MockServerClient"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="contextPath">The context path.</param>
        /// <param name="httpClient">The http client.</param>
        public MockServerClient(string host, int port, string contextPath = "", HttpClient httpClient = null)
            : base(host, port, contextPath, httpClient)
        {
        }

        /// <inheritdoc/>
        public async Task<Expectation[]> RetrieveActiveExpectationsAsync(HttpRequest httpRequest)
        {
            using (var message = new HttpRequestMessage()
                .WithMethod("PUT")
                .WithPath(this.CalculatePath("/mockserver/retrieve?type=ACTIVE_EXPECTATIONS&format=JSON"))
                .WithBody(httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty, Encoding.UTF8))
            {
                var res = await this.SendRequestAsync(message).ConfigureAwait(false);

                var body = res != null ? await res.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

                if (!string.IsNullOrEmpty(body))
                {
                    return this.ExpectationSerializer.DeserializeArray(body);
                }
            }

            return System.Array.Empty<Expectation>();
        }

        /// <inheritdoc/>
        public Expectation[] RetrieveActiveExpectations(HttpRequest httpRequest)
        {
            using (var message = new HttpRequestMessage()
                .WithMethod("PUT")
                .WithPath(this.CalculatePath("/mockserver/retrieve?type=ACTIVE_EXPECTATIONS&format=JSON"))
                .WithBody(httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty, Encoding.UTF8))
            {
                var res = this.SendRequest(message);

                var body = res?.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(body))
                {
                    return this.ExpectationSerializer.DeserializeArray(body);
                }
            }

            return System.Array.Empty<Expectation>();
        }

        /// <inheritdoc/>
        public async Task<string> RetrieveLogMessagesAsync(HttpRequest httpRequest = null)
        {
            string body = string.Empty;
            using (var message = new HttpRequestMessage()
                    .WithMethod("PUT")
                    .WithPath(this.CalculatePath("/mockserver/retrieve?type=LOGS&format=JSON"))
                    .WithBody(httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty, Encoding.UTF8))
            {
                var res = await this.SendRequestAsync(message).ConfigureAwait(false);

                body = res != null ? await res.Content.ReadAsStringAsync().ConfigureAwait(false) : null;
            }

            return body;
        }

        /// <inheritdoc/>
        public string RetrieveLogMessages(HttpRequest httpRequest = null)
        {
            string body = string.Empty;
            using (var message = new HttpRequestMessage()
                    .WithMethod("PUT")
                    .WithPath(this.CalculatePath("/mockserver/retrieve?type=LOGS&format=JSON"))
                    .WithBody(httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty, Encoding.UTF8))
            {
                var res = this.SendRequest(message);

                body = res?.Content.ReadAsStringAsync().Result;
            }

            return body;
        }

        /// <inheritdoc/>
        public async Task<HttpRequest[]> RetrieveRecordedRequestsAsync(HttpRequest httpRequest = null)
        {
            using (var message = new HttpRequestMessage()
                                  .WithMethod("PUT")
                                  .WithPath(this.CalculatePath("/mockserver/retrieve?type=REQUESTS&format=JSON"))
                                  .WithBody(httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty, Encoding.UTF8))
            {
                var res = await this.SendRequestAsync(message).ConfigureAwait(false);

                var body = res != null ? await res.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

                if (!string.IsNullOrEmpty(body))
                {
                    return this.HttpRequestSerializer.DeserializeArray(body);
                }
            }

            return System.Array.Empty<HttpRequest>();
        }

        /// <inheritdoc/>
        public HttpRequest[] RetrieveRecordedRequests(HttpRequest httpRequest = null)
        {
            using (var message = new HttpRequestMessage()
                                  .WithMethod("PUT")
                                  .WithPath(this.CalculatePath("/mockserver/retrieve"))
                                  .WithBody(httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty, Encoding.UTF8))
            {
                var res = this.SendRequest(message);

                var body = res?.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(body))
                {
                    return this.HttpRequestSerializer.DeserializeArray(body);
                }
            }

            return System.Array.Empty<HttpRequest>();
        }

        /// <inheritdoc/>
        public async Task SendExpectationAsync(Expectation expectation)
        {
            var expectationBody = expectation != null ? this.ExpectationSerializer.Serialize(expectation) : string.Empty;

            using (var message = new HttpRequestMessage()
              .WithMethod(HttpMethod.Put)
              .WithPath(this.CalculatePath("/mockserver/expectation"))
              .WithBody(expectationBody))
            {
                using (var httpResponse = await this.SendRequestAsync(message).ConfigureAwait(false))
                {
                    if (httpResponse != null && httpResponse.StatusCode != HttpStatusCode.Created)
                    {
                        throw new ClientException($"\n\nerror: {httpResponse}\n{expectationBody}\n");
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void SendExpectation(Expectation expectation)
        {
            var expectationBody = expectation != null ? this.ExpectationSerializer.Serialize(expectation) : string.Empty;

            using (var message = new HttpRequestMessage()
              .WithMethod(HttpMethod.Put)
              .WithPath(this.CalculatePath("/mockserver/expectation"))
              .WithBody(expectationBody))
            {
                using (HttpResponseMessage httpResponse = this.SendRequest(message))
                {
                    if (httpResponse != null && httpResponse.StatusCode != HttpStatusCode.Created)
                    {
                        throw new ClientException($"\n\nerror: {httpResponse}\n{expectationBody}\n");
                    }
                }
            }
        }

        /// <inheritdoc/>
        public ForwardChainExpectation When(HttpRequest httpRequest, Times times)
        {
            return new ForwardChainExpectation(this, new Expectation(httpRequest, times, TimeToLive.Unlimited()));
        }

        /// <inheritdoc/>
        public ForwardChainExpectation When(HttpRequest httpRequest, Times times, TimeToLive timeToLive)
        {
            return new ForwardChainExpectation(this, new Expectation(httpRequest, times, timeToLive));
        }

        /// <inheritdoc/>
        async Task<IMockServerClient> IAbstractClient<IMockServerClient>.ClearAsync(HttpRequest httpRequest)
        {
            return await this.ClearAsync(httpRequest).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        IMockServerClient IAbstractClient<IMockServerClient>.Clear(HttpRequest httpRequest)
        {
            return this.Clear(httpRequest);
        }

        /// <inheritdoc/>
        async Task<IMockServerClient> IAbstractClient<IMockServerClient>.ResetAsync()
        {
            return await this.ResetAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        IMockServerClient IAbstractClient<IMockServerClient>.Reset()
        {
            return this.Reset();
        }

        /// <inheritdoc/>
        async Task<IMockServerClient> IAbstractClient<IMockServerClient>.StopAsync(bool ignoreFailure)
        {
            return await this.StopAsync(ignoreFailure).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        IMockServerClient IAbstractClient<IMockServerClient>.Stop(bool ignoreFailure)
        {
            return this.Stop(ignoreFailure);
        }

        /// <inheritdoc/>
        async Task<IMockServerClient> IAbstractClient<IMockServerClient>.VerifyAsync(HttpRequest httpRequest, VerificationTimes times)
        {
            return await this.VerifyAsync(httpRequest, times).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        async Task<IMockServerClient> IAbstractClient<IMockServerClient>.VerifyAsync(params HttpRequest[] httpRequests)
        {
            return await this.VerifyAsync(httpRequests).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        IMockServerClient IAbstractClient<IMockServerClient>.Verify(HttpRequest httpRequest, VerificationTimes times)
        {
            return this.Verify(httpRequest, times);
        }

        /// <inheritdoc/>
        IMockServerClient IAbstractClient<IMockServerClient>.Verify(params HttpRequest[] httpRequests)
        {
            return this.Verify(httpRequests);
        }

        /// <inheritdoc/>
        async Task<IMockServerClient> IAbstractClient<IMockServerClient>.VerifyZeroInteractionsAsync()
        {
            return await this.VerifyZeroInteractionsAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        IMockServerClient IAbstractClient<IMockServerClient>.VerifyZeroInteractions()
        {
            return this.VerifyZeroInteractions();
        }
    }
}
