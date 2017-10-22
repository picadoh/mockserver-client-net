namespace MockServerClientCSharp.Model
{
  using System.Collections.Generic;
  using System.Linq;
  using Newtonsoft.Json;

  public class HttpRequest
  {
    [JsonProperty(PropertyName = "method")]
    public string Method { get; private set; } = "GET";

    [JsonProperty(PropertyName = "path")]
    public string Path { get; private set; } = string.Empty;

    [JsonProperty(PropertyName = "queryStringParameters")]
    public List<Parameter> Parameters { get; private set; } = new List<Parameter>();

    [JsonProperty(PropertyName = "headers")]
    public List<Header> Headers { get; private set; } = new List<Header>();

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
      this.Parameters = parameters.ToList();
      return this;
    }

    public HttpRequest WithHeader(string name, string value)
    {
      this.Headers.Add(new Header(name, value));
      return this;
    }

    public HttpRequest WithBody(string body)
    {
      this.Body = body;
      return this;
    }
  }
}
