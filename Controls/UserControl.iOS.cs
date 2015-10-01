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
                oldValue?.NativeUIElement.RemoveFromSuperview();
                this.AddLogicalChild(newValue);
                this.NativeUIElement.AddSubview(newValue.NativeUIElement);
            }
            else
            {
                this.AddLogicalChild(newValue);
            }
        }
    }
}