using System.Diagnostics;

using CouchNet.Impl;
using CouchNet.Tests.Integration.Model;
using NUnit.Framework;

namespace CouchNet.Tests.Integration
{
    [TestFixture]
    public class ViewFixture
    {
        [Test]
        public void KeyMatch_String()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "integrationtest");
            var view = db.GetView("example", "stringtest");

            var query = new CouchViewQuery().Key("apple");

            var results = db.ExecuteView<TestEntity>(view, query);

            Debug.WriteLine("-----------------------------------------");

            if (results.IsOk)
            {
                if (results.HasResults)
                {
                    foreach (var result in results)
                    {
                        Debug.WriteLine("Name : " + result.name);
                    }
                }
                else
                {
                    Debug.WriteLine("No results found :(");
                }
            }
            else
            {
                Debug.WriteLine("Error: " + results.Response.ErrorType + " / " + results.Response.ErrorMessage);
            }

            Assert.IsTrue(results.IsOk);
            Assert.Greater(results.Count, 0);
        }

        [Test]
        public void KeyMatch_String_LimitZero_ShouldReturnTotals()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "integrationtest");
            var view = db.GetView("example", "stringtest");

            var query = new CouchViewQuery().Key("apple").Limit(0);

            var results = db.ExecuteView<TestEntity>(view, query);

            Debug.WriteLine("-----------------------------------------");

            if (results.IsOk)
            {
                if (results.HasResults)
                {
                    foreach (var result in results)
                    {
                        Debug.WriteLine("Name : " + result.name);
                    }
                }
                else
                {
                    Debug.WriteLine("TotalRows : " + results.TotalRows);
                    Debug.WriteLine("Offset : " + results.Offset);
                }
            }
            else
            {
                Debug.WriteLine("Error: " + results.Response.ErrorType + " / " + results.Response.ErrorMessage);
            }

            Assert.Greater(results.TotalRows, 0);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void KeyMatch_Array()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "integrationtest");
            var view = db.GetView("example", "arraytest");

            var query = new CouchViewQuery().Key(new[] { "apple", "orange" });

            Debug.WriteLine("View : " + view.ToString());
            Debug.WriteLine("Query : " + query.ToString());

            var results = db.ExecuteView<TestEntity>(view, query);

            Debug.WriteLine("-----------------------------------------");

            if (results.IsOk)
            {
                if (results.HasResults)
                {
                    foreach (var result in results)
                    {
                        Debug.WriteLine("Name : " + result.name);
                    }
                }
                else
                {
                    Debug.WriteLine("No results found :(");
                }
            }
            else
            {
                Debug.WriteLine("Error: " + results.Response.ErrorType + " / " + results.Response.ErrorMessage);
            }

            Assert.IsTrue(results.IsOk);
            Assert.Greater(results.Count, 0);
        }

        [Test]
        public void KeyMatch_Array_LimitZero_ShouldReturnTotals()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "integrationtest");
            var view = db.GetView("example", "arraytest");

            var query = new CouchViewQuery().Key(new[] { "apple", "orange" }).Limit(0);

            var results = db.ExecuteView<TestEntity>(view, query);

            Debug.WriteLine("-----------------------------------------");

            if (results.IsOk)
            {
                if (results.HasResults)
                {
                    foreach (var result in results)
                    {
                        Debug.WriteLine("Name : " + result.name);
                    }
                }
                else
                {
                    Debug.WriteLine("TotalRows : " + results.TotalRows);
                    Debug.WriteLine("Offset : " + results.Offset);
                }
            }
            else
            {
                Debug.WriteLine("Error: " + results.Response.ErrorType + " / " + results.Response.ErrorMessage);
            }

            Assert.Greater(results.TotalRows, 0);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void KeyMatch_Array_WildCardQuery()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "integrationtest");
            var view = db.GetView("example", "arraytest");

            var query = new CouchViewQuery().Key(new[] { "apple", "cats" }).EndKey(new[] { "apple", "*" });

            Debug.WriteLine("View : " + view.ToString());
            Debug.WriteLine("Query : " + query.ToString());

            var results = db.ExecuteView<TestEntity>(view, query);

            Debug.WriteLine("-----------------------------------------");

            if (results.IsOk)
            {
                if (results.HasResults)
                {
                    foreach (var result in results)
                    {
                        Debug.WriteLine("Name : " + result.name);
                    }
                }
                else
                {
                    Debug.WriteLine("No results found :(");
                }
            }
            else
            {
                Debug.WriteLine("Error: " + results.Response.ErrorType + " / " + results.Response.ErrorMessage);
            }

            Assert.IsTrue(results.IsOk);
            Assert.Greater(results.Count, 0);
        }

        [Test]
        public void TempView_CanSerialize()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "unittest");
            var temp = new CouchTempView { Map = "function(doc) {\n  emit(null, doc);\n}" };
            var results = db.ExecuteView<BusinessCard>(temp, new CouchViewQuery());

            if (results.IsOk)
            {
                if (results.HasResults)
                {
                    foreach (var result in results)
                    {
                        Debug.WriteLine(result.Name);
                    }
                }
                else
                {
                    Debug.WriteLine("Nothing found :(");
                }
            }
            else
            {
                Debug.WriteLine("Error: " + results.Response.ErrorType + " / " + results.Response.ErrorMessage);
            }
        }

        public void DesignDoc()
        {
            //What purpose does a DesignDocument object serve ?
            //1>Info function
            //2>list of views

            //var db = new CouchDatabase(_conn, "unitest");

            //var newView = new CouchView("example", "monkeyview");
            //newView.Map = "blah";
            //newView.Reduce = "blah";
            //newView.Langauge = "badgers";

            //db.SaveChanges(newView);

            //ICouchDesignDocument doc = db.GetDesignDocument("example");
            //var view = doc.View["monkeyview"];
            //var info = doc.Info();
            //doc.SaveChanges();

            //db.ExecuteView<BusinessCard>(newView, new BaseViewQuery());
        }
    }

    public class TestEntity : CouchDocument
    {
        public string name { get; set; }
        public string[] tags { get; set; }
    }
}