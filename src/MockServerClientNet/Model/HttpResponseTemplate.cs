﻿using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace MockServerClientNet.Model
{
    public class HttpResponseTemplate
    {
        [JsonProperty(PropertyName = "delay")]
        public Delay Delay { get; private set; } = Delay.NoDelay();

        [JsonProperty(PropertyName = "reasonPhrase", NullValueHandling = NullValueHandling.Ignore)]
        public string ReasonPhrase { get; private set; }

        [JsonProperty(PropertyName = "template")]
        public string Template { get; private set; }

        [JsonProperty(PropertyName = "templateType")]
        public string TemplateType { get; private set; } = TemplateTypes.JavascriptTemplate;

        internal static class TemplateTypes
        {
            public const string JavascriptTemplate = "JAVASCRIPT";
            public const string VelocityTemplate = "VELOCITY";
        }

        public static HttpResponseTemplate ResponseTemplate()
        {
            return new HttpResponseTemplate();
        }

        public HttpResponseTemplate WithReasonPhrase(string reasonPhrase)
        {
            ReasonPhrase = reasonPhrase;
            return this;
        }

        public HttpResponseTemplate WithDelay(TimeSpan delay)
        {
            Delay = Delay.FromTimeSpan(delay);
            return this;
        }

        public HttpResponseTemplate WithTemplate(string template, string templateType)
        {
            Template = template;
            TemplateType = templateType;
            return this;
        }

        public HttpResponseTemplate WithJavascriptTemplate(string template)
        {
            return WithTemplate(template, TemplateTypes.JavascriptTemplate);
        }

        public HttpResponseTemplate WithVelocityTemplate(string template)
        {
            return WithTemplate(template, TemplateTypes.VelocityTemplate);
        }
    }
}