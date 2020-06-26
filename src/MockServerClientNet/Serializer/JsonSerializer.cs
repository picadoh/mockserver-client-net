using MockServerClientNet.Extensions;
using Newtonsoft.Json;

namespace MockServerClientNet.Serializer
{
    public class JsonSerializer<T>
    {
        public string Serialize(T obj, string defaultIfObjNull = null)
        {
            return obj == null ? defaultIfObjNull : JsonConvert.SerializeObject(obj);
        }

        public T[] DeserializeArray(string payload, T[] defaultIfPayloadNullOrEmpty)
        {
            return payload.IsNullOrEmpty() ? defaultIfPayloadNullOrEmpty : JsonConvert.DeserializeObject<T[]>(payload);
        }
    }
}