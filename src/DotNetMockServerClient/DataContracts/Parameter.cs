// -----------------------------------------------------------------------
// <copyright file="Parameter.cs" company="Calrom Ltd.">
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
    public class Parameter
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        public Parameter()
        {
            this.Values = new List<string>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">the name.</param>
        /// <param name="values">the values.</param>
        public Parameter(string name, params string[] values)
        {
            this.Name = name;
            this.Values = values.ToList();
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets search data.
        /// </summary>
        [JsonPropertyName("values")]
        public List<string> Values { get; }
    }
}
