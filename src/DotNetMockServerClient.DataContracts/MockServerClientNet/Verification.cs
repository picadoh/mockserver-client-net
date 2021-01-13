// -----------------------------------------------------------------------
// <copyright file="Verification.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The Verification.
    /// </summary>
    public class Verification
    {
        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("httpRequest")]
        public HttpRequest HttpRequest { get; set; } = HttpRequest.Request();

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("times")]
        public VerificationTimes Times { get; set; } = VerificationTimes.WithAtLeast(1);

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="httpRequest">the name.</param>
        /// <returns>response.</returns>
        public Verification WithRequest(HttpRequest httpRequest)
        {
            this.HttpRequest = httpRequest;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="times">the name.</param>
        /// <returns>response.</returns>
        public Verification WithTimes(VerificationTimes times)
        {
            this.Times = times;
            return this;
        }
    }
}
