// -----------------------------------------------------------------------
// <copyright file="Delay.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The Delay.
    /// </summary>
    public class Delay
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Delay"/> class.
        /// </summary>
        /// <param name="value">the value.</param>
        public Delay(int value)
        {
            this.TimeUnit = "MILLISECONDS";
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("timeUnit")]
        public string TimeUnit { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
