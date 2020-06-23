using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class Times
    {
        public Times(int count, bool unlimited)
        {
            Count = count;
            IsUnlimited = unlimited;
        }

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
    }
}