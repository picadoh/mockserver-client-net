namespace MockServerClientCSharp.Model
{
  using System;
  using Newtonsoft.Json;

  public class TimeToLive
  {
    [JsonProperty(PropertyName = "timeUnit")]
    public string TimeUnit { get; private set; }

    [JsonProperty(PropertyName = "timeToLive")]
    public int ttlMillis { get; private set; }

    [JsonProperty(PropertyName = "unlimited")]
    public bool IsUnlimited { get; private set; }

    public static TimeToLive Unlimited()
    {
      return new TimeToLive(TimeSpan.MinValue, true);
    }

    public static TimeToLive Exactly(TimeSpan timeToLive)
    {
      return new TimeToLive(timeToLive, false);
    }

    private TimeToLive(TimeSpan timeToLive, bool unlimited)
    {
      this.TimeUnit = "MILLISECONDS";
      this.ttlMillis = (int)timeToLive.TotalMilliseconds;
      this.IsUnlimited = unlimited;
    }
  }
}
