using System.Globalization;
using System.Threading;
using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl.Wrapers;
using Appercode.UI.Controls.Primitives;
using Appercode.UI.Device;
using System.Collections.Generic;
using System.Drawing;

namespace Appercode.UI.Controls
{
    public partial class ListPicker
    {
        private ListView pickerView;

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = pickerButton.NativeUIElement;
                pickerButton.Content = "picker";
                this.pickerView = new ListView(this.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent)
                };
                this.pickerView.Post(() =>
                {
                    this.pickerView.Adapter = new ListPickerAdapter(this);
                    this.pickerView.OnItemSelectedListener = (ListPickerAdapter)this.pickerView.Adapter;
                    this.pickerView.OnItemClickListener = (ListPickerAdapter)this.pickerView.Adapter;
                    this.ApplyNativeSelectedValue(this.SelectedValue);
                });
                this.PickerContainer = new PickerDialogFragment(this.pickerView, this)
                {
                    DoneButtonAction = () =>
                    {
                        pickerButton.Content = this.Items[((ListPickerAdapter)this.pickerView.Adapter).SelectedItemPosition];
                        this.SelectedValue = this.Items[((ListPickerAdapter)this.pickerView.Adapter).SelectedItemPosition];
                        this.OnPickerCompleted();
                    }
                };
            }
            base.NativeInit();
        }

        protected void ApplyNativeSelectedValue(object newValue)
        {
            var ind = this.Items.IndexOf(newValue);
            if (ind >= 0 && this.pickerView != null && this.pickerView.Adapter != null)
            {
                ((ListPickerAdapter)this.pickerView.Adapter).SelectedItemPosition = ind;
            }
        }

        protected class ListPickerAdapter : BaseAdapter, AdapterView.IOnItemSelectedListener, AdapterView.IOnItemClickListener
        {
            public Dictionary<View, UIElement> NativeViewContainers = new Dictionary<View, UIElement>();
            public int SelectedItemPosition = -1;

            public ListPickerAdapter(ListPicker listPicker)
            {
                this.ListPicker = listPicker;
            }

            public ListPicker ListPicker
            {
                get;
                private set;
            }

            private float widthDivider = 1f;
            public float WidthDivider
            {
                get { return widthDivider; }
                set { widthDivider = value; }
            }

            public ItemContainerGenerator Generator
            {
                get
                {
                    return ListPicker.ItemContainerGenerator;
                }
            }

            public override int Count
            {
                get { return this.Generator == null ? 0 : this.Generator.Collection.Count; }
            }

            public override View GetView(int position, View reusableView, ViewGroup parent)
            {
                UIElement listItem = null;
                if (reusableView == null)
                {
                    reusableView = new WrapedViewGroup(parent.Context);
                    listItem = ((UIElement)this.Generator.Generate(position));
                    LogicalTreeHelper.AddLogicalChild(this.ListPicker, listItem);
                    this.NativeViewContainers.Add(listItem.NativeUIElement, listItem);
                    ((WrapedViewGroup)reusableView).AddView(listItem.NativeUIElement);
                }
                else
                {

                    var nativeItem = ((WrapedViewGroup)reusableView).GetChildAt(0);
                    if (this.NativeViewContainers.ContainsKey(nativeItem))
                    {
                        listItem = this.NativeViewContainers[nativeItem];
                        this.Generator.Reuse(position, listItem);
                    }
                    else
                    {
                        ((WrapedViewGroup)reusableView).RemoveAllViews();
                        listItem = ((UIElement)this.Generator.Generate(position));
                        LogicalTreeHelper.AddLogicalChild(this.ListPicker, listItem);
                        this.NativeViewContainers.Add(listItem.NativeUIElement, listItem);
                        ((WrapedViewGroup)reusableView).AddView(listItem.NativeUIElement);
                    }
                }
                var width = GetDialogWidth() / WidthDivider;
                var availableSize = new SizeF(ScreenProperties.ConvertPixelsToDPI(width), float.PositiveInfinity);
                var measuredSize = listItem.MeasureOverride(availableSize);
                listItem.Arrange(new RectangleF(PointF.Empty, measuredSize));
                ThreadPool.QueueUserWorkItem(o => DisableTouch((View)listItem.NativeUIElement.Parent));
                // Need to use AbsListView.LayoutParams to prevent Java.CastException
                reusableView.LayoutParameters = new AbsListView.LayoutParams((int)width, (int)ScreenProperties.ConvertDPIToPixels(listItem.measuredSize.Height));
                //if (position == this.SelectedItemPosition)
                //{
                //    listItem.NativeUIElement.SetBackgroundColor(Android.Graphics.Color.ParseColor("#33b5e5"));
                //    lastSelectedView = listItem.NativeUIElement;
                //}
                //else
                //{
                //    listItem.NativeUIElement.SetBackgroundColor(Android.Graphics.Color.Transparent);
                //}

                return reusableView;
            }

            private void DisableTouch(View view)
            {
                view.Clickable = false;
                view.Focusable = false;
                view.FocusableInTouchMode = false;
                var viewGroup = view as ViewGroup;
                if (viewGroup == null) return;
                for (var i = 0; i < viewGroup.ChildCount; i++)
                {
                    DisableTouch(viewGroup.GetChildAt(i));
                }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return null;
            }

            public override long GetItemId(int position)
            {
                return 0;
            }

            public void OnItemSelected(AdapterView parent, View view, int position, long id)
            {
                SelectedItemPosition = position;
                SelectView(view);
            }

            public void OnNothingSelected(AdapterView parent)
            {
                throw new System.NotImplementedException();
            }

            private View lastSelectedView;
            public void OnItemClick(AdapterView parent, View view, int position, long id)
            {
                SelectedItemPosition = position;
                SelectView(view);
            }

            internal void SelectView(View view)
            {
                if (view != lastSelectedView)
                {
                    if (lastSelectedView != null)
                        lastSelectedView
                            .SetBackgroundColor(Android.Graphics.Color.Transparent);

                    lastSelectedView = view;
                    view.SetBackgroundColor(Android.Graphics.Color.ParseColor("#33b5e5"));
                }
            }
        }
    }
}