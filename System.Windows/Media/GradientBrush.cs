using Appercode.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace System.Windows.Media
{
    public abstract class GradientBrush : Brush
    {
        public static readonly DependencyProperty SpreadMethodProperty =
            DependencyProperty.Register("SpreadMethod", typeof(GradientSpreadMethod), typeof(GradientBrush), new PropertyMetadata(default(GradientSpreadMethod)));

        public static readonly DependencyProperty MappingModeProperty =
            DependencyProperty.Register("MappingMode", typeof(BrushMappingMode), typeof(GradientBrush), new PropertyMetadata(default(BrushMappingMode)));

        public static readonly DependencyProperty ColorInterpolationModeProperty =
            DependencyProperty.Register("ColorInterpolationMode", typeof(ColorInterpolationMode), typeof(GradientBrush), new PropertyMetadata(default(ColorInterpolationMode)));

        public static readonly DependencyProperty GradientStopsProperty =
            DependencyProperty.Register("GradientStops", typeof(GradientStopCollection), typeof(GradientBrush), new PropertyMetadata(default(GradientStopCollection)));

        public GradientSpreadMethod SpreadMethod
        {
            get { return (GradientSpreadMethod)GetValue(SpreadMethodProperty); }
            set { this.SetValue(SpreadMethodProperty, value); }
        }

        public BrushMappingMode MappingMode
        {
            get { return (BrushMappingMode)GetValue(MappingModeProperty); }
            set { this.SetValue(MappingModeProperty, value); }
        }

        public ColorInterpolationMode ColorInterpolationMode
        {
            get { return (ColorInterpolationMode)GetValue(ColorInterpolationModeProperty); }
            set { this.SetValue(ColorInterpolationModeProperty, value); }
        }

        public GradientStopCollection GradientStops
        {
            get { return (GradientStopCollection)GetValue(GradientStopsProperty); }
            set { this.SetValue(GradientStopsProperty, value); }
        }
    }
}