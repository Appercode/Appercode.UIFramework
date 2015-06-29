using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class Control
    {
        private bool nativeIsEnabled = true;
        private double nativeFontSize;
        private FontWeight nativeFontWeight;
        private FontStyle nativeFontStyle;
        private FontFamily nativeFontFamily;
        private Brush nativeForeground;

        private Thickness systemControlsPadding;
        private bool isSetSystemControlsPadding = false;

        protected bool NativeIsEnabled
        {
            get
            {
                return this.nativeIsEnabled;
            }
            set
            {
                this.nativeIsEnabled = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeIsEnabled(value);
                }
            }
        }
        protected double NativeFontSize
        {
            get
            {
                return this.nativeFontSize;
            }
            set
            {
                this.nativeFontSize = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeFontSize(value);
                }
            }
        }
        protected FontWeight NativeFontWeight
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
                    this.ApplyNativeFontWeight(value);
                }
            }
        }
        protected FontStyle NativeFontStyle
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
                    this.ApplyNativeFontStyle(value);
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
                this.nativeFontFamily = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeFontFamily(value);
                }
            }
        }
        protected Brush NativeForeground
        {
            get
            {
                return this.nativeForeground;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeForeground(value);
                }

                this.nativeForeground = value;
            }
        }

        protected internal override void NativeInit()
        {
            if (this.controlTemplateInstance == null && this.Template != null)
            {
                this.OnTemplateChanged(null, this.Template);
            }

            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement is TextView)
                {
                    var textBlock = (Android.Widget.TextView)this.NativeUIElement;
                    this.ApplyNativeIsEnabled(this.NativeIsEnabled);

                    if (this.Foreground != null)
                    {
                        this.ApplyNativeForeground(this.Foreground);
                    }

                    this.ApplyNativeFontSize(this.FontSize);
                    this.ApplyNativePadding(this.Padding);

                    if (this.FontWeight != null)
                    {
                        this.ApplyNativeFontWeight(this.FontWeight);
                    }

                    if (this.FontStyle != null)
                    {
                        this.ApplyNativeFontStyle(this.FontStyle);
                    }

                    if (this.FontFamily != null)
                    {
                        this.ApplyNativeFontFamily(this.FontFamily);
                    }

                    textBlock.LayoutParameters = this.CreateWrapContentLayoutParams();

                    this.NativeUIElement = textBlock;
                }

                base.NativeInit();

                //TODO:
                //<Need-approve>
                this.NativeOnbackgroundChange();
                //</Need-approve>
            }
        }

        private static double GetDefaultFontSize()
        {
            var t = Android.App.Application.Context.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.TextSize });
            return t.GetDimensionPixelOffset(0, -1);
        }

        protected ViewGroup.LayoutParams CreateWrapContentLayoutParams()
        {
            var layoutParams = new ViewGroup.LayoutParams(0, 0);
            layoutParams.Width = double.IsNaN(this.NativeWidth) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeWidth;
            layoutParams.Height = double.IsNaN(this.NativeHeight) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeHeight;
            return layoutParams;
        }

        protected virtual bool InternalFocus()
        {
            return true;
        }

        protected virtual void OnIsEnabledChanged()
        {
        }

        protected virtual void NativeOnbackgroundChange()
        {
            //TODO:
            //<Need-approve>
            if (this.NativeUIElement != null)
            {
                if (this.Background == null)
                {
                    this.NativeUIElement.SetBackgroundDrawable(null);
                }
                else
                {
                    if (this.Background is SolidColorBrush)
                    {
                        this.NativeUIElement.SetBackgroundColor(((SolidColorBrush)this.Background).Color.ToNativeColor());
                    }
                    else
                    {
                        this.NativeUIElement.SetBackgroundDrawable(this.Background.ToDrawable());
                    }
                }
            }
            //</Need-approve>
        }

        protected void ApplyNativePadding(Thickness value)
        {
            if (this.NativeUIElement != null)
            {
                var textView = this.NativeUIElement as TextView;
                if (this.isSetSystemControlsPadding == false && textView != null)
                {
                    this.systemControlsPadding = new Thickness(textView.CompoundPaddingLeft, textView.CompoundPaddingTop, textView.CompoundPaddingRight, textView.CompoundPaddingBottom);
                    this.isSetSystemControlsPadding = true;
                }

                this.NativeUIElement.SetPadding((int)value.Left + (int)this.systemControlsPadding.Left, (int)value.Top + (int)this.systemControlsPadding.Top, (int)value.Right + (int)this.systemControlsPadding.Right, (int)value.Bottom + (int)this.systemControlsPadding.Bottom);
            }
        }

        private void ApplyNativeFontWeight(FontWeight value)
        {
            switch (value.ToString())
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

        private void ApplyNativeFontStyle(FontStyle value)
        {
            if (!(this.NativeUIElement is Android.Widget.TextView))
            {
                return;
            }

            if (value.ToString() == "Italic")
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

        private void ApplyNativeIsEnabled(bool value)
        {
            this.NativeUIElement.Enabled = value;
        }

        private void ApplyNativeFontSize(double value)
        {
            if (this.NativeUIElement is Android.Widget.TextView)
            {
                ((Android.Widget.TextView)this.NativeUIElement).TextSize = value != (double)0.0 ? (float)value : (float)16;
            }
        }

        private void ApplyNativeFontFamily(FontFamily value)
        {
            if (value != null)
            {
                int indBegin = value.Source.IndexOf("Assets");
                int indEnd = value.Source.IndexOf('#');
                if (indEnd < 0) indEnd = value.Source.Length - 1;
                string font = "";

                if (indBegin != -1)
                {
                    font = value.Source.Substring(indBegin + 7, indEnd - 1 - indBegin - 6);
                    this.nativeFontFamily = new FontFamily(font);
                }

                if (this.NativeUIElement != null)
                {
                    try
                    {
                        ((Android.Widget.TextView)this.NativeUIElement).SetTypeface(Android.Graphics.Typeface.CreateFromAsset(this.Context.Assets, this.nativeFontFamily.Source), Android.Graphics.TypefaceStyle.Normal);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void ApplyNativeForeground(Brush value)
        {
            if (value == null)
            {
                return;
            }

            if (!(value is SolidColorBrush))
            {
                throw new ArgumentOutOfRangeException("value", "Only SolidColorBrush is supported");
            }

            Color color = ((SolidColorBrush)value).Color;
            ((Android.Widget.TextView)this.NativeUIElement).SetTextColor(new Android.Graphics.Color(color.R, color.G, color.B, color.A));
        }

        private void RemoveControlTemplateInstance()
        {
        }

        private void AddControlTemplateInstance()
        {
            var oldView = this.NativeUIElement;
            this.NativeUIElement = new WrapedViewGroup(this.Context);

            if (this.Parent != null && this.Parent.NativeUIElement != null && oldView.Parent != null)
            {
                ((ViewGroup)this.Parent.NativeUIElement).RemoveView(oldView);
                ((ViewGroup)this.Parent.NativeUIElement).AddView(this.NativeUIElement);
            }
            ((WrapedViewGroup)this.NativeUIElement).AddView(this.controlTemplateInstance.NativeUIElement);
        }
    }
}