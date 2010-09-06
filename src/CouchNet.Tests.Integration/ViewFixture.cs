using System.Collections.Specialized;
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

            var query = new CouchViewQuery().Key("apple");

            var results = db.DesignDocument("example").ExecuteView<TestEntity>("stringtest", query);

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

            var query = new CouchViewQuery().Key("apple").Limit(0);

            var results = db.DesignDocument("example").ExecuteView<TestEntity>("stringtest", query);

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

            var query = new CouchViewQuery().Key(new[] { "apple", "orange" });

            var results = db.DesignDocument("example").ExecuteView<TestEntity>("arraytest", query);

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

            var query = new CouchViewQuery().Key(new[] { "apple", "orange" }).Limit(0);

            var results = db.DesignDocument("example").ExecuteView<TestEntity>("arraytest", query);

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

            var query = new CouchViewQuery().Key(new[] { "apple", "cats" }).EndKey(new[] { "apple", "*" });

            var results = db.DesignDocument("example").ExecuteView<TestEntity>("arraytest", query);

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
            var results = db.ExecuteTempView<BusinessCard>(temp, new CouchViewQuery());

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

        [Test]
        public void Show_CanExecuteCorrectly()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "unittest");

            var result = db.DesignDocument("example").ExecuteShow("test");
            Debug.WriteLine(result.Output);

            result = db.DesignDocument("example").ExecuteShow("test", "e99b84cd49824eaf90b5f5c164b39e12");
            Debug.WriteLine(result.Output);

            result = db.DesignDocument("example").ExecuteShow("test", "e99b84cd49824eaf90b5f5c164b39e12", new NameValueCollection { { "format", "xml" } });
            Debug.WriteLine(result.Output);
        }

        [Test]
        public void List_CanExecuteCorrectly()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "unittest");

            var result = db.DesignDocument("example").ExecuteList("htmlList", "test", new CouchViewQuery());

            Debug.WriteLine(result.Output);

            var query = new CouchViewQuery().Key("e99b84cd49824eaf90b5f5c164b39e12");
            result = db.DesignDocument("example").ExecuteList("htmlList", "test", query );

            Debug.WriteLine(result.Output);
        }

        [Test]
        public void NewSyntax()
        {
            var conn = new CouchConnection("http://localhost:5984/");
            var db = new CouchDatabase(conn, "unittest");
            var doc = db.DesignDocument("example");

            //-[ View Execution ]---------------------------------------------

            var results = doc.ExecuteView<BusinessCard>("test", new CouchViewQuery());

            if (results.IsOk && results.HasResults)
            {
                foreach (var i in results)
                {
                    Debug.WriteLine(i.Name);
                }
            }

            //-[ Add New View ]---------------------------------------------

            //var newView = new CouchView("myNewView");
            //newView.Map = "function(test)";
            //newView.Reduce = "function(blah);
            //doc.Add(newView);
            //db.SaveChanges(doc);

            //-[ Edit Existing View ]---------------------------------------------


            //var oldView = doc.View("test");
            //Debug.WriteLine(oldView.Map);

            //doc.View("test").Map = "function(new)"; //Set HasChanges()

            //Debug.WriteLine(doc.View("test").Map);

            //db

            //-[ Show ]---------------------------------------------
            //var results = doc.ExecuteShow(doc.Show("test"));
            //var results = doc.ExecuteShow(doc.Show("test"),"docId");
            var showResults = doc.ExecuteShow("test", "e99b84cd49824eaf90b5f5c164b39e12", new NameValueCollection { { "format", "xml" } });

            if (showResults.IsOk)
            {
                Debug.WriteLine(showResults.Output);
            }
        }
    }

    public class TestEntity : CouchDocument
    {
        public string name { get; set; }
        public string[] tags { get; set; }
    }
}