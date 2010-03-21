using System;
using System.Linq;
using System.Net;
using CouchNet.Impl;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchConnectionFixture
    {
        [Test]
        public void Ctor_Empty_CanCreateLocalUrl()
        {
            var ch = new CouchConnection();
            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(5984, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_FromUri_CanCreate()
        {
            var ch = new CouchConnection(new Uri("http://localhost"));
            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreate()
        {
            var ch = new CouchConnection("http://localhost");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateHttps()
        {
            var ch = new CouchConnection("https://localhost");

            Assert.AreEqual("https", ch.BaseAddress.Scheme);
            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(443, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateWithoutHttp()
        {
            var ch = new CouchConnection("localhost");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateWithEndSlash()
        {
            var ch = new CouchConnection("localhost/");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreatewithPortInString()
        {
            var ch = new CouchConnection("http://localhost:80/");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(80, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_UrlandPort_CanCreate()
        {
            var ch = new CouchConnection("http://localhost/", 1234);

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_UrlandPort_WithoutHttp()
        {
            var ch = new CouchConnection("localhost", 1234);

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_UrlandPort_WithTrailingSlash()
        {
            var ch = new CouchConnection("localhost/", 1234);

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/json", ch.RequestEncoding);
        }

        [Test]
        public void Ctor_Full_CanCreate()
        {
            var ch = new CouchConnection("http://localhost/", 1234, "application/xml");

            Assert.AreEqual("localhost", ch.BaseAddress.Host);
            Assert.AreEqual(1234, ch.BaseAddress.Port);
            Assert.AreEqual("application/xml", ch.RequestEncoding);
        }

        [Test]
        public void Header_SetHeader_AppearsInCollection()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader("User-Agent", "user1");

            Assert.AreEqual(1, ch.Client.DefaultHeaders.Count());
        }

        [Test]
        public void Header_Duplicate_ShouldOverwrite()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader("User-Agent", "user1");
            ch.SetHeader("User-Agent", "user2");

            Assert.AreEqual(1, ch.Client.DefaultHeaders.Count());
            Assert.AreEqual("user2", ch.Client.DefaultHeaders["User-Agent"]);
        }

        [Test]
        public void Header_Nulls_DoesNothing()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader(null,null);

            Assert.AreEqual(0, ch.Client.DefaultHeaders.Count());
        }

        [Test]
        public void Header_ClearHeader_ShouldBeZero()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.SetHeader("User-Agent", "user1");
            ch.SetHeader("User-Agent", "user2");
            ch.ClearHeaders();

            Assert.AreEqual(0, ch.Client.DefaultHeaders.Count());
        }

        [Test]
        public void Header_SetCredentials_EncodesCorrectly()
        {
            var ch = new CouchConnection("http://localhost");
            ch.SetCredentials("user","pass");

            Assert.AreEqual("Basic dXNlcjpwYXNz",ch.Client.DefaultHeaders["Authorization"]);
        }

        [Test]
        public void Header_SetCredentials_UsingNetworkCredential()
        {
            var ch = new CouchConnection("http://localhost");
            ch.SetCredentials(new NetworkCredential("user","pass"));

            Assert.AreEqual("Basic dXNlcjpwYXNz", ch.Client.DefaultHeaders["Authorization"]);
        }


        [Test]
        public void Header_DisableCache_SetsCorrectly()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.DisableCache();

            Assert.AreEqual("no-cache", ch.Client.DefaultHeaders["Cache-Control"]);
        }

    }
}