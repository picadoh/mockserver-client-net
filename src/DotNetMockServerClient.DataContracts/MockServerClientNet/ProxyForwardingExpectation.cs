// -----------------------------------------------------------------------
// <copyright file="ProxyForwardingExpectation.cs" company="Calrom Ltd.">
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
    /// The Proxy Forwarding Expectation.
    /// </summary>
    [DataContract]
    public class ProxyForwardingExpectation
    {
        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [DataMember(Name = "httpRequest")]
        [JsonPropertyName("httpRequest")]
        public MockServerHttpRequest MockServerHttpRequest { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [DataMember(Name = "httpForward")]
        [JsonPropertyName("httpForward")]
        public MockServerHttpForward MockServerHttpForward { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [DataMember(Name = "times")]
        [JsonPropertyName("times")]
        public Times Times { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [DataMember(Name = "timeToLive")]
        [JsonPropertyName("timeToLive")]
        public TimeToLive TimeToLive { get; set; }
    }
}
