using System;
using CouchNet.Base;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchViewQueryFixture
    {
        [Test]
        public void BaseQuery_NothingSet_CanSerialize()
        {
            var bq = new BaseViewQuery();
            Assert.IsEmpty(bq.ToString());
        }

        [Test]
        public void BaseQuery_AllSettings_CanSerialize()
        {
            var bq = new BaseViewQuery();
            bq.DisableReduce = true;
            bq.Group = true;
            bq.GroupLevel = 1;
            bq.IncludeDocs = true;
            bq.Limit = 10;
            bq.Skip = 10;
            bq.SortDescending = true;
            bq.UseStale = true;

            Assert.AreEqual("?limit=10&skip=10&stale=ok&descending=true&group=true&group_level=1&reduce=false&include_docs=true", bq.ToString());
        }

        [Test]
        public void KeyMatchViewQuery_String_CanSerialize()
        {
            var sq = new CouchViewQuery();
            sq.Key = "example";
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
    }
}