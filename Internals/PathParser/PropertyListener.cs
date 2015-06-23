using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal abstract class PropertyListener
    {
        protected readonly IRaisePropertyPathStepChanged PathStep;

        protected bool listenToChanges;

        internal PropertyListener(IRaisePropertyPathStepChanged pathStep, bool listenToChanges)
        {
            this.PathStep = pathStep;
            this.listenToChanges = listenToChanges;
        }

        internal abstract object Property
        {
            get;
        }

        internal string PropertyName
        {
            get
            {
                return ((PropertyPathStep)this.PathStep).PropertyName;
            }
        }

        internal abstract Type PropertyType
        {
            get;
        }

        internal abstract object Source
        {
            get;
            set;
        }

        internal abstract Type SourceType
        {
            get;
        }

        internal abstract object Value
        {
            get;
            set;
        }

        internal abstract void Disconnect();
    }
}
