using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class StackPanel
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UIView { ClipsToBounds = true };
                }

                base.NativeInit();
            }
        }

        protected override void ArrangeChilds(CGSize size)
        {
            this.UpdateLayout();
        }

        private void NativeChildrenCollectionChanged()
        {
        }
    }
}
