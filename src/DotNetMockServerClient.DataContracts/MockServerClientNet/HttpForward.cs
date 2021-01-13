// -----------------------------------------------------------------------
// <copyright file="HttpForward.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
namespace DotNetMockServerClient.DataContracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The payloads.
    /// </summary>
    public class HttpForward
    {
        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("host")]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("port")]
        public int Port { get; set; } = 80;

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("scheme")]
        public string Scheme { get; set; } = "HTTP";

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        public static HttpForward Forward()
        {
            return new HttpForward();
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="host">the values.</param>
        /// <returns>response.</returns>
        public HttpForward WithHost(string host)
        {
            this.Host = host;
            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="port">the values.</param>
        /// <returns>response.</returns>
        public HttpForward WithPort(int port)
        {
            this.Port = port;
            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="scheme">the values.</param>
        /// <returns>response.</returns>
        public HttpForward WithScheme(string scheme)
        {
            this.Scheme = scheme;
            return this;
        }
    }
}
