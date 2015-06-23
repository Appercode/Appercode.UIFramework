using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal class IndexerPathStepDescriptor : PropertyPathStepDescriptor
    {
        private string index;

        internal IndexerPathStepDescriptor(string index)
        {
            this.index = index;
        }

        internal override PropertyPathStep CreateStep(PropertyPathListener listener, object source, bool listenToChanges)
        {
            return new IndexerPathStep(listener, source, this.index, listenToChanges);
        }
    }
}
