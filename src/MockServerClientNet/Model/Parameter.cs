using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MockServerClientNet.Model
{
    public class Parameter
    {
        public Parameter(string name, params string[] values)
        {
            Name = name;
            Values = values.ToList();
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonProperty(PropertyName = "values")]
        public List<string> Values { get; private set; }
    }
}