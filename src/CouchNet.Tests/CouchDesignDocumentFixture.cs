using System;
using System.Collections.Generic;
using System.Net;
using CouchNet.HttpTransport;
using CouchNet.Impl;
using CouchNet.Internal;
using CouchNet.Tests.Model;
using Moq;
using NUnit.Framework;

namespace CouchNet.Tests
{
    [TestFixture]
    public class CouchDesignDocumentFixture
    {
        private Mock<ICouchConnection> _connectionMock;
        private Mock<IHttpResponse> _viewEmptyResults;
        private Mock<IHttpResponse> _viewDefinition;
        private CouchDesignDocumentDefinition _designDocumentDefinition;
        private Mock<IHttpResponse> _httpResponse;

        [SetUp]
        public void Setup()
        {
            _viewEmptyResults = new Mock<IHttpResponse>(MockBehavior.Strict);
            _viewEmptyResults.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _viewEmptyResults.Setup(s => s.Data).Returns("{\"total_rows\":4,\"offset\":2,\"rows\":[]}");

            _viewDefinition = new Mock<IHttpResponse>(MockBehavior.Strict);
            _viewDefinition.Setup(s => s.StatusCode).Returns(HttpStatusCode.OK);
            _viewDefinition.Setup(s => s.Data).Returns("{\"_id\":\"_design/example\",\"_rev\":\"1-f1afd087bf27328f6e96291f24b45925\",\"language\":\"javascript\",\"views\":{\"testView\":{\"map\":\"function(doc) {\n  emit(doc._id, doc);\n}\"}}}");

            _httpResponse = new Mock<IHttpResponse>(MockBehavior.Strict);
            _httpResponse.SetupAllProperties();

            _designDocumentDefinition = new CouchDesignDocumentDefinition
                                 {
                                     Id = "_design/example",
                                     Language = "javascript",
                                     Views = new Dictionary<string, CouchViewDefinition>(),
                                     Lists = new Dictionary<string, string>(),
                                     Shows = new Dictionary<string, string>(),
                                     RewriteRules = new List<CouchRewriteRuleDefinition>(),
                                     DocumentUpdateHandlers = new Dictionary<string, string>(),
                                     DocumentUpdateValidation = string.Empty,
                                     Revision = "1234"
                                 };
            _designDocumentDefinition.Views.Add("testView", new CouchViewDefinition { Map = "function(doc){emit(doc._id,null);}", Reduce = "function(blah){emit(null,null)}"});
            _designDocumentDefinition.Shows.Add("testShow", "blahShow");
            _designDocumentDefinition.Lists.Add("testList", "hatCat");
            _designDocumentDefinition.DocumentUpdateHandlers.Add("testDocHandle", "blah");
            _designDocumentDefinition.DocumentUpdateValidation = "testingValidation";
            _designDocumentDefinition.RewriteRules.Add(new CouchRewriteRuleDefinition { From = "/abc/", To = "/dfe/", Method = "GET", Query = "func()"});
        }

        #region Ctor
        [Test]
        public void PublicCreate()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var svc = new CouchService(_connectionMock.Object);
            var db = svc["unittest"];

            var dd = new CouchDesignDocument("example", db);

            Assert.IsNotNull(dd);
            Assert.AreEqual("_design/example",dd.Id);
            Assert.AreEqual(0,dd.Views.Count);
            Assert.AreEqual(0,dd.Shows.Count);
            Assert.AreEqual(0,dd.Lists.Count);
            Assert.AreEqual(0,dd.RewriteRules.Count);
            Assert.AreEqual(0,dd.DocumentUpdaters.Count);
            Assert.IsTrue(dd.HasPendingChanges);
            Assert.AreEqual("javascript",dd.Language);
            Assert.IsNull(dd.Revision);
            Assert.AreEqual("example",dd.Name);
        }

