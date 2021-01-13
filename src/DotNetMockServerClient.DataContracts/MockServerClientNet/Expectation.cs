// -----------------------------------------------------------------------
// <copyright file="Expectation.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
namespace DotNetMockServerClient.DataContracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The payloads.
    /// </summary>
    public class Expectation
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Expectation"/> class.
        /// </summary>
        public Expectation()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Expectation"/> class.
        /// </summary>
        /// <param name="httpRequest">the httpRequest.</param>
        /// <param name="times">the times.</param>
        /// <param name="timeToLive">the timeToLive.</param>
        public Expectation(HttpRequest httpRequest, Times times, TimeToLive timeToLive)
        {
            this.HttpRequest = httpRequest;
            this.Times = times;
            this.TimeToLive = timeToLive;
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("httpForward")]
        public HttpForward HttpForward { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("httpRequest")]
        public HttpRequest HttpRequest { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("httpResponse")]
        public HttpResponse HttpResponse { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("times")]
        public Times Times { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("timeToLive")]
        public TimeToLive TimeToLive { get; set; }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="httpForward">the httpForward.</param>
        /// <returns>response.</returns>
        public Expectation ThenForward(HttpForward httpForward)
        {
            if (httpForward != null)
            {
                this.HttpForward = httpForward;
            }

            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="httpResponse">the httpResponse.</param>
        /// <returns>response.</returns>
        public Expectation ThenRespond(HttpResponse httpResponse)
        {
            if (httpResponse != null)
            {
                this.HttpResponse = httpResponse;
            }

            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="times">the httpForward.</param>
        /// <returns>response.</returns>
        public Expectation WithTimes(Times times)
        {
            if (times != null)
            {
                this.Times = times;
            }

            return this;
        }
    }
}
