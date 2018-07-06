namespace MockServerClientNet.Model
{
  using System.Collections.Generic;
  using System.Linq;
  using Newtonsoft.Json;

  public class Header
  {
    public Header(string name, params string[] values)
    {
      this.Name = name;
      this.Values = values.ToList();
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "values")]
    public List<string> Values { get; private set; }
  }
}
