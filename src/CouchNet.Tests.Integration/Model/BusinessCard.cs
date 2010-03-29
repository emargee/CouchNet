using CouchNet.Impl;
using Newtonsoft.Json;

namespace CouchNet.Tests.Integration.Model
{
    public class BusinessCard : CouchDocument
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Employer { get; set; }
    }
}