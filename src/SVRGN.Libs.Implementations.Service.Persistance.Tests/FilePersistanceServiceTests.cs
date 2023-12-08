using Microsoft.Extensions.DependencyInjection;
using Moq;
using SVRGN.Libs.Contracts.Base.IO;
using SVRGN.Libs.Contracts.Service.Persistance;
using SVRGN.Libs.Implementations.DependencyInjection;
using SVRGN.Libs.Implementations.Service.Persistance.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVRGN.Libs.Implementations.Service.Persistance.Tests
{
    [TestClass]
    [TestCategory("IPersistanceService")]
    public class FilePersistanceServiceTests : BasePersistanceServiceTests
    {
        // following the Advice from https://makolyte.com/csharp-unit-testing-code-that-does-file-io/
        #region Methods

        #region GetContainerPath
        private static string GetContainerPath()
        {
            string result;

            string applicationBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            result = Path.Join(applicationBaseDirectory, "Files", "ExistingContainer");

            return result;
        }
        #endregion GetContainerPath

        #region IndividualInitialization
        protected override void IndividualInitialization()
        {
            IServiceCollection services = new ServiceCollection();
            DiContainer.SetServiceCollection(services);
            TestBootStrapper.RegisterWithFilePersistance(new ServiceCollection());
        }
        #endregion IndividualInitialization

        #region BuildMock
        public static Mock<IFileIOWrapper> BuildMock()
        {
            //TODO: set up and somehow inject a Moq for IFileIOWrapper
            Mock<IFileIOWrapper> result = new Mock<IFileIOWrapper>();

            string pathToExistingContainer01 = FilePersistanceServiceTests.GetContainerPath();
            List<string> filesInExistingContainer01 = new List<string>() { "1.txt", "2.txt", "3.txt" };

            string pathToExistingContainer02 = "c:\\ReadTest";
            List<string> filesInExistingContainer02 = new List<string>() { "1.strm", "2.txt", "3.txt" };

            string endpointForReadTests = "c:\\ReadTest\\1.strm";

            result.Setup(t => t.DirectoryGetFiles(pathToExistingContainer01)).Returns(filesInExistingContainer01);
            result.Setup(t => t.DirectoryGetFiles(pathToExistingContainer02)).Returns(filesInExistingContainer02);
            result.Setup(t => t.Exists(endpointForReadTests)).Returns(true);
            result.Setup(t => t.ReadAllText(endpointForReadTests)).Returns("1");
            result.Setup(t => t.WriteAllText("c:\\WriteTest\\1.strm", "666")).Returns(true);
            result.Setup(t => t.WriteAllText("c:\\WriteTest\\2.strm", "666")).Returns(true);

            //throw new NotImplementedException();

            return result;
        }
        #endregion BuildMock

        #region ListFilesInExistingDirectory
        [TestMethod]
        public void ListFilesInExistingDirectory()
        {
            string containerDirectory = FilePersistanceServiceTests.GetContainerPath();

            this.service.AddContainer("MyContainer", containerDirectory);
            List<string> existingItems = this.service.GetItemNamesFromContainer("MyContainer");

            Assert.AreEqual(3, existingItems.Count);
        }
        #endregion ListFilesInExistingDirectory

        #region FileExistsInExistingDirectory
        [TestMethod]
        public void FileExistsInExistingDirectory()
        {
            string containerDirectory = FilePersistanceServiceTests.GetContainerPath();

            this.service.AddContainer("MyContainer", containerDirectory);
            bool doesExist = this.service.ValueExistsInContainer("MyContainer", "2.txt");

            Assert.IsTrue(doesExist);
        }
        #endregion FileExistsInExistingDirectory

        #region Read
        [TestMethod]
        public void Read()
        {
            this.service.AddEndpoint("Read", "c:\\ReadTest\\1.strm");
            int resultValue = this.service.ReadFromEndpoint<int>("Read");

            Assert.AreEqual(1, resultValue);
        }
        #endregion Read

        #region ReadFromContainer
        [TestMethod]
        public void ReadFromContainer()
        {
            this.service.AddContainer("ReadContainer", "c:\\ReadTest");
            int resultValue = this.service.ReadFromContainer<int>("ReadContainer", "1.strm");

            Assert.AreEqual(1, resultValue);
        }
        #endregion ReadFromContainer

        #region Write
        [TestMethod]
        public void Write()
        {
            this.service.AddEndpoint("Write", "c:\\WriteTest\\1.strm");
            bool wasWriteSuccessful = this.service.WriteToEndpoint<int>("Write", 666);

            Assert.IsTrue(wasWriteSuccessful);
        }
        #endregion Write

        #region WriteToContainer
        [TestMethod]
        public void WriteToContainer()
        {
            this.service.AddContainer("WriteContainer", "c:\\WriteTest");
            bool wasWriteScuccessful = this.service.WriteToContainer<int>("WriteContainer", "2.strm", 666);

            Assert.IsTrue(wasWriteScuccessful);
        }
        #endregion WriteToContainer

        #region WriteStreamToContainer
        [TestMethod]
        public void WriteStreamToContainer()
        {
            this.service.AddContainer("WriteContainer", "c:\\WriteTest");
            MemoryStream testStream = new MemoryStream();
            using (StreamWriter streamWriter = new StreamWriter(testStream))
            {
                streamWriter.Write("Hello World");
                streamWriter.Flush();//otherwise you are risking empty stream
                testStream.Seek(0, SeekOrigin.Begin);
            }

            bool wasWriteScuccessful = this.service.WriteToContainer<Stream>("WriteContainer", "3.strm", testStream);

            //Assert.IsTrue(wasWriteScuccessful);  //TODO: mock stream writing
        }
        #endregion WriteStreamToContainer

        #endregion Methods
    }
}