        [Test]
        public void PublicCreate_WithNulls_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new CouchDesignDocument("", null));
        }

        [Test]
        public void PublicCreate_WithDesignInName_ShouldFormatName()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var svc = new CouchService(_connectionMock.Object);
            var db = svc["unittest"];

            var dd = new CouchDesignDocument("_design/example", db);
            Assert.AreEqual("example", dd.Name);
        }

        [Test]
        public void InternalCreate_Full()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            var svc = new CouchService(_connectionMock.Object);
            var db = svc["unittest"];

            var dd = new CouchDesignDocument(_designDocumentDefinition, db);

            Assert.AreEqual(_designDocumentDefinition.Id, dd.Id);
            Assert.AreEqual(_designDocumentDefinition.Revision, dd.Revision);
            Assert.AreEqual(1, dd.Views.Count);
            Assert.AreEqual(1, dd.Shows.Count);
            Assert.AreEqual(1, dd.Lists.Count);
            Assert.AreEqual(1, dd.RewriteRules.Count);
            Assert.AreEqual(1, dd.DocumentUpdaters.Count);
            Assert.AreEqual("example", dd.Name);
        }

        #endregion

        #region View

        [Test]
        public void CreateView()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var svc = new CouchService(_connectionMock.Object);
            var db = svc["unittest"];

            var dd = new CouchDesignDocument("example", db);
            var view = dd.CreateView("testView");

            Assert.IsAssignableFrom<CouchView>(view);
            Assert.IsNull(view.Map);
            Assert.IsNull(view.Reduce);
            Assert.IsTrue(view.HasPendingChanges);
            Assert.AreEqual("testView",view.Name);
        }

        [Test]
        public void DropView()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Put("unittest/_design/example","{\"_id\":\"_design/example\",\"_rev\":\"1234\",\"language\":\"javascript\",\"shows\":{},\"lists\":{},\"rewrites\":[],\"updates\":{}}")).Returns(_httpResponse.Object);
            
            var svc = new CouchService(_connectionMock.Object);
            var db = svc["unittest"];
            var dd = new CouchDesignDocument(_designDocumentDefinition, db);
            dd.DropView("testView");

            Assert.AreEqual(0, dd.Views.Count);
        }

        #endregion

        #region Show
        
        [Test]
        public void CreateShow()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);

            var svc = new CouchService(_connectionMock.Object);
            var db = svc["unittest"];

            var dd = new CouchDesignDocument("example", db);
            var show = dd.CreateShow("testShow");

            Assert.IsAssignableFrom<CouchShowHandler>(show);
            Assert.IsNull(show.Function);
            Assert.IsTrue(show.HasPendingChanges);
            Assert.AreEqual("testShow", show.Name);
            Assert.AreEqual(dd,show.DesignDocument);
            Assert.AreEqual("_design/example/_show/testShow", show.ToString());
        }

        [Test]
        public void DropShow()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Put("unittest/_design/example", "{\"_id\":\"_design/example\",\"_rev\":\"1234\",\"language\":\"javascript\",\"shows\":{},\"lists\":{},\"rewrites\":[],\"updates\":{}}")).Returns(_httpResponse.Object);

            var svc = new CouchService(_connectionMock.Object);
            var db = svc["unittest"];
            var dd = new CouchDesignDocument(_designDocumentDefinition, db);
            dd.DropShow("testShow");

            Assert.AreEqual(0, dd.Shows.Count);
        }

        #endregion

        [Test]
        public void ExecuteView_EmptyResults_ShouldBeOk()
        {
            _connectionMock = new Mock<ICouchConnection>(MockBehavior.Strict);
            _connectionMock.Setup(x => x.Get("unittest/_design/example/_view/testView?key=%22apple%22")).Returns(_viewEmptyResults.Object);
            _connectionMock.Setup(x => x.Get("unittest/_design/example")).Returns(_viewDefinition.Object);

            var svc = new CouchService(_connectionMock.Object);

            var db = svc.GetDatabase("unittest");

            var query = new CouchViewQuery().Key("apple");

            var results = db.GetDesignDocument("example").ExecuteView<ExampleEntity>("testView", query);
            Assert.IsTrue(results.IsOk);
            Assert.IsFalse(results.HasResults);
            Assert.AreEqual(0, results.Count);
        }
    }
}