using System.Diagnostics;
using CouchNet.Impl;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CouchNet.Tests.Integration
{
    [TestFixture]
    public class BasicFixture
    {
        [Test]
        [Ignore("Needs running copy of CouchDB")]
        public void QuickTest()
        {
            var conn = new CouchConnection("http://localhost", 5984);
            var db = new CouchDatabase(conn, "unittest");

            // -- ADDING FROM NEW --- 

            var card = new BusinessCard {Name = "Bob Smith", Employer = "GiantMart", JobTitle = "Manager"};

            db.Add(card);

            if(db.ServerResponse.Ok)
            {
                Debug.WriteLine("Added ! Id : " + db.ServerResponse.Id + " / Revision: " + db.ServerResponse.Revision );
            }
            else
            {
                Debug.WriteLine("Problem updating : " + db.ServerResponse.Error + " / Reason : " + db.ServerResponse.Reason);
            }
            
            // -- RETREIVEING & UPDATING -- 

            var newCard = db.Get<BusinessCard>(db.ServerResponse.Id);

            newCard.Name = "Bobbyd Smith";

            db.Save(newCard);

            if(db.ServerResponse.Ok)
            {
                Debug.WriteLine("Updated ! Id : " + db.ServerResponse.Id + " / Revision: " + db.ServerResponse.Revision);    
            }

            // -- DELETEING --

            db.Delete(db.ServerResponse.Id, db.ServerResponse.Revision);

            if(db.ServerResponse.Ok)
            {
                Debug.WriteLine("Deleted ! Id : " + db.ServerResponse.Id + " / Revision: " + db.ServerResponse.Revision);
            }           
        }
    }

    public class BusinessCard : CouchDocument
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Employer { get; set; }
    }
}