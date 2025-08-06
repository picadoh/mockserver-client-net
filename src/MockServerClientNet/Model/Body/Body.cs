using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

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

        [JsonProperty(PropertyName = "rawBytes", NullValueHandling = NullValueHandling.Ignore)]
        public string RawBytes { get; set; }

        [JsonProperty(PropertyName = "xml", NullValueHandling = NullValueHandling.Ignore)]
        public string XmlValue { get; set; }

        [JsonProperty(PropertyName = "json", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(JsonValueConverter))]
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
                if (reader.ValueType == typeof(string))
                {
                    return DeserializeFromString(reader.Value?.ToString());
                }

                var body = (T)base.ReadJson(reader, objectType, existingValue, serializer);
                
                // If RawBytes and Type are present, handle RawBytes instead
                if (body == null || string.IsNullOrEmpty(body.RawBytes) || string.IsNullOrEmpty(body.Type))
                {
                    return body;
                }

                var rawContent = Encoding.UTF8.GetString(Convert.FromBase64String(body.RawBytes));

                switch (body.Type?.ToUpperInvariant())
                {
                    case Types.JsonType when string.IsNullOrEmpty(body.JsonValue):
                        body.JsonValue = rawContent;
                        break;
                    case Types.XmlType when string.IsNullOrEmpty(body.XmlValue):
                        body.XmlValue = rawContent;
                        break;
                    case Types.StringType when string.IsNullOrEmpty(body.StringValue):
                        body.StringValue = rawContent;
                        break;
                }

                return body;
            }

            public override T Create(Type objectType)
            {
                return NewInstance();
            }

            protected abstract T NewInstance();

            protected abstract T DeserializeFromString(string value);
        }

        private class JsonValueConverter : JsonConverter<string>
        {
            public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
            {
                writer.WriteValue(value);
            }

            public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.StartObject)
                {
                    return reader.TokenType == JsonToken.Null ? null : reader.Value?.ToString();
                }
                    
                // Read the JSON object and serialize it back to a string
                var jsonObject = JObject.Load(reader);
                return jsonObject.ToString(Formatting.None);
            }
        }
    }
}
