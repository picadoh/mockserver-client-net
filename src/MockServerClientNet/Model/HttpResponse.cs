namespace MockServerClientNet.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Newtonsoft.Json;

  public class HttpResponse
  {
    [JsonProperty(PropertyName = "statusCode")]
    public int StatusCode { get; private set; }

    [JsonProperty(PropertyName = "body", NullValueHandling = NullValueHandling.Ignore)]
    public Body Body { get; private set; }

    [JsonProperty(PropertyName = "delay")]
    public Delay Delay { get; private set; }

    [JsonProperty(PropertyName = "headers")]
    public List<Header> Headers { get; private set; } = new List<Header>();

    public static HttpResponse Response()
    {
      return new HttpResponse();
    }

    public HttpResponse WithStatusCode(int statusCode)
    {
      this.StatusCode = statusCode;
      return this;
    }

    public HttpResponse WithHeaders(params Header[] headers)
    {
      this.Headers = headers.ToList();
      return this;
    }

    public HttpResponse WithBody(string body)
    {
      this.Body = new Body(body);
      return this;
    }
        public HttpResponse WithBody(Body body)
        {
            this.Body = body;
            return this;
        }

        public HttpResponse WithDelay(TimeSpan delay)
    {
      this.Delay = new Delay((int)delay.TotalMilliseconds);
      return this;
    }
  }
}