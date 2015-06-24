using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class UserControl
    {
        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new UIView();
            }
            if (this.Content != null && this.Content.NativeUIElement != null)
            {
                this.NativeUIElement.AddSubview(this.Content.NativeUIElement);
            }
            this.NativeUIElement.ClipsToBounds = true;
            base.NativeInit();
        }

        protected void NativeArrangeContent(CGRect finalRect)
        {
            this.Content.Arrange(finalRect);
        }

        private void OnNativeContentChanged(UIElement oldValue, UIElement newValue)
        {
            if (this.NativeUIElement != null)
            {
                if (oldValue != null)
                {
                    oldValue.NativeUIElement.RemoveFromSuperview();
                }
                LogicalTreeHelper.AddLogicalChild(this, newValue);
                this.NativeUIElement.AddSubview(newValue.NativeUIElement);
            }
            else
            {
                LogicalTreeHelper.AddLogicalChild(this, newValue);
            }
        }
    }
}