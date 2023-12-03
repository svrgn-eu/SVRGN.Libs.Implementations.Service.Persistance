using SVRGN.Libs.Contracts.Service.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace SVRGN.Libs.Implementations.Service.Persistance
{
    public class PersistanceValue : IPersistanceValue
    {
        #region Properties

        public string Name { get; private set; }
        public Type OriginalType { get; private set; }

        public object Value { get; private set; }

        #endregion Properties

        #region Construction

        public PersistanceValue(string Name, object Value)
        {
            this.Name = Name;
            this.Value = Value;
            this.OriginalType = Value.GetType();
        }

        #endregion Construction

        #region Methods

        #endregion Methods

        #region Events

        #endregion Events
    }
}
