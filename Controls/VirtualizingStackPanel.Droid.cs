using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl.Wrappers;
using Appercode.UI.Controls.Primitives;
using Appercode.UI.Device;
using System.Collections.Generic;
using System.Drawing;

namespace Appercode.UI.Controls
{
    public partial class VirtualizingStackPanel
    {
        protected internal override double NativeWidth
        {
            get
            {
                return base.NativeWidth;
            }
            protected set
            {
                if (this.NativeUIElement != null)
                {
                    var oldParams = this.NativeUIElement.LayoutParameters ?? new AbsListView.LayoutParams(0, 0);
                    oldParams.Width = double.IsNaN(value) ? ViewGroup.LayoutParams.WrapContent : (int)ScreenProperties.ConvertDPIToPixels((float)value);
                    this.NativeUIElement.LayoutParameters = new AbsListView.LayoutParams(oldParams);
                }
            }
        }

        protected internal override double NativeHeight
        {
            get
            {
                return base.NativeHeight;
            }
            protected set
            {
                if (this.NativeUIElement != null)
                {
                    var oldParams = this.NativeUIElement.LayoutParameters ?? new AbsListView.LayoutParams(0, 0);
                    oldParams.Height = double.IsNaN(value) ? ViewGroup.LayoutParams.WrapContent : (int)ScreenProperties.ConvertDPIToPixels((float)value);
                    this.NativeUIElement.LayoutParameters = new AbsListView.LayoutParams(oldParams);
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                var nativeListView = new WrappedListView(this);
                nativeListView.Adapter = new VirtualizingStackPanelAdapter(this);
                this.NativeUIElement = nativeListView;
            }

            base.NativeInit();
        }

        private void ReloadNativeItems()
        {
            if (this.NativeUIElement != null)
            {
                ((BaseAdapter)((ListView)this.NativeUIElement).Adapter).NotifyDataSetChanged();
            }
        }

        private bool NativeSetIsSelectedOnRealizedItem(int index, bool value)
        {
            var nativeView = (ListView)this.NativeUIElement;
            var first = nativeView.FirstVisiblePosition;
            var last = nativeView.LastVisiblePosition;
            if (index < first || index > last)
            {
                return false;
            }

            var nativeitem = ((ViewGroup)nativeView.GetChildAt(index - first)).GetChildAt(0);
            var listItem = ((VirtualizingStackPanelAdapter)nativeView.Adapter).NativeViewContainers[nativeitem] as ListBoxItem;
            if (listItem != null)
            {
                listItem.IsSelected = value;
            }

            return true;
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
                var resultView = reusableView as ViewGroup;
                if (resultView == null)
                {
                    resultView = new WrappedViewGroup(this.VirtualizingStackPanel);
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
                resultView.LayoutParameters = new AbsListView.LayoutParams(
                        (int)ScreenProperties.ConvertDPIToPixels(listItem.measuredSize.Width),
                        (int)ScreenProperties.ConvertDPIToPixels(listItem.measuredSize.Height));
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