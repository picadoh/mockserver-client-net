// -----------------------------------------------------------------------
// <copyright file="TimeToLive.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
namespace DotNetMockServerClient.DataContracts
{
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The payloads.
    /// </summary>
    [DataContract]
    public class TimeToLive
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TimeToLive"/> class.
        /// </summary>
        public TimeToLive()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="TimeToLive"/> class.
        /// </summary>
        /// <param name="timeToLive">the time to live.</param>
        /// <param name="unlimited">the unlimited.</param>
        public TimeToLive(TimeSpan timeToLive, bool unlimited)
        {
            this.TimeUnit = "MILLISECONDS";
            this.TtlMillis = (int)timeToLive.TotalMilliseconds;
            this.IsUnlimited = unlimited;
        }

        /// <summary>
        /// Gets or sets a value indicating whether search data.
        /// </summary>
        [JsonPropertyName("unlimited")]
        [DataMember(Name = "unlimited")]
        public bool IsUnlimited { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("timeUnit")]
        [DataMember(Name = "timeUnit")]
        public string TimeUnit { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("timeToLive")]
        [DataMember(Name = "timeToLive")]
        public int TtlMillis { get; set; }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="timeToLive">the count.</param>
        /// <returns>response.</returns>
        public static TimeToLive Exactly(TimeSpan timeToLive)
        {
            return new TimeToLive(timeToLive, false);
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <returns>response.</returns>
        public static TimeToLive Unlimited()
        {
            return new TimeToLive(TimeSpan.MinValue, true);
        }
    }
}
