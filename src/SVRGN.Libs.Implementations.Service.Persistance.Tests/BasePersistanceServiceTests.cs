using Microsoft.Extensions.DependencyInjection;
using SVRGN.Libs.Contracts.Service.Logging;
using SVRGN.Libs.Contracts.Service.Object;
using SVRGN.Libs.Contracts.Service.Persistance;
using SVRGN.Libs.Implementations.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVRGN.Libs.Implementations.Service.Persistance.Tests
{
    [TestClass]
    [TestCategory("IPersistanceService")]
    public abstract class BasePersistanceServiceTests
    {
        #region Properties

        protected IObjectService ObjectService;
        protected ILogService LogService;
        protected IPersistanceService service;

        #endregion Properties

        #region Methods

        #region Initialize
        [TestInitialize]
        public void Initialize()
        {
            this.IndividualInitialization();
            this.ObjectService = DiContainer.Resolve<IObjectService>();
            this.LogService = DiContainer.Resolve<ILogService>();
            this.service = this.ObjectService.Create<IPersistanceService>();
        }
        #endregion Initialize

        #region IndividualInitialization: is being called BEFORE the service creation - ideal to bring some individual Di registrations
        /// <summary>
        /// is being called BEFORE the service creation - ideal to bring some individual Di registrations
        /// </summary>
        protected abstract void IndividualInitialization();
        #endregion IndividualInitialization

        #region AddEndpoint
        [TestMethod]
        public void AddEndpoint()
        {
            bool wasSuccessful = this.service.AddEndpoint("Read", "Read/int");

            Assert.AreEqual(true, wasSuccessful);
            Assert.AreEqual(true, this.service.Exists("Read"));
        }
        #endregion AddEndpoint

        #region AddEndpointNegative
        [TestMethod]
        public void AddEndpointNegative()
        {
            bool wasSuccessful = this.service.AddEndpoint("Read", "Read/int");
            bool wasSuccessful2 = this.service.AddEndpoint("Read", "Read/int");

            Assert.AreEqual(true, wasSuccessful);
            Assert.AreEqual(true, this.service.Exists("Read"));
            Assert.AreEqual(false, wasSuccessful2);
        }
        #endregion AddEndpointNegative

        #region AddContainer
        [TestMethod]
        public void AddContainer()
        {
            bool wasSuccessful = this.service.AddContainer("Read", "Read/int");

            Assert.AreEqual(true, wasSuccessful);
            Assert.AreEqual(true, this.service.Exists("Read"));
        }
        #endregion AddContainer

        #endregion Methods
    }
}
