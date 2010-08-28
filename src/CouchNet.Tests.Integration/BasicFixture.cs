using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using CouchNet.Impl;
using CouchNet.Tests.Integration.Model;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CouchNet.Tests.Integration
{
    [TestFixture]
    public class BasicFixture
    {
        //private ILookup<string, string> testy; 

        [Test]
        [Ignore("Needs running copy of CouchDB")]
        public void QuickTest()
        {
            var conn = new CouchConnection("http://localhost", 5984);
            var db = new CouchDatabase(conn, "unittest");

            // -- ADDING FROM NEW --- 

            var card = new BusinessCard { Name = "Bob Smith", Employer = "GiantMart", JobTitle = "Manager" };

            var resp = db.Add(card);

            if (resp.IsOk)
            {
                Debug.WriteLine("Added ! Id : " + resp.Id + " / Revision: " + resp.Revision);
            }
            else
            {
                Debug.WriteLine("Problem updating : " + resp.ErrorType + " / Reason : " + resp.ErrorMessage);
            }

            // -- RETREIVEING & UPDATING -- 

            var newCard = db.Get<BusinessCard>(resp.Id);

            newCard.Name = "Bobbyd Smith";

            resp = db.Save(newCard);

            if (resp.IsOk)
            {
                Debug.WriteLine("Updated ! Id : " + resp.Id + " / Revision: " + resp.Revision);
            }

            // -- DELETEING --

            resp = db.Delete(resp.Id, resp.Revision);

            if (resp.IsOk)
            {
                Debug.WriteLine("Deleted ! Id : " + resp.Id + " / Revision: " + resp.Revision);
            }
        }

        [Test]
        public void ViewSyntaxCheck()
        {
            var conn = new CouchConnection("http://localhost", 5984);
            var db = new CouchDatabase(conn, "unittest");

            var view = new CouchView("example", "test");
            var query = new CouchViewQuery { Key = "e99b84cd49824eaf90b5f5c164b39e12" };

            var results = db.ExecuteView<BusinessCard>(view, query);

            if (results.IsOk)
            {
                Debug.WriteLine("Error: " + results.Response.ErrorType + " / Reason : " + results.Response.ErrorMessage);
                return;
            }

            if(results.HasResults)
            {
                foreach (var result in results)
                {
                    Debug.WriteLine("Name: " + result.Name + " - Id : " + result.Id);
                }    
            }
        }

        [Test]
        public void ExampleSyntax()
        {
            //var conn = new CouchConnection("http://localhost", 5984);
            //var db = new CouchDatabase(conn, "unittest");

            //db.Add(new BusinessCard { Name = "Mr Chicken"});

            //var doc = db.Get<BusinessCard>(db.ServerResponse.Id);
            //var data = File.ReadAllBytes("hats.jpg");
            //db.Attach(doc, "hats.jpg", "image/jpg", data);

            ////OR

            //doc.Attach("hats.jpg", "image/jpg", data); // Load into temp Dictionary<string, KeyValuePair<string, byte[]>>       
            //db.Save(doc); //Loop through temp and PUT the data.

            //doc.Attachments.Count();
            //doc.Attachments["hats.jpg"].Length;
            //doc.Attachments["hats.jpg"].RevisionPosition;
            //doc.Attachments["hats.jpg"].ContentType;

            //Stream att = doc.GetAttachment("hats.jpg");

        }


    }


}