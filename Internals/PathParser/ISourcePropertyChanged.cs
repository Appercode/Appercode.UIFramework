using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal interface ISourcePropertyChanged
    {
        void SourcePropertyChanged(object sender, PropertyChangedEventArgs args);
    }
}
