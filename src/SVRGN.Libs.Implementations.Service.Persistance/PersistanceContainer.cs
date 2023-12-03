using SVRGN.Libs.Contracts.Service.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVRGN.Libs.Implementations.Service.Persistance
{
    public class PersistanceContainer : IPersistanceContainer
    {
        #region Properties

        public string Name { get; private set; }
        public IList<IPersistanceValue> Values { get; private set; }

        public IList<IPersistancePath> Paths { get; private set; }

        #endregion Properties

        #region Construction

        public PersistanceContainer(string Name)
        {
            this.Name = Name;

            this.Values = new List<IPersistanceValue>();
            this.Paths = new List<IPersistancePath>();
        }
        #endregion Construction

        #region Methods

        #region AddValue
        public bool AddValue(IPersistanceValue NewValue)
        {
            bool result = false;

            if (!this.Values.Any(x => x.Name.Equals(NewValue.Name)))
            {
                this.Values.Add(NewValue);
                result = true;
            }

            return result;
        }
        #endregion AddValue

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

        #endregion Methods
    }
}
