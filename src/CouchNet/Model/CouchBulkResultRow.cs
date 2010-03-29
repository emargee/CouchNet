using Newtonsoft.Json;

namespace CouchNet.Model
{
    [JsonObject]
    public class CouchBulkResultRow<T>
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
        public ResultFragment Value { get; set; }

        [JsonProperty(PropertyName = "doc")]
        public T Document { get; set; }
    }
}