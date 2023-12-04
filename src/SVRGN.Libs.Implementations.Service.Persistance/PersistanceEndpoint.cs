using SVRGN.Libs.Contracts.Service.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVRGN.Libs.Implementations.Service.Persistance
{
    /// <summary>
    /// Implementation of the persistance endpoint interface.
    /// </summary>
    public class PersistanceEndpoint : IPersistanceEndpoint
    {
        #region Properties

        public string Name { get; private set; }

        public IPersistanceValue Value { get; private set; }

        public IList<IPersistancePath> Paths { get; private set; }

        #endregion Properties

        #region Construction

        public PersistanceEndpoint(string Name)
        {
            this.Name = Name;
            this.Value = default;

            this.Paths = new List<IPersistancePath>();
        }
        #endregion Construction 

        #region Methods

        #region AddPath
        public bool AddPath(IPersistancePath NewPath)
        {
            bool result = false;

            if (!this.Paths.Any(x => x.Path.Equals(NewPath.Path)))
            {
                this.Paths.Add(NewPath);
                result = true;
            }

            return result;
        }
        #endregion AddPath

        #region SetValue
        public void SetValue(IPersistanceValue NewValue)
        {
            this.Value = NewValue;
        }
        #endregion SetValue

        #endregion Methods
    }
}
