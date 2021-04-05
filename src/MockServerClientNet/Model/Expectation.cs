namespace MockServerClientNet.Model
{
    using Newtonsoft.Json;

    public class Expectation
    {
        public Expectation(HttpRequest httpRequest, Times times, TimeToLive timeToLive, int priority)
        {
            HttpRequest = httpRequest;
            Times = times;
            TimeToLive = timeToLive;
            Priority = priority;
        }

        [JsonProperty(PropertyName = "httpRequest")]
        public HttpRequest HttpRequest { get; private set; }

        [JsonProperty(PropertyName = "httpResponse", NullValueHandling = NullValueHandling.Ignore)]
        public HttpResponse HttpResponse { get; private set; }

        [JsonProperty(PropertyName = "httpResponseTemplate", NullValueHandling = NullValueHandling.Ignore)]
        public HttpResponseTemplate HttpResponseTemplate { get; private set; }

        [JsonProperty(PropertyName = "httpForward", NullValueHandling = NullValueHandling.Ignore)]
        public HttpForward HttpForward { get; private set; }

        [JsonProperty(PropertyName = "times")]
        public Times Times { get; private set; }

        [JsonProperty(PropertyName = "timeToLive")]
        public TimeToLive TimeToLive { get; private set; }

        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; private set; }

        public Expectation ThenRespond(HttpResponse httpResponse)
        {
            if (httpResponse != null)
            {
                HttpResponse = httpResponse;
            }

            return this;
        }

        public Expectation ThenRespond(HttpResponseTemplate httpResponseTemplate)
        {
            if (httpResponseTemplate != null)
            {
                HttpResponseTemplate = httpResponseTemplate;
            }

            return this;
        }

        public Expectation ThenForward(HttpForward httpForward)
        {
            if (httpForward != null)
            {
                HttpForward = httpForward;
            }

            return this;
        }
    }
}