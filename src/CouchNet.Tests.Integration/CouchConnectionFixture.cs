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
            var svc = new CouchService(conn);
            var db = svc.Database("unittest");
            var card = new BusinessCard { Name = "Billy Smith", Employer = "Smith Industries", JobTitle = "Eating Horses" };

            var response = db.Add(card);

            if(response.IsOk)
            {
                card = db.Get<BusinessCard>(response.Id);
                Assert.IsNotNullOrEmpty(card.Id);

                var resp = conn.Copy("/unittest/" + card.Id, card.Id + "-22");
            }
        }
    }
}