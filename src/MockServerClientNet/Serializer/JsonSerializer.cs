namespace MockServerClientNet.Serializer
{
  using Newtonsoft.Json;

  public class JsonSerializer<T>
  {
    public string Serialize(T obj)
    {
      return JsonConvert.SerializeObject(obj);
    }

    public T[] DeserializeArray(string payload)
    {
      return JsonConvert.DeserializeObject<T[]>(payload);
    }
  }
}
