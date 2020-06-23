﻿using System;
using System.Net.Http;
using System.Text;

namespace MockServerClientNet.Extensions
{
    public static class HttpMessageExtensions
    {
        public static HttpRequestMessage WithMethod(this HttpRequestMessage request, string method)
        {
            request.Method = new HttpMethod(method);
            return request;
        }

        public static HttpRequestMessage WithMethod(this HttpRequestMessage request, HttpMethod method)
        {
            request.Method = method;
            return request;
        }

        public static HttpRequestMessage WithUri(this HttpRequestMessage request, Uri uri)
        {
            request.RequestUri = uri;
            return request;
        }

        public static HttpRequestMessage WithBody(this HttpRequestMessage request, string body)
        {
            return WithBody(request, body, Encoding.UTF8, "application/json");
        }

        public static HttpRequestMessage WithBody(this HttpRequestMessage request, string body, Encoding encoding)
        {
            return WithBody(request, body, encoding, "application/json");
        }

        public static HttpRequestMessage WithBody(this HttpRequestMessage request, string body, Encoding encoding,
            string mediaType)
        {
            request.Content = new StringContent(body, encoding, mediaType);
            return request;
        }

        public static HttpRequestMessage WithHeader(this HttpRequestMessage request, string name,
            params string[] values)
        {
            request.Headers.Add(name, values);
            return request;
        }
    }
}