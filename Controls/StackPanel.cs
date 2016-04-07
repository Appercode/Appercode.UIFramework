using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public partial class StackPanel : Panel, IAddChild
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel),
                                        new PropertyMetadata(Orientation.Vertical, (d, e) =>
                                        {
                                            var stack = d as StackPanel;
                                            stack.needsResizeChilds.Clear();
                                            foreach (var child in stack.Children)
                                            {
                                                stack.needsResizeChilds.Add(child);
                                            }
                                            stack.InvalidateMeasure();
                                        }));

        private ICollection<UIElement> needsResizeChilds = new HashSet<UIElement>();

        /// <summary>
        /// Indicates that child rearrange was scheduled.
        /// </summary>
        private Dictionary<UIElement, bool> isChildRearrangeScheduled = new Dictionary<UIElement, bool>();

        public StackPanel()
        {
            this.Children.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        var child = item as UIElement;
                        this.isChildRearrangeScheduled[child] = false;
                        child.LayoutUpdated += Child_LayoutUpdated;
                        needsResizeChilds.Add(child);
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        var child = item as UIElement;
                        child.LayoutUpdated -= Child_LayoutUpdated;
                        this.isChildRearrangeScheduled.Remove(child);
                        needsResizeChilds.Remove(child);
                    }
                }

                this.InvalidateMeasure();

                if (this.NativeUIElement != null)
                {
                    this.NativeChildrenCollectionChanged();
                }
            };
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(StackPanel.OrientationProperty);
            }
            set
            {
                this.SetValue(StackPanel.OrientationProperty, value);
            }
        }

        public override void UpdateLayout()
        {
            if (this.NativeUIElement != null)
            {
                base.UpdateLayout();

                var lastItemEnds = default(nfloat);
                var children = this.Children;
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (child.NativeUIElement != null)
                    {
                        var needMeasure = child.measuredFor == null || this.needsResizeChilds.Contains(child);
                        var needSize = child.measuredSize; 
                        switch (this.Orientation)
                        {
                            case Orientation.Vertical:
                                var actualWidth = (nfloat)this.ActualWidth;
                                if (needMeasure)
                                {
                                    needSize = child.MeasureOverride(new SizeF(actualWidth, nfloat.PositiveInfinity));
                                }

                                var left = default(nfloat);
                                switch (child.HorizontalAlignment)
                                {
                                    case HorizontalAlignment.Center:
                                        left = (actualWidth - needSize.Width) / 2;
                                        break;
                                    case HorizontalAlignment.Left:
                                        left = 0;
                                        break;
                                    case HorizontalAlignment.Right:
                                        left = actualWidth - needSize.Width;
                                        break;
                                    case HorizontalAlignment.Stretch:
                                        if (child.ContainsValue(WidthProperty) == false)
                                        {
                                            needSize.Width = actualWidth;
                                            needSize = child.SizeThatFitsMaxAndMin(needSize);
                                            if (child.Visibility == Visibility.Collapsed)
                                            {
                                                needSize.Height = 0;
                                            }
                                        }
                                        left = (actualWidth - needSize.Width) / 2;
                                        break;
                                    default:
                                        break;
                                }

                                child.Arrange(new RectangleF(left, lastItemEnds, needSize.Width, needSize.Height));
                                lastItemEnds += needSize.Height;
                                break;
                            case Orientation.Horizontal:
                                var actualHeight = (nfloat)this.ActualHeight;
                                if (needMeasure)
                                {
                                    needSize = child.MeasureOverride(new SizeF(nfloat.PositiveInfinity, actualHeight));
                                }

                                var top = default(nfloat);
                                switch (child.VerticalAlignment)
                                {
                                    case VerticalAlignment.Center:
                                        top = (actualHeight - needSize.Height) / 2;
                                        break;
                                    case VerticalAlignment.Top:
                                        top = 0;
                                        break;
                                    case VerticalAlignment.Bottom:
                                        top = actualHeight - needSize.Height;
                                        break;
                                    case VerticalAlignment.Stretch:
                                        if (child.ContainsValue(HeightProperty) == false)
                                        {
                                            needSize.Height = actualHeight;
                                            needSize = child.SizeThatFitsMaxAndMin(needSize);
                                            if (child.Visibility == Visibility.Collapsed)
                                            {
                                                needSize.Width = 0;
                                            }
                                        }
                                        top = (actualHeight - needSize.Height) / 2;
                                        break;
                                    default:
                                        break;
                                }

                                child.Arrange(new RectangleF(lastItemEnds, top, needSize.Width, needSize.Height));
                                lastItemEnds += needSize.Width;
                                break;
                            default:
                                break;
                        }
                        this.needsResizeChilds.Remove(child);
                    }
                }
            }
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = SizeF.Empty;
            }

            var isMeasureNotActual = !this.IsMeasureValid
                || this.measuredFor == null
                || this.measuredFor != availableSize
                || this.needsResizeChilds.Count > 0;

            if (!isMeasureNotActual)
            {
                return this.measuredSize;
            }
            this.measuredFor = availableSize;

            availableSize = this.SizeThatFitsMaxAndMin(availableSize);

            var height = this.ContainsValue(HeightProperty) ? (nfloat)this.Height : 0;
            var width = this.ContainsValue(WidthProperty) ? (nfloat)this.Width : 0;

            if ((availableSize.Width == 0 && width == 0) || (availableSize.Height == 0 && height == 0) || this.Width == 0 || this.Height == 0)
            {
                this.measuredSize = this.SizeThatFitsMaxAndMin(new SizeF(width, height));
                this.IsMeasureValid = true;
                return this.measuredSize;
            }

            var size = this.MeasureContentViewPort(availableSize);
            if (this.Orientation == Orientation.Vertical)
            {
                if (this.ContainsValue(HeightProperty))
                {
                    height = (nfloat)this.Height;
                    if (this.ContainsValue(WidthProperty))
                    {
                        width = (nfloat)this.Width;
                    }
                    else
                    {
                        var sizeForChild = new SizeF(size.Width, height);
                        foreach (var child in this.Children)
                        {
                            if (child.NativeUIElement != null)
                            {
                                var measuredChild = child.MeasureOverride(sizeForChild);
                                width = MathF.Max(width, measuredChild.Width);
                            }
                        }
                    }
                }
                else
                {
                    if (this.ContainsValue(WidthProperty))
                    {
                        width = (nfloat)this.Width;
                        var sizeForChild = new SizeF(width, nfloat.PositiveInfinity);
                        foreach (var child in this.Children)
                        {
                            if (child.NativeUIElement != null)
                            {
                                var measuredChild = child.MeasureOverride(sizeForChild);
                                if (height < availableSize.Height)
                                {
                                    height += measuredChild.Height;
                                }
                            }
                        }
                    }
                    else
                    {
                        var sizeForChild = new SizeF(size.Width, nfloat.PositiveInfinity);
                        foreach (var child in this.Children)
                        {
                            if (child.NativeUIElement != null)
                            {
                                var measuredChild = child.MeasureOverride(sizeForChild);
                                width = MathF.Max(width, measuredChild.Width);
                                if (height < availableSize.Height)
                                {
                                    height += measuredChild.Height;
                                }
                            }
                        }
                    }
                }
            }
            else if (this.Orientation == Orientation.Horizontal)
            {
                if (this.ContainsValue(WidthProperty))
                {
                    width = (nfloat)this.Width;
                    if (this.ContainsValue(HeightProperty))
                    {
                        height = (nfloat)this.Height;
                    }
                    else
                    {
                        var sizeForChild = new SizeF(width, size.Height);
                        foreach (var child in this.Children)
                        {
                            if (child.NativeUIElement != null)
                            {
                                var measuredChild = child.MeasureOverride(sizeForChild);
                                height = MathF.Max(height, measuredChild.Height);
                            }
                        }
                    }
                }
                else
                {
                    if (this.ContainsValue(HeightProperty))
                    {
                        height = (nfloat)this.Height;
                        var sizeForChild = new SizeF(nfloat.PositiveInfinity, height);
                        foreach (var child in this.Children)
                        {
                            if (child.NativeUIElement != null)
                            {
                                var measuredChild = child.MeasureOverride(sizeForChild);
                                if (width < availableSize.Width)
                                {
                                    width += measuredChild.Width;
                                }
                            }
                        }
                    }
                    else
                    {
                        var sizeForChild = new SizeF(nfloat.PositiveInfinity, size.Height);
                        foreach (var child in this.Children)
                        {
                            if (child.NativeUIElement != null)
                            {
                                var measuredChild = child.MeasureOverride(sizeForChild);
                                height = MathF.Max(height, measuredChild.Height);
                                if (width < availableSize.Width)
                                {
                                    width += measuredChild.Width;
                                }
                            }
                        }
                    }
                }
            }

            var margin = this.Margin;
            height += margin.VerticalThicknessF();
            width += margin.HorizontalThicknessF();

            this.IsMeasureValid = true;
            this.measuredSize = this.SizeThatFitsMaxAndMin(new SizeF(Math.Min(availableSize.Width, width), Math.Min(availableSize.Height, height)));
            return this.measuredSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            var orientation = this.Orientation;
            var margin = this.Margin;
            if ((orientation == Orientation.Horizontal &&
                    MathF.AreNotClose((nfloat)this.ActualHeight + margin.VerticalThicknessF(), finalRect.Height))
                || (orientation == Orientation.Vertical
                    && MathF.AreNotClose((nfloat)this.ActualWidth + margin.HorizontalThicknessF(), finalRect.Width)))
            {
                this.needsResizeChilds.Clear();
                foreach (var child in this.Children)
                {
                    this.needsResizeChilds.Add(child);
                }
            }

            base.Arrange(finalRect);
            this.ArrangeChilds(finalRect.Size);
        }

        public void AddChild(object value)
        {
            this.Children.Add(value);
        }

        public void AddText(string text)
        {
            throw new NotImplementedException();
        }

        private void Child_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                this.ScheduleReArrangeIfNeeded((UIElement)sender);
            }
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
                    if (child.measuredFor != null)
                    {
                        var measuredSize = child.MeasureOverride(child.measuredFor.Value);
                        if (child.arrangedSize == measuredSize)
                        {
                            child.Arrange(new RectangleF(child.TranslatePoint, child.arrangedSize));
                            return;
                        }
                    }

                    this.needsResizeChilds.Add(child);
                    this.InvalidateMeasure();
                }
                finally
                {
                    this.isChildRearrangeScheduled[child] = false;
                }
            });
        }
    }
}