using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Appercode.UI.Controls
{
    [ContentProperty("Content")]
    public partial class UserControl
    {
        private void OnNativeContentChanged(UIElement oldValue, UIElement newValue)
        {
        }

        protected void NativeArrangeContent(RectangleF finalRect)
        {
        }

        private SizeF NativeMeasureContent(SizeF availableSize)
        {
            return new SizeF();
        }
    }
}
