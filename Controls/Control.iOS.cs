using System.Windows;
using System.Windows.Media;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class Control
    {
        private bool nativeIsEnabled;
        private Brush nativeForeground;
        private FontFamily nativeFontFamily;
        private FontStyle nativeFontStyle;
        private FontWeight nativeFontWeight;
        private double nativeFontSize;

        private bool NativeIsEnabled
        {
            get
            {
                return this.nativeIsEnabled;
            }
            set
            {
                this.nativeIsEnabled = value;
                if (this.NativeUIElement != null && this.controlTemplateInstance == null)
                {
                    (this.NativeUIElement as UIControl).Enabled = value;
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
                this.UpdateForeground();
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
                this.UpdateFontFamily();
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
                this.nativeFontSize = value;
                this.UpdateFontSize();
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
                if (this.NativeUIElement != null && this.controlTemplateInstance == null)
                {
                    if (this.FontFamily != null)
                    {
                        return;
                    }
                    this.UpdateFontWeightAndStyle();
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
                if (this.NativeUIElement != null && this.controlTemplateInstance == null)
                {
                    if (this.FontFamily != null)
                    {
                        return;
                    }
                    this.UpdateFontWeightAndStyle();
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement != null)
            {
                this.NativeOnbackgroundChange();
                this.NativeForeground = this.Foreground;
                this.NativeFontSize = this.FontSize;
                this.NativeFontWeight = this.FontWeight;
                this.NativeFontFamily = this.FontFamily;
                if (this.controlTemplateInstance == null && this.ContainsValue(TemplateProperty))
                {
                    this.OnTemplateChanged(null, this.Template);
                }
            }

            base.NativeInit();
        }

        protected virtual void UpdateFontWeightAndStyle()
        {
        }

        protected virtual void UpdateFontSize()
        {
        }

        protected virtual void UpdateFontFamily()
        {
        }

        protected virtual void UpdateForeground()
        {
        }

        protected virtual bool InternalFocus()
        {
            if (this.NativeUIElement != null)
            {
                return this.NativeUIElement.BecomeFirstResponder();
            }
            return false;
        }

        protected virtual void RemoveControlTemplateInstance()
        {
        }

        protected virtual void AddControlTemplateInstance()
        {
            if (this.NativeUIElement != null)
            {
                this.NativeUIElement.RemoveFromSuperview();
            }

            this.NativeUIElement = new UIButton(UIButtonType.Custom);
            this.Parent?.NativeUIElement?.AddSubview(this.NativeUIElement);
            this.NativeUIElement.AddSubview(this.controlTemplateInstance.NativeUIElement);
        }

        protected virtual void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null)
            {
                if (this.Background != null)
                {
                    this.NativeUIElement.BackgroundColor = this.Background.ToUIColor(this.NativeUIElement.Frame.Size);
                }
                else
                {
                    this.NativeUIElement.BackgroundColor = UIColor.Clear;
                }
            }
        }

        protected virtual void OnIsEnabledChanged()
        {
            if (this.NativeUIElement != null && this.controlTemplateInstance == null)
            {
                ((UIControl)this.NativeUIElement).Enabled = this.IsEnabled;
            }
        }

        protected void ApplyNativePadding(Thickness value)
        {
        }

        private static double GetDefaultFontSize()
        {
            return UIFont.SystemFontSize;
        }
    }
}
