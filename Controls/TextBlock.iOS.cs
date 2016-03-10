using CoreGraphics;
using Foundation;
using System;
using System.Windows;
using System.Windows.Media;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class TextBlock
    {
        private string nativeText;

        private nfloat nativeFontSize = UIFont.SystemFontSize;

        private TextWrapping nativeTextWrapping;

        private TextTrimming nativeTextTrimming;

        private FontWeight nativeFontWeight = FontWeights.Normal;

        private FontStyle nativeFontStyle = FontStyles.Normal;

        private TextAlignment nativeTextAlignment;

        private FontFamily nativeFontFamily;

        private Brush nativeForeground;

        private string NativeText
        {
            get
            {
                return this.nativeText;
            }
            set
            {
                this.nativeText = value;
                if (this.NativeUIElement != null)
                {
                    (this.NativeUIElement as UILabel).Text = value;
                }
            }
        }

        private double NativeFontSize
        {
            get
            {
                return this.nativeFontSize;
            }
            set
            {
                this.nativeFontSize = (nfloat)value;
                if (this.NativeUIElement != null)
                {
                    (this.NativeUIElement as UILabel).Font = UIFont.FromName((this.NativeUIElement as UILabel).Font.Name, this.nativeFontSize);
                }
            }
        }

        private TextWrapping NativeTextWrapping
        {
            get
            {
                return this.nativeTextWrapping;
            }
            set
            {
                this.nativeTextWrapping = value;
                if (this.NativeUIElement != null)
                {
                    (this.NativeUIElement as UILabel).Lines = value == Controls.TextWrapping.Wrap ? 0 : 1;
                    this.UpdateLineBreakMode();
                }
            }
        }

        private TextTrimming NativeTextTrimming
        {
            get
            {
                return this.nativeTextTrimming;
            }
            set
            {
                this.nativeTextTrimming = value;
                if (this.NativeUIElement != null)
                {
                    this.UpdateLineBreakMode();
                }
            }
        }

        private FontWeight NativeFontWeight
        {
            get
            {
                return this.nativeFontWeight;
            }
            set
            {
                this.nativeFontWeight = value;
                if (this.NativeUIElement != null)
                {
                    if (this.FontFamily != null)
                    {
                        return;
                    }
                    this.UpdateFontWeightAndStyle();
                }
            }
        }

        private FontStyle NativeFontStyle
        {
            get
            {
                return this.nativeFontStyle;
            }
            set
            {
                this.nativeFontStyle = value;
                if (this.NativeUIElement != null)
                {
                    if (this.FontFamily != null)
                    {
                        return;
                    }
                    this.UpdateFontWeightAndStyle();
                }
            }
        }

        private Brush NativeForeground
        {
            get
            {
                return this.nativeForeground;
            }
            set
            {
                this.nativeForeground = value;
                if (this.NativeUIElement != null && value != null)
                {
                    (this.NativeUIElement as UILabel).TextColor = value.ToUIColor(this.NativeUIElement.Frame.Size);
                }
            }
        }

        private TextAlignment NativeTextAlignment
        {
            get
            {
                return this.nativeTextAlignment;
            }
            set
            {
                this.nativeTextAlignment = value;
                if (this.NativeUIElement != null)
                {
                    UITextAlignment nativeAlignment = UITextAlignment.Left;

                    switch (value)
                    {
                        case TextAlignment.Left:
                            nativeAlignment = UITextAlignment.Left;
                            break;
                        case TextAlignment.Center:
                            nativeAlignment = UITextAlignment.Center;
                            break;
                        case TextAlignment.Right:
                            nativeAlignment = UITextAlignment.Right;
                            break;
                        default:
                            nativeAlignment = UITextAlignment.Left;
                            break;
                    }

                    (this.NativeUIElement as UILabel).TextAlignment = nativeAlignment;
                }
            }
        }

        private FontFamily NativeFontFamily
        {
            get
            {
                return this.nativeFontFamily;
            }
            set
            {
                this.nativeFontFamily = value;
                if (this.NativeUIElement != null)
                {
                    if (value == null || string.IsNullOrWhiteSpace(value.Source))
                    {
                        this.UpdateFontWeightAndStyle();
                        return;
                    }
                    int ind = value.Source.IndexOf('#');
                    UIFont f = null;
                    if (ind == -1)
                    {
                        f = UIFont.FromName(value.Source, (nfloat)this.FontSize);
                    }
                    else
                    {
                        f = UIFont.FromName(value.Source.Substring(ind + 1), (nfloat)this.FontSize);
                    }
                    if (f == null)
                    {
                        this.UpdateFontWeightAndStyle();
                        return;
                    }
                    (this.NativeUIElement as UILabel).Font = f;
                }
            }
        }

        private Thickness NativePadding
        {
            set
            {
                if (this.NativeUIElement != null)
                {
                    ((VerticalAlignedLabel)this.NativeUIElement).Padding = value;
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new VerticalAlignedLabel();
                    this.NativeUIElement.BackgroundColor = UIColor.Clear;
                }
                this.NativeText = (string)this.GetValue(TextProperty);
                this.NativeFontSize = (double)this.GetValue(FontSizeProperty);
                this.NativeTextAlignment = (TextAlignment)this.GetValue(TextAlignmentProperty);
                this.NativeTextWrapping = (TextWrapping)this.GetValue(TextWrappingProperty);
                this.NativeFontFamily = (FontFamily)this.GetValue(FontFamilyProperty);
                this.NativeFontWeight = this.FontWeight;
                this.NativeFontStyle = this.FontStyle;
                this.NativeForeground = this.Foreground;
                this.InvalidateMeasure();
                base.NativeInit();
            }
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var size = base.NativeMeasureOverride(availableSize);

            var label = this.NativeUIElement as VerticalAlignedLabel;

            var width = (nfloat)this.Width;
            var height = (nfloat)this.Height;
            var margin = this.Margin;
            var padding = this.Padding;

            var fits = new CGSize(
                (nfloat.IsNaN(width) ? availableSize.Width - margin.HorizontalThicknessF() : MathF.Min(availableSize.Width, width)) - padding.HorizontalThicknessF(),
                (nfloat.IsNaN(height) ? availableSize.Height - margin.VerticalThicknessF() : MathF.Min(availableSize.Height, height)) - padding.VerticalThicknessF());
            if (nfloat.IsPositiveInfinity(fits.Height))
            {
                fits.Height = nfloat.MaxValue;
            }

            if (this.TextWrapping == TextWrapping.NoWrap)
            {
                fits.Width = nfloat.MaxValue;
                fits = label.GetTextSize(fits);
                fits.Width = MathF.Min(fits.Width, availableSize.Width);
            }
            else
            {
                fits = label.GetTextSize(fits);
            }

            if (nfloat.IsNaN(width))
            {
                size.Width += fits.Width + padding.HorizontalThicknessF();
            }

            if (nfloat.IsNaN(height))
            {
                size.Height += fits.Height + padding.VerticalThicknessF();
            }

            return size;
        }

        protected override void NativeArrange(CGRect finalRect)
        {
            var padding = this.Padding;
            finalRect = new CGRect(
                finalRect.X + padding.LeftF(),
                finalRect.Y + padding.TopF(),
                finalRect.Width - padding.HorizontalThicknessF(),
                finalRect.Height - padding.VerticalThicknessF());
            base.NativeArrange(finalRect);
        }

        private static double GetDefaultFontSize()
        {
            return UIFont.LabelFontSize;
        }

        private void UpdateLineBreakMode()
        {
            UILineBreakMode lb = UILineBreakMode.Clip;
            if (this.TextTrimming == Controls.TextTrimming.WordEllipsis)
            {
                lb = UILineBreakMode.TailTruncation;
            }
            else if (this.NativeTextWrapping == Controls.TextWrapping.Wrap)
            {
                lb = UILineBreakMode.WordWrap;
            }
            (this.NativeUIElement as UILabel).LineBreakMode = lb;
        }

        private void UpdateFontWeightAndStyle()
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
                    (this.NativeUIElement as UILabel).Font = UIFont.BoldSystemFontOfSize((nfloat)this.FontSize);
                }
                else if (this.FontStyle == FontStyles.Italic)
                {
                    var systemFontName = int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) >= 7 ? "HelveticaNeue" : UIFont.SystemFontOfSize((nfloat)this.FontSize).Name;

                    (this.NativeUIElement as UILabel).Font = UIFont.FromName(string.Format("{0}-BoldItalic", systemFontName), (nfloat)this.FontSize);
                }
                return;
            }

            if (this.FontWeight == FontWeights.Light
                || this.FontWeight == FontWeights.ExtraLight
                || this.FontWeight == FontWeights.Thin)
            {
                (this.NativeUIElement as UILabel).Font = UIFont.FromName(string.Format("{0}-Light", "HelveticaNeue"), (nfloat)this.FontSize);
            }

            // it may be FontWeights.Normal                
            if (this.FontStyle == FontStyles.Normal)
            {
                (this.NativeUIElement as UILabel).Font = UIFont.SystemFontOfSize((nfloat)this.FontSize);
            }
            else if (this.FontStyle == FontStyles.Italic)
            {
                (this.NativeUIElement as UILabel).Font = UIFont.ItalicSystemFontOfSize((nfloat)this.FontSize);
            }
        }

        private class VerticalAlignedLabel : UILabel
        {
            private Thickness padding;
            private NSString nativeText;
            private UIStringAttributes attributes;

            public VerticalAlignedLabel()
            {
            }

            public VerticalAlignedLabel(CGRect rF)
                : base(rF)
            {
            }

            public VerticalAlignedLabel(IntPtr h)
                : base(h)
            {
            }

            public override UIFont Font
            {
                get
                {
                    return base.Font;
                }
                set
                {
                    base.Font = value;
                    this.attributes = new UIStringAttributes { Font = value };
                }
            }

            public Thickness Padding
            {
                get
                {
                    return this.padding;
                }
                set
                {
                    this.padding = value;
                    this.SetNeedsLayout();
                    this.LayoutIfNeeded();
                }
            }

            public override string Text
            {
                get
                {
                    return base.Text;
                }
                set
                {
                    base.Text = value;
                    this.nativeText = new NSString(value ?? string.Empty);
                }
            }

            public CGSize GetTextSize(CGSize size)
            {
                NSStringDrawingOptions options;
                switch (this.LineBreakMode)
                {
                    case UILineBreakMode.CharacterWrap:
                    case UILineBreakMode.WordWrap:
                        options = NSStringDrawingOptions.UsesLineFragmentOrigin;
                        break;
                    case UILineBreakMode.TailTruncation:
                        options = NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.TruncatesLastVisibleLine;
                        break;
                    default:
                        options = default(NSStringDrawingOptions);
                        break;
                }

                size = this.nativeText.GetBoundingRect(size, options, this.attributes, null).Size;
                return new CGSize(size.Width + this.Padding.HorizontalThicknessF(), size.Height + this.Padding.VerticalThicknessF());
            }

            public override CGSize SizeThatFits(CGSize size)
            {
                return this.GetTextSize(size);
            }

            // normally it uses full size of the control - we change this 
            public override void DrawText(CGRect rect)
            {
                var insets = (UIEdgeInsets)this.Padding;
                base.DrawText(insets.InsetRect(rect));
            }
        }
    }
}