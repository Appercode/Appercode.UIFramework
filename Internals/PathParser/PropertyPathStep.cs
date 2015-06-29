using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal abstract class PropertyPathStep
    {
        internal PropertyPathStep(PropertyPathListener listener)
        {
            this.Listener = listener;
        }

        internal abstract bool IsConnected
        {
            get;
        }

        internal PropertyPathListener Listener
        {
            get;
            private set;
        }

        internal PropertyPathStep NextStep
        {
            get;
            set;
        }

        internal abstract object Property
        {
            get;
        }

        internal abstract string PropertyName
        {
            get;
        }

        internal abstract object Source
        {
            get;
        }

        internal abstract Type Type
        {
            get;
        }

        internal abstract object Value
        {
            get;
            set;
        }

        internal abstract void Disconnect();

        internal abstract void ReConnect(object newSource);
    }
}
