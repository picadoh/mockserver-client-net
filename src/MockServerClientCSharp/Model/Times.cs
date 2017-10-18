namespace MockServerClientCSharp.Model
{
  using Newtonsoft.Json;

  public class Times
  {
    [JsonProperty(PropertyName = "remainingTimes")]
    public int Count { get; private set; }

    [JsonProperty(PropertyName = "unlimited")]
    public bool IsUnlimited { get; private set; }

    public static Times Unlimited()
    {
      return new Times(0, true);
    }

    public static Times Exactly(int count)
    {
      return new Times(count, false);
    }

    private Times(int count, bool unlimited)
    {
      this.Count = count;
      this.IsUnlimited = unlimited;
    }
  }
}
