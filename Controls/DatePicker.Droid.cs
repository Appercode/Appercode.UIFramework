using Appercode.UI.Controls.NativeControl.Wrappers;
using System;
using ADatePicker = Android.Widget.DatePicker;

namespace Appercode.UI.Controls
{
    public partial class DatePicker
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                this.NativeUIElement = new WrappedViewGroup(this);
            }

            base.NativeInit();
        }

        partial void ApplyDate(DateTimeOffset value)
        {
            this.pickerView?.NativePicker.UpdateDate(value.Year, value.Month - 1, value.Day);
        }

        partial void ApplyMaxYear(DateTimeOffset value)
        {
            var nativePicker = this.pickerView?.NativePicker;
            if (nativePicker != null)
            {
                nativePicker.MaxDate = value.ToUnixTimeMilliseconds();
            }
        }

        partial void ApplyMinYear(DateTimeOffset value)
        {
            var nativePicker = this.pickerView?.NativePicker;
            if (nativePicker != null)
            {
                nativePicker.MinDate = value.ToUnixTimeMilliseconds();
            }
        }

        private class NativePickerView : ADatePicker, ADatePicker.IOnDateChangedListener
        {
            private readonly DatePicker parent;

            public NativePickerView(DatePicker parent)
                : base(parent.Context)
            {
                this.parent = parent;
                this.SpinnersShown = true;
                this.CalendarViewShown = false;
                this.MinDate = parent.MinYear.ToUnixTimeMilliseconds();
                this.MaxDate = parent.MaxYear.ToUnixTimeMilliseconds();
                var date = parent.Date;
                this.Init(date.Year, date.Month - 1, date.Day, this);
            }

            public void OnDateChanged(ADatePicker view, int year, int monthOfYear, int dayOfMonth)
            {
                this.parent.Date = new DateTimeOffset(year, monthOfYear + 1, dayOfMonth, 0, 0, 0, TimeSpan.Zero);
            }
        }

        private partial class PickerView
        {
            internal NativePickerView NativePicker
            {
                get { return this.NativeUIElement as NativePickerView; }
            }

            protected internal override void NativeInit()
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new NativePickerView(this.parent);
                }

                base.NativeInit();
            }
        }
    }
}
