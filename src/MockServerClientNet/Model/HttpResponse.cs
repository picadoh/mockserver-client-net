namespace MockServerClientNet.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Body;
using Newtonsoft.Json;

public class HttpResponse
{
    [JsonProperty(PropertyName = "statusCode")]
    public int StatusCode { get; private set; } = 200;

    [JsonProperty(PropertyName = "body")]
    public BodyContent Body { get; private set; }

    [JsonProperty(PropertyName = "delay")]
    public Delay Delay { get; private set; } = Delay.NoDelay();

    [JsonProperty(PropertyName = "headers")]
    public List<Header> Headers { get; private set; } = new List<Header>();

    [JsonProperty(PropertyName = "reasonPhrase", NullValueHandling = NullValueHandling.Ignore)]
    public string ReasonPhrase { get; private set; }

    public static HttpResponse Response()
    {
        return new HttpResponse();
    }

    public HttpResponse WithStatusCode(int statusCode)
    {
        StatusCode = statusCode;
        return this;
    }

    public HttpResponse WithStatusCode(HttpStatusCode statusCode)
    {
        return WithStatusCode((int) statusCode);
    }

    public HttpResponse WithReasonPhrase(string reasonPhrase)
    {
        ReasonPhrase = reasonPhrase;
        return this;
    }

    public HttpResponse WithHeaders(params Header[] headers)
    {
        Headers = headers.ToList();
        return this;
    }

    public HttpResponse WithHeader(string name, params string[] value)
    {
        Headers.Add(new Header(name, value));
        return this;
    }

    public HttpResponse WithBody(string body)
    {
        return WithBody(Contents.Text(body));
    }

    public HttpResponse WithBody(BodyContent body)
    {
        Body = body;
        return this;
    }

    public HttpResponse WithDelay(TimeSpan delay)
    {
        Delay = Delay.FromTimeSpan(delay);
        return this;
    }
}
