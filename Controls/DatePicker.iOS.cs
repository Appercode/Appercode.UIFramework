using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Globalization;

namespace Appercode.UI.Controls
{
    public partial class DatePicker
    {
        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new UIView();
            }

            base.NativeInit();
        }

        private static NSDate ConvertDate(DateTimeOffset value)
        {
            return (NSDate)value.LocalDateTime.Date;
        }

        partial void ApplyDate(DateTimeOffset value)
        {
            this.pickerView?.NativePicker?.SetDate(ConvertDate(value), false);
        }

        partial void ApplyMaxYear(DateTimeOffset value)
        {
            var nativePicker = this.pickerView?.NativePicker;
            if (nativePicker != null)
            {
                nativePicker.MaximumDate = ConvertDate(value);
            }
        }

        partial void ApplyMinYear(DateTimeOffset value)
        {
            var nativePicker = this.pickerView?.NativePicker;
            if (nativePicker != null)
            {
                nativePicker.MinimumDate = ConvertDate(value);
            }
        }

        private partial class PickerView
        {
            private const float DefaultHeight = 162f;

            internal UIDatePicker NativePicker
            {
                get { return this.NativeUIElement as UIDatePicker; }
            }

            protected override CGSize NativeMeasureOverride(CGSize availableSize)
            {
                if (this.NativeUIElement == null)
                {
                    return base.NativeMeasureOverride(availableSize); 
                }

                var size = this.NativePicker.Bounds.Size;
                if (this.ContainsValue(HeightProperty))
                {
                    size.Height = (nfloat)this.Height;
                }
                else if (size.Height == 0)
                {
                    size.Height = DefaultHeight;
                }

                if (this.ContainsValue(WidthProperty))
                {
                    size.Width = (nfloat)this.Width;
                }

                var margin = this.Margin;
                size.Height += margin.VerticalThicknessF();
                size.Width += margin.HorizontalThicknessF();
                return size;
            }

            protected internal override void NativeInit()
            {
                if (this.NativeUIElement == null)
                {
                    var nativePicker = new UIDatePicker
                    {
                        Locale = NSLocale.FromLocaleIdentifier(CultureInfo.CurrentUICulture.Name.Replace('-', '_')),
                        MaximumDate = ConvertDate(parent.MaxYear),
                        MinimumDate = ConvertDate(parent.MinYear),
                        Mode = UIDatePickerMode.Date
                    };
                    nativePicker.SetDate(ConvertDate(parent.Date), false);
                    nativePicker.ValueChanged += OnDateChanged;
                    this.NativeUIElement = nativePicker;
                }

                base.NativeInit();
            }

            private void OnDateChanged(object sender, EventArgs e)
            {
                this.parent.Date = (DateTime)this.NativePicker.Date;
            }
        }
    }
}
