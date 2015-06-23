using Appercode.UI.Controls;
using System.Windows;

namespace Appercode.UI.Data
{
    public static class BindingOperations
    {
        public static void SetBinding(UIElement target, DependencyProperty dp, Binding binding)
        {
            target.SetBinding(dp, binding);
        }
    }
}