using MockServerClientNet.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockServerClientNet.Serializer
{
    public class BodyConverter : CustomCreationConverter<Body>
    {
        public override Body Create(Type objectType)
        {
            return new Body("");
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(reader.ValueType == typeof(string))
            {
                return new Body(reader.Value.ToString());
            }
            else
                return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || base.CanConvert(objectType);
        }
    }
}
