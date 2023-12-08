// -----------------------------------------------------------------------
// <copyright file="BodyCheck.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The BodyCheck.
    /// </summary>
    public class BodyCheck
    {
        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("string")]
        public string BodyString { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("not")]
        public bool? Negate { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("regex")]
        public string Regex { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("subString")]
        public bool? SubString { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("xpath")]
        public string XPath { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the match.
        /// </summary>
        /// <value>
        /// The type of the match.
        /// </value>
        [JsonPropertyName("matchType")]
        public string MatchType { get; set; }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="regex">the httpForward.</param>
        /// <returns>response.</returns>
        public static BodyCheck WithRegex(string regex)
        {
            return new BodyCheck
            {
                Type = "REGEX",
                Regex = regex,
                BodyString = null,
                SubString = null,
                Negate = null,
                ContentType = null,
            };
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="body">the httpForward.</param>
        /// <returns>response.</returns>
        public static BodyCheck WithString(string body)
        {
            return new BodyCheck
            {
                Type = "STRING",
                BodyString = body,
                SubString = true,
                ContentType = "text/plain; charset=utf-16",
            };
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>response.</returns>
        public static BodyCheck WithXPath(string element)
        {
            return new BodyCheck
            {
                Type = "XPATH",
                XPath = element,
            };
        }

        /// <summary>
        /// With json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="matchType">Type of the match.</param>
        /// <returns>response.</returns>
        public static BodyCheck WithJson(string value, string matchType = "STRICT")
        {
            return new BodyCheck
            {
                Type = "JSON",
                Value = value,
                MatchType = matchType,
            };
        }

        /// <summary>
        /// With json schema.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>response.</returns>
        public static BodyCheck WithJsonSchema(string value)
        {
            return new BodyCheck
            {
                Type = "JSON_SCHEMA",
                Value = value,
            };
        }
    }
}
