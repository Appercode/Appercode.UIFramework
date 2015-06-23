using Appercode.UI;
using System;

namespace System.Windows.Media
{
    public abstract partial class Brush : DependencyObject
    {
        public static readonly DependencyProperty OpacityProperty;

        //// TODO TransformProperty, RelativeTransformProperty

        public double Opacity
        {
            get
            {
                return (double)this.GetValue(Brush.OpacityProperty);
            }
            set
            {
                this.SetValue(Brush.OpacityProperty, value);
            }
        }
    }
}