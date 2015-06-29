using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Appercode.UI.Controls.Popups
{
    public delegate void UICommandInvokedHandler(UICommand command);

    public sealed class UICommand
    {
        public UICommand(string label):this(label, null)
        {
        }

        public UICommand(string label, UICommandInvokedHandler action)
        {
            this.Label = label;
            this.Action = action;
        }

        internal string Label
        {
            get;
            set;
        }

        internal UICommandInvokedHandler Action
        {
            get;
            set;
        }
    }
}