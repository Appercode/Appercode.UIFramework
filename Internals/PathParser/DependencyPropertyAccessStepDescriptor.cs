using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal class DependencyPropertyAccessStepDescriptor : PropertyPathStepDescriptor
    {
        private DependencyProperty property;

        internal DependencyPropertyAccessStepDescriptor(DependencyProperty property)
        {
            this.property = property;
        }

        internal override PropertyPathStep CreateStep(PropertyPathListener listener, object source, bool listenToChanges)
        {
            return new PropertyAccessPathStep(listener, source, this.property, listenToChanges);
        }
    }
}
