using SVRGN.Libs.Contracts.Service.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace SVRGN.Libs.Implementations.Service.Persistance
{
    public class PersistancePath : IPersistancePath
    {
        #region Properties

        public string Target { get; private set; }

        public string Path { get; private set; }

        #endregion Properties

        #region Construction

        public PersistancePath(string Target, string Path)
        {
            this.Target = Target;
            this.Path = Path;
        }

        #endregion Construction

        #region Methods

        #endregion Methods
    }
}
