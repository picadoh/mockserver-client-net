using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class HttpRequest
    {
        [JsonProperty(PropertyName = "headers")]
        private Dictionary<string, string[]> _headers = new Dictionary<string, string[]>();

        [JsonProperty(PropertyName = "queryStringParameters")]
        private Dictionary<string, string[]> _parameters = new Dictionary<string, string[]>();

        [JsonIgnore]
        public List<Header> Headers
        {
            get { return _headers.Select(entry => new Header(entry.Key, entry.Value)).ToList(); }
        }

        [JsonIgnore]
        public List<Parameter> Parameters
        {
            get { return _parameters.Select(entry => new Parameter(entry.Key, entry.Value)).ToList(); }
        }

        [JsonProperty(PropertyName = "method")]
        public string Method { get; private set; } = "GET";

        [JsonProperty(PropertyName = "path")]
        public string Path { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "body")]
        public string Body { get; private set; } = string.Empty;

        [JsonProperty(PropertyName = "secure", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSecure { get; private set; }

        [JsonProperty(PropertyName = "keepAlive", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsKeepAlive { get; private set; }

        public static HttpRequest Request()
        {
            return new HttpRequest();
        }

        public HttpRequest WithMethod(string method)
        {
            Method = method;
            return this;
        }

        public HttpRequest WithMethod(HttpMethod method)
        {
            return WithMethod(method.Method);
        }

        public HttpRequest WithPath(string path)
        {
            Path = path;
            return this;
        }

        public HttpRequest WithKeepAlive(bool keepAlive)
        {
            IsKeepAlive = keepAlive;
            return this;
        }

        public HttpRequest WithSecure(bool secure)
        {
            IsSecure = secure;
            return this;
        }

        public HttpRequest WithQueryStringParameters(params Parameter[] parameters)
        {
            _parameters = parameters.ToDictionary(p => p.Name, p => p.Values.ToArray());
            return this;
        }

        public HttpRequest WithHeader(string name, params string[] value)
        {
            _headers.Add(name, value);
            return this;
        }

        public HttpRequest WithBody(string body)
        {
            Body = body;
            return this;
        }
    }
}