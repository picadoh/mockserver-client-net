using System;
using System.Collections.Generic;
using System.Linq;
using MockServerClientNet.Model.Body;
using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class HttpResponse
    {
        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; private set; } = 200;

        [JsonProperty(PropertyName = "body")]
        public BodyContent Body { get; private set; }

        [JsonProperty(PropertyName = "delay")]
        public Delay Delay { get; private set; } = Delay.NoDelay();

        [JsonProperty(PropertyName = "headers")]
        public List<Header> Headers { get; private set; } = new List<Header>();

        public static HttpResponse Response()
        {
            return new HttpResponse();
        }

        public HttpResponse WithStatusCode(int statusCode)
        {
            StatusCode = statusCode;
            return this;
        }

        public HttpResponse WithHeaders(params Header[] headers)
        {
            Headers = headers.ToList();
            return this;
        }

        public HttpResponse WithBody(string body)
        {
            return WithBody(Contents.Text(body));
        }

        public HttpResponse WithBody(BodyContent body)
        {
            Body = body;
            return this;
        }

        public HttpResponse WithDelay(TimeSpan delay)
        {
            Delay = Delay.FromTimeSpan(delay);
            return this;
        }
    }
}