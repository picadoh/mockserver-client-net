using Newtonsoft.Json;

namespace MockServerClientNet.Model.Body
{
    [JsonConverter(typeof(BodyMatcherDeserializer))]
    public class BodyMatcher : Body
    {
        [JsonProperty(PropertyName = "not", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsNot { get; set; }

        [JsonProperty(PropertyName = "subString", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSubString { get; set; }

        [JsonProperty(PropertyName = "matchType", NullValueHandling = NullValueHandling.Ignore)]
        public string JsonMatchType { get; set; }

        [JsonProperty(PropertyName = "xpath", NullValueHandling = NullValueHandling.Ignore)]
        public string XPathValue { get; set; }

        [JsonProperty(PropertyName = "jsonPath", NullValueHandling = NullValueHandling.Ignore)]
        public string JsonPathValue { get; set; }

        [JsonProperty(PropertyName = "xmlSchema", NullValueHandling = NullValueHandling.Ignore)]
        public string XmlSchemaValue { get; set; }

        [JsonProperty(PropertyName = "jsonSchema", NullValueHandling = NullValueHandling.Ignore)]
        public string JsonSchemaValue { get; set; }

        [JsonProperty(PropertyName = "regex", NullValueHandling = NullValueHandling.Ignore)]
        public string RegexValue { get; set; }

        public BodyMatcher Clone()
        {
            return (BodyMatcher) MemberwiseClone();
        }

        private class BodyMatcherDeserializer : BodyDeserializer<BodyMatcher>
        {
            protected override BodyMatcher NewInstance()
            {
                return new BodyMatcher();
            }

            protected override BodyMatcher DeserializeFromString(string value)
            {
                return Matchers.MatchingExactString(value);
            }
        }
    }
}