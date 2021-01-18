// -----------------------------------------------------------------------
// <copyright file="MockServerHttpForward.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
// <summary>
//   Defines the DefaultClientHandler type.
// </summary>

namespace DotNetMockServerClient.DataContracts
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// MockServerHttpForward class.
    /// </summary>
    [DataContract]
    public class MockServerHttpForward
    {
        /// <summary>
        /// Gets or sets the Host.
        /// </summary>
        [DataMember(Name = "Host")]
        [JsonPropertyName("host")]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the Port.
        /// </summary>
        [DataMember(Name = "Port")]
        [JsonPropertyName("port")]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the Scheme.
        /// </summary>
        [DataMember(Name = "Scheme")]
        [JsonPropertyName("scheme")]
        public string Scheme { get; set; }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="host">the values.</param>
        /// <returns>response.</returns>
        public MockServerHttpForward WithHost(string host)
        {
            this.Host = host;
            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="port">the values.</param>
        /// <returns>response.</returns>
        public MockServerHttpForward WithPort(int port)
        {
            this.Port = port;
            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="scheme">the values.</param>
        /// <returns>response.</returns>
        public MockServerHttpForward WithScheme(string scheme)
        {
            this.Scheme = scheme;
            return this;
        }
    }
}
