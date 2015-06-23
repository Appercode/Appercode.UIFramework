using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    public class ValidationErrorEventArgs : RoutedEventArgs
    {
        internal ValidationErrorEventArgs(ValidationErrorEventAction action, ValidationError error)
        {
            this.Action = action;
            this.Error = error;
            this.Handled = false;
        }

        public ValidationErrorEventAction Action
        {
            get;
            internal set;
        }

        public ValidationError Error
        {
            get;
            internal set;
        }

        public bool Handled
        {
            get;
            set;
        }
    }
}
