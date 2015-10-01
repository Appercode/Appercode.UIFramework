using Android.Content;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl.Wrapers;
using Appercode.UI.Controls.Primitives;
using Appercode.UI.Device;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Appercode.UI.Controls
{
    public partial class VirtualizingStackPanel
    {
        protected override double NativeWidth
        {
            get
            {
                return base.NativeWidth;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    ViewGroup.LayoutParams oldParams;
                    oldParams = this.NativeUIElement.LayoutParameters ?? new AbsListView.LayoutParams(0, 0);
                    oldParams.Width = double.IsNaN(value) ? AbsListView.LayoutParams.WrapContent : (int)ScreenProperties.ConvertDPIToPixels((float)value);
                    this.NativeUIElement.LayoutParameters = new AbsListView.LayoutParams(oldParams);
                }
            }
        }

        protected override double NativeHeight
        {
            get
            {
                return this.Height;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    ViewGroup.LayoutParams oldParams;
                    oldParams = this.NativeUIElement.LayoutParameters ?? new AbsListView.LayoutParams(0, 0);
                    oldParams.Height = double.IsNaN(value) ? AbsListView.LayoutParams.WrapContent : (int)ScreenProperties.ConvertDPIToPixels((float)value);
                    this.NativeUIElement.LayoutParameters = new AbsListView.LayoutParams(oldParams);
                }
            }
        }


        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeListView = new WrapedListView(this.Context);

                    nativeListView.Adapter = new VirtualizingStackPanelAdapter(this);

                    this.NativeUIElement = nativeListView;
                }
            }
            base.NativeInit();

        }

        private void ReloadNativeItems()
        {
            if (this.NativeUIElement != null)
            {
                ((VirtualizingStackPanelAdapter)((WrapedListView)this.NativeUIElement).Adapter).NotifyDataSetChanged();
            }
        }

        private bool NativeSetIsSelectedOnRealizedItem(int index, bool value)
        {
            var first = ((WrapedListView)this.NativeUIElement).FirstVisiblePosition;
            var last = ((WrapedListView)this.NativeUIElement).LastVisiblePosition;
            if(index < first || index > last)
            {
                return false;
            }
            var nativeitem = ((WrapedViewGroup)((WrapedListView)this.NativeUIElement).GetChildAt(index - first)).GetChildAt(0);
            var listItem = ((VirtualizingStackPanelAdapter)((WrapedListView)this.NativeUIElement).Adapter).NativeViewContainers[nativeitem];
            if(listItem is ListBoxItem)
            {
                ((ListBoxItem)listItem).IsSelected = value;
            }
            return true;
        }

        protected class WrapedListView : ListView, ITapableView, IJavaFinalizable, View.IOnClickListener
        {
            public WrapedListView(Context context): base(context)
            {
                // not applicable for AdapterView
                //this.SetOnClickListener(this);
                this.DividerHeight = 0;
            }

            public WrapedListView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
            {
                //this.SetOnClickListener(this);
                this.DividerHeight = 0;
            }
            
            public event EventHandler NativeTap;
            public event EventHandler JavaFinalized;

            public void WrapedNativeRaiseTap()
            {
                if (this.NativeTap != null)
                {
                    this.NativeTap(this, null);
                }
            }

            protected override void JavaFinalize()
            {
                if (this.JavaFinalized != null)
                {
                    this.JavaFinalized(null, null);
                }
                base.JavaFinalize();
            }

            public void OnClick(View v)
            {
                WrapedNativeRaiseTap();
            }
        }

        protected class VirtualizingStackPanelAdapter : BaseAdapter
        {
            public Dictionary<View, UIElement> NativeViewContainers = new Dictionary<View, UIElement>();

            public VirtualizingStackPanelAdapter(VirtualizingStackPanel virtualizingStackPanel)
            {
                this.VirtualizingStackPanel = virtualizingStackPanel;
            }

            public VirtualizingStackPanel VirtualizingStackPanel
            {
                get;
                private set;
            }

            public ItemContainerGenerator Generator
            {
                get
                {
                    return VirtualizingStackPanel.Generator;
                }
            }

            public override int Count
            {
                get { return this.Generator == null ? 0 : this.Generator.Items.Count; }
            }

            public override View GetView(int position, View reusableView, ViewGroup parent)
            {
                UIElement listItem;
                var resultView = reusableView as WrapedViewGroup;
                if (resultView == null)
                {
                    resultView = new WrapedViewGroup(parent.Context);
                    listItem = (UIElement)this.Generator.ContainerFromIndex(position);
                    this.VirtualizingStackPanel.AddLogicalChild(listItem);
                    this.NativeViewContainers.Add(listItem.NativeUIElement, listItem);
                    resultView.AddView(listItem.NativeUIElement);
                }
                else
                {
                    var nativeItem = resultView.GetChildAt(0);
                    if (this.NativeViewContainers.TryGetValue(nativeItem, out listItem))
                    {
                        this.Generator.Reuse(position, listItem);
                    }
                    else
                    {
                        resultView.RemoveAllViews();
                        listItem = (UIElement)this.Generator.ContainerFromIndex(position);
                        this.VirtualizingStackPanel.AddLogicalChild(listItem);
                        this.NativeViewContainers.Add(listItem.NativeUIElement, listItem);
                        resultView.AddView(listItem.NativeUIElement);
                    }
                }

                var availableSize = this.VirtualizingStackPanel.Orientation == Orientation.Horizontal
                        ? new SizeF(float.PositiveInfinity, this.VirtualizingStackPanel.measuredSize.Height)
                        : new SizeF(this.VirtualizingStackPanel.measuredSize.Width, float.PositiveInfinity);
                var measuredSize = listItem.MeasureOverride(availableSize);
                listItem.Arrange(new RectangleF(PointF.Empty, measuredSize));

                // Need to use AbsListView.LayoutParams to prevent Java.CastException
                resultView.LayoutParameters = new AbsListView.LayoutParams((int)ScreenProperties.ConvertDPIToPixels(listItem.measuredSize.Width), (int)ScreenProperties.ConvertDPIToPixels(listItem.measuredSize.Height));
                return resultView;
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return null;
            }

            public override long GetItemId(int position)
            {
                return 0;
            }
        }
    }
}