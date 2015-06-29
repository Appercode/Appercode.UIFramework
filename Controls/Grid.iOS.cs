using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class Grid
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
            this.ArrangeCells(size);
        }

        private void NativeChildrenCollectionChanged()
        {
        }
    }
}