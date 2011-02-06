using System;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchViewQueryFixture
    {
        [Test]
        public void CouchViewQuery_NothingSet_CanSerialize()
        {
            var bq = new CouchViewQuery();
            Assert.IsEmpty(bq.ToString());
        }

        [Test]
        public void CouchViewQuery_AllSettings_CanSerialize()
        {
            var bq = new CouchViewQuery();
            bq.DisableInclusiveEnd = true;
            bq.DisableReduce = true;
            bq.Group = true;
            bq.GroupLevel = 1;
            bq.IncludeDocs = true;
            bq.Limit = 10;
            bq.Skip = 10;
            bq.SortDescending = true;
            bq.UseStale = true;
            bq.StartDocId = "1234";
            bq.EndDocId = "5678";

            Assert.AreEqual("?limit=10&skip=10&stale=ok&descending=true&group=true&group_level=1&reduce=false&include_docs=true&inclusive_end=false&startkey_docid=%221234%22&endkey_docid=%225678%22", bq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_String_CanSerialize()
        {
            var sq = new CouchViewQuery();
            sq.Key = "example";
            Assert.AreEqual("?key=%22example%22", sq.ToString());
        }

        [Test]
        public void KeyMatchViewQueryUsingCtor_String_CanSerialize()
        {
            var sq = new CouchViewQuery("example");
            Assert.AreEqual("?key=%22example%22", sq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_Array_CanSerialize()
        {
            var sq = new CouchViewQuery();
            sq.Key = new[] {"apple", "orange"};
            Assert.AreEqual("?key=%5b%22apple%22%2c%22orange%22%5d", sq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_String_BaseOptionsPassThrough()
        {
            var sq = new CouchViewQuery();
            sq.Key = "example";
            sq.Limit = 0;
            Assert.AreEqual("?limit=0&key=%22example%22", sq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_EndKey_String_CanSerialize()
        {
            var seq = new CouchViewQuery();
            seq.Key = "apple";
            seq.EndKey = "badgers";
            Assert.AreEqual("?startkey=%22apple%22&endkey=%22badgers%22",seq.ToString());
        }

        [Test]
        public void KeyMatchViewQueryUasingCtor_EndKey_String_CanSerialize()
        {
            var seq = new CouchViewQuery("apple","badgers");
            Assert.AreEqual("?startkey=%22apple%22&endkey=%22badgers%22", seq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_EndKey_Array_CanSerialize()
        {
            var seq = new CouchViewQuery();
            seq.Key = new[] { "apple", "cats" };
            seq.EndKey = new[] { "apple", "orange" };
            Assert.AreEqual("?startkey=%5b%22apple%22%2c%22cats%22%5d&endkey=%5b%22apple%22%2c%22orange%22%5d", seq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_WildcardString_CanSerialize()
        {
            var seq = new CouchViewQuery();
            seq.Key = "apple";
            seq.EndKey = "apple*";
            Assert.AreEqual("?startkey=%22apple%22&endkey=%22apple%5cu9999%22", seq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_WildcardArray_CanSerialize()
        {
            var seq = new CouchViewQuery();
            seq.Key = new[] { "apple", "cats" };
            seq.EndKey = new[] { "apple", "*" };
            Assert.AreEqual("?startkey=%5b%22apple%22%2c%22cats%22%5d&endkey=%5b%22apple%22%2c%7b%7d%5d", seq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_NonMatchingTypes_ShouldThrow()
        {
            var seq = new CouchViewQuery();
            seq.Key = new[] { "apple", "cats" };
            seq.EndKey = "apple";

            Assert.Throws(typeof (ArgumentException), () => seq.ToString());
        }

        [Test]
        public void ViewQueryDsl_BasicStringTest_CanSerialize()
        {
            var seq = new CouchViewQuery();
            seq.Key("apple").EndKey("apple*").Limit(2).DisableReduce().DisableInclusiveEnd().StartDocId("1234").EndDocId("5678").IncludeDocs().Skip(5);
            Assert.AreEqual("?limit=2&skip=5&reduce=false&include_docs=true&inclusive_end=false&startkey=%22apple%22&endkey=%22apple%5cu9999%22&startkey_docid=%221234%22&endkey_docid=%225678%22", seq.ToString());
        }
    }
}