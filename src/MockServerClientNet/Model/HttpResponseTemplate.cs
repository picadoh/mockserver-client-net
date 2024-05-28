﻿using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class HttpResponseTemplate
    {
        [JsonProperty(PropertyName = "template")]
        public string Template { get; private set; }

        [JsonProperty(PropertyName = "templateType")]
        public string TemplateType { get; private set; }

        public static HttpResponseTemplate ResponseTemplate()
        {
            return new HttpResponseTemplate();
        }

        public HttpResponseTemplate WithTemplate(string template, string templateType)
        {
            Template = template;
            TemplateType = templateType;
            return this;
        }
    }
}