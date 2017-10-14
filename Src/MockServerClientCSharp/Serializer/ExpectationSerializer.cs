namespace MockServerClientCSharp.Serializer
{
    using MockServerClientCSharp.Model;
    using Newtonsoft.Json;

    public class ExpectationSerializer
    {
        public string Serialize(Expectation expectation)
        {
            return JsonConvert.SerializeObject(expectation);
        }
    }
}
