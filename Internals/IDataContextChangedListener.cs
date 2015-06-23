using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal interface IDataContextChangedListener
    {
        void OnDataContextChanged(object sender, DataContextChangedEventArgs e);
    }
}
