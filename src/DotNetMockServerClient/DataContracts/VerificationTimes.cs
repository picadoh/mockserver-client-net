// -----------------------------------------------------------------------
// <copyright file="VerificationTimes.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The Verification Times.
    /// </summary>
    public class VerificationTimes
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="VerificationTimes"/> class.
        /// </summary>
        public VerificationTimes()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="VerificationTimes"/> class.
        /// </summary>
        /// <param name="count">the count.</param>
        /// <param name="exact">the unlimited.</param>
        public VerificationTimes(int count, bool exact)
        {
            this.AtLeast = count;
            if (exact)
            {
                this.AtMost = count;
            }
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("atLeast")]
        public int AtLeast { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("atMost")]
        public int AtMost { get; set; }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="count">the count.</param>
        /// <returns>response.</returns>
        public static VerificationTimes Exactly(int count)
        {
            return new VerificationTimes(count, true);
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <returns>response.</returns>
        public static VerificationTimes Once()
        {
            return Exactly(1);
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="count">the count.</param>
        /// <returns>response.</returns>
        public static VerificationTimes WithAtLeast(int count)
        {
            return new VerificationTimes(count, false);
        }
    }
}
