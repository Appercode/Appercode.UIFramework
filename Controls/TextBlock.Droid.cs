using Android.Text;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrappers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class TextBlock
    {
        private FontFamily nativeFontFamily;

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
        protected FontFamily NativeFontFamily
        {
            get
            {
                return this.nativeFontFamily;
            }
            set
            {
                this.ApplyNativeFontFamily(value);
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
                this.ApplyNativeFontFamily(this.FontFamily);
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
            ((Android.Widget.TextView)this.NativeUIElement).Text = text != null ? (string)text : string.Empty;
        }
        private void ApplyNativeFontSize(double fontSize)
        {
            ((Android.Widget.TextView)this.NativeUIElement).TextSize = (float)fontSize;
        }
        private void ApplyNativeTextWrapping(TextWrapping textWrapping)
        {
            if (textWrapping == TextWrapping.Wrap)
            {
                ((Android.Widget.TextView)this.NativeUIElement).SetSingleLine(false);
            }
            else
            {
                ((Android.Widget.TextView)this.NativeUIElement).SetSingleLine(true);
            }
        }
        private void ApplyNativeTextTrimming(TextTrimming textTrimming)
        {
            if (textTrimming == TextTrimming.WordEllipsis)
            {
                ((Android.Widget.TextView)this.NativeUIElement).Ellipsize = TextUtils.TruncateAt.End;
            }
            else
            {
                ((Android.Widget.TextView)this.NativeUIElement).Ellipsize = null;
            }
        }
        private void ApplyNativeFontWeight(FontWeight fontWeight)
        {
            switch (fontWeight.ToString())
            {
                case "Black":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                        }
                        else
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                        }
                        break;
                    }
                case "Bold":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                        }
                        else
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                        }
                        break;
                    }
                case "ExtraBlack":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                        }
                        else
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                        }
                        break;
                    }
                case "ExtraBold":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                        }
                        else
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                        }
                        break;
                    }
                case "ExtraLight":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Italic);
                        }
                        else
                        {
                            var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                        }
                        break;
                    }
                case "Light":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Italic);
                        }
                        else
                        {
                            var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                        }
                        break;
                    }
                case "SemiBold":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                        }
                        else
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                        }
                        break;
                    }
                case "Thin":
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            var tf = Android.Graphics.Typeface.Create("sans-serif" + "-thin", Android.Graphics.TypefaceStyle.Normal);
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Italic);
                        }
                        else
                        {
                            var tf = Android.Graphics.Typeface.Create("sans-serif" + "-thin", Android.Graphics.TypefaceStyle.Normal);
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                        }
                        break;
                    }
                default:
                    {
                        if (this.NativeFontStyle != null && this.NativeFontStyle.ToString() == "Italic")
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Italic);
                        }
                        else
                        {
                            ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Normal);
                        }
                        break;
                    }
            }
        }
        private void ApplyNativeFontStyle(FontStyle fontStyle)
        {
            if (fontStyle.ToString() == "Italic")
            {
                if (this.NativeFontWeight != null)
                {
                    switch (this.NativeFontWeight.ToString())
                    {
                        case "Black":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                                break;
                            }
                        case "Bold":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                                break;
                            }
                        case "ExtraBlack":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                                break;
                            }
                        case "ExtraBold":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                                break;
                            }
                        case "ExtraLight":
                            {
                                var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Italic);
                                break;
                            }
                        case "Light":
                            {
                                var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Italic);
                                break;
                            }
                        case "SemiBold":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.BoldItalic);
                                break;
                            }
                        case "Thin":
                            {
                                var tf = Android.Graphics.Typeface.Create("sans-serif" + "-thin", Android.Graphics.TypefaceStyle.Normal);
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Italic);
                                break;
                            }
                        default:
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Italic);
                                break;
                            }
                    }
                }
                else
                {
                    ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Italic);
                }
            }
            else
            {
                if (this.NativeFontWeight != null)
                {
                    switch (this.NativeFontWeight.ToString())
                    {
                        case "Black":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                                break;
                            }
                        case "Bold":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                                break;
                            }
                        case "ExtraBlack":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                                break;
                            }
                        case "ExtraBold":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                                break;
                            }
                        case "ExtraLight":
                            {
                                var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                                break;
                            }
                        case "Light":
                            {
                                var tf = Android.Graphics.Typeface.Create("sans-serif" + "-light", Android.Graphics.TypefaceStyle.Normal);
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                                break;
                            }
                        case "SemiBold":
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Bold);
                                break;
                            }
                        case "Thin":
                            {
                                var tf = Android.Graphics.Typeface.Create("sans-serif" + "-thin", Android.Graphics.TypefaceStyle.Normal);
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(tf, Android.Graphics.TypefaceStyle.Normal);
                                break;
                            }
                        default:
                            {
                                ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Normal);
                                break;
                            }
                    }
                }
                else
                {
                    ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Normal);
                }
            }
        }
        private void ApplyNativeTextAlignment(TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case Controls.TextAlignment.Center:
                    {
                        ((Android.Widget.TextView)this.NativeUIElement).Gravity = GravityFlags.Center;
                        break;
                    }
                case Controls.TextAlignment.Left:
                    {
                        ((Android.Widget.TextView)this.NativeUIElement).Gravity = GravityFlags.Left;
                        break;
                    }
                case Controls.TextAlignment.Right:
                    {
                        ((Android.Widget.TextView)this.NativeUIElement).Gravity = GravityFlags.Right;
                        break;
                    }
                default:
                    {
                        ((Android.Widget.TextView)this.NativeUIElement).Gravity = GravityFlags.Left;
                        break;
                    }
            }
        }
        private void ApplyNativeFontFamily(FontFamily fontFamily)
        {
            if (fontFamily != null)
            {
                int indBegin = fontFamily.Source.IndexOf("Assets");
                int indEnd = fontFamily.Source.IndexOf('#');
                if (indEnd < 0) indEnd = fontFamily.Source.Length-1;
                string font = "";

                if (indBegin != -1)
                {
                    font = fontFamily.Source.Substring(indBegin + 7, indEnd - 1 - indBegin - 6);
                    this.nativeFontFamily = new FontFamily(font);

                }

                
                if (this.NativeUIElement != null)
                {
                    try
                    {
                        ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.CreateFromAsset(this.Context.Assets, this.nativeFontFamily.Source), Android.Graphics.TypefaceStyle.Normal);
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }
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
                    Color color = ((SolidColorBrush)foreground).Color;
                    ((Android.Widget.TextView)this.NativeUIElement).SetTextColor(new Android.Graphics.Color(color.R, color.G, color.B, color.A));
                }
            }
        }
        private void ApplyNativePadding(Thickness padding)
        {
            ((Android.Widget.TextView)this.NativeUIElement).SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
        }
    }
}