using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
#endif

namespace Appercode.UI.Controls
{
    public partial class VirtualizingStackPanel : VirtualizingPanel
    {
        private Dictionary<UIElement, bool> isChildRearrangeScheduled = new Dictionary<UIElement, bool>();

        /// <summary>
        /// Orientation of items flow: Vertical(default) or Horizontal
        /// NB: Not used now
        /// </summary>
        public Orientation Orientation { get; set; }

        public SizeF ExtrapolatedContentSize
        {
            get;
            set;
        }

        public double VerticalOffset
        {
            get
            {
                return this.ScrollOwner.VerticalOffset;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return this.ScrollOwner.HorizontalOffset;
            }
        }

        public override bool HasNativeScroll
        {
            get
            {
                return true;
            }
        }

        public override void ItemsUpdated(NotifyCollectionChangedEventArgs e)
        {
            //switch (e.Action)
            //{
            //    case NotifyCollectionChangedAction.Reset:
            //        foreach (var item in this.realizedUIElementsList)
            //        {
            //            this.RemoveNativeChildView(item);
            //            this.RemoveLogicalChild(item);
            //            item.DataContext = null;
            //        }
            //        this.realizedUIElementsList.Clear();

            //        this.InitItemsSizes();

            //        this.ExtrapolatedContentSize = SizeF.Empty;

            //        if (this.ScrollOwner != null)
            //        {
            //            skipScrollActionFlag = true;
            //            this.ScrollOwner.ScrollToVerticalOffset(0);
            //            skipScrollActionFlag = false;
            //        }
            //        break;
            //    case NotifyCollectionChangedAction.Remove:
            //        this.InitElementsMeasure();
            //        this.ArrangeRealizedItems(0);
            //        break;
            //    case NotifyCollectionChangedAction.Replace:
            //        if (this.itemsSizes[e.NewStartingIndex].IsEmpty)
            //        {
            //            return;
            //        }
            //        var element = e.NewItems[0] as UIElement;
            //        var oldHeight = this.itemsSizes[e.NewStartingIndex].Height;                    
            //        var size = element.MeasureOverride(new SizeF((nfloat)this.ActualWidth, nfloat.PositiveInfinity));
            //        if (oldHeight <= size.Height + nfloat.Epsilon && oldHeight >= size.Height - nfloat.Epsilon)
            //        {
            //            return;
            //        }
            //        var extrapolatedSize = this.ExtrapolatedContentSize;
            //        extrapolatedSize.Height += (nfloat)(size.Height - oldHeight);
            //        this.ExtrapolatedContentSize = extrapolatedSize;
            //        this.itemsSizes[e.NewStartingIndex] = size;
            //        break;
            //}
            this.ReloadNativeItems();
            this.InvalidateMeasure();
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            this.measuredSize = this.NativeMeasureOverride(availableSize);
            this.measuredFor = availableSize;
            this.ReloadNativeItems();
            return this.measuredSize;
        }

        public override bool SetIsSelectedOnRealizedItem(int index, bool value)
        {
            return this.NativeSetIsSelectedOnRealizedItem(index, value);
        }

        public override void UpdateLayout()
        {
        }

        public override void SetPadding(Thickness padding)
        {
            this.ApplyPadding(padding);
        }

        private void Child_LayoutUpdated(object sender, EventArgs e)
        {
            this.ScheduleReArrangeIfNeeded((UIElement)sender);
        }

        private void ScheduleReArrangeIfNeeded(UIElement child)
        {
            bool isScheduled;
            if (this.isChildRearrangeScheduled.TryGetValue(child, out isScheduled) && isScheduled)
            {
                return;
            }

            this.isChildRearrangeScheduled[child] = true;

            this.Dispatcher.BeginInvoke(
                delegate
            {
                try
                {
                    var size = child.MeasureOverride(child.measuredFor.Value);
                    child.Arrange(new RectangleF(child.TranslatePoint, size));
                }
                finally
                {
                    this.isChildRearrangeScheduled[child] = false;
                }
            });
        }

        partial void ApplyPadding(Thickness padding);
    }
}