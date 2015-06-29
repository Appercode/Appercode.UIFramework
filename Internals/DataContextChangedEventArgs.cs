using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal sealed class DataContextChangedEventArgs : EventArgs
    {
        public DataContextChangedEventArgs(DataContextChangedReason reason)
        {
            this.ChangeReason = reason;
        }
        
        public DataContextChangedReason ChangeReason
        {
            get;
            private set;
        }
    }
}
