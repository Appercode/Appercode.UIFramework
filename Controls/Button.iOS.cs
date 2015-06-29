using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class Button
    {
        protected internal override void NativeInit()
        {
            if (Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UIButton(UIButtonType.RoundedRect);
                    if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7)
                    {
                        UIView background = new UIView();
                        background.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
                        background.UserInteractionEnabled = false;
                        this.NativeUIElement.InsertSubview(background, 1);
                        background.Layer.CornerRadius = 10.0f;
                        background.Layer.BorderWidth = 1f;
                        background.Layer.BorderColor = UIColor.Clear.CGColor;
                    }
                }

                base.NativeInit();
            }
        }

        protected override void SetTextContent(string text)
        {
            ((UIButton)this.NativeUIElement).SetTitle(text, UIControlState.Normal);
        }

        protected override Thickness GetNativePadding()
        {
            if (this.controlTemplateInstance != null && int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7)
            {
                return new Thickness(6, 3, 6, 3);
            }
            return new Thickness(0);
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.controlTemplateInstance == null && this.NativeUIElement != null && this.Background != null)
            {
                if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7)
                {
                    ((UIButton)this.NativeUIElement).Subviews[1].BackgroundColor = this.Background.ToUIColor(this.RenderSize);
                }
                else
                {
                    this.NativeUIElement.BackgroundColor = this.Background.ToUIColor(this.RenderSize);
                }
            }
        }
    }
}