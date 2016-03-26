using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Globalization;

namespace Appercode.UI.Controls
{
    public partial class DatePicker
    {
        private PickerView pickerView;

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new UIView();
            }

            base.NativeInit();
        }

        partial void ApplyDate(DateTimeOffset value)
        {
            this.pickerView?.NativePicker?.SetDate((NSDate)value.LocalDateTime.Date, false);
        }

        partial void AddPickerView()
        {
            if (this.pickerView == null)
            {
                this.pickerView = new PickerView(this);
                this.pickerPresenter.Content = pickerView;
            }
        }

        private class PickerView : UIElement
        {
            private const float DefaultHeight = 162f;
            private readonly DatePicker parent;

            internal PickerView(DatePicker parent)
            {
                this.parent = parent;
            }

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
                        Mode = UIDatePickerMode.Date
                    };
                    nativePicker.SetDate((NSDate)this.parent.Date.LocalDateTime.Date, false);
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
