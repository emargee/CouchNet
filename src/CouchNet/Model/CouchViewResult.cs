using System.Collections.Generic;
using Newtonsoft.Json;

namespace CouchNet.Model
{
    [JsonObject]
    public class CouchViewResult<T>
    {
        [JsonProperty(PropertyName = "total_rows")]
        public int TotalRows { get; set; }

        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; }

        [JsonProperty(PropertyName = "rows")]
        public IEnumerable<T> Rows { get; set; }
    }
}