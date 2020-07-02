using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MockServerClientNet.Model.Body
{
    public class Body
    {
        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "string", NullValueHandling = NullValueHandling.Ignore)]
        public string StringValue { get; set; }

        [JsonProperty(PropertyName = "base64Bytes", NullValueHandling = NullValueHandling.Ignore)]
        public string Base64Bytes { get; set; }

        [JsonProperty(PropertyName = "xml", NullValueHandling = NullValueHandling.Ignore)]
        public string XmlValue { get; set; }

        [JsonProperty(PropertyName = "json", NullValueHandling = NullValueHandling.Ignore)]
        public string JsonValue { get; set; }

        internal static class Types
        {
            public const string StringType = "STRING";
            public const string BinaryType = "BINARY";
            public const string JsonType = "JSON";
            public const string XmlType = "XML";
            public const string XPathType = "XPATH";
            public const string JsonPathType = "JSON_PATH";
            public const string XmlSchemaType = "XML_SCHEMA";
            public const string JsonSchemaType = "JSON_SCHEMA";
            public const string RegexType = "REGEX";
        }

        internal abstract class BodyDeserializer<T> : CustomCreationConverter<T> where T : Body
        {
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                // this keeps backward compatibility when loading expectations from file
                return reader.ValueType == typeof(string)
                    ? DeserializeFromString(reader.Value?.ToString())
                    : base.ReadJson(reader, objectType, existingValue, serializer);
            }

            public override T Create(Type objectType)
            {
                return NewInstance();
            }

            protected abstract T NewInstance();

            protected abstract T DeserializeFromString(string value);
        }
    }
}