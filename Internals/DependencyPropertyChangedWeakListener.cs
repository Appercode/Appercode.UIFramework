using System;
using System.Windows;

namespace Appercode.UI.Internals
{
    internal class DependencyPropertyChangedWeakListener
    {
        private DependencyObject source;

        private WeakReference expression;

        internal DependencyPropertyChangedWeakListener(DependencyObject source, TemplateBindingExpression expression)
        {
            this.source = source;
            this.expression = new WeakReference(expression);
            this.source.DPChanged += this.SourcePropertyChanged;
        }

        internal void Disconnect()
        {
            if (this.source == null)
            {
                return;
            }
            this.source.DPChanged -= this.SourcePropertyChanged;
            this.source = null;
            this.expression = null;
        }

        internal void SourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TemplateBindingExpression target;
            if (this.expression != null)
            {
                target = this.expression.Target as TemplateBindingExpression;
            }
            else
            {
                target = null;
            }
            TemplateBindingExpression templateBindingExpression = target;
            if (templateBindingExpression == null)
            {
                this.Disconnect();
                return;
            }

            templateBindingExpression.SourcePropertyChanged(sender, e.Property);
        }
    }
}