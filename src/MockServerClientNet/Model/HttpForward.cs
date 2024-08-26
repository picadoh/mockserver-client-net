namespace MockServerClientNet.Model;

using System;
using Extensions;
using Newtonsoft.Json;

public class HttpForward
{
    [JsonProperty(PropertyName = "host")]
    public string Host { get; private set; }

    [JsonProperty(PropertyName = "port")]
    public int Port { get; private set; } = 80;

    [JsonProperty(PropertyName = "scheme")]
    public string Scheme { get; private set; } = "HTTP";

    [JsonProperty(PropertyName = "delay")]
    public Delay Delay { get; private set; } = Delay.NoDelay();

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

    public HttpForward WithDelay(TimeSpan delay)
    {
        Delay = Delay.FromTimeSpan(delay);
        return this;
    }
}
