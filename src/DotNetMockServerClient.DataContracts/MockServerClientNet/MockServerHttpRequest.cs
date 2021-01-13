// -----------------------------------------------------------------------
// <copyright file="MockServerHttpRequest.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
// <summary>
//   Defines the DefaultClientHandler type.
// </summary>

namespace DotNetMockServerClient.DataContracts
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// MockServerHttpRequest class.
    /// </summary>
    [DataContract]
    public class MockServerHttpRequest
    {
        /// <summary>
        /// Gets or sets the Method.
        /// </summary>
        [DataMember(Name = "method")]
        [JsonPropertyName("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the Path.
        /// </summary>
        [DataMember(Name = "path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the Cookies.
        /// </summary>
        [DataMember(Name = "cookies")]
        [JsonPropertyName("cookies")]
        public Cookies Cookies { get; set; }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="cookies">the cookies.</param>
        /// <returns>response.</returns>
        public MockServerHttpRequest WithCookies(Cookies cookies)
        {
            if (this.Cookies == null)
            {
                this.Cookies = new Cookies();
            }

            this.Cookies = cookies;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="method">the values.</param>
        /// <returns>response.</returns>
        public MockServerHttpRequest WithMethod(string method)
        {
            this.Method = method;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="path">the values.</param>
        /// <returns>response.</returns>
        public MockServerHttpRequest WithPath(string path)
        {
            this.Path = path;
            return this;
        }
    }
}
