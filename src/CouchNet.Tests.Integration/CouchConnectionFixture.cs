using CouchNet.Impl;
using CouchNet.Tests.Integration.Model;
using NUnit.Framework;

namespace CouchNet.Tests.Integration
{
    [TestFixture]
    [Ignore("Requires a running version of CouchDB")]
    public class CouchConnectionFixture
    {
        [Test]
        public void Copy_Ids_CanCopy()
        {
            var conn = new CouchConnection("http://localhost", 5984);
            var db = new CouchDatabase(conn, "unittest");
            var card = new BusinessCard { Name = "Billy Smith", Employer = "Smith Industries", JobTitle = "Eating Horses" };

            db.Add(card);

            if(db.ServerResponse.Ok)
            {
                card = db.Get<BusinessCard>(db.ServerResponse.Id);
                Assert.IsNotNullOrEmpty(card.Id);

                var response = conn.Copy("/unittest/" + card.Id, card.Id + "-22");
            }
        }
    }
}