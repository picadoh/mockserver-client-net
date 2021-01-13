// -----------------------------------------------------------------------
// <copyright file="HttpResponse.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
namespace DotNetMockServerClient.DataContracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The payloads.
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("delay")]
        public Delay Delay { get; set; }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("headers")]
        public Dictionary<string, string[]> Headers { get; } = new Dictionary<string, string[]>();

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonIgnore]
        public List<Header> HeadersList
        {
            get
            {
                if (this.Headers == null)
                {
                    return new List<Header>();
                }

                List<Header> result = new List<Header>();
                foreach (var entry in this.Headers)
                {
                    result.Add(new Header(entry.Key, entry.Value));
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets reasonPhrase data.
        /// </summary>
        [JsonPropertyName("reasonPhrase")]
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <returns>response.</returns>
        public static HttpResponse Response()
        {
            return new HttpResponse();
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="body">the values.</param>
        /// <returns>response.</returns>
        public HttpResponse WithBody(string body)
        {
            this.Body = body;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="delay">the values.</param>
        /// <returns>response.</returns>
        public HttpResponse WithDelay(TimeSpan delay)
        {
            this.Delay = new Delay((int)delay.TotalMilliseconds);
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="name">the name.</param>
        /// <param name="value">the values.</param>
        /// <returns>response.</returns>
        public HttpResponse WithHeaders(string name, params string[] value)
        {
            this.Headers.Add(name, value);
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="headers">the name.</param>
        /// <returns>response.</returns>
        public HttpResponse WithHeaders(Dictionary<string, string[]> headers)
        {
            if (headers == null)
            {
                return this;
            }

            foreach (var header in headers)
            {
                this.Headers.Add(header.Key, header.Value);
            }

            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="value">the name.</param>
        /// <returns>response.</returns>
        public HttpResponse WithHeaders(List<Header> value)
        {
            if (value == null || !value.Any())
            {
                return this;
            }

            foreach (var header in value)
            {
                this.Headers.Add(header.Name, header.Values.ToArray());
            }

            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="value">the name.</param>
        /// <returns>response.</returns>
        public HttpResponse WithHeaders(params Header[] value)
        {
            if (value != null)
            {
                foreach (var header in value)
                {
                    this.Headers.Add(header.Name, header.Values.ToArray());
                }
            }

            return this;
        }

        /// <summary>
        /// reasonPhrase data.
        /// </summary>
        /// <param name="reasonPhrase">the values.</param>
        /// <returns>response.</returns>
        public HttpResponse WithReasonPhrase(string reasonPhrase)
        {
            this.ReasonPhrase = reasonPhrase;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="statusCode">the values.</param>
        /// <returns>response.</returns>
        public HttpResponse WithStatusCode(int statusCode)
        {
            this.StatusCode = statusCode;
            return this;
        }
    }
}
