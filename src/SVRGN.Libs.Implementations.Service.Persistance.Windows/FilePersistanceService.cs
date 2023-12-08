using SVRGN.Libs.Contracts.Base.IO;
using SVRGN.Libs.Contracts.Service.Logging;
using SVRGN.Libs.Contracts.Service.Object;
using SVRGN.Libs.Contracts.Service.Persistance;
using SVRGN.Libs.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVRGN.Libs.Implementations.Service.Persistance.Windows
{
    /// <summary>
    /// Windows implementation of a file based persistance service
    /// </summary>
    public class FilePersistanceService : BasePersistanceService
    {
        #region Properties

        private IFileIOWrapper fileIOWrapper;

        private string applicationBaseDirectory; //  TODO: find out other interesting spots

        #endregion Properties

        #region Construction

        public FilePersistanceService(ILogService LogService, IObjectService ObjectService, IFileIOWrapper FileIOWrapper)
            : base(LogService, ObjectService)
        {
            this.fileIOWrapper = FileIOWrapper;

            this.persistanceType = "File";

            //set base Path to app directory
            this.applicationBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;  // see https://stackoverflow.com/questions/6041332/best-way-to-get-application-folder-path
        }

        #endregion Construction

        #region Methods

        #region Read
        public override T ReadFromEndpoint<T>(string EndPoint)
        {
            T result = default;

            this.logService.Debug("FilePersistanceService", "ReadFromEndpoint", $"Starting Reading File / Endpoint'{EndPoint}'");

            IPersistanceEndpoint persistanceEndpoint = this.GetEndpoint(EndPoint);

            if (persistanceEndpoint != null)
            {
                if (this.ValueExists(EndPoint))
                {
                    this.logService.Debug("FilePersistanceService", "ReadFromEndpoint", $"Successfully found Endpoint '{EndPoint}'");
                    string pathToValue = this.GetAbsolutePath(persistanceEndpoint.Paths[0]);
                    this.logService.Debug("FilePersistanceService", "ReadFromEndpoint", $"Attempting to read file '{pathToValue}'");
                    result = this.ReadFile<T>(pathToValue);
                    this.logService.Info("FilePersistanceService", "ReadFromEndpoint", $"Read file '{pathToValue}' of Type '{typeof(T)}' in Endpoint '{EndPoint}' with result '{result}'");
                }
                else
                {
                    //error, file does not exists
                    this.logService.Error("FilePersistanceService", "ReadFromEndpoint", $"End Point '{EndPoint}' does not exist!");
                }
            }
            else
            {
                // error, end point does not exists
                this.logService.Error("FilePersistanceService", "ReadFromEndpoint", $"End Point '{EndPoint}' is not registered with the service!");
            }

            return result;
        }
        #endregion Read

        #region ReadFile
        private T ReadFile<T>(string Filename)
        {
            //TODO: move to FileHelper class
            T result = default;

            bool isStream = typeof(Stream).IsAssignableFrom(typeof(T));

            if (isStream)
            {
                Stream targetStream = this.fileIOWrapper.ReadStream(Filename);

                result = (T)((object)targetStream);
            }
            else
            {
                string text = this.fileIOWrapper.ReadAllText(Filename);
                object propvalue = text.ConvertTo<T>();
                result = (T)propvalue;
            }

            return result;
        }
        #endregion ReadFile

        #region WriteFile
        private bool WriteFile<T>(string Filename, T Value)
        {
            //TODO: move to FileHelper class
            bool result = false;

            bool isStream = typeof(Stream).IsAssignableFrom(typeof(T));
            if (isStream)
            {
                result = fileIOWrapper.WriteStream(Filename, (Stream)((object)Value));
            }
            else
            {
                result = this.fileIOWrapper.WriteAllText(Filename, Value.ToString());
            }

            return result;
        }
        #endregion WriteFile

        #region ReadFromContainer
        public override T ReadFromContainer<T>(string ContainerName, string ValueName)
        {
            T result = default;

            this.logService.Debug("FilePersistanceService", "ReadFromContainer", $"Starting Reading File '{ValueName}' from Container '{ContainerName}'");

            IPersistanceContainer container = this.GetContainer(ContainerName);

            if (container != null)
            {
                this.logService.Debug("FilePersistanceService", "ReadFromContainer", $"Successfully found Container '{ContainerName}'");
                if (this.ValueExistsInContainer(ContainerName, ValueName))
                {
                    IPersistancePath pathToValue = this.GetContainerValuePath(ContainerName, ValueName);
                    this.logService.Debug("FilePersistanceService", "ReadFromContainer", $"Attempting to read file '{pathToValue}'");
                    result = this.ReadFile<T>(System.IO.Path.Combine(pathToValue.Path, ValueName));
                    this.logService.Info("FilePersistanceService", "ReadFromContainer", $"Read file '{pathToValue}' of Type '{typeof(T)}' in Container '{ContainerName}' with result '{result}'");
                }
                else
                {
                    //error, file does not exists
                    this.logService.Error("FilePersistanceService", "ReadFromContainer", $"File '{ValueName}' does not exist in Container '{ContainerName}'!");
                }
            }
            else
            {
                //error, container does not exists
                this.logService.Error("FilePersistanceService", "ReadFromContainer", $"Container '{ContainerName}' is not registered with the service!");
            }

            return result;
        }
        #endregion ReadFromContainer

        #region GetContainerValuePath
        private IPersistancePath GetContainerValuePath(string ContainerName, string ValueName)
        {
            IPersistancePath result = default;

            if (this.Exists(ContainerName))
            {
                IPersistanceContainer container = this.GetContainer(ContainerName);

                foreach (IPersistancePath path in container.Paths)
                {
                    string absolutepath = this.GetAbsolutePath(path);
                    List<string> filenames = this.fileIOWrapper.DirectoryGetFiles(absolutepath);

                    if (filenames.Any(x => x.Equals(ValueName)))
                    {
                        result = path;
                        break;
                    }

                }
            }

            return result;
        }
        #endregion GetContainerValuePath

        #region ValueExists
        public override bool ValueExists(string EndPoint)
        {
            bool result = false;

            this.logService.Debug("FilePersistanceService", "ValueExists", $"Trying to find a value in End Point '{EndPoint}'");

            IPersistanceEndpoint persistanceEndpoint = this.GetEndpoint(EndPoint);

            if (persistanceEndpoint != null)
            {
                string absoluteFilename = this.GetAbsolutePath(persistanceEndpoint.Paths[0]);
                if (this.fileIOWrapper.Exists(absoluteFilename))
                {
                    result = true;
                }
                else
                {
                    //log file does not exist
                    this.logService.Warning("FilePersistanceService", "ValueExists", $"File '{absoluteFilename}' stored under End Point '{EndPoint}' does not exist!");
                }
            }
            return result;
        }
        #endregion ValueExists

        #region ValueExistsInContainer
        public override bool ValueExistsInContainer(string ContainerName, string ValueName)
        {
            bool result = false;

            this.logService.Debug("FilePersistanceService", "ValueExistsInContainer", $"Trying to find a value in Container '{ContainerName}', ValueName '{ValueName}'.");

            if (this.Exists(ContainerName))
            {
                result = this.GetItemNamesFromContainer(ContainerName).Any(x => x.Equals(ValueName));
                if (result)
                {
                    this.logService.Debug("FilePersistanceService", "ValueExistsInContainer", $"Found a value for the given parameters.");
                }
                else
                {
                    this.logService.Warning("FilePersistanceService", "ValueExistsInContainer", $"Did not find a clue of ValueName '{ValueName}' in Container '{ContainerName}'.");
                }
            }
            else
            {
                this.logService.Warning("FilePersistanceService", "ValueExistsInContainer", $"Container '{ContainerName}' does not exist!");
            }

            return result;
        }
        #endregion ValueExistsInContainer

        #region GetAbsolutePath: returns the absolute path of a given path - if no drive is specified, the path will be treated as subdir of the application directory.
        /// <summary>
        /// returns the absolute path of a given path - if no drive is specified, the path will be treated as subdir of the application directory.
        /// </summary>
        /// <param name="Path">the path to make absolute</param>
        /// <returns>the absolute path of a given path</returns>
        private string GetAbsolutePath(IPersistancePath Path)
        {
            string result = string.Empty;

            if (System.IO.Path.IsPathFullyQualified(Path.Path))
            {
                result = Path.Path;
            }
            else
            {
                result = System.IO.Path.Combine(this.applicationBaseDirectory, Path.Path);
            }

            return result;
        }
        #endregion GetAbsolutePath

        #region Write
        public override bool WriteToEndpoint<T>(string EndPoint, T Value)
        {
            bool result = false;

            IPersistanceEndpoint persistanceEndpoint = this.GetEndpoint(EndPoint);
            if (persistanceEndpoint != null)
            {
                this.logService.Debug("FilePersistanceService", "Write", $"Attempting to write End Point '{EndPoint}'");
                string filename = this.GetAbsolutePath(persistanceEndpoint.Paths[0]);
                this.logService.Debug("FilePersistanceService", "WriteToContainer", $"Attempting to use File Name '{filename}'");
                result = this.WriteFile<T>(filename, Value);
                this.logService.Debug("FilePersistanceService", "Write", $"Wrote to Endpoint '{EndPoint}' with result '{result}'");
            }
            else
            {
                //log, endpoint not existing
                this.logService.Error("FilePersistanceService", "Write", $"End Point '{EndPoint}' does not exist in internal container registry");
            }

            return result;
        }
        #endregion Write

        #region WriteToContainer
        public override bool WriteToContainer<T>(string ContainerName, string ValueName, T Value)
        {
            bool result = false;

            IPersistanceContainer persistanceContainer = this.GetContainer(ContainerName);
            if (persistanceContainer != null)
            {
                this.logService.Debug("FilePersistanceService", "WriteToContainer", $"Attempting to write File '{ValueName}' into Container '{ContainerName}'");
                string absolutePath = this.GetAbsolutePath(persistanceContainer.Paths[0]);
                string filename = System.IO.Path.Combine(absolutePath, ValueName);
                this.logService.Debug("FilePersistanceService", "WriteToContainer", $"Attempting to use File Name '{filename}'");
                result = this.WriteFile<T>(filename, Value);  // write into the first path of the container
                this.logService.Debug("FilePersistanceService", "WriteToContainer", $"Wrote File '{ValueName}' into Container '{ContainerName}' with result '{result}'");
            }
            else
            {
                //container not existing
                this.logService.Error("FilePersistanceService", "WriteToContainer", $"Container '{ContainerName}' does not exist in internal container registry");
            }

            return result;
        }
        #endregion WriteToContainer

        #region GetItemNamesFromContainer
        public override List<string> GetItemNamesFromContainer(string ContainerName)
        {
            List<string> result = default;

            this.logService.Debug("FilePersistanceService", "GetItemNamesFromContainer", $"Trying to get all item names for Container '{ContainerName}'");

            IPersistanceContainer container = this.GetContainer(ContainerName);
            if (container != null)
            {
                if (container.Paths != null)
                {
                    result = new List<string>();
                    foreach (IPersistancePath path in container.Paths)
                    {
                        string absolutepath = this.GetAbsolutePath(path);
                        this.logService.Debug("FilePersistanceService", "GetItemNamesFromContainer", $"Searching in Absolute Path for Container '{ContainerName}' is '{absolutepath}'.");
                        List<string> filenames = this.fileIOWrapper.DirectoryGetFiles(absolutepath);
                        if (filenames != null && filenames.Count > 0)
                        {
                            result.AddRange(filenames);
                            this.logService.Debug("FilePersistanceService", "GetItemNamesFromContainer", $"Found '{filenames.Count}' files in Container '{ContainerName}', path '{absolutepath}'.");
                        }
                        else
                        {
                            //log filenames is null, no files
                            this.logService.Debug("FilePersistanceService", "GetItemNamesFromContainer", $"Did not find any files in Container '{ContainerName}', path '{absolutepath}'!");
                        }
                    }
                    this.logService.Info("FilePersistanceService", "GetItemNamesFromContainer", $"Found '{result.Count}' files in Container '{ContainerName}'.");
                }
                else
                {
                    this.logService.Error("FilePersistanceService", "GetItemNamesFromContainer", $"Container '{ContainerName}' does not have any path assigned!");
                }
            }
            else
            {
                this.logService.Error("FilePersistanceService", "GetItemNamesFromContainer", $"Did not find Container '{ContainerName}'!");
            }

            return result;
        }
        #endregion GetItemNamesFromContainer

        #endregion Methods
    }
}
