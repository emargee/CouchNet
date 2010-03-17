using System;
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
            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(5984, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_FromUri_CanCreate()
        {
            var ch = new CouchConnection(new Uri("http://localhost"));
            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(80, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreate()
        {
            var ch = new CouchConnection("http://localhost");

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(80, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateHttps()
        {
            var ch = new CouchConnection("https://localhost");

            Assert.AreEqual("https", ch.Address.Scheme);
            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(443, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateWithoutHttp()
        {
            var ch = new CouchConnection("localhost");

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(80, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreateWithEndSlash()
        {
            var ch = new CouchConnection("localhost/");

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(80, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_JustUrl_CanCreatewithPortInString()
        {
            var ch = new CouchConnection("http://localhost:80/");

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(80, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_UrlandPort_CanCreate()
        {
            var ch = new CouchConnection("http://localhost/", 1234);

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(1234, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_UrlandPort_WithoutHttp()
        {
            var ch = new CouchConnection("localhost", 1234);

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(1234, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_UrlandPort_WithTrailingSlash()
        {
            var ch = new CouchConnection("localhost/", 1234);

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(1234, ch.Address.Port);
            Assert.AreEqual("application/json", ch.Encoding);
        }

        [Test]
        public void Ctor_Full_CanCreate()
        {
            var ch = new CouchConnection("http://localhost/", 1234, "application/xml");

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(1234, ch.Address.Port);
            Assert.AreEqual("application/xml", ch.Encoding);
        }

        [Test]
        public void Ctor_SetHeader_PassesToRequest()
        {
            var ch = new CouchConnection("http://localhost/");
            ch.CustomHeaders.Add("User-Agent", "admin");
            ch.Get("/");

            Assert.AreEqual("localhost", ch.Address.Host);
            Assert.AreEqual(80, ch.Address.Port);
            Assert.AreEqual("admin", ch.Client.DefaultHeaders["User-Agent"]);
        }    
    }
}