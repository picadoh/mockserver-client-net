namespace MockServerClientNet.Model
{
  using System.Collections.Generic;
  using System.Linq;
    using MockServerClientNet.Serializer;
    using Newtonsoft.Json;

  public class HttpRequest
  {
    [JsonProperty(PropertyName = "headers")]
    private Dictionary<string, string[]> _headers = new Dictionary<string, string[]>();

    [JsonProperty(PropertyName = "queryStringParameters")]
    private Dictionary<string, string[]> _parameters = new Dictionary<string, string[]>();

    [JsonIgnore]
    public List<Header> Headers
    {
      get
      {
        List<Header> result = new List<Header>();
        foreach (KeyValuePair<string, string[]> entry in _headers)
        {
          result.Add(new Header(entry.Key, entry.Value));
        }
        return result;
      }
    }

    [JsonIgnore]
    public List<Parameter> Parameters
    {
      get
      {
        List<Parameter> result = new List<Parameter>();
        foreach (KeyValuePair<string, string[]> entry in _parameters)
        {
          result.Add(new Parameter(entry.Key, entry.Value));
        }
        return result;
      }
    }

    [JsonProperty(PropertyName = "method")]
    public string Method { get; private set; } = "GET";

    [JsonProperty(PropertyName = "path")]
    public string Path { get; private set; } = string.Empty;

    [JsonProperty(PropertyName = "body", NullValueHandling = NullValueHandling.Ignore)]
        public Body Body { get; private set; } 

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
      this.Method = method;
      return this;
    }

    public HttpRequest WithPath(string path)
    {
      this.Path = path;
      return this;
    }

    public HttpRequest WithKeepAlive(bool keepAlive)
    {
      this.IsKeepAlive = keepAlive;
      return this;
    }

    public HttpRequest WithSecure(bool isSsl)
    {
      this.IsSecure = isSsl;
      return this;
    }

    public HttpRequest WithQueryStringParameters(params Parameter[] parameters)
    {
      this._parameters = parameters.ToDictionary(p => p.Name, p => p.Values.ToArray());
      return this;
    }

    public HttpRequest WithHeader(string name, params string[] value)
    {
      this._headers.Add(name, value);
      return this;
    }

    public HttpRequest WithBody(string body)
    {
      this.Body = new Body(body);
      return this;
    }
  }
}
