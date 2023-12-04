using SVRGN.Libs.Contracts.Service.Logging;
using SVRGN.Libs.Contracts.Service.Object;
using SVRGN.Libs.Contracts.Service.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVRGN.Libs.Implementations.Service.Persistance
{
    /// <summary>
    /// base persistance service for all others to inherit.
    /// </summary>
    public abstract class BasePersistanceService : IPersistanceService
    {
        #region Properties

        public bool IsInitialized { get; internal set; } = false;

        protected List<IPersistanceEndpoint> endpoints;
        protected List<IPersistanceContainer> containers;

        protected IObjectService objectService;
        protected ILogService logService;

        protected string persistanceType = "not set";

        #endregion Properties

        #region Construction

        public BasePersistanceService(ILogService LogService, IObjectService ObjectService)
        {
            this.objectService = ObjectService;
            this.logService = LogService;
        }

        #endregion Construction

        #region Methods

        #region Initialize
        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                this.endpoints = new List<IPersistanceEndpoint>();
                this.containers = new List<IPersistanceContainer>();
                this.IsInitialized = true;
            }
        }
        #endregion Initialize

        #region AddContainer
        public bool AddContainer(string ContainerName, string Path)
        {
            bool result = false;

            if (!this.IsInitialized)
            {
                this.Initialize();  //kind of workaround, though
            }

            try
            {
                this.logService.Debug("BasePersistanceService", "AddContainer", $"Attempting to create a container object '{ContainerName}'");
                IPersistanceContainer newContainer = this.objectService.Create<IPersistanceContainer>(ContainerName);
                this.logService.Debug("BasePersistanceService", "AddContainer", $"Attempting to create a path object '{Path}'");
                IPersistancePath firstPath = this.objectService.Create<IPersistancePath>(this.persistanceType, Path);
                this.logService.Debug("BasePersistanceService", "AddContainer", $"Attempting to add the path object to the container");
                newContainer.AddPath(firstPath);
                this.logService.Debug("BasePersistanceService", "AddContainer", $"Attempting to add the container to the list of containers");
                this.containers.Add(newContainer);
                result = true;
            }
            catch (Exception ex)
            {
                this.logService.Error("BasePersistanceService", "AddContainer", $"Error when adding a new container named '{ContainerName}' with Path '{Path}'", ex);
            }

            return result;
        }
        #endregion AddContainer

        #region AddEndpoint
        public bool AddEndpoint(string EndpointName, string Path)
        {
            bool result = false;

            if (!this.IsInitialized)
            {
                this.Initialize();  //kind of workaround, though
            }

            if (!this.Exists(EndpointName))
            {
                IPersistanceEndpoint newEndpoint = this.objectService.Create<IPersistanceEndpoint>(EndpointName);
                IPersistancePath newPath = this.objectService.Create<IPersistancePath>(this.persistanceType, Path);
                newEndpoint.AddPath(newPath);
                this.endpoints.Add(newEndpoint);
                result = true;
            }
            return result;
        }
        #endregion AddEndpoint

        #region Exists
        public bool Exists(string EndpointName)
        {
            bool result = false;

            if (this.endpoints.Any(x => x.Name.Equals(EndpointName)))
            {
                result = true;
            }
            else if (this.containers.Any(x => x.Name.Equals(EndpointName)))
            {
                result = true;
            }

            return result;
        }
        #endregion Exists

        #region GetPaths
        protected IList<IPersistancePath> GetPaths(string Endpoint)
        {
            IList<IPersistancePath> result = default;

            if (this.Exists(Endpoint))
            {
                result = this.endpoints.Where(x => x.Name.Equals(Endpoint)).FirstOrDefault()?.Paths;
            }

            return result;
        }
        #endregion GetPaths

        #region GetContainer
        protected IPersistanceContainer GetContainer(string ContainerName)
        {
            IPersistanceContainer result = null;

            if (this.containers.Any(x => x.Name.Equals(ContainerName)))
            {
                result = this.containers.Where(x => x.Name.Equals(ContainerName)).FirstOrDefault();
            }

            return result;
        }
        #endregion GetContainer

        #region GetEndpoint
        protected IPersistanceEndpoint GetEndpoint(string EndpointName)
        {
            IPersistanceEndpoint result = null;

            if (this.endpoints.Any(x => x.Name.Equals(EndpointName)))
            {
                result = this.endpoints.Where(x => x.Name.Equals(EndpointName)).FirstOrDefault();
            }

            return result;
        }
        #endregion GetEndpoint

        public abstract List<string> GetItemNamesFromContainer(string ContainerName);

        public abstract T ReadFromEndpoint<T>(string EndPoint);

        public abstract T ReadFromContainer<T>(string ContainerName, string ValueName);

        public abstract bool ValueExists(string EndPoint);

        public abstract bool ValueExistsInContainer(string ContainerName, string ValueName);

        public abstract bool WriteToEndpoint<T>(string EndPoint, T Value);

        public abstract bool WriteToContainer<T>(string ContainerName, string ValueName, T Value);
        #endregion Methods
    }
}
