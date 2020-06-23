using MockServerClientNet.Model;
using Newtonsoft.Json;

namespace MockServerClientNet.Verify
{
    public class Verification
    {
        [JsonProperty(PropertyName = "httpRequest")]
        public HttpRequest HttpRequest { get; private set; } = HttpRequest.Request();

        [JsonProperty(PropertyName = "times")]
        public VerificationTimes Times { get; private set; } = VerificationTimes.AtLeast(1);

        public Verification WithRequest(HttpRequest httpRequest)
        {
            HttpRequest = httpRequest;
            return this;
        }

        public Verification WithTimes(VerificationTimes times)
        {
            Times = times;
            return this;
        }
    }
}