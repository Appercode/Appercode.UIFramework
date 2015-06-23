using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal interface IErrorsChangedListener
    {
        void OnErrorsChanged(object sender, bool notifyChild, DataErrorsChangedEventArgs e);
    }
}
