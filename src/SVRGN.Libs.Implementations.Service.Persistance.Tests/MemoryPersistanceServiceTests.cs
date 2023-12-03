using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVRGN.Libs.Implementations.Service.Persistance.Tests
{
    [TestClass]
    [TestCategory("IPersistanceService")]
    public class MemoryPersistanceServiceTests : BasePersistanceServiceTests
    {
        #region Methods
        #region IndividualInitialization
        protected override void IndividualInitialization()
        {
            TestBootStrapper.RegisterWithMemoryPersistance(new ServiceCollection());
        }
        #endregion IndividualInitialization

        #region ListItemsInContainer
        [TestMethod]
        public void GetItemNamesFromContainer()
        {
            this.service.AddContainer("Write", "Write/int");
            this.service.WriteToContainer<int>("Write", "ValueName", 1);
            this.service.WriteToContainer<int>("Write", "ValueName2", 666);
            this.service.WriteToContainer<string>("Write2", "ValueName3", "Something");

            List<string> items = this.service.GetItemNamesFromContainer("Write");

            Assert.AreEqual(2, items.Count);
        }
        #endregion GetItemNamesFromContainer

        #region Read
        [TestMethod]
        public void Read()
        {
            int value = 1;
            this.service.AddEndpoint("Read", "Read/int");
            this.service.WriteToEndpoint<int>("Read", value);
            int resultValue = this.service.ReadFromEndpoint<int>("Read");

            Assert.AreEqual(value, resultValue);
        }
        #endregion Read

        #region Write
        [TestMethod]
        public void Write()
        {
            this.service.AddEndpoint("Write", "Write/int");
            this.service.WriteToEndpoint<int>("Write", 1);

            Assert.AreEqual(true, this.service.ValueExists("Write"));
        }
        #endregion Write

        #region WriteToContainer
        [TestMethod]
        public void WriteToContainer()
        {
            this.service.AddContainer("Write", "Write/int");
            this.service.WriteToContainer<int>("Write", "ValueName", 1);
            this.service.WriteToContainer<int>("Write", "ValueName2", 666);
            this.service.WriteToContainer<string>("Write", "ValueName3", "Something");

            Assert.AreEqual(true, this.service.ValueExistsInContainer("Write", "ValueName"));
            Assert.AreEqual(true, this.service.ValueExistsInContainer("Write", "ValueName2"));
            Assert.AreEqual(true, this.service.ValueExistsInContainer("Write", "ValueName3"));
        }
        #endregion WriteToContainer

        #region ReadFromContainer
        [TestMethod]
        public void ReadFromContainer()
        {
            int value = 1;
            this.service.AddContainer("Read", "Read/int");
            this.service.WriteToContainer<int>("Read", "ValueName", value);
            int resultValue = this.service.ReadFromContainer<int>("Read", "ValueName");

            Assert.AreEqual(value, resultValue);
        }
        #endregion ReadFromContainer

        #endregion Methods
    }
}
