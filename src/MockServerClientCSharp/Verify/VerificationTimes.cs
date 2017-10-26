using System;

namespace MockServerClientCSharp.Verify
{
  using Newtonsoft.Json;

  public class VerificationTimes
  {
    [JsonProperty(PropertyName = "count")]
    public int Count { get; private set; }

    [JsonProperty(PropertyName = "exact")]
    public bool Exact { get; private set; }

    public VerificationTimes(int count, bool exact)
    {
      this.Count = count;
      this.Exact = exact;
    }

    public static VerificationTimes Once()
    {
      return Exactly(1);
    }

    public static VerificationTimes Exactly(int count)
    {
      return new VerificationTimes(count, true);
    }

    public static VerificationTimes AtLeast(int count)
    {
      return new VerificationTimes(count, false);
    }
  }
}