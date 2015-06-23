using Appercode.UI.Controls;
using Appercode.UI.Internals;
using Appercode.UI.Markup;
using System;
using System.Windows;

namespace Appercode.UI
{
    public class TemplateBindingExpression : Expression
    {
        internal DependencyObject Source;

        private DependencyProperty sourceProperty;

        private UIElement target;

        private DependencyProperty targetProperty;

        private DependencyPropertyChangedWeakListener listener;

        private bool runtimeCheck;

        private TemplateBindingExtension bindingExtension;

        internal TemplateBindingExpression()
        {
        }

        internal TemplateBindingExpression(DependencyObject source, TemplateBindingExtension binding, bool runtimeCheck)
        {
            this.Source = source;
            this.sourceProperty = binding.Property;
            this.bindingExtension = binding;
            this.runtimeCheck = runtimeCheck;
        }

        internal static bool PropertyTypesCompatible(Type sourceType, Type targetType, out bool runtimeCheck)
        {
            if (sourceType == targetType || targetType.IsAssignableFrom(sourceType))
            {
                runtimeCheck = false;
                return true;
            }
            if (sourceType.IsAssignableFrom(targetType))
            {
                runtimeCheck = true;
                return true;
            }
            runtimeCheck = false;
            return false;
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            object value = this.Source.GetValue(this.sourceProperty);
            if (this.bindingExtension.Converter != null)
            {
                value = this.bindingExtension.Converter.Convert(value, this.targetProperty.PropertyType, this.bindingExtension.ConverterParameter, System.Threading.Thread.CurrentThread.CurrentCulture);
            }
            if (!this.runtimeCheck || this.IsValidValueForUpdate(value, this.targetProperty.PropertyType))
            {
                return value;
            }
            return this.targetProperty.GetDefaultValue(this.target);
        }

        internal override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            this.target = d as UIElement;
            this.targetProperty = dp;
            this.listener = new DependencyPropertyChangedWeakListener(this.Source, this);
        }

        internal override void OnDetach(DependencyObject d, DependencyProperty dp)
        {
            this.listener.Disconnect();
        }

        internal void SourcePropertyChanged(object sender, DependencyProperty dp)
        {
            if (dp == this.sourceProperty)
            {
                this.target.RefreshExpression(this.targetProperty);
            }
        }
    }
}