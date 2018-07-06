namespace MockServerClientNet.Model
{
  using Newtonsoft.Json;

  public class HttpForward
  {
    [JsonProperty(PropertyName = "host")]
    public string Host { get; private set; }

    [JsonProperty(PropertyName = "port")]
    public int Port { get; private set; } = 80;

    [JsonProperty(PropertyName = "scheme")]
    public string Scheme { get; private set; } = "HTTP";

    public static HttpForward Forward() {
      return new HttpForward();
    }

    public HttpForward WithHost(string host)
    {
      this.Host = host;
      return this;
    }

    public HttpForward WithPort(int port) {
      this.Port = port;
      return this;
    }

    public HttpForward WithScheme(string scheme)
    {
      this.Scheme = scheme;
      return this;
    }
  }
}
