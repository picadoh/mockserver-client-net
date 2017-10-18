namespace MockServerClientCSharp.Model
{
  using Newtonsoft.Json;

  public class Delay
  {
    [JsonProperty(PropertyName = "timeUnit")]
    public string TimeUnit { get; private set; }

    [JsonProperty(PropertyName = "value")]
    public int Value { get; private set; }

    public Delay(int value)
    {
      this.TimeUnit = "MILLISECONDS";
      this.Value = value;
    }
  }
}
