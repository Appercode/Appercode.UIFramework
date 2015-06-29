using Appercode.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace System.Windows.Media
{
    public sealed class GradientStop : DependencyObject
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(GradientStop), new PropertyMetadata(default(Color)));

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(GradientStop), new PropertyMetadata(default(double)));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { this.SetValue(OffsetProperty, value); }
        } 
    }
}