// -----------------------------------------------------------------------
// <copyright file="AbstractClient.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotNetMockServerClient.DataContracts;
    using DotNetMockServerClient.Extensions;
    using DotNetMockServerClient.Serializer;

    /// <summary>
    /// The abstract client.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public abstract class AbstractClient<T> : IDisposable, IAbstractClient<T>
        where T : AbstractClient<T>
    {
        private readonly string contextPath;

        private readonly JsonSerializer<VerificationSequence> verificationSequenceSerializer = new JsonSerializer<VerificationSequence>();

        private readonly JsonSerializer<Verification> verificationSerializer = new JsonSerializer<Verification>();

        private readonly HttpClient httpClient = new HttpClient();

        private readonly bool httpClientSetExternally;

        /// <summary>
        /// Initialises a new instance of the <see cref="AbstractClient{T}"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="contextPath">The context path.</param>
        /// <param name="httpClient">The http client.</param>
        protected AbstractClient(string host, int port, string contextPath = "", HttpClient httpClient = null)
        {
            this.Host = host;
            this.Port = port;
            this.contextPath = contextPath;
            this.AddSuffix = string.Empty;

            if (httpClient != null)
            {
                this.httpClient = httpClient;
                this.httpClientSetExternally = true;
            }
        }

        /// <inheritdoc/>
        public string AddSuffix { get; set; }

        /// <inheritdoc/>
        public string Host { get; set; }

        /// <inheritdoc/>
        public int Port { get; set; }

        /// <summary>
        /// Gets the http request serializer.
        /// </summary>
        protected JsonSerializer<HttpRequest> HttpRequestSerializer => new JsonSerializer<HttpRequest>();

        /// <summary>
        /// Gets the expectation serializer.
        /// </summary>
        protected JsonSerializer<Expectation> ExpectationSerializer => new JsonSerializer<Expectation>();

        /// <inheritdoc/>
        public async Task<T> ClearAsync(HttpRequest httpRequest)
        {
            var body = httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty;
            using (var message = new HttpRequestMessage()
                           .WithMethod("PUT")
                           .WithPath(this.CalculatePath("mockserver/clear"))
                           .WithBody(body))
            {
                await this.SendRequestAsync(message).ConfigureAwait(false);
            }

            return (T)this;
        }

        /// <inheritdoc/>
        public T Clear(HttpRequest httpRequest)
        {
            var body = httpRequest != null ? this.HttpRequestSerializer.Serialize(httpRequest) : string.Empty;
            using (var message = new HttpRequestMessage()
                .WithMethod("PUT")
                .WithPath(this.CalculatePath("mockserver/clear"))
                .WithBody(body))
            {
                this.SendRequest(message);
            }

            return (T)this;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task<bool> IsRunningAsync(int attempts = 10, int timeoutMillis = 500)
        {
            try
            {
                while (attempts-- > 0)
                {
                    using (var message = new HttpRequestMessage().WithMethod("PUT").WithPath(this.CalculatePath("mockserver/status")))
                    {
                        var httpResponse = await this.SendRequestAsync(message).ConfigureAwait(false);

                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                    }

                    Thread.Sleep(timeoutMillis);
                }

                return false;
            }
#pragma warning disable CA1031 // Do not catch general exception types - https://github.com/dotnet/roslyn-analyzers/issues/2181
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsRunning(int attempts = 10, int timeoutMillis = 500)
        {
            try
            {
                while (attempts-- > 0)
                {
                    using (var message = new HttpRequestMessage().WithMethod("PUT").WithPath(this.CalculatePath("mockserver/status")))
                    {
                        var httpResponse = this.SendRequest(message);

                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                    }

                    Thread.Sleep(timeoutMillis);
                }

                return false;
            }
#pragma warning disable CA1031 // Do not catch general exception types - https://github.com/dotnet/roslyn-analyzers/issues/2181
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<T> ResetAsync()
        {
            using (var message = new HttpRequestMessage().WithMethod("PUT").WithPath(this.CalculatePath("mockserver/reset")))
            {
                await this.SendRequestAsync(message).ConfigureAwait(false);
            }

            return (T)this;
        }

        /// <inheritdoc/>
        public T Reset()
        {
            using (var message = new HttpRequestMessage().WithMethod("PUT").WithPath(this.CalculatePath("mockserver/reset")))
            {
                this.SendRequest(message);
            }

            return (T)this;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequest)
        {
            var response = await this.httpClient.SendAsync(
              httpRequest.WithHeader(HttpRequestHeader.Host.ToString(), $"{this.Host}:{this.Port}")).ConfigureAwait(false);

            if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }

            return response;
        }

        /// <inheritdoc/>
        public HttpResponseMessage SendRequest(HttpRequestMessage mockServerRequest)
        {
            return this.SendRequestAsync(mockServerRequest).Result;
        }

        /// <inheritdoc/>
        public async Task<T> StopAsync(bool ignoreFailure)
        {
            try
            {
                using (var message = new HttpRequestMessage().WithMethod("PUT").WithPath(this.CalculatePath("mockserver/stop")))
                {
                    await this.SendRequestAsync(message).ConfigureAwait(false);

                    var attempts = 0;
                    while (await this.IsRunningAsync().ConfigureAwait(false) && attempts++ < 10)
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

            return (T)this;
        }

        /// <inheritdoc/>
        public T Stop(bool ignoreFailure)
        {
            try
            {
                using (var message = new HttpRequestMessage().WithMethod("PUT").WithPath(this.CalculatePath("mockserver/stop")))
                {
                    this.SendRequest(message);

                    int attemps = 0;
                    while (this.IsRunning() && attemps++ < 10)
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

            return (T)this;
        }

        /// <inheritdoc/>
        public Task<T> VerifyAsync(HttpRequest httpRequest, VerificationTimes times)
        {
            if (httpRequest == null)
            {
                throw new ArgumentException("Required: Non-Null request");
            }

            if (times == null)
            {
                throw new ArgumentException("Required: Non-Null verification times");
            }

            return this.VerifyInternalAsync(httpRequest, times);
        }

        /// <inheritdoc/>
        public Task<T> VerifyAsync(params HttpRequest[] httpRequests)
        {
            if (httpRequests == null || httpRequests.Length == 0 || httpRequests[0] == null)
            {
                throw new ArgumentException("Required: Non-Null and Non-Empty array of requests");
            }

            return this.VerifyInternalAsync(httpRequests);
        }

        /// <inheritdoc/>
        public T Verify(HttpRequest httpRequest, VerificationTimes times)
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
            using (var message = new HttpRequestMessage()
                                  .WithMethod("PUT")
                                  .WithPath(this.CalculatePath("mockserver/verify"))
                                  .WithBody(this.verificationSerializer.Serialize(verification), Encoding.UTF8))
            {
                var res = this.SendRequest(message);

                var body = res?.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(body))
                {
                    throw new AssertionException(body);
                }
            }

            return (T)this;
        }

        /// <inheritdoc/>
        public T Verify(params HttpRequest[] httpRequests)
        {
            if (httpRequests == null || httpRequests.Length == 0 || httpRequests[0] == null)
            {
                throw new ArgumentException("Required: Non-Null and Non-Empty array of requests");
            }

            var sequence = new VerificationSequence().WithRequests(httpRequests);
            using (var message = new HttpRequestMessage()
                                  .WithMethod("PUT")
                                  .WithPath(this.CalculatePath("mockserver/verifySequence"))
                                  .WithBody(this.verificationSequenceSerializer.Serialize(sequence), Encoding.UTF8))
            {
                var res = this.SendRequest(message);

                var body = res?.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(body))
                {
                    throw new AssertionException(body);
                }
            }

            return (T)this;
        }

        /// <inheritdoc/>
        public async Task<T> VerifyZeroInteractionsAsync()
        {
            var verification = new Verification().WithRequest(new HttpRequest()).WithTimes(VerificationTimes.Exactly(0));

            using (var message = new HttpRequestMessage()
                                  .WithMethod("PUT")
                                  .WithPath(this.CalculatePath("mockserver/verify"))
                                  .WithBody(this.verificationSerializer.Serialize(verification), Encoding.UTF8))
            {
                var res = await this.SendRequestAsync(message).ConfigureAwait(false);

                var body = res != null ? await res.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

                if (!string.IsNullOrEmpty(body))
                {
                    throw new AssertionException(body);
                }
            }

            return (T)this;
        }

        /// <inheritdoc/>
        public T VerifyZeroInteractions()
        {
            var verification = new Verification().WithRequest(new HttpRequest()).WithTimes(VerificationTimes.Exactly(0));

            using (var message = new HttpRequestMessage()
                .WithMethod("PUT")
                .WithPath(this.CalculatePath("mockserver/verify"))
                .WithBody(this.verificationSerializer.Serialize(verification), Encoding.UTF8))
            {
                var res = this.SendRequest(message);

                var body = res?.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(body))
                {
                    throw new AssertionException(body);
                }
            }

            return (T)this;
        }

        /// <summary>
        /// The calculate path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The cleaned path.</returns>
        protected string CalculatePath(string path)
        {
            var cleanedPath = path;

            if (string.IsNullOrEmpty(this.contextPath))
            {
                cleanedPath = this.contextPath.PrefixWith("/").SuffixWith("/") + cleanedPath.RemovePrefix("/");
            }

            return $"{this.Host}:{this.Port}{cleanedPath.PrefixWith("/")}{this.AddSuffix}";
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">Whether to dispose.</param>
        protected virtual void Dispose(bool disposing)
        {
            //// uncomment to clear the instance
            ////this.Stop();
            if (!this.httpClientSetExternally)
            {
                this.httpClient.Dispose();
            }
        }

        private async Task<T> VerifyInternalAsync(HttpRequest httpRequest, VerificationTimes times)
        {
            var verification = new Verification().WithRequest(httpRequest).WithTimes(times);
            using (var message = new HttpRequestMessage()
                                  .WithMethod("PUT")
                                  .WithPath(this.CalculatePath("mockserver/verify"))
                                  .WithBody(this.verificationSerializer.Serialize(verification), Encoding.UTF8))
            {
                var res = await this.SendRequestAsync(message).ConfigureAwait(false);

                var body = res != null ? await res.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

                if (!string.IsNullOrEmpty(body))
                {
                    throw new AssertionException(body);
                }
            }

            return (T)this;
        }

        private async Task<T> VerifyInternalAsync(HttpRequest[] httpRequests)
        {
            var sequence = new VerificationSequence().WithRequests(httpRequests);
            using (var message = new HttpRequestMessage()
                                  .WithMethod("PUT")
                                  .WithPath(this.CalculatePath("mockserver/verifySequence"))
                                  .WithBody(this.verificationSequenceSerializer.Serialize(sequence), Encoding.UTF8))
            {
                var res = await this.SendRequestAsync(message).ConfigureAwait(false);

                var body = res != null ? await res.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

                if (!string.IsNullOrEmpty(body))
                {
                    throw new AssertionException(body);
                }
            }

            return (T)this;
        }
    }
}
