using System.Windows;
using UIKit;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ToggleButton
    {
        private bool? nativeIsChecked = false;

        static ToggleButton()
        {
            if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7)
            {
                Button.PaddingProperty.AddOwner(typeof(ToggleButton), new PropertyMetadata(new Thickness(6, 3, 6, 3)));
            }
        }

        protected bool? NativeIsChecked
        {
            get
            {
                return this.nativeIsChecked;
            }
            set
            {
                this.nativeIsChecked = value;

                var b = this.NativeUIElement as UIButton;
                if (b != null)
                {
                    b.Selected = value.HasValue ? value.Value : false;
                    if (b.ButtonType == UIButtonType.RoundedRect)
                    {
                        b.BeginInvokeOnMainThread(() => b.Highlighted = b.Selected);
                    }
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    var b = (UIButton)this.NativeUIElement ?? new UIButton(UIButtonType.RoundedRect);
                    this.NativeUIElement = b;
                    b.TouchUpOutside += (s, e) =>
                        {
                            if (b.ButtonType == UIButtonType.RoundedRect)
                            {
                                b.BeginInvokeOnMainThread(() => b.Highlighted = b.Selected);
                            }
                        };
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
                this.NativeIsChecked = this.IsChecked;
            }
        }

        protected override void SetTextContent(string text)
        {
            ((UIButton)this.NativeUIElement).SetTitle(text, UIControlState.Normal);
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null && this.Background != null)
            {
                if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7 && this.NativeUIElement.Subviews.Length > 1)
                {
                    this.NativeUIElement.Subviews[1].BackgroundColor = this.Background.ToUIColor(this.RenderSize);
                }
                else
                {
                    this.NativeUIElement.BackgroundColor = this.Background.ToUIColor(this.RenderSize);
                }
            }
        }
    }
}