using Appercode.UI.Controls;
using System;
using System.Windows;

namespace Appercode.UI.Data
{
    public static class BindingOperations
    {
        /// <summary>
        /// Retrieves a BindingExpressionBase.
        /// </summary>
        /// <param name="target">Object from which to retrieve the BindingExpressionBase.</param>
        /// <param name="dp">Dependency Property from which to retrieve the BindingExpressionBase.</param>
        /// <returns>A BindingExpressionBase object or null if no Binding has been set on the given property.</returns>
        public static BindingExpressionBase GetBindingExpressionBase(DependencyObject target, DependencyProperty dp)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }

            return target.GetExpression(dp) as BindingExpressionBase;
        }

        /// <summary>
        /// Retrieves a BindingExpression.
        /// </summary>
        /// <param name="target">Object from which to retrieve the BindingExpression.</param>
        /// <param name="dp">Dependency Property from which to retrieve the BindingExpression.</param>
        /// <returns>A BindingExpression object or null if no Binding has been set on the given property.</returns>
        public static BindingExpression GetBindingExpression(DependencyObject target, DependencyProperty dp)
        {
            return GetBindingExpressionBase(target, dp) as BindingExpression;
        }

        public static void SetBinding(UIElement target, DependencyProperty dp, Binding binding)
        {
            target.SetBinding(dp, binding);
        }
    }
}