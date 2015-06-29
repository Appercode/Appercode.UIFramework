using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    [ContentProperty("Child")]
    public partial class Border
    {

        private Thickness NativePadding { get; set; }
        private Thickness NativeBorderThickness { get; set; }
        private CornerRadius NativeCornerRadius { get; set; }
        private Brush NativeBackground { get; set; }
        private Brush NativeBorderBrush { get; set; }

        private void OnNativeContentChanged(UIElement oldValue, UIElement newValue)
        {
        }

        private void NativeArrangeContent(RectangleF finalRect)
        {
        }

        private SizeF NativeMeasureContent(SizeF availableSize)
        {
            return new SizeF();
        }
        private void NativeOnBackgroundChanged()
        {
        }

        private void NativeOnBorderBrushChanged()
        {
        }
    }
}
