using System.Linq;
using System.Net;
using CouchNet.Impl;
using CouchNet.Tests.Integration.Model;
using NUnit.Framework;

namespace CouchNet.Tests.Integration
{
    [TestFixture]
    public class CouchDatabaseFixture
    {
        [Test]
        public void Add_Single_ReturnCodeIsCorrect()
        {
            var conn = new CouchConnection("http://127.0.0.1", 5984);
            var db = new CouchDatabase(conn, "integrationtest");

            var card = new BusinessCard { Name = "Bob Smith", Employer = "GiantMart", JobTitle = "Manager" };

            var resp = db.Add(card);

            Assert.IsNotNull(resp);
            Assert.IsNotNull(db.RawResponse);
            Assert.AreEqual(HttpStatusCode.Created, db.RawResponse.StatusCode);
        }

        [Test]
        public void Update_Single_ReturnCodeIsCorrect()
        {
            var conn = new CouchConnection("http://127.0.0.1", 5984);
            var db = new CouchDatabase(conn, "integrationtest");

            var card = new BusinessCard { Name = "Bob Smith", Employer = "GiantMart", JobTitle = "Manager" };

            var resp = db.Add(card);
            var newCard = db.Get<BusinessCard>(resp.Id);
            newCard.Name = "Bobby Smith";
            resp = db.Save(newCard);

            Assert.IsNotNull(resp);
            Assert.IsNotNull(db.RawResponse);
            Assert.AreEqual(HttpStatusCode.Created, db.RawResponse.StatusCode);
        }
    
        [Test]
        public void Update_Multiple_CreatesCorrectly()
        {
            var conn = new CouchConnection("http://127.0.0.1", 5984);
            var db = new CouchDatabase(conn, "integrationtest");

            var card1 = new BusinessCard { Name = "Bob Smith", Employer = "GiantMart", JobTitle = "Manager" };
            var card2 = new BusinessCard { Name = "Jack Smith", Employer = "MediumMart", JobTitle = "Manager" };
            var card3 = new BusinessCard { Name = "Bill Smith", Employer = "TinyMart", JobTitle = "Manager" };

            var resp = db.Add(new[] {card1, card2, card3}).ToList();
            
            Assert.IsNotNull(resp);
            Assert.AreEqual(3, resp.Count);
            Assert.IsNotNull(db.RawResponse);
            Assert.AreEqual(HttpStatusCode.Created, db.RawResponse.StatusCode);

            card1 = db.Get<BusinessCard>(resp[0].Id);
            card2 = db.Get<BusinessCard>(resp[1].Id);
            card3 = db.Get<BusinessCard>(resp[2].Id);

            card1.JobTitle = "Chicken";
            card2.JobTitle = "Horse";
            card3.JobTitle = "Monkey";

            resp = db.Save(new[] {card1, card2, card3}).ToList();

            Assert.AreEqual(3, resp.Count);
            Assert.IsTrue(resp[0].Revision.Contains("2-"));
            Assert.IsTrue(resp[1].Revision.Contains("2-"));
            Assert.IsTrue(resp[2].Revision.Contains("2-"));
            Assert.IsNotNull(db.RawResponse);
            Assert.AreEqual(HttpStatusCode.Created, db.RawResponse.StatusCode);
        }
    }
}