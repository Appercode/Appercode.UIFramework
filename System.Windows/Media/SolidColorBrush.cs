using Appercode.UI;
using System;
using System.Windows.Media;

namespace System.Windows.Media
{
    public sealed partial class SolidColorBrush : Brush
    {
        public static readonly DependencyProperty ColorProperty =
                    DependencyProperty.Register("Color", typeof(object), typeof(System.Windows.Media.Color),
                                                new PropertyMetadata(null, (d, e) =>
                                                {
                                                }));

        public SolidColorBrush(Color color)
        {
            this.Color = color;
        }

        public System.Windows.Media.Color Color
        {
            get
            {
                return (Color)this.GetValue(SolidColorBrush.ColorProperty);
            }
            set
            {
                this.SetValue(SolidColorBrush.ColorProperty, value);
            }
        }
    }
}