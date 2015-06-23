using Appercode.UI.Data;
using System.Windows;

namespace Appercode.UI.Internals
{
    internal struct TriggerCondition
    {
        internal readonly DependencyProperty Property;

        internal readonly BindingBase Binding;

        internal readonly LogicalOp LogicalOp;

        internal readonly object Value;

        internal readonly string SourceName;

        internal int SourceChildIndex;

        internal BindingValueCache BindingValueCache;

        internal TriggerCondition(DependencyProperty dp, LogicalOp logicalOp, object value, string sourceName)
        {
            this.Property = dp;
            this.Binding = null;
            this.LogicalOp = logicalOp;
            this.Value = value;
            this.SourceName = sourceName;
            this.SourceChildIndex = 0;
            this.BindingValueCache = new BindingValueCache(null, null);
        }

        internal TriggerCondition(BindingBase binding, LogicalOp logicalOp, object value)
            : this(binding, logicalOp, value, "~Self")
        {
        }

        internal TriggerCondition(BindingBase binding, LogicalOp logicalOp, object value, string sourceName)
        {
            this.Property = null;
            this.Binding = binding;
            this.LogicalOp = logicalOp;
            this.Value = value;
            this.SourceName = sourceName;
            this.SourceChildIndex = 0;
            this.BindingValueCache = new BindingValueCache(null, null);
        }

        /*
        //internal bool ConvertAndMatch(object state)
        //{
        //    Type type;
        //    object value = this.Value;
        //    string str = value as string;
        //    if (state != null)
        //    {
        //        type = state.GetType();
        //    }
        //    else
        //    {
        //        type = null;
        //    }
        //    Type type1 = type;
        //    if (str != null && type1 != null && type1 != typeof(string))
        //    {
        //        BindingValueCache bindingValueCache = this.BindingValueCache;
        //        Type bindingValueType = bindingValueCache.BindingValueType;
        //        object valueAsBindingValueType = bindingValueCache.ValueAsBindingValueType;
        //        if (type1 != bindingValueType)
        //        {
        //            valueAsBindingValueType = value;
        //            TypeConverter converter = DefaultValueConverter.GetConverter(type1);
        //            if (converter != null && converter.CanConvertFrom(typeof(string)))
        //            {
        //                try
        //                {
        //                    valueAsBindingValueType = converter.ConvertFromString(null, TypeConverterHelper.EnglishUSCulture, str);
        //                }
        //                catch (Exception exception)
        //                {
        //                    if (CriticalExceptions.IsCriticalException(exception))
        //                    {
        //                        throw;
        //                    }
        //                }
        //                catch
        //                {
        //                }
        //            }
        //            bindingValueCache = new BindingValueCache(type1, valueAsBindingValueType);
        //            this.BindingValueCache = bindingValueCache;
        //        }
        //        value = valueAsBindingValueType;
        //    }
        //    return this.Match(state, value);
        //}
        */

        internal bool Match(object state)
        {
            return this.Match(state, this.Value);
        }

        internal bool TypeSpecificEquals(TriggerCondition value)
        {
            if (this.Property == value.Property && this.Binding == value.Binding && this.LogicalOp == value.LogicalOp && this.Value == value.Value && this.SourceName == value.SourceName)
            {
                return true;
            }
            return false;
        }

        private bool Match(object state, object referenceValue)
        {
            if (this.LogicalOp == Appercode.UI.Internals.LogicalOp.Equals)
            {
                return object.Equals(state, referenceValue);
            }
            return !object.Equals(state, referenceValue);
        }
    } 
}