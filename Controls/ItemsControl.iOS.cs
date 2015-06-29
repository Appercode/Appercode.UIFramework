using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ItemsControl
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UIView();
                    this.NativeUIElement.ClipsToBounds = true;
                    this.AddPanelToNativeContainer();
                }
                base.NativeInit();
            }
        }
        protected override void NativeArrange(CGRect finalRect)
        {
            base.NativeArrange(finalRect);
            finalRect.Width -= this.Margin.HorizontalThicknessF();
            finalRect.Height -= this.Margin.VerticalThicknessF();
            this.panel.Arrange(new CGRect(CGPoint.Empty, finalRect.Size));
        }

        protected virtual void AddPanelToNativeContainer()
        {
            if (this.NativeUIElement != null)
            {
                this.NativeUIElement.AddSubview(this.panel.NativeUIElement);
            }
        }

        protected virtual void RemovePanelFromNativeContainer()
        {
            this.panel.NativeUIElement.RemoveFromSuperview();
        }
    }
}
