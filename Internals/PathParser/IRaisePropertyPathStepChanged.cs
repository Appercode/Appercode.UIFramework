using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal interface IRaisePropertyPathStepChanged
    {
        void RaisePropertyPathStepChanged(PropertyListener source);
    }
}
