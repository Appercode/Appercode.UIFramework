using System;

namespace Appercode.UI.Internals
{
    internal abstract class DeferredReference
    {
        protected DeferredReference()
        {
        }

        internal abstract object GetValue(BaseValueSourceInternal valueSource);

        internal abstract Type GetValueType();
    }
}
