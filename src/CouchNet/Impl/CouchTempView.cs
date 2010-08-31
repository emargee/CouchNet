using CouchNet.Internal;
using Newtonsoft.Json;

namespace CouchNet.Impl
{
    public class CouchTempView : ICouchView
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };

        public string DesignDocument { get; internal set; }
        public string Name { get; internal set; }

        public string Map { get; set; }
        public string Reduce { get; set; }

        public string Langauge { get; set; }

        public CouchTempView()
        {
            DesignDocument = string.Empty;
            Name = "_temp_view";
            Langauge = "javascript";
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);    
        }

        public string ToJson()
        {
            var temp = new CouchTempViewDefinition { Language = Langauge, Map = Map, Reduce = Reduce };
            return JsonConvert.SerializeObject(temp, Formatting.None, _settings);
        }
    }
}