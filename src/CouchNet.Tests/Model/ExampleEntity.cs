using CouchNet.Impl;
using CouchNet.Model;
using Newtonsoft.Json;

namespace CouchNet.Tests.Model
{
    [JsonObject]
    public class ExampleEntity : CouchDocument
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; }

        [JsonProperty(PropertyName = "isAlive")]
        public bool IsAlive { get; set; }
    }
}