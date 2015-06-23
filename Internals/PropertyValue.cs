using System.Windows;

namespace Appercode.UI.Internals
{
    internal struct PropertyValue
    {
        internal PropertyValueType ValueType;

        ////internal TriggerCondition[] Conditions;

        internal string ChildName;

        internal DependencyProperty Property;

        internal object ValueInternal;

        internal object Value
        {
            get
            {
                DeferredReference valueInternal = this.ValueInternal as DeferredReference;
                if (valueInternal != null)
                {
                    this.ValueInternal = valueInternal.GetValue(BaseValueSourceInternal.Unknown);
                }
                return this.ValueInternal;
            }
        }
    }
}
