using CoreGraphics;
using System.Linq;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class Panel
    {
        protected internal override void NativeInit()
        {
            if (this.Children.Count > 1)
            {
                this.NativeUIElement.AddSubviews(this.Children.Select(child => child.NativeUIElement).ToArray());
            }
            else if (this.Children.Count > 0)
            {
                this.NativeUIElement.AddSubview(this.Children[0].NativeUIElement);
            }

            base.NativeInit();
            this.NativeOnbackgroundChange();
        }

        protected void AddNativeChildView(UIElement child)
        {
            if (this.NativeUIElement != null)
            {
                this.NativeUIElement.InvokeOnMainThread(() =>
                {
                    child.NativeUIElement.RemoveFromSuperview();
                    this.NativeUIElement.AddSubview(child.NativeUIElement);
                });
            }
        }

        protected void RemoveNativeChildView(UIElement child)
        {
            this.NativeUIElement.InvokeOnMainThread(() =>
            {
                child.NativeUIElement.RemoveFromSuperview();
            });
        }

        protected virtual void ArrangeChilds(CGSize size)
        {
        }

        protected void SetMeasuredSize(CGSize measuredSize)
        {
        }

        private void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null)
            {
                this.NativeUIElement.BackgroundColor = this.Background != null ? this.Background.ToUIColor(this.RenderSize) : UIColor.Clear;
            }
        }
    }
}