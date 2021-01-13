// -----------------------------------------------------------------------
// <copyright file="HttpRequest.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------
namespace DotNetMockServerClient.DataContracts
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The HttpRequest.
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("body")]
        public BodyCheck Body { get; set; } = BodyCheck.WithString(string.Empty);

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("cookies")]
        public Dictionary<string, string> Cookies { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonIgnore]
        public List<Cookie> CookiesList
        {
            get
            {
                if (this.Cookies == null)
                {
                    return new List<Cookie>();
                }

                List<Cookie> result = new List<Cookie>();
                foreach (var entry in this.Cookies)
                {
                    result.Add(new Cookie(entry.Key, entry.Value));
                }

                return result;
            }
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("headers")]
        public Dictionary<string, string[]> Headers { get; } = new Dictionary<string, string[]>();

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonIgnore]
        public List<Header> HeadersList
        {
            get
            {
                if (this.Headers == null)
                {
                    return new List<Header>();
                }

                List<Header> result = new List<Header>();
                foreach (var entry in this.Headers)
                {
                    result.Add(new Header(entry.Key, entry.Value));
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("keepAlive")]
        public bool? IsKeepAlive { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("secure")]
        public bool? IsSecure { get; set; }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("method")]
        public string Method { get; set; } = "GET";

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("queryStringParameters")]
        public Dictionary<string, string[]> Parameters { get; } = new Dictionary<string, string[]>();

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonIgnore]
        public List<Parameter> ParametersList
        {
            get
            {
                if (this.Parameters == null)
                {
                    return new List<Parameter>();
                }

                List<Parameter> result = new List<Parameter>();
                foreach (var entry in this.Parameters)
                {
                    result.Add(new Parameter(entry.Key, entry.Value));
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets search data.
        /// </summary>
        /// <returns>response.</returns>
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Search data.
        /// </summary>
        /// <returns>response.</returns>
        public static HttpRequest Request()
        {
            return new HttpRequest();
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="body">the values.</param>
        /// <returns>response.</returns>
        public HttpRequest WithBody(string body)
        {
            this.Body = BodyCheck.WithString(body);
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="body">the values.</param>
        /// <returns>response.</returns>
        public HttpRequest WithBody(BodyCheck body)
        {
            this.Body = body;
            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="cookies">the cookies.</param>
        /// <returns>response.</returns>
        public HttpRequest WithCookie(params Cookie[] cookies)
        {
            if (cookies == null)
            {
                return this;
            }

            foreach (var cookie in cookies)
            {
                this.Cookies.Add(cookie.Name, cookie.Value);
            }

            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="name">the name.</param>
        /// <param name="value">the values.</param>
        /// <returns>response.</returns>
        public HttpRequest WithHeaders(string name, params string[] value)
        {
            this.Headers.Add(name, value);
            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="keepAlive">the cookies.</param>
        /// <returns>response.</returns>
        public HttpRequest WithKeepAlive(bool keepAlive)
        {
            this.IsKeepAlive = keepAlive;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="method">the values.</param>
        /// <returns>response.</returns>
        public HttpRequest WithMethod(string method)
        {
            this.Method = method;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="path">the values.</param>
        /// <returns>response.</returns>
        public HttpRequest WithPath(string path)
        {
            this.Path = path;
            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="parameters">the values.</param>
        /// <returns>response.</returns>
        public HttpRequest WithQueryStringParameters(params Parameter[] parameters)
        {
            if (parameters == null)
            {
                return this;
            }

            foreach (var parameter in parameters)
            {
                this.Parameters.Add(parameter.Name, parameter.Values.ToArray());
            }

            return this;
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="regEx">the values.</param>
        /// <returns>response.</returns>
        public HttpRequest WithRegexBody(string regEx)
        {
            this.Body = BodyCheck.WithRegex(regEx);
            return this;
        }

        /// <summary>
        /// Gets search data.
        /// </summary>
        /// <param name="isSsl">the cookies.</param>
        /// <returns>response.</returns>
        public HttpRequest WithSecure(bool isSsl)
        {
            this.IsSecure = isSsl;
            return this;
        }
    }
}
