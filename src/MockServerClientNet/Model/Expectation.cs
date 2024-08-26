namespace MockServerClientNet.Model;

using Newtonsoft.Json;

public class Expectation(HttpRequest httpRequest, Times times, TimeToLive timeToLive, int priority)
{
    [JsonProperty(PropertyName = "httpRequest")]
    public HttpRequest HttpRequest { get; private set; } = httpRequest;

    [JsonProperty(PropertyName = "httpResponse", NullValueHandling = NullValueHandling.Ignore)]
    public HttpResponse HttpResponse { get; private set; }

    [JsonProperty(PropertyName = "httpResponseTemplate", NullValueHandling = NullValueHandling.Ignore)]
    public HttpTemplate HttpResponseTemplate { get; private set; }

    [JsonProperty(PropertyName = "httpForward", NullValueHandling = NullValueHandling.Ignore)]
    public HttpForward HttpForward { get; private set; }

    [JsonProperty(PropertyName = "httpForwardTemplate", NullValueHandling = NullValueHandling.Ignore)]
    public HttpTemplate HttpForwardTemplate { get; private set; }

    [JsonProperty(PropertyName = "times")]
    public Times Times { get; private set; } = times;

    [JsonProperty(PropertyName = "timeToLive")]
    public TimeToLive TimeToLive { get; private set; } = timeToLive;

    [JsonProperty(PropertyName = "priority")]
    public int Priority { get; private set; } = priority;

    public Expectation ThenRespond(HttpResponse httpResponse)
    {
        if (httpResponse != null)
        {
            HttpResponse = httpResponse;
        }

        return this;
    }

    public Expectation ThenRespond(HttpTemplate httpTemplate)
    {
        if (httpTemplate != null)
        {
            HttpResponseTemplate = httpTemplate;
        }

        return this;
    }

    public Expectation ThenForward(HttpForward httpForward)
    {
        if (httpForward != null)
        {
            HttpForward = httpForward;
        }

        return this;
    }

    public Expectation ThenForward(HttpTemplate httpTemplate)
    {
        if (httpTemplate != null)
        {
            HttpForwardTemplate = httpTemplate;
        }

        return this;
    }
}
