using System;

namespace Appercode.UI.Internals
{
    internal class BindingValueCache
    {
        internal readonly Type BindingValueType;

        internal readonly object ValueAsBindingValueType;

        internal BindingValueCache(Type bindingValueType, object valueAsBindingValueType)
        {
            this.BindingValueType = bindingValueType;
            this.ValueAsBindingValueType = valueAsBindingValueType;
        }
    }
}
