using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CouchNet.Impl;
using Newtonsoft.Json;

namespace CouchNet.Tests.Model
{
    public class BadJsonPropertyClass : CouchDocument
    {
        [JsonProperty("pie")]
        public string Pie = "Yum";

        public string pie = "PieChart!";
    }
}
