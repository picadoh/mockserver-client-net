// -----------------------------------------------------------------------
// <copyright file="VerificationSequence.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The Verification.
    /// </summary>
    public class VerificationSequence
    {
        /// <summary>
        /// Gets search data.
        /// </summary>
        [JsonPropertyName("httpRequests")]
        public List<HttpRequest> HttpRequests { get; } = new List<HttpRequest>();

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="httpRequests">the name.</param>
        /// <returns>response.</returns>
        public VerificationSequence WithRequests(params HttpRequest[] httpRequests)
        {
            this.HttpRequests.AddRange(httpRequests);
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="httpRequests">the name.</param>
        /// <returns>response.</returns>
        public VerificationSequence WithRequests(IEnumerable<HttpRequest> httpRequests)
        {
            this.HttpRequests.AddRange(httpRequests);
            return this;
        }
    }
}
