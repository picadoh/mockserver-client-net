// -----------------------------------------------------------------------
// <copyright file="HttpMessageExtensions.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.Extensions
{
    using System;
    using System.Net.Http;
    using System.Text;

    /// <summary>
    /// The http message extensions class.
    /// </summary>
    public static class HttpMessageExtensions
    {
        /// <summary>
        /// The with body extension.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="body">The body.</param>
        /// <returns>The request with body.</returns>
        public static HttpRequestMessage WithBody(this HttpRequestMessage request, string body)
        {
            return WithBody(request, body, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// The with body extension.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="body">The body.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The request with body.</returns>
        public static HttpRequestMessage WithBody(this HttpRequestMessage request, string body, Encoding encoding)
        {
            return WithBody(request, body, encoding, "application/json");
        }

        /// <summary>
        /// The with body extension.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="body">The body.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="mediaType">The mediaType.</param>
        /// <returns>The request with body.</returns>
        public static HttpRequestMessage WithBody(this HttpRequestMessage request, string body, Encoding encoding, string mediaType)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Content = new StringContent(body, encoding, mediaType);
            return request;
        }

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="name">The name.</param>
        /// <param name="values">The value.</param>
        /// <returns>The request with header.</returns>
        public static HttpRequestMessage WithHeader(this HttpRequestMessage request, string name, params string[] values)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Headers.Add(name, values);
            return request;
        }

        /// <summary>
        /// The with method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="method">The method.</param>
        /// <returns>The request with message.</returns>
        public static HttpRequestMessage WithMethod(this HttpRequestMessage request, string method)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Method = new HttpMethod(method);
            return request;
        }

        /// <summary>
        /// The with method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="method">The method.</param>
        /// <returns>The request with message.</returns>
        public static HttpRequestMessage WithMethod(this HttpRequestMessage request, HttpMethod method)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Method = method;
            return request;
        }

        /// <summary>
        /// The with path.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="path">The path.</param>
        /// <returns>The request with path.</returns>
        public static HttpRequestMessage WithPath(this HttpRequestMessage request, string path)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.RequestUri = new Uri($"http://{path}");
            return request;
        }
    }
}
