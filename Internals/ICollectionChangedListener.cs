using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal interface ICollectionChangedListener
    {
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
    }
}
