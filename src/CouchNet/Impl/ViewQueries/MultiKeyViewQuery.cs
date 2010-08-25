using System.Collections.Generic;
using System.Linq;
using CouchNet.Utils;
using Newtonsoft.Json;

namespace CouchNet.Impl.ViewQueries
{
    public class MultiKeyViewQuery : BaseViewQuery
    {
        public IEnumerable<string> Keys { get; set; }

        public string SerializeKeys()
        {
            var keys = new { keys = Keys.ToArray() };
            return JsonConvert.SerializeObject(keys);
        }
    }
}