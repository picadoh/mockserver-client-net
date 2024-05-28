﻿﻿using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class HttpForwardTemplate
    {
        [JsonProperty(PropertyName = "template")]
        public string Template { get; private set; }

        [JsonProperty(PropertyName = "templateType")]
        public string TemplateType { get; private set; }

        public static HttpForwardTemplate ForwardTemplate()
        {
            return new HttpForwardTemplate();
        }

        public HttpForwardTemplate WithTemplate(string template, string templateType)
        {
            Template = template;
            TemplateType = templateType;
            return this;
        }
    }
}