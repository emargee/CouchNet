using System.Diagnostics;
using System.Net;
using CouchNet.Impl;
using CouchNet.Impl.Caching;
using CouchNet.Tests.Integration.Model;
using NUnit.Framework;

namespace CouchNet.Tests.Integration
{
    [TestFixture]
    public class CouchCacheFixture
    {
        [Test]
        public void CacheHeaderTest()
        {
            var conn = new CouchConnection("http://localhost:5984");

            var cache = new HttpRuntimeCache();
            cache.ExpirationWindow = 5;

            conn.Cache = cache;

            var resp = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Status Code : " + resp.StatusCode);
            Debug.WriteLine("--------------------------------");

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);

            var resp2 = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Status Code : " + resp2.StatusCode);
            Debug.WriteLine("--------------------------------");

            Assert.AreEqual(HttpStatusCode.NotModified, resp2.StatusCode);

            resp2 = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Status Code : " + resp2.StatusCode);
            Debug.WriteLine("--------------------------------");

            Assert.AreEqual(HttpStatusCode.NotModified, resp2.StatusCode);

            conn.DisableCache();

            resp2 = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Status Code : " + resp2.StatusCode);
            Debug.WriteLine("--------------------------------");

            Assert.AreEqual(HttpStatusCode.OK, resp2.StatusCode);

            conn.EnableCache();

            resp2 = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Status Code : " + resp2.StatusCode);
            Debug.WriteLine("--------------------------------");

            Assert.AreEqual(HttpStatusCode.NotModified, resp2.StatusCode);

            resp2 = conn.Get("/integrationtest/d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Status Code : " + resp2.StatusCode);
            Debug.WriteLine("--------------------------------");

            Assert.AreEqual(HttpStatusCode.NotModified, resp2.StatusCode);
        }

        [Test]
        public void CacheObjectTest()
        {
            var conn = new CouchConnection("http://localhost:5984");

            var cache = new HttpRuntimeCache { ExpirationWindow = 5 };
            conn.Cache = cache;

            var svc = new CouchService(conn);
            var db = svc.GetDatabase("integrationtest");

            var resp = db.Get<BusinessCard>("d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Name : " + resp.Name);
            Debug.WriteLine("--------------------------------");

            var resp2 = db.Get<BusinessCard>("d1d2bac2b4e65baf10be20bf08000189");
            Debug.WriteLine("Name : " + resp2.Name);
            Debug.WriteLine("--------------------------------");
        }
    }
}