using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class BindingValueChangedEventArgs : EventArgs
    {
        private object oldValue;

        private object newValue;

        internal BindingValueChangedEventArgs(object oldValue, object newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public object NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        public object OldValue
        {
            get
            {
                return this.oldValue;
            }
        }
    }
}
