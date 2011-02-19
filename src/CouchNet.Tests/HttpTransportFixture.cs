using System;
using CouchNet.HttpTransport.Impl;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class HttpTransportFixture
    {
        [Test]
        public void Factory_String_CanCreate()
        {
            var fac = new HttpTransportFactory();
            var ht = fac.Create("http://localhost:5984");
            Assert.IsNotNull(ht);
        }

        [Test]
        public void Factory_Uri_CanCreate()
        {
            var fac = new HttpTransportFactory();
            var ht = fac.Create(new Uri("http://localhost:5984"));
            Assert.IsNotNull(ht);
        }

        [Test]
        public void Ctor_String_CanCreate()
        {
            var ht = new HttpTransport.Impl.HttpTransport("http://localhost:5984");
            Assert.IsNotNull(ht);
            Assert.IsNotNull(ht.Client);
        }

        [Test]
        public void Ctor_Uri_CanCreate()
        {
            var ht = new HttpTransport.Impl.HttpTransport(new Uri("http://localhost:5984"));
            Assert.IsNotNull(ht);
            Assert.IsNotNull(ht.Client);
        }
    }
}