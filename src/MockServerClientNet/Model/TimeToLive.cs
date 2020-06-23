using System;
using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class TimeToLive
    {
        public TimeToLive(TimeSpan timeToLive, bool unlimited)
        {
            TimeUnit = "MILLISECONDS";
            TtlMillis = (int) timeToLive.TotalMilliseconds;
            IsUnlimited = unlimited;
        }

        [JsonProperty(PropertyName = "timeUnit")]
        public string TimeUnit { get; private set; }

        [JsonProperty(PropertyName = "timeToLive")]
        public int TtlMillis { get; private set; }

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
    }
}