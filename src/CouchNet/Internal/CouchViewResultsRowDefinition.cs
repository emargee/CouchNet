using Newtonsoft.Json;

namespace CouchNet.Internal
{
    [JsonObject]
    internal class CouchViewResultsRowDefinition<T> where T : ICouchDocument
    {
        private object _key;

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "key")]
        public object Key
        {
            get { return _key; }
            set
            {
                _key = value.GetType() == typeof(object[]) ? ((object[])value)[0] : value;
            }
        }

        [JsonProperty(PropertyName = "value")]
        public T Value { get; set; }
    }
}