using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class Delay
    {
        public Delay(int value)
        {
            TimeUnit = "MILLISECONDS";
            Value = value;
        }

        [JsonProperty(PropertyName = "timeUnit")]
        public string TimeUnit { get; private set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; private set; }
    }
}