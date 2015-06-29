using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal class SourcePathStepDescriptor : PropertyPathStepDescriptor
    {
        public SourcePathStepDescriptor()
        {
        }

        internal override PropertyPathStep CreateStep(PropertyPathListener listener, object source, bool listenToChanges)
        {
            return new SourcePropertyPathStep(listener, source);
        }
    }
}
