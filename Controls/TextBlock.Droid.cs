using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Controls.NativeControl.Wrappers;
using System;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class TextBlock
    {
        private static readonly Lazy<FontManager> FontLoader = new Lazy<FontManager>();

        protected string NativeText
        {
            get
            {
                return this.Text;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeText(value);
                }
            }
        }
        protected double NativeFontSize
        {
            get
            {
                return this.FontSize;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeFontSize(value);
                }
            }
        }
        protected TextWrapping NativeTextWrapping
        {
            get
            {
                return this.TextWrapping;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeTextWrapping(value);
                }
            }
        }
        protected TextTrimming NativeTextTrimming
        {
            get
            {
                return this.TextTrimming;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeTextTrimming(value);
                }
            }
        }
        protected FontWeight NativeFontWeight
        {
            get
            {
                return this.FontWeight;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeFontWeight(value);
                }
            }
        }
        protected FontStyle NativeFontStyle
        {
            get
            {
                return this.FontStyle;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeFontStyle(value);
                }
            }
        }
        protected TextAlignment NativeTextAlignment
        {
            get
            {
                return this.TextAlignment;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeTextAlignment(value);
                }
            }
        }

        protected Brush NativeForeground
        {
            get
            {
                return this.Foreground;
            }
            set
            {
                this.ApplyNativeForeground(value);
            }
        }
        protected Thickness NativePadding
        {
            get
            {
                return this.Padding;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativePadding(value);
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new WrappedTextView(this);
                }

                this.ApplyNativeText(this.Text);
                this.ApplyNativeFontSize(this.FontSize);
                this.ApplyNativeTextWrapping(this.TextWrapping);
                this.ApplyNativeTextTrimming(this.TextTrimming);
                this.ApplyNativeFontWeight(this.FontWeight);
                this.ApplyNativeFontStyle(this.FontStyle);
                this.ApplyNativeTextAlignment(this.TextAlignment);
                this.ApplyFontFamily(this.FontFamily);
                this.ApplyNativeForeground(this.Foreground);
                this.ApplyNativePadding(this.Padding);

                base.NativeInit();
            }
        }

        private static double GetDefaultFontSize()
        {
            var t = Android.App.Application.Context.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.TextSize });
            return t.GetDimensionPixelOffset(0, -1);
        }

        private void ApplyNativeText(string text)
        {
            ((TextView)this.NativeUIElement).Text = text ?? string.Empty;
        }

        private void ApplyNativeFontSize(double fontSize)
        {
            ((TextView)this.NativeUIElement).TextSize = (float)fontSize;
        }

        private void ApplyNativeTextWrapping(TextWrapping textWrapping)
        {
            ((TextView)this.NativeUIElement).SetSingleLine(textWrapping != TextWrapping.Wrap);
        }

        private void ApplyNativeTextTrimming(TextTrimming textTrimming)
        {
            ((TextView)this.NativeUIElement).Ellipsize =
                textTrimming == TextTrimming.WordEllipsis ? TextUtils.TruncateAt.End : null;
        }

        private void ApplyNativeFontWeight(FontWeight fontWeight)
        {
            this.ApplyFont(this.NativeFontStyle, fontWeight);
        }

        private void ApplyNativeFontStyle(FontStyle fontStyle)
        {
            this.ApplyFont(fontStyle, this.NativeFontWeight);
        }

        private void ApplyFont(FontStyle fontStyle, FontWeight fontWeight)
        {
            var isItalic = fontStyle.ToString() == "Italic";
            var typeface = Typeface.Default;
            var typefaceStyle = isItalic ? TypefaceStyle.Italic : TypefaceStyle.Normal;
            switch (fontWeight.ToString())
            {
                case "Black":
                case "Bold":
                case "ExtraBlack":
                case "ExtraBold":
                case "SemiBold":
                    typefaceStyle = isItalic ? TypefaceStyle.BoldItalic : TypefaceStyle.Bold;
                    break;
                case "ExtraLight":
                case "Light":
                    typeface = Typeface.Create("sans-serif-light", TypefaceStyle.Normal);
                    break;
                case "Thin":
                    typeface = Typeface.Create("sans-serif-thin", TypefaceStyle.Normal);
                    break;
            }

            ((TextView)this.NativeUIElement).SetTypeface(typeface, typefaceStyle);
        }

        private void ApplyNativeTextAlignment(TextAlignment textAlignment)
        {
            GravityFlags gravity;
            switch (textAlignment)
            {
                case TextAlignment.Center:
                    gravity = GravityFlags.Center;
                    break;
                case TextAlignment.Right:
                    gravity = GravityFlags.Right;
                    break;
                default:
                    gravity = GravityFlags.Left;
                    break;
            }

            ((TextView)this.NativeUIElement).Gravity = gravity;
        }

        private void ApplyNativeForeground(Brush foreground)
        {
            if (foreground != null)
            {
                if (!(foreground is SolidColorBrush))
                {
                    throw new ArgumentOutOfRangeException("value", "Only SolidColorBrush is supported");
                }

                if (this.NativeUIElement != null)
                {
                    var color = ((SolidColorBrush)foreground).Color;
                    ((TextView)this.NativeUIElement).SetTextColor(new Android.Graphics.Color(color.R, color.G, color.B, color.A));
                }
            }
        }

        private void ApplyNativePadding(Thickness padding)
        {
            ((TextView)this.NativeUIElement).SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
        }

        partial void ApplyFontFamily(FontFamily value)
        {
            if (value != null && this.NativeUIElement != null)
            {
                var font = FontLoader.Value.GetFont(this.Context.Assets, value);
                ((TextView)this.NativeUIElement).SetTypeface(font, TypefaceStyle.Normal);
            }
        }
    }
}