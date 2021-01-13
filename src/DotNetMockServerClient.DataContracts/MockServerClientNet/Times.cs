// -----------------------------------------------------------------------
// <copyright file="Times.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
namespace DotNetMockServerClient.DataContracts
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The Times.
    /// </summary>
    [DataContract]
    public class Times
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Times"/> class.
        /// </summary>
        public Times()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Times"/> class.
        /// </summary>
        /// <param name="count">the count.</param>
        /// <param name="unlimited">the unlimited.</param>
        public Times(int count, bool unlimited)
        {
            this.Count = count;
            this.IsUnlimited = unlimited;
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [DataMember(Name = "remainingTimes")]
        [JsonPropertyName("remainingTimes")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether search data.
        /// </summary>
        [DataMember(Name = "unlimited")]
        [JsonPropertyName("unlimited")]
        public bool IsUnlimited { get; set; }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="count">the count.</param>
        /// <returns>response.</returns>
        public static Times Exactly(int count)
        {
            return new Times(count, false);
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <returns>response.</returns>
        public static Times Unlimited()
        {
            return new Times(0, true);
        }
    }
}
