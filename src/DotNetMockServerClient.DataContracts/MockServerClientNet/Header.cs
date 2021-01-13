// -----------------------------------------------------------------------
// <copyright file="Header.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The payloads.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Header"/> class.
        /// </summary>
        public Header()
        {
            this.Values = new List<string>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Header"/> class.
        /// </summary>
        /// <param name="name">the count.</param>
        /// <param name="values">the values.</param>
        public Header(string name, params string[] values)
        {
            this.Name = name;
            this.Values = values.ToList();
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("values")]
        public List<string> Values { get; }
    }
}
