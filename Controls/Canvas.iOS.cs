using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class Canvas
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UIView();
                }
                base.NativeInit();
            }
        }

        protected override void ArrangeChilds(CGSize size)
        {
            this.UpdateLayout();
        }

        private void NativeReorderChildren()
        {
            foreach (var child in this.Children)
            {
                this.NativeUIElement.BringSubviewToFront(child.NativeUIElement);
            }
        }
    }
}