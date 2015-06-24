using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class HyperlinkButton
    {
        protected Uri NativeNavigateUri { get; set; }

        protected internal override void NativeInit()
        {
            if (Parent == null)
            {
                return;
            }
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new UIButton(UIButtonType.Custom);
            }
            base.NativeInit();
        }

        protected override void SetTextContent(string text)
        {
            ((UIButton)this.NativeUIElement).SetAttributedTitle(new NSAttributedString(
                text,
                underlineStyle: NSUnderlineStyle.Single, 
                foregroundColor: this.Foreground.ToUIColor(this.NativeUIElement.Frame.Size)),
                UIControlState.Normal);
        }

        protected override CGSize NativeMeasureContent(CGSize availableSize)
        {
            var size = ((UIButton)this.NativeUIElement).TitleLabel.SizeThatFits(availableSize);
            return size;
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            return base.NativeMeasureOverride(availableSize);
        }

        protected override void OnClick()
        {
            if (this.NativeNavigateUri != null)
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(this.NativeNavigateUri.AbsoluteUri));
            }
            base.OnClick();
        }

        protected override void UpdateFontSize()
        {
            if (this.NativeUIElement != null)
            {
                ((UIButton)this.NativeUIElement).Font = ((UIButton)this.NativeUIElement).Font.WithSize((nfloat)this.FontSize);
            }
        }

        protected override void UpdateForeground()
        {
            if (this.NativeUIElement != null)
            {
                if(this.Content is String)
                {
                    this.SetTextContent((string)this.Content);
                }
            }
        }

        protected override void UpdateFontFamily()
        {
            var fontFamily = this.FontFamily;
            if (this.NativeUIElement != null)
            {
                if (fontFamily == null || string.IsNullOrWhiteSpace(fontFamily.Source))
                {
                    this.UpdateFontWeightAndStyle();
                    return;
                }
                int ind = fontFamily.Source.IndexOf('#');
                UIFont f = null;
                if (ind == -1)
                {
                    f = UIFont.FromName(fontFamily.Source, (nfloat)this.FontSize);
                }
                else
                {
                    f = UIFont.FromName(fontFamily.Source.Substring(ind + 1), (nfloat)this.FontSize);
                }
                var fonts = UIFont.FamilyNames;
                if (f == null)
                {
                    this.UpdateFontWeightAndStyle();
                    return;
                }
                (this.NativeUIElement as UIButton).Font = f;
            }
        }

        protected override void UpdateFontWeightAndStyle()
        {
            if (this.FontWeight == FontWeights.Black
                || this.FontWeight == FontWeights.Bold
                || this.FontWeight == FontWeights.ExtraBlack
                || this.FontWeight == FontWeights.ExtraBold
                || this.FontWeight == FontWeights.SemiBold
                || this.FontWeight == FontWeights.Medium)
            {
                if (this.FontStyle == FontStyles.Normal)
                {
                    (this.NativeUIElement as UIButton).Font = UIFont.BoldSystemFontOfSize((nfloat)this.FontSize);
                }
                else if (this.FontStyle == FontStyles.Italic)
                {
                    var systemFontName = int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) >= 7 ? "HelveticaNeue" : UIFont.SystemFontOfSize((nfloat)this.FontSize).Name;

                    (this.NativeUIElement as UIButton).Font = UIFont.FromName(string.Format("{0}-BoldItalic", systemFontName), (nfloat)this.FontSize);
                }
                return;
            }

            if (this.FontWeight == FontWeights.Light
                || this.FontWeight == FontWeights.ExtraLight
                || this.FontWeight == FontWeights.Thin)
            {
                (this.NativeUIElement as UIButton).Font = UIFont.FromName(string.Format("{0}-Light", "HelveticaNeue"), (nfloat)this.FontSize);
            }

            // it may be FontWeights.Normal                
            if (this.FontStyle == FontStyles.Normal)
            {
                (this.NativeUIElement as UIButton).Font = UIFont.SystemFontOfSize((nfloat)this.FontSize);
            }
            else if (this.FontStyle == FontStyles.Italic)
            {
                (this.NativeUIElement as UIButton).Font = UIFont.ItalicSystemFontOfSize((nfloat)this.FontSize);
            }
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null && this.Background != null)
            {
                this.NativeUIElement.BackgroundColor = this.Background.ToUIColor(this.RenderSize);
            }
        }
    }
}