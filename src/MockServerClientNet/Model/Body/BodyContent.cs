using Newtonsoft.Json;

namespace MockServerClientNet.Model.Body
{
    [JsonConverter(typeof(BodyContentDeserializer))]
    public class BodyContent : Body
    {
        [JsonProperty(PropertyName = "contentType", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentTypeValue { get; set; }

        private class BodyContentDeserializer : BodyDeserializer<BodyContent>
        {
            protected override BodyContent NewInstance()
            {
                return new BodyContent();
            }

            protected override BodyContent DeserializeFromString(string value)
            {
                return Contents.Text(value);
            }
        }
    }
}