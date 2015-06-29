using System;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class RangePicker
    {
        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = pickerButton.NativeUIElement;
                pickerButton.Content = "picker";
                this.pickerView = new UIPickerView() { Model = new RangePickerViewModel(this) };
                this.PickerContainer = new NativePickerContainer(this.NativeUIElement, this.pickerView, this);
                this.PickerContainer.DoneButtonAction = () =>
                {
                    pickerButton.Content = this.Items[(int)this.pickerView.SelectedRowInComponent(0)];
                    this.LeftValue = this.Items[(int)this.pickerView.SelectedRowInComponent(0)];
                    this.RightValue = this.Items[(int)this.pickerView.SelectedRowInComponent(1)];
                    this.OnPickerCompleted();
                };
            }
            base.NativeInit();
        }

        protected override void NativeShow()
        {
            this.ApplyNativeLeftValue(this.LeftValue);
            this.ApplyNativeRightValue(this.RightValue);
            base.NativeShow();
        }

        protected void ApplyNativeLeftValue(object newValue)
        {
            var ind = this.Items.IndexOf(newValue);
            if(ind >= 0 && this.pickerView != null)
            {
                this.pickerView.Select(ind, 0, false);
            }
        }

        protected void ApplyNativeRightValue(object newValue)
        {
            var ind = this.Items.IndexOf(newValue);
            if(ind >= 0 && this.pickerView != null)
            {
                this.pickerView.Select(ind, 1, false);
            }
        }

        protected class RangePickerViewModel : ListPickerViewModel
        {
            public RangePickerViewModel(RangePicker owner) : base(owner)
            {
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if(component == 0 && row > picker.SelectedRowInComponent(1))
                {
                    picker.Select(row, 1, true);
                }
                if(component == 1 && row < picker.SelectedRowInComponent(0))
                {
                    picker.Select(row, 0, true);
                }
            }

            public override nint GetComponentCount(UIPickerView picker)
            {
                return 2;
            }
        }
    }
}

