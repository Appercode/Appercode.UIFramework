using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls.Primitives
{
    public abstract partial class IconElement : UIElement
    {
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(IconElement));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { this.SetValue(ForegroundProperty, value); }
        }
    }
}