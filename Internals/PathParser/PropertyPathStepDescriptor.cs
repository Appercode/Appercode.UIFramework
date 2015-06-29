using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal abstract class PropertyPathStepDescriptor
    {
        protected PropertyPathStepDescriptor()
        {
        }

        internal abstract PropertyPathStep CreateStep(PropertyPathListener listener, object source, bool listenToChanges);
    }
}
