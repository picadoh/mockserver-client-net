// -----------------------------------------------------------------------
// <copyright file="Cookie.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
namespace DotNetMockServerClient.DataContracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The Cookie.
    /// </summary>
    public class Cookie
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Cookie"/> class.
        /// </summary>
        public Cookie()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Cookie"/> class.
        /// </summary>
        /// <param name="name">the name.</param>
        /// <param name="value">the value.</param>
        public Cookie(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
