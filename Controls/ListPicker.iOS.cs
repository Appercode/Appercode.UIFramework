using CoreGraphics;
using System;
using System.Collections;
using System.Collections.Generic;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ListPicker
    {
        protected UIPickerView pickerView;

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = pickerButton.NativeUIElement;
                pickerButton.Content = "picker";
                this.pickerView = new UIPickerView { Model = new ListPickerViewModel(this) };
                this.PickerContainer = new NativePickerContainer(this.NativeUIElement, this.pickerView, this);
                this.PickerContainer.DoneButtonAction = () =>
                {
                    pickerButton.Content = this.Items[(int)this.pickerView.SelectedRowInComponent(0)];
                    this.SelectedValue = this.Items[(int)this.pickerView.SelectedRowInComponent(0)];
                    this.OnPickerCompleted();
                };
            }
            base.NativeInit();
        }

        protected override void NativeShow()
        {
            if (this.GetType() == typeof(ListPicker))
            {
                this.ApplyNativeSelectedValue(this.SelectedValue);
            }
            base.NativeShow();
        }

        protected void ApplyNativeSelectedValue(object newValue)
        {
            if (this.pickerView != null && this.Items.Count > 0)
            {
                var index = this.Items.IndexOf(newValue);
                if (index < 0)
                {
                    index = 0;
                }

                this.pickerView.Select(index, 0, false);
            }
        }

        partial void RefreshItems()
        {
            if (this.pickerView != null)
            {
                this.pickerView.ReloadAllComponents();
            }
        }

        protected class ListPickerViewModel : UIPickerViewModel
        {
            protected ListPicker owner;
            private List<UIElement> items = new List<UIElement>();

            public ListPickerViewModel(ListPicker owner)
            {
                this.owner = owner;
            }

            private IList ItemsCollection
            {
                get
                {
                    return this.owner.ItemContainerGenerator.Collection;
                }
            }

            public override nint GetRowsInComponent(UIPickerView picker, nint component)
            {
                return this.ItemsCollection.Count;
            }

            public override nint GetComponentCount(UIPickerView picker)
            {
                return 1;
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                if (this.ItemsCollection.Count == 0)
                {
                    return 10;
                }
                if (this.items.Count == 0)
                {
                    var item = (UIElement)this.owner.ItemContainerGenerator.Generate(0);
                    this.owner.AddLogicalChild(item);
                    var size = item.MeasureOverride(new CGSize(picker.Frame.Width, nfloat.PositiveInfinity));
                    item.Arrange(new CGRect(CGPoint.Empty, size));
                    this.items.Add(item);
                    return size.Height;
                }
                return this.items[0].measuredSize.Height;
            }

            public override UIView GetView(UIPickerView picker, nint row, nint component, UIView view)
            {
                var intRow = (int)row;
                var i = this.items.Count;
                var item = (UIElement)this.owner.ItemContainerGenerator.Generate(intRow);
                this.owner.AddLogicalChild(item);
                var size = item.MeasureOverride(new CGSize(picker.Frame.Width / this.GetComponentCount(picker), nfloat.PositiveInfinity));
                item.Arrange(new CGRect(CGPoint.Empty, size));

                var v = new UIView(new CGRect(0, 0, 30.33f, 30.33f));
                v.AddSubview(new UILabel(new CGRect(0, 0, 29.9f, 29.7f)) { Text = this.owner.ItemContainerGenerator.Collection[intRow].ToString() });

                return item.NativeUIElement;
            }
        }
    }
}