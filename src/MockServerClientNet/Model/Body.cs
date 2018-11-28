using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockServerClientNet.Model
{
    public class Body
    {
        public Body(string stringBody)
        {
            Type = "STRING";
            StringBody = stringBody ?? throw new ArgumentNullException(nameof(stringBody));
            Not = false;
        }
        [JsonProperty(PropertyName = "not")]
        public bool Not { get; private set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }
        [JsonProperty(PropertyName = "string", NullValueHandling = NullValueHandling.Ignore)]
        public string StringBody { get; private set; }
        [JsonProperty(PropertyName = "contentType", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentType { get; private set; }

    }
}
