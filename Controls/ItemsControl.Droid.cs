using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrappers;
using System.Drawing;

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
                    this.NativeUIElement = new WrappedViewGroup(this);
                    this.AddPanelToNativeContainer();
                }

                base.NativeInit();
            }
        }

        protected override void NativeArrange(RectangleF finalRect)
        {
            base.NativeArrange(finalRect);
            finalRect.Width -= this.Margin.HorizontalThicknessF();
            finalRect.Height -= this.Margin.VerticalThicknessF();
            this.panel.Arrange(new RectangleF(PointF.Empty, finalRect.Size));
        }

        protected virtual void AddPanelToNativeContainer()
        {
            if (this.NativeUIElement != null)
            {
                ((ViewGroup)this.NativeUIElement).AddView(this.panel.NativeUIElement);
            }
        }

        protected virtual void RemovePanelFromNativeContainer()
        {
            if (this.NativeUIElement != null)
            {
                ((ViewGroup)this.NativeUIElement).RemoveAllViews();
            }
        }
    }
}