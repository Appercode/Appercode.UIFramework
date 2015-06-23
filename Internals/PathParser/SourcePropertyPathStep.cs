using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal class SourcePropertyPathStep : PropertyPathStep
    {
        private object source;

        internal SourcePropertyPathStep(PropertyPathListener listener, object source)
            : base(listener)
        {
            this.source = source;
        }

        internal override bool IsConnected
        {
            get
            {
                return this.source != null;
            }
        }

        internal override object Property
        {
            get
            {
                return null;
            }
        }

        internal override string PropertyName
        {
            get
            {
                return string.Empty;
            }
        }

        internal override object Source
        {
            get
            {
                return this.source;
            }
        }

        internal override Type Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal override object Value
        {
            get
            {
                return this.source;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        internal override void Disconnect()
        {
            this.source = null;
        }

        internal override void ReConnect(object newSource)
        {
            this.source = newSource;
        }
    }
}