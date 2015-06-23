using System.Windows;
using System.Windows.Markup;

namespace Appercode.UI
{
    [ContentProperty("VisualTree")]
    public abstract partial class FrameworkTemplate 
    {
        /// <summary>
        /// This definition is only for XAML XSD schema generation
        /// </summary>
        public static readonly DependencyProperty VisualTreeProperty =
            DependencyProperty.Register(
                "VisualTree",
                typeof(FrameworkElementFactory),
                typeof(FrameworkTemplate),
                new PropertyMetadata());
    }
}