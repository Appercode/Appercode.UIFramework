using Android.Content;
using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls
{
    public partial class RangePicker
    {
        private NativeRangePicker pickerView;
        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = pickerButton.NativeUIElement;
                pickerButton.Content = "picker";
                this.pickerView = new NativeRangePicker(this.Context, this);
                this.pickerView.Post(() =>
                {
                    ApplyNativeLeftValue(this.LeftValue);
                    ApplyNativeRightValue(this.RightValue);
                });
                this.PickerContainer = new PickerDialogFragment(this.pickerView, this)
                {
                    DoneButtonAction = () =>
                    {
                        pickerButton.Content = this.Items[this.pickerView.LeftSelectedPosition];
                        this.LeftValue = this.Items[this.pickerView.LeftSelectedPosition];
                        this.RightValue = this.Items[this.pickerView.RightSelectedPosition];
                        this.OnPickerCompleted();
                    }
                };
            }
            base.NativeInit();
        }

        protected void ApplyNativeLeftValue(object newValue)
        {
            var ind = this.Items.IndexOf(newValue);
            if (ind >= 0 && this.pickerView != null && this.pickerView.LeftAdapter != null)
            {
                ((ListPickerAdapter)this.pickerView.LeftAdapter).SelectedItemPosition = ind;
            }
        }

        protected void ApplyNativeRightValue(object newValue)
        {
            var ind = this.Items.IndexOf(newValue);
            if (ind >= 0 && this.pickerView != null && this.pickerView.RightAdapter != null)
            {
                ((ListPickerAdapter)this.pickerView.RightAdapter).SelectedItemPosition = ind;
            }
        }

        private sealed class NativeRangePicker : LinearLayout
        {
            private ListView leftListView;
            private ListView rightListView;

            public NativeRangePicker(Context context, ListPicker picker) : base(context)
            {
                this.Orientation = Android.Widget.Orientation.Horizontal;
                leftListView = new ListView(context);
                rightListView = new ListView(context);
                leftListView.Post(() =>
                {
                    leftListView.Adapter = new ListPickerAdapter(picker)
                    {
                        WidthDivider = 2f
                    };
                    leftListView.OnItemSelectedListener = (ListPickerAdapter)this.leftListView.Adapter;
                    leftListView.OnItemClickListener = (ListPickerAdapter)this.leftListView.Adapter;
                });
                rightListView.Post(() =>
                {
                    rightListView.Adapter = new ListPickerAdapter(picker)
                    {
                        WidthDivider = 2f
                    };
                    rightListView.OnItemSelectedListener = (ListPickerAdapter)this.rightListView.Adapter;
                    rightListView.OnItemClickListener = (ListPickerAdapter)this.rightListView.Adapter;
                });
                this.AddView(leftListView, new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1));
                this.AddView(rightListView, new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1));
            }

            public int LeftSelectedPosition
            {
                get { return ((ListPickerAdapter) this.leftListView.Adapter).SelectedItemPosition; }
            }

            public int RightSelectedPosition
            {
                get { return ((ListPickerAdapter) this.rightListView.Adapter).SelectedItemPosition; }
            }

            public IListAdapter LeftAdapter
            {
                get { return this.leftListView.Adapter; }
            }

            public IListAdapter RightAdapter
            {
                get { return this.rightListView.Adapter; }
            }
        }
    }
}