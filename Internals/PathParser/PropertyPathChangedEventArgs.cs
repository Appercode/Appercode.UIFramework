using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal class PropertyPathChangedEventArgs
    {
        internal PropertyPathChangedEventArgs(PropertyPathStep source)
        {
            this.ChangedPart = source;
        }

        internal PropertyPathStep ChangedPart
        {
            get;
            private set;
        }
    }
}
