using System;
using System.Diagnostics;
using CouchNet.Impl;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

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
            var view = new CouchView("example", "stringtest");

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
            var view = new CouchView("example", "stringtest");

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
            var view = new CouchView("example", "arraytest");

            var query = new CouchViewQuery().Key(new[] {"apple", "orange"});

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
            var view = new CouchView("example", "arraytest");

            var query = new CouchViewQuery().Key(new[] {"apple", "orange"}).Limit(0);

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
            var view = new CouchView("example", "arraytest");

            var query = new CouchViewQuery().Key(new[] {"apple", "cats"}).EndKey(new[] {"apple", "*"});

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
    }

    public class TestEntity : CouchDocument
    {
        public string name { get; set; }
        public string[] tags { get; set; }
    }
}