using NUnit.Framework;

namespace CouchNet.Tests.Integration
{
    [TestFixture]
    public class CouchServiceFixture
    {
        [Test]
        public void CreateDatabase_HeadCheck()
        {
            var svc = new CouchService("http://www.couchdbtest.com:5984");
            var db = svc.Database("monkeytennis");
        }
    }
}