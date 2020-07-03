﻿using MockServerClientNet.Extensions;
using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class HttpForward
    {
        [JsonProperty(PropertyName = "host")]
        public string Host { get; private set; }

        [JsonProperty(PropertyName = "port")]
        public int Port { get; private set; } = 80;

        [JsonProperty(PropertyName = "scheme")]
        public string Scheme { get; private set; } = "HTTP";

        public static HttpForward Forward()
        {
            return new HttpForward();
        }

        public HttpForward WithHost(string host)
        {
            Host = host;
            return this;
        }

        public HttpForward WithPort(int port)
        {
            Port = port;
            return this;
        }

        public HttpForward WithScheme(string scheme)
        {
            Scheme = scheme;
            return this;
        }

        public HttpForward WithScheme(HttpScheme scheme)
        {
            return WithScheme(scheme.Value().ToUpper());
        }
    }
}