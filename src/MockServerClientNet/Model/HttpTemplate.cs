namespace MockServerClientNet.Model;

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class HttpTemplate(TemplateType type)
{
    [JsonProperty(PropertyName = "template")]
    public string StringTemplate { get; private set; }

    [JsonProperty(PropertyName = "templateType")]
    [JsonConverter(typeof(StringEnumConverter))]
    public TemplateType Type { get; private set; } = type;

    [JsonProperty(PropertyName = "delay")]
    public Delay Delay { get; private set; } = Delay.NoDelay();

    public static HttpTemplate Template(TemplateType type)
    {
        return new HttpTemplate(type);
    }

    public HttpTemplate WithTemplate(string template)
    {
        StringTemplate = template;
        return this;
    }

    public HttpTemplate WithDelay(TimeSpan delay)
    {
        Delay = Delay.FromTimeSpan(delay);
        return this;
    }
}
