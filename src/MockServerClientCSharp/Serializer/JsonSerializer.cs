namespace MockServerClientCSharp.Serializer
{
    using Newtonsoft.Json;

    public class JsonSerializer<T>
    {
        public string Serialize(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
