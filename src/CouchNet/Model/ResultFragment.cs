using Newtonsoft.Json;

namespace CouchNet.Model
{
    [JsonObject]
    public class ResultFragment
    {
        [JsonProperty(PropertyName = "rev")]
        public string Revision { get; set; }
    }
}