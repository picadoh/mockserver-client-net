using System.Collections.Generic;
using System.Linq;
using MockServerClientNet.Model;
using Newtonsoft.Json;

namespace MockServerClientNet.Verify
{
    public class VerificationSequence
    {
        [JsonProperty(PropertyName = "httpRequests")]
        public IEnumerable<HttpRequest> HttpRequests { get; private set; } = new List<HttpRequest>();

        public VerificationSequence WithRequests(params HttpRequest[] httpRequests)
        {
            HttpRequests = HttpRequests.Concat(httpRequests);
            return this;
        }

        public VerificationSequence WithRequests(IEnumerable<HttpRequest> httpRequests)
        {
            HttpRequests = HttpRequests.Concat(httpRequests);
            return this;
        }
    }
}