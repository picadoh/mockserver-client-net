namespace MockServerClientNet.Model
{
    using Newtonsoft.Json;

    public class Expectation
    {
        public Expectation(HttpRequest httpRequest, Times times, TimeToLive timeToLive)
        {
            HttpRequest = httpRequest;
            Times = times;
            TimeToLive = timeToLive;
        }

        [JsonProperty(PropertyName = "httpRequest")]
        public HttpRequest HttpRequest { get; private set; }

        [JsonProperty(PropertyName = "httpResponse", NullValueHandling = NullValueHandling.Ignore)]
        public HttpResponse HttpResponse { get; private set; }

        [JsonProperty(PropertyName = "httpForward", NullValueHandling = NullValueHandling.Ignore)]
        public HttpForward HttpForward { get; private set; }

        [JsonProperty(PropertyName = "times")]
        public Times Times { get; private set; }

        [JsonProperty(PropertyName = "timeToLive")]
        public TimeToLive TimeToLive { get; private set; }

        public Expectation ThenRespond(HttpResponse httpResponse)
        {
            if (httpResponse != null)
            {
                this.HttpResponse = httpResponse;
            }

            return this;
        }

        public Expectation ThenForward(HttpForward httpForward)
        {
            if (httpForward != null)
            {
                this.HttpForward = httpForward;
            }

            return this;
        }
    }
}