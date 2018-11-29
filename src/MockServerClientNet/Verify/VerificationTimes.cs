using System;

namespace MockServerClientNet.Verify
{
    using Newtonsoft.Json;

    public class VerificationTimes
    {
        [JsonProperty(PropertyName = "atLeast")]
        public int atLeast { get; private set; }

        [JsonProperty(PropertyName = "atMost")]
        public int atMost { get; private set; }

        public VerificationTimes(int atLeast, int atMost)
        {
            this.atLeast = atLeast;
            this.atMost = atMost;
        }

        public static VerificationTimes Once()
        {
            return Exactly(1);
        }

        public static VerificationTimes Exactly(int count)
        {
            return new VerificationTimes(count, count);
        }

        public static VerificationTimes AtLeast(int count)
        {
            return new VerificationTimes(count, 0);
        }
    }
}