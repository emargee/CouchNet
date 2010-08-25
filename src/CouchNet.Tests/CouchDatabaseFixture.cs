using System;
using System.Linq;
using System.Net;
using CouchNet.Enums;
using CouchNet.HttpTransport;
using CouchNet.Impl;
using CouchNet.Impl.ViewQueries;
using CouchNet.Tests.Model;
using Moq;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchDatabaseFixture
    {
        private ExampleEntity _exampleObject;
        private Mock<ICouchConnection> _connectionMock;
        private Mock<IHttpResponse> _basicResponse;
        private Mock<IHttpResponse> _notFoundResponse;
        private Mock<IHttpResponse> _missingFieldResponse;
        private Mock<IHttpResponse> _revisionsResponse;
        private Mock<IHttpResponse> _revisionInfoResponse;
        private Mock<IHttpResponse> _statusResponse;
        private Mock<IHttpResponse> _revisionLimitGetResponse;
        private Mock<IHttpResponse> _revisionLimitErrorResponse;
        private Mock<IHttpResponse> _revisionLimitSetResponse;
        private Mock<IHttpResponse> _addDocumentResponse;
        private Mock<IHttpResponse> _addConflictDocumentResponse;
        private Mock<IHttpResponse> _revisionLimitCastErrorResponse;
        private Mock<IHttpResponse> _revisionLimitSetErrorResponse;
        private Mock<IHttpResponse> _statusErrorResponse;
        private Mock<IHttpResponse> _compactResponse;
        private Mock<IHttpResponse> _viewCleanResponse;
        private Mock<IHttpResponse> _compactErrorResponse;
        private Mock<IHttpResponse> _viewCleanErrorResponse;
        private Mock<IHttpResponse> _deleteDocumentResponse;
        private Mock<IHttpResponse> _deleteDocumentErrorResponse;
        private Mock<IHttpResponse> _getAllIdsResponse;
        private Mock<IHttpResponse> _getAllIdsErrorResponse;
        private Mock<IHttpResponse> _getAllObjResponse;
        private Mock<IHttpResponse> _getAllObjMixedResponse;
        private Mock<IHttpResponse> _getManyResponse;
        private Mock<IHttpResponse> _bulkAddResponse;
        private Mock<IHttpResponse> _bulkAddNoIdsResponse;
        private Mock<IHttpResponse> _bulkSaveErrorResponse;
        private Mock<IHttpResponse> _bulkAddErrorResponse;
        private Mock<IHttpResponse> _saveDocumentResponse;
        private Mock<IHttpResponse> _getManyErrorResponse;
        private Mock<IHttpResponse> _copyResponse;
        private Mock<IHttpResponse> _copyErrorResponse;

        [SetUp]
        public void Setup()
        {
            _exampleObject = new ExampleEntity
            {
                Id = "abc123",
                Revision = "946B7D1C",
                Age = 23,
                IsAlive = true,
                Name = "Bob Smith"
            };

            _basicResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _basicResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _basicResponse.Setup(s => s.Data).Returns("{\"name\":\"Bob Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"946B7D1C\"}");

            _missingFieldResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _missingFieldResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _missingFieldResponse.Setup(s => s.Data).Returns("{\"_id\":\"abc123\",\"_rev\":\"946B7D1C\"}");

            _notFoundResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _notFoundResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _notFoundResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionsResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _revisionsResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionsResponse.Setup(s => s.Data).Returns("{\"name\":\"Bob Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"946B7D1C\",\"_revisions\":{\"start\":3,\"ids\":[\"54c962f10a96d251fa5ef6bddc4c98cc\",\"9f7bedc5be50f1e745a7cfa4696507fd\",\"b587febf4b8d4a36493e6a9b41261a4c\"]}}");

            _revisionInfoResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _revisionInfoResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionInfoResponse.Setup(s => s.Data).Returns("{\"name\":\"Bob Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"946B7D1C\",\"_revs_info\":[{\"rev\":\"3-54c962f10a96d251fa5ef6bddc4c98cc\",\"status\":\"available\"},{\"rev\":\"2-9f7bedc5be50f1e745a7cfa4696507fd\",\"status\":\"missing\"},{\"rev\":\"1-b587febf4b8d4a36493e6a9b41261a4c\",\"status\":\"available\"}]}");

            _statusResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _statusResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _statusResponse.Setup(s => s.Data).Returns("{\"db_name\":\"unittest\",\"doc_count\":1,\"doc_del_count\":30,\"update_seq\":85,\"purge_seq\":0,\"compact_running\":false,\"disk_size\":53339,\"instance_start_time\":\"1268879146201288\",\"disk_format_version\":4}");

            _statusErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _statusErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _statusErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionLimitGetResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _revisionLimitGetResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionLimitGetResponse.Setup(s => s.Data).Returns("1000");

            _revisionLimitSetResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _revisionLimitSetResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _revisionLimitSetResponse.Setup(s => s.Data).Returns("1000");

            _revisionLimitSetErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _revisionLimitSetErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _revisionLimitSetErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionLimitErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _revisionLimitErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _revisionLimitErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _revisionLimitCastErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _revisionLimitCastErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _revisionLimitCastErrorResponse.Setup(s => s.Data).Returns("Chickens");

            _addDocumentResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _addDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Created);
            _addDocumentResponse.Setup(s => s.Data).Returns("{\"ok\":true, \"id\":\"some_doc_id\", \"rev\":\"2774761002\"}");

            _addConflictDocumentResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _addConflictDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Conflict);
            _addConflictDocumentResponse.Setup(s => s.Data).Returns("{\"error\":\"conflict\",\"reason\":\"Document update conflict.\"}");

            _compactResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _compactResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _compactResponse.Setup(s => s.Data).Returns("{\"ok\":true}");

            _compactErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _compactErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _compactErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _viewCleanResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _viewCleanResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            _viewCleanResponse.Setup(s => s.Data).Returns("{\"ok\":true}");

            _viewCleanErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _viewCleanErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _viewCleanErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _deleteDocumentResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _deleteDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _deleteDocumentResponse.Setup(s => s.Data).Returns("{\"ok\":true,\"id\":\"some_id\",\"rev\":\"2-1234\"}");

            _deleteDocumentErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _deleteDocumentErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Conflict);
            _deleteDocumentErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"conflict\",\"reason\":\"Document update conflict.\"}");

            _getAllIdsResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _getAllIdsResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _getAllIdsResponse.Setup(s => s.Data).Returns("{\"total_rows\":2,\"offset\":0,\"rows\":[{\"id\":\"4c51bc81501dd2ee3d20e981a8000562\",\"key\":\"4c51bc81501dd2ee3d20e981a8000562\",\"value\":{\"rev\":\"1-0aaca901d8459d637c4cdc143dc49f65\"}},{\"id\":\"attachment_doc\",\"key\":\"attachment_doc\",\"value\":{\"rev\":\"2-dce2006ce41f3ab6c3e6e3b9e6bca1cb\"}}]}");

            _getAllIdsErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _getAllIdsErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _getAllIdsErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _getAllObjResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _getAllObjResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _getAllObjResponse.Setup(s => s.Data).Returns("{\"total_rows\":2,\"offset\":0,\"rows\":[{\"id\":\"abc123\",\"key\":\"abc123\",\"value\":{\"rev\":\"1-946B7D1C\"},\"doc\":{\"name\":\"Fred Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"1-946B7D1C\"} },{\"id\":\"abc456\",\"key\":\"abc456\",\"value\":{\"rev\":\"2-DCE2006C\"},\"doc\":{\"name\":\"Bill Smith\",\"age\":27,\"isAlive\":true,\"_id\":\"abc456\",\"_rev\":\"2-DCE2006C\"} }]}");

            _getAllObjMixedResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _getAllObjMixedResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _getAllObjMixedResponse.Setup(s => s.Data).Returns("{\"total_rows\":2,\"offset\":0,\"rows\":[{\"id\":\"abc123\",\"key\":\"abc123\",\"value\":{\"rev\":\"1-946B7D1C\"},\"doc\":{\"name\":\"Fred Smith\",\"age\":23,\"isAlive\":true,\"_id\":\"abc123\",\"_rev\":\"1-946B7D1C\"} },{\"id\":\"abc456\",\"key\":\"abc456\",\"value\":{\"rev\":\"2-DCE2006C\"},\"doc\":{\"_id\": \"abc456\",\"_rev\": \"2-DCE2006C\",\"Name\": \"Billy Bob\",\"Telephone\": 1234,\"Fax\": 5678} }]}");

            _getManyResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _getManyResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _getManyResponse.Setup(s => s.Data).Returns("{\"total_rows\":3,\"offset\":0,\"rows\":[{\"id\":\"bar\",\"key\":\"bar\",\"value\":{\"rev\":\"1-4057566831\"},\"doc\":{\"_id\":\"bar\",\"_rev\":\"1-4057566831\",\"name\":\"jim\"}},{\"id\":\"baz\",\"key\":\"baz\",\"value\":{\"rev\":\"1-2842770487\"},\"doc\":{\"_id\":\"baz\",\"_rev\":\"1-2842770487\",\"name\":\"trunky\"}}]}");

            _bulkAddResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _bulkAddResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Created);
            _bulkAddResponse.Setup(s => s.Data).Returns("[{\"id\":\"0\",\"rev\":\"1-f5f3f3e496c72307975a69c73fd53d42\"},{\"id\":\"1\",\"rev\":\"1-8ad0e70d5e6edd474ec190eac2376bde\"}]");

            _bulkAddNoIdsResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _bulkAddNoIdsResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Created);
            _bulkAddNoIdsResponse.Setup(s => s.Data).Returns("[{\"id\":\"d9c308eef23bbfff46826135fb000883\",\"rev\":\"1-f5f3f3e496c72307975a69c73fd53d42\"},{\"id\":\"d9c308eef23bbfff46826135fb00131b\",\"rev\":\"1-8ad0e70d5e6edd474ec190eac2376bde\"}]");

            _bulkAddErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _bulkAddErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _bulkAddErrorResponse.Setup(s => s.Data).Returns("[{\"id\":\"0\",\"error\":\"conflict\",\"reason\":\"Document update conflict.\"},{\"id\":\"1\",\"rev\":\"2-1579510027\"},{\"id\":\"2\",\"rev\":\"2-3978456339\"}]");

            _bulkSaveErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _bulkSaveErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _bulkSaveErrorResponse.Setup(s => s.Data).Returns("[{\"id\":\"0\",\"error\":\"conflict\",\"reason\":\"Document update conflict.\"},{\"id\":\"1\",\"rev\":\"2-1579510027\"},{\"id\":\"2\",\"rev\":\"2-3978456339\"}]");

            _saveDocumentResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _saveDocumentResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Created);
            _saveDocumentResponse.Setup(s => s.Data).Returns("{\"ok\":true, \"id\":\"4847d6617eda4b7f97c38feff9bf66f1\", \"rev\":\"2-2774761002\"}");

            _getManyErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _getManyErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _getManyErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

            _copyResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _copyResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Created);
            _copyResponse.Setup(s => s.Data).Returns("{\"ok\":true,\"id\":\"56789\",\"rev\":\"355068078\"}");

            _copyErrorResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _copyErrorResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            _copyErrorResponse.Setup(s => s.Data).Returns("{\"error\":\"not_found\",\"reason\":\"missing\"}");

        }

        [Test]
        public void Ctor_Create_DbNamesMustBeLowercase()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "UnItTeSt");
            Assert.IsNotNull(db);
            Assert.AreEqual("unittest", db.Name);
        }

        [Test]
        public void Ctor_Create_DbNamesMustEscapeForwardSlash()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "his/her");
            Assert.IsNotNull(db);
            Assert.AreEqual("his%2Fher", db.Name);
        }

        [Test]
        public void Ctor_Create_CreatesCorrectly()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.IsNotNull(db);
        }

        [Test]
        public void Ctor_Create_ThrowsOnNullName()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            Assert.Throws<ArgumentNullException>(() => new CouchDatabase(_connectionMock.Object, null));
        }

        [Test]
        public void Ctor_Create_ThrowsOnNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() => new CouchDatabase(null, "unittest"));
        }

        [Test]
        public void Get_NullRef_ShouldThrow()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            string test = null;
            Assert.Throws<ArgumentNullException>(() => db.Get<ExampleEntity>(test));
        }

        [Test]
        public void Status_Error_ReturnNull()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Status();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsOk);
        }

        [Test]
        public void Status_GetStatus_ShouldSerializeCorrectly()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Status();

            Assert.AreEqual("unittest", result.DatabaseName);
            Assert.AreEqual(1, result.DocumentCount);
            Assert.AreEqual(30, result.DocumentDeletedCount);
            Assert.AreEqual(85, result.UpdateSequence);
            Assert.AreEqual(0, result.PurgeSequence);
            Assert.AreEqual(false, result.IsCompactRunning);
            Assert.AreEqual(53339, result.DiskSize);
            Assert.AreEqual("1268879146201288", result.InstanceStartTime);
            Assert.AreEqual(4, result.DiskFormatVersion);
        }

        [Test]
        public void Count_Get_ReturnsValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Count();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void Count_Error_ReturnsNegative()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest")).Returns(_statusErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Count();

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Get_BasicDocument_SerializesBackToObject()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123")).Returns(_basicResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123");

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(_exampleObject.Age, result.Age);
            Assert.AreEqual(_exampleObject.IsAlive, result.IsAlive);
        }

        [Test]
        public void Get_NotFound_ShouldBeNull()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123")).Returns(_notFoundResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123");

            Assert.IsNull(result);
        }

        [Test]
        public void Get_FieldsMissing_NotFoundFieldsShouldBeDefault()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123")).Returns(_missingFieldResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123");

            Assert.IsNotNull(result);
            Assert.IsNull(result.Name);
            Assert.AreEqual(0, result.Age);
            Assert.False(result.IsAlive);
        }

        [Test]
        public void Get_GetRevision_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?rev=946B7D1C")).Returns(_basicResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", "946B7D1C");

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(_exampleObject.Age, result.Age);
            Assert.AreEqual(_exampleObject.IsAlive, result.IsAlive);
            Assert.AreEqual(_exampleObject.Revision, "946B7D1C");
        }

        [Test]
        public void Get_IncludeRevisions_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?revs=true")).Returns(_revisionsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", CouchDocumentOptions.IncludeRevisions);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.Revisions.Start);
            Assert.AreEqual(3, result.Revisions.RevisionIds.Length);
        }

        [Test]
        public void Get_RevisionInfo_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?revs_info=true")).Returns(_revisionInfoResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", CouchDocumentOptions.RevisionInfo);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.RevisionsInfo.Length);
            Assert.AreEqual("available", result.RevisionsInfo[0].Status);
        }

        [Test]
        public void Get_AllOptions_SerializeNormally()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/abc123?revs_info=true&revs=true")).Returns(_revisionInfoResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.Get<ExampleEntity>("abc123", CouchDocumentOptions.RevisionInfo | CouchDocumentOptions.IncludeRevisions);

            Assert.IsNotNull(result);
            Assert.AreEqual(_exampleObject.Name, result.Name);
            Assert.AreEqual(3, result.RevisionsInfo.Length);
            Assert.AreEqual("available", result.RevisionsInfo[0].Status);
        }

        [Test]
        public void Get_NullValues_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Get<ExampleEntity>(null, null));

        }

        [Test]
        public void Get_NullValue_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Get<ExampleEntity>(null, CouchDocumentOptions.None));
        }

        [Test]
        public void RevisionLimit_Get_ShouldParseValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitGetResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.AreEqual(1000, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Get_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.AreEqual(-1, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Get_CastError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_revs_limit")).Returns(_revisionLimitCastErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.AreEqual(0, db.RevisionsLimit);
        }

        [Test]
        public void RevisionLimit_Set_ShouldSetValue()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/_revs_limit", "1500")).Returns(_revisionLimitSetResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.RevisionsLimit = 1500;
        }

        [Test]
        public void RevisionLimit_Set_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/_revs_limit", "1500")).Returns(_revisionLimitSetErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.RevisionsLimit = 1500;
        }

        [Test]
        public void Compact_Begin_Return()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_compact", null)).Returns(_compactResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginCompact();

            Assert.IsTrue(result.IsOk);
        }

        [Test]
        public void Compact_Error_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_compact", null)).Returns(_compactErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginCompact();

            Assert.IsFalse(result.IsOk);
        }

        [Test]
        public void ViewCleanup_Begin_Return()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_view_cleanup", null)).Returns(_viewCleanResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginViewCleanup();

            Assert.IsTrue(result.IsOk);
        }

        [Test]
        public void ViewCleanup_Error_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_view_cleanup", null)).Returns(_viewCleanErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var result = db.BeginViewCleanup();

            Assert.IsFalse(result.IsOk);
        }

        [Test]
        public void Add_NullDocument_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            ICouchDocument test = null;
            Assert.Throws<ArgumentNullException>(() => db.Add(test));
        }

        [Test]
        public void Add_SimpleAdd_CreatesId()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put(It.IsAny<string>(), It.IsAny<string>())).Returns(_addDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var response = db.Add(new ExampleEntity { Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsOk);
            Assert.IsNotNullOrEmpty(response.Id);
            Assert.AreEqual("some_doc_id", response.Id);
        }

        [Test]
        public void Add_SimpleAddWithSpecifiedId_CreatesWithId()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/some_doc_id", It.IsAny<string>())).Returns(_addDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var response = db.Add(new ExampleEntity { Id = "some_doc_id", Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsOk);
            Assert.IsNotNullOrEmpty(response.Id);
            Assert.AreEqual("some_doc_id", response.Id);
        }

        [Test]
        public void Add_Conflict_HandleError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Put("unittest/some_doc_id", It.IsAny<string>())).Returns(_addConflictDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var response = db.Add(new ExampleEntity { Id = "some_doc_id", Age = 22, IsAlive = false, Name = "Bob" });

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsOk);
            Assert.IsNullOrEmpty(response.Id);
            Assert.AreEqual("conflict", response.ErrorType);
            Assert.AreEqual("Document update conflict.", response.ErrorMessage);
        }

        [Test]
        public void Save_NullDocument_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            ICouchDocument test = null;
            Assert.Throws<ArgumentNullException>(() => db.Save(test));
        }

        [Test]
        public void Save_DocumentNoRevision_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<InvalidOperationException>(() => db.Save(new ExampleEntity { Id = "some_doc_id", Age = 22, IsAlive = false, Name = "Bob" }));
        }

        [Test]
        public void Save_CanSaveCorrectly()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Put("unittest/4847d6617eda4b7f97c38feff9bf66f1", "{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_id\":\"4847d6617eda4b7f97c38feff9bf66f1\",\"_rev\":\"1-946B7D1C\"}")).Returns(_saveDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.Save(new ExampleEntity { Id = "4847d6617eda4b7f97c38feff9bf66f1", Name = "jim", Revision = "1-946B7D1C" });

            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.IsOk);
            Assert.AreNotEqual("1-946B7D1C", resp.Revision);
        }

        [Test]
        public void Delete_NullValues_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Delete(null, null));

        }

        [Test]
        public void Delete_NullValue_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            Assert.Throws<ArgumentNullException>(() => db.Delete(null));
        }

        [Test]
        public void Delete_NameRevision_Deleted()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Delete("unittest/some_doc_id?rev=1234")).Returns(_deleteDocumentResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var response = db.Delete("some_doc_id", "1234");

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsOk);
            Assert.AreEqual("2-1234", response.Revision);
        }

        [Test]
        public void Delete_DocumentObject_Deleted()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Delete("unittest/some_doc_id?rev=1234")).Returns(_deleteDocumentResponse.Object);

            var docMock = new Mock<ICouchDocument>(MockBehavior.Strict);
            docMock.Setup(s => s.Id).Returns("some_doc_id");
            docMock.Setup(s => s.Revision).Returns("1234");

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var response = db.Delete(docMock.Object);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsOk);
            Assert.AreEqual("2-1234", response.Revision);
        }

        [Test]
        public void Delete_NoRevision_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var docMock = new Mock<ICouchDocument>(MockBehavior.Strict);
            docMock.SetupAllProperties();

            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            Assert.Throws<InvalidOperationException>(() => db.Delete(docMock.Object));
        }

        [Test]
        public void Delete_Error_HandlesError()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Delete("unittest/some_doc_id?rev=1234")).Returns(_deleteDocumentErrorResponse.Object);

            var docMock = new Mock<ICouchDocument>(MockBehavior.Strict);
            docMock.Setup(s => s.Id).Returns("some_doc_id");
            docMock.Setup(s => s.Revision).Returns("1234");

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var response = db.Delete(docMock.Object);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsOk);
        }

        [Test]
        public void GetAll_Get_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs")).Returns(_getAllIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll().ToList();

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("4c51bc81501dd2ee3d20e981a8000562", results[0].Id);
            Assert.AreEqual("1-0aaca901d8459d637c4cdc143dc49f65", results[0].Revision);
        }

        [Test]
        public void GetAll_WithOptions_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?limit=10&descending=true&startkey=%22test1%22&endkey=%22test52%22")).Returns(_getAllIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            var query = new StartEndViewQuery
            {
                Limit = 10,
                StartKey = "test1",
                EndKey = "test52",
                SortDescending = true
            };

            var results = db.GetAll(query);

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
        }

        [Test]
        public void GetAll_Error_CanHandle()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs")).Returns(_getAllIdsErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll();

            Assert.NotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public void GetAllObj_Get_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?include_docs=true")).Returns(_getAllObjResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll<ExampleEntity>();

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Fred Smith", results.ToList()[0].Name);
            Assert.AreEqual("Bill Smith", results.ToList()[1].Name);
        }

        [Test]
        public void GetAllObj_WithOptions_CanParse()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?limit=10&descending=true&include_docs=true&startkey=%22test1%22&endkey=%22test52%22")).Returns(_getAllObjResponse.Object);

            var query = new StartEndViewQuery
                            {
                                Limit = 10,
                                StartKey = "test1",
                                EndKey = "test52",
                                SortDescending = true
                            };

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll<ExampleEntity>(query);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Fred Smith", results.ToList()[0].Name);
            Assert.AreEqual("Bill Smith", results.ToList()[1].Name);
        }

        [Test]
        public void GetAllObj_Error_CanHandle()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Get("unittest/_all_docs?include_docs=true")).Returns(_getAllIdsErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetAll<ExampleEntity>();

            Assert.NotNull(results);
            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public void GetMany_Request_CanSerialize()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_all_docs?include_docs=true", "{\"keys\":[\"bar\",\"baz\"]}")).Returns(_getManyResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetMany<ExampleEntity>(new[] { "bar", "baz" });

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("jim", results.ToList()[0].Name);
        }

        [Test]
        public void GetMany_PlainDocument()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_all_docs", "{\"keys\":[\"bar\",\"baz\"]}")).Returns(_getManyResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetMany(new[] { "bar", "baz" });

            Assert.NotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("bar", results.ToList()[0].Id);
            Assert.AreEqual("baz", results.ToList()[1].Id);
        }

        [Test]
        public void GetMany_Error_CanHandle()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_all_docs?include_docs=true", "{\"keys\":[\"bar\",\"baz\"]}")).Returns(_getManyErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetMany<ExampleEntity>(new[] { "bar", "baz" });

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
            Assert.IsFalse(results.IsOk);
        }

        [Test]
        public void GetMany_Plain_Error_CanHandle()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_all_docs", "{\"keys\":[\"bar\",\"baz\"]}")).Returns(_getManyErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var results = db.GetMany(new[] { "bar", "baz" });

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count());
            Assert.IsFalse(results.IsOk);
        }

        [Test]
        public void BulkAdd_WithIds_CanSerialize()
        {
            var postData = "{\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_id\":\"0\"},{\"name\":\"billy\",\"age\":0,\"isAlive\":false,\"_id\":\"1\"}]}";
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_bulk_docs", postData)).Returns(_bulkAddResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.AddMany(new[] { new ExampleEntity { Name = "jim", Id = "0" }, new ExampleEntity { Id = "1", Name = "billy" } });

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            Assert.AreEqual("0", resp.ToList()[0].Id);
            Assert.AreEqual("1", resp.ToList()[1].Id);
            Assert.AreEqual("1-f5f3f3e496c72307975a69c73fd53d42", resp.ToList()[0].Revision);
        }

        [Test]
        public void BulkAdd_WithoutIds_CreatesIds()
        {
            var postData = "{\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false},{\"name\":\"billy\",\"age\":0,\"isAlive\":false}]}";
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_bulk_docs", postData)).Returns(_bulkAddNoIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.AddMany(new[] { new ExampleEntity { Name = "jim" }, new ExampleEntity { Name = "billy" } });

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            Assert.AreEqual("d9c308eef23bbfff46826135fb000883", resp.ToList()[0].Id);
            Assert.AreEqual("d9c308eef23bbfff46826135fb00131b", resp.ToList()[1].Id);
        }

        [Test]
        public void BulkAdd_Error_CanHandle()
        {
            var postData = "{\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_id\":\"0\"},{\"name\":\"billy\",\"age\":0,\"isAlive\":false,\"_id\":\"1\"}]}";
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_bulk_docs", postData)).Returns(_bulkAddErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.AddMany(new[] { new ExampleEntity { Name = "jim", Id = "0" }, new ExampleEntity { Id = "1", Name = "billy" } });

            Assert.IsNotNull(resp);
            Assert.AreEqual(3, resp.Count()); // Should all return even if one errors.
            Assert.AreEqual("conflict", resp[0].ErrorType);

        }

        [Test]
        public void BulkAdd_BulkUpdateBehaviour()
        {
            var postData = "{\"all_or_nothing\":true,\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false},{\"name\":\"billy\",\"age\":0,\"isAlive\":false}]}";
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_bulk_docs", postData)).Returns(_bulkAddNoIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.BulkUpdateBehaviour = CouchBulkUpdateBehaviour.AllOrNothing;
            var resp = db.AddMany(new[] { new ExampleEntity { Name = "jim" }, new ExampleEntity { Name = "billy" } });

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            Assert.AreEqual("d9c308eef23bbfff46826135fb000883", resp.ToList()[0].Id);
            Assert.AreEqual("d9c308eef23bbfff46826135fb00131b", resp.ToList()[1].Id);
        }

        [Test]
        public void BulkSave_NoRevisionIds_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            Assert.Throws<InvalidOperationException>(() => db.SaveMany(new[] { new ExampleEntity { Name = "jim" }, new ExampleEntity { Name = "billy" } }));
        }

        [Test]
        public void BulkSave_CanSave()
        {
            var postData = "{\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_rev\":\"1-946B7D1C\"},{\"name\":\"billy\",\"age\":0,\"isAlive\":false,\"_rev\":\"1-946B7D1C\"}]}";
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_bulk_docs", postData)).Returns(_bulkAddNoIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            var resp = db.SaveMany(new[]
                            {
                                new ExampleEntity {Name = "jim", Revision = "1-946B7D1C"},
                                new ExampleEntity {Name = "billy", Revision = "1-946B7D1C"}
                            });

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            Assert.AreEqual("d9c308eef23bbfff46826135fb000883", resp.ToList()[0].Id);
            Assert.AreEqual("d9c308eef23bbfff46826135fb00131b", resp.ToList()[1].Id);
        }

        [Test]
        public void BulkSave_BulkUpdateBehaviour()
        {
            var postData = "{\"all_or_nothing\":true,\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_rev\":\"1-946B7D1C\"},{\"name\":\"billy\",\"age\":0,\"isAlive\":false,\"_rev\":\"1-946B7D1C\"}]}";
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_bulk_docs", postData)).Returns(_bulkAddNoIdsResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            db.BulkUpdateBehaviour = CouchBulkUpdateBehaviour.AllOrNothing;

            var resp = db.SaveMany(new[]
                            {
                                new ExampleEntity {Name = "jim", Revision = "1-946B7D1C"},
                                new ExampleEntity {Name = "billy", Revision = "1-946B7D1C"}
                            });

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            Assert.AreEqual("d9c308eef23bbfff46826135fb000883", resp.ToList()[0].Id);
            Assert.AreEqual("d9c308eef23bbfff46826135fb00131b", resp.ToList()[1].Id);
        }

        [Test]
        public void BulkSave_Error_CanHandle()
        {
            var postData = "{\"docs\":[{\"name\":\"jim\",\"age\":0,\"isAlive\":false,\"_rev\":\"1-946B7D1C\"},{\"name\":\"billy\",\"age\":0,\"isAlive\":false,\"_rev\":\"1-946B7D1C\"}]}";

            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(s => s.Post("unittest/_bulk_docs", postData)).Returns(_bulkSaveErrorResponse.Object);

            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            var resp = db.SaveMany(new[]
                            {
                                new ExampleEntity {Name = "jim", Revision = "1-946B7D1C"},
                                new ExampleEntity {Name = "billy", Revision = "1-946B7D1C"}
                            });

            Assert.IsNotNull(resp);
            Assert.AreEqual(3, resp.Count()); // Should all return even if one errors.
            Assert.AreEqual("conflict", resp[0].ErrorType);
        }

        [Test]
        public void Copy_NullValues_Throws()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");

            Assert.Throws<ArgumentNullException>(() => db.Copy(null, null, null));

            Assert.Throws<ArgumentNullException>(() => db.Copy((string)null, null));

            Assert.Throws<ArgumentNullException>(() => db.Copy((ICouchDocument)null, null));
        }

        [Test]
        public void Copy_WithDocument_CanIssueCopy()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Copy("unittest/123456", "56789")).Returns(_copyResponse.Object);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.Copy(new ExampleEntity { Id = "123456" }, "56789");

            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.IsOk);
            Assert.AreEqual("56789", resp.Id);
        }

        [Test]
        public void Copy_WithStrings_CanIssueCopy()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Copy("unittest/123456", "56789")).Returns(_copyResponse.Object);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.Copy("123456", "56789");

            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.IsOk);
            Assert.AreEqual("56789", resp.Id);
        }

        [Test]
        public void Copy_WithRevision_CanIssueCopy()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Copy("unittest/123456", "56789?rev=1-12345678")).Returns(_copyResponse.Object);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.Copy("123456", "56789", "1-12345678");

            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.IsOk);
            Assert.AreEqual("56789", resp.Id);
        }

        [Test]
        public void Copy_Error_CanHandle()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Copy("unittest/123456", "56789?rev=1-12345678")).Returns(_copyErrorResponse.Object);
            var db = new CouchDatabase(_connectionMock.Object, "unittest");
            var resp = db.Copy("123456", "56789", "1-12345678");

            Assert.IsNotNull(resp);
            Assert.IsFalse(resp.IsOk);
            Assert.IsNotNull(resp.ErrorMessage);
        }
    }
}