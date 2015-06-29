using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

namespace Appercode.UI.Controls
{
    [ContentProperty("Text")]
    public partial class TextBlock
    {
        protected string NativeText { get; set; }

        protected double NativeFontSize { get; set; }

        protected TextWrapping NativeTextWrapping { get; set; }

        protected TextTrimming NativeTextTrimming { get; set; } 
        
        protected FontWeight NativeFontWeight { get; set; }
        
        protected FontStyle NativeFontStyle { get; set; }
        
        protected TextAlignment NativeTextAlignment { get; set; }
        
        protected FontFamily NativeFontFamily { get; set; }
        
        protected Brush NativeForeground { get; set; }
        
        protected Thickness NativePadding { get; set; }

        private static double GetDefaultFontSize()
        {
            return 17;
        }
    }
}