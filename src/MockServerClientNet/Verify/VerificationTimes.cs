using Newtonsoft.Json;

namespace MockServerClientNet.Verify
{
    public class VerificationTimes
    {
        private const int Unbounded = -1;

        [JsonProperty(PropertyName = "atLeast")]
        public int LowerBound { get; private set; }

        [JsonProperty(PropertyName = "atMost")]
        public int UpperBound { get; private set; }

        public VerificationTimes(int lowerBound, int upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public static VerificationTimes Once()
        {
            return Exactly(1);
        }

        public static VerificationTimes Exactly(int count)
        {
            return Between(count, count);
        }

        public static VerificationTimes AtLeast(int lowerBound)
        {
            return Between(lowerBound, Unbounded);
        }

        public static VerificationTimes AtMost(int upperBound)
        {
            return Between(Unbounded, upperBound);
        }

        public static VerificationTimes Between(int lowerBound, int upperBound)
        {
            return new VerificationTimes(lowerBound, upperBound);
        }
    }
}