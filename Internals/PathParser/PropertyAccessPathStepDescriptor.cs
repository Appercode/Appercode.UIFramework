using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal class PropertyAccessPathStepDescriptor : PropertyPathStepDescriptor
    {
        private string name;

        internal PropertyAccessPathStepDescriptor(string name)
        {
            this.name = name;
        }

        internal override PropertyPathStep CreateStep(PropertyPathListener listener, object source, bool listenToChanges)
        {
            return new PropertyAccessPathStep(listener, source, this.name, listenToChanges);
        }
    }
}
