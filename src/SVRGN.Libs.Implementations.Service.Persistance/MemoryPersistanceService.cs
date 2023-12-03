using SVRGN.Libs.Contracts.Service.Logging;
using SVRGN.Libs.Contracts.Service.Object;
using SVRGN.Libs.Contracts.Service.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVRGN.Libs.Implementations.Service.Persistance
{
    public class MemoryPersistanceService : BasePersistanceService
    {
        #region Properties

        #endregion Properties

        #region Construction

        public MemoryPersistanceService(ILogService LogService, IObjectService ObjectService)
            : base(LogService, ObjectService)
        {
            this.persistanceType = "Memory";
        }

        #endregion Construction

        #region Methods

        #region Write
        public override bool WriteToEndpoint<T>(string EndpointName, T Value)
        {
            bool result = false;
            if (this.Exists(EndpointName))
            {
                if (this.endpoints.Any(x => x.Name.Equals(EndpointName)))
                {
                    //end point exists already
                    this.endpoints.RemoveAll(x => x.Name.Equals(EndpointName));
                }
                else
                {
                    // end point does not exist yet
                    // do nothing
                }
                IPersistanceEndpoint newEndpoint = this.objectService.Create<IPersistanceEndpoint>(EndpointName);
                IPersistanceValue newValue = this.objectService.Create<IPersistanceValue>("default", Value);
                newEndpoint.SetValue(newValue);
                this.endpoints.Add(newEndpoint);
                result = true;
            }
            return result;
        }
        #endregion Write

        #region Read
        public override T ReadFromEndpoint<T>(string EndpointName)
        {
            T result = default;

            if (this.Exists(EndpointName))
            {
                result = (T)this.endpoints.Where(x => x.Name.Equals(EndpointName)).FirstOrDefault()?.Value.Value;
            }

            return result;
        }
        #endregion Read

        #region ValueExists
        public override bool ValueExists(string EndpointName)
        {
            bool result = false;

            if (this.Exists(EndpointName))
            {
                result = this.endpoints.Any(x => x.Name.Equals(EndpointName));
            }

            return result;
        }
        #endregion ValueExists

        #region WriteToContainer
        public override bool WriteToContainer<T>(string ContainerName, string ValueName, T Value)
        {
            bool result = false;
            if (this.containers.Any(x => x.Name.Equals(ContainerName)))
            {
                try
                {
                    IPersistanceValue newValue = this.objectService.Create<IPersistanceValue>(ValueName, Value);
                    IPersistanceContainer container = this.GetContainer(ContainerName);
                    if (container != null)
                    {
                        container.AddValue(newValue);
                        result = true;
                    }
                    else
                    {
                        //TODO: container is null
                    }
                }
                catch (Exception ex)
                {
                    this.logService.Error("MemoryPersistanceService", "WriteToContainer", $"Error when writing to container named '{ContainerName}'", ex);
                }
            }
            else
            {
                this.logService.Error("MemoryPersistanceService", "WriteToContainer", $"You are trying to persist to container '{ContainerName}', qhich does not exist");
            }
            return result;
        }
        #endregion WriteToContainer

        #region ValueExistsInContainer
        public override bool ValueExistsInContainer(string ContainerName, string ValueName)
        {
            bool result = false;

            if (this.Exists(ContainerName))
            {
                result = this.GetContainer(ContainerName).Values.Any(x => x.Name.Equals(ValueName));
            }

            return result;
        }
        #endregion ValueExistsInContainer

        #region ReadFromContainer
        public override T ReadFromContainer<T>(string ContainerName, string ValueName)
        {
            T result = default;

            if (this.ValueExistsInContainer(ContainerName, ValueName))
            {
                result = (T)this.GetContainer(ContainerName).Values.Where(x => x.Name.Equals(ValueName)).FirstOrDefault().Value;
            }

            return result;
        }
        #endregion ReadFromContainer

        #region ListItemsInContainer
        public override List<string> GetItemNamesFromContainer(string ContainerName)
        {
            List<string> result = default;

            if (this.Exists(ContainerName))
            {
                result = new List<string>();

                IPersistanceContainer container = this.GetContainer(ContainerName);
                foreach (IPersistanceValue value in container.Values)
                {
                    result.Add(value.Name);
                }
            }

            return result;
        }
        #endregion ListItemsInContainer

        #endregion Methods
    }
}
