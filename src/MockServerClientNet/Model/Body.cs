using MockServerClientNet.Serializer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MockServerClientNet.Model
{
    [JsonConverter(typeof(BodyConverter))]
    public class Body
    {
        public Body(string stringBody)
        {
            Type = "STRING";
            StringBody = stringBody ?? throw new ArgumentNullException(nameof(stringBody));
            Not = false;
        }

        protected Body()
        { }

        public static Body BinaryBody(byte[] binary, string contentType)
        {
            return new Body()
            {
                Type = "BINARY",
                Not = false,
                ContentType = contentType,
                Base64Bytes = Convert.ToBase64String(binary)
            };
        }

        [JsonProperty(PropertyName = "not")]
        public bool Not { get; private set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }
        [JsonProperty(PropertyName = "string", NullValueHandling = NullValueHandling.Ignore)]
        public string StringBody { get; private set; }
        [JsonProperty(PropertyName = "contentType", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentType { get; private set; }

        [JsonProperty(PropertyName = "base64Bytes", NullValueHandling = NullValueHandling.Ignore)]
        public string Base64Bytes { get; private set; }

    }
}
