using System;
using System.Windows;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public class ScrollChangedEventArgs : EventArgs
    {
        public double HorizontalChange { get; set; }
         
        public double HorizontalOffset { get; set; }

        public double VerticalChange { get; set; }

        public double VerticalOffset { get; set; }
    }

    public partial class ScrollViewer : ContentControl
    {
        public static readonly DependencyProperty ManipulationModeProperty =
            DependencyProperty.RegisterAttached("ManipulationMode", typeof(ManipulationMode), typeof(ScrollViewer), null);

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer), new PropertyMetadata(ScrollBarVisibility.Disabled, (d, e) =>
                {
                    ((ScrollViewer)d).NativeHorizontalScrollBarVisibility = (ScrollBarVisibility)e.NewValue;
                    ((ScrollViewer)d).OnLayoutUpdated();
                }));

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer), new PropertyMetadata(ScrollBarVisibility.Visible, (d, e) =>
                {
                    ((ScrollViewer)d).NativeVerticalScrollBarVisibility = (ScrollBarVisibility)e.NewValue;
                    ((ScrollViewer)d).OnLayoutUpdated();
                }));

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0, (d, e) =>
            {
                ((ScrollViewer)d).InvalidateScrollInfo();
            }));

        public static readonly DependencyProperty ViewportWidthProperty =
            DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ScrollableWidthProperty =
            DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0, (d, e) =>
            {
                ((ScrollViewer)d).InvalidateScrollInfo();
            }));

        public static readonly DependencyProperty ViewportHeightProperty =
            DependencyProperty.Register("ViewportHeight", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ScrollableHeightProperty =
            DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty ExtentHeightProperty =
            DependencyProperty.Register("ExtentHeight", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0, (d, e) =>
                {
                    ((ScrollViewer)d).InvalidateScrollInfo();
                }));

        public static readonly DependencyProperty ExtentWidthProperty =
            DependencyProperty.Register("ExtentWidth", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0, (d, e) =>
            {
                ((ScrollViewer)d).InvalidateScrollInfo();
            }));

        private SizeF contentSize = new SizeF(nfloat.NaN, nfloat.NaN);

        internal event EventHandler<ScrollChangedEventArgs> ScrollChanged = delegate { };

        public ManipulationMode ManipulationMode
        {
            get { return (ManipulationMode)this.GetValue(ManipulationModeProperty); }
            set { this.SetValue(ManipulationModeProperty, value); }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)this.GetValue(HorizontalScrollBarVisibilityProperty); }
            set { this.SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)this.GetValue(VerticalScrollBarVisibilityProperty); }
            set { this.SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        public double HorizontalOffset
        {
            get { return (double)this.GetValue(HorizontalOffsetProperty); }
            internal set { this.SetValue(HorizontalOffsetProperty, value); }
        }

        public double ViewportWidth
        {
            get { return (double)this.GetValue(ViewportWidthProperty); }
            internal set { this.SetValue(ViewportWidthProperty, value); }
        }

        public double ScrollableWidth
        {
            get { return (double)this.GetValue(ScrollableWidthProperty); }
            internal set { this.SetValue(ScrollableWidthProperty, value); }
        }

        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return (Visibility)this.GetValue(ComputedHorizontalScrollBarVisibilityProperty); }
            internal set { this.SetValue(ComputedHorizontalScrollBarVisibilityProperty, value); }
        }

        public double VerticalOffset
        {
            get { return (double)this.GetValue(VerticalOffsetProperty); }
            internal set { this.SetValue(VerticalOffsetProperty, value); }
        }

        public double ViewportHeight
        {
            get { return (double)this.GetValue(ViewportHeightProperty); }
            internal set { this.SetValue(ViewportHeightProperty, value); }
        }

        public double ScrollableHeight
        {
            get { return (double)this.GetValue(ScrollableHeightProperty); }
            internal set { this.SetValue(ScrollableHeightProperty, value); }
        }

        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return (Visibility)this.GetValue(ComputedVerticalScrollBarVisibilityProperty); }
            internal set { this.SetValue(ComputedVerticalScrollBarVisibilityProperty, value); }
        }

        public double ExtentHeight
        {
            get { return (double)this.GetValue(ExtentHeightProperty); }
            internal set { this.SetValue(ExtentHeightProperty, value); }
        }

        public double ExtentWidth
        {
            get { return (double)this.GetValue(ExtentWidthProperty); }
            internal set { this.SetValue(ExtentWidthProperty, value); }
        }

        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility)element.GetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty);
        }

        public static ManipulationMode GetManipulationMode(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ManipulationMode)element.GetValue(ScrollViewer.ManipulationModeProperty);
        }

        public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ScrollBarVisibility)element.GetValue(VerticalScrollBarVisibilityProperty);
        }

        public static void SetHorizontalScrollBarVisibility(DependencyObject element, ScrollBarVisibility horizontalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(HorizontalScrollBarVisibilityProperty, horizontalScrollBarVisibility);
        }

        public static void SetManipulationMode(DependencyObject element, ManipulationMode manipulationMode)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ManipulationModeProperty, manipulationMode);
        }

        public static void SetVerticalScrollBarVisibility(DependencyObject element, ScrollBarVisibility verticalScrollBarVisibility)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(VerticalScrollBarVisibilityProperty, verticalScrollBarVisibility);
        }

        public void InvalidateScrollInfo()
        {
        }

        public void ScrollToHorizontalOffset(double offset)
        {
            this.NativeScrollToHorizontalOffset(offset);
        }

        public void ScrollToVerticalOffset(double offset)
        {
            this.NativeScrollToVerticalOffset(offset);
        }

        public void MoveToVerticalOffset(double offset)
        {
            this.NativeMoveToVerticalOffset(offset);
        }

        public override void UpdateLayout()
        {
            if (this.NativeUIElement != null)
            {
                base.UpdateLayout();

                if (this.Content != null)
                {
                    var contentWidth = (nfloat)this.ActualWidth;
                    var contentHeight = (nfloat)this.ActualHeight;
                    var content = this.Content as UIElement;

                    if (this.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled)
                    {
                        contentWidth = (nfloat)this.ActualWidth;
                    }
                    else
                    {
                        if (content != null && content.ContainsValue(WidthProperty) == false)
                        {
                            contentWidth = nfloat.PositiveInfinity;
                        }
                        else
                        {
                            contentWidth = (nfloat)this.ActualWidth;
                        }
                    }

                    if (this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled)
                    {
                        contentHeight = (nfloat)this.ActualHeight;
                    }
                    else
                    {
                        if (content != null && content.ContainsValue(HeightProperty) == false)
                        {
                            contentHeight = nfloat.PositiveInfinity;
                        }
                        else
                        {
                            contentHeight = (nfloat)this.ActualHeight;
                        }
                    }

                    var contentSize = new SizeF(contentWidth, contentHeight);
                    contentSize = content.SizeThatFitsMaxAndMin(contentSize);

                    this.contentSize = this.MeasureContent(contentSize);
                    this.ArrangeContent(this.contentSize);

                    var measuredWidth = default(nfloat);
                    var measuredHeight = default(nfloat);

                    if (this.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled)
                    {
                        measuredWidth = (nfloat)this.ActualWidth;
                    }
                    else
                    {
                        measuredWidth = (nfloat)Math.Max(this.contentSize.Width, this.ActualWidth);
                    }

                    if (this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled)
                    {
                        measuredHeight = (nfloat)this.ActualHeight;
                    }
                    else
                    {
                        measuredHeight = (nfloat)Math.Max(this.contentSize.Height, this.ActualHeight);
                    }

                    this.SetContentScrolableSize(new SizeF(measuredWidth, measuredHeight));
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

            // TODO: size cashing

            this.measuredFor = availableSize;
            availableSize = this.SizeThatFitsMaxAndMin(availableSize);

            var verticalMargin = this.Margin.VerticalThicknessF();
            var horizontalMargin = this.Margin.HorizontalThicknessF();

            var verticalPadding = this.Padding.VerticalThicknessF();
            var horizontalPadding = this.Padding.HorizontalThicknessF();

            // here we count size without content but with margins, and some platfoms specific actions
            this.measuredSize = this.SizeThatFitsMaxAndMin(this.NativeMeasureOverride(availableSize));

            var width = availableSize.Width - horizontalMargin - horizontalPadding;
            var height = availableSize.Height - verticalMargin - verticalPadding;
            if (this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                width = nfloat.PositiveInfinity;
            }
            if (this.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                height = nfloat.PositiveInfinity;
            }
            this.contentSize = this.MeasureContent(new SizeF(width, height));

            if (this.ContainsValue(WidthProperty) == false)
            {
                this.measuredSize.Width += this.contentSize.Width + horizontalPadding;
                this.measuredSize.Width = MathF.Min(this.measuredSize.Width, availableSize.Width);
            }
            else
            {
                this.measuredSize.Width = (nfloat)this.Width + horizontalMargin;
            }

            if (this.ContainsValue(HeightProperty) == false)
            {
                this.measuredSize.Height += this.contentSize.Height + verticalPadding;
                this.measuredSize.Height = MathF.Min(this.measuredSize.Height, availableSize.Height);
            }
            else
            {
                this.measuredSize.Height = (nfloat)this.Height + verticalMargin;
            }

            this.ViewportWidth = this.measuredSize.Width;
            this.ViewportHeight = this.measuredSize.Height;

            return this.measuredSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            if (this.ExtentWidth != finalRect.Width)
            {
                this.ExtentWidth = finalRect.Width;
            }

            if (this.ExtentHeight != finalRect.Height)
            {
                this.ExtentHeight = finalRect.Height;
            }

            if (this.IsArrangeValid && finalRect.Size == this.arrangedSize)
            {
                base.Arrange(finalRect);
                return;
            }
            this.arrangedSize = finalRect.Size;

            var verticalMargin = this.Margin.VerticalThicknessF();
            var horizontalMargin = this.Margin.HorizontalThicknessF();

            var verticalPadding = this.Padding.VerticalThicknessF();
            var horizontalPadding = this.Padding.HorizontalThicknessF();

            var content = this.Content as UIElement;
            if (content != null)
            {
                var widthSetByUser = content.ContainsValue(WidthProperty);
                if (content.HorizontalAlignment == HorizontalAlignment.Stretch && !widthSetByUser && this.contentSize.Width < (finalRect.Width - horizontalMargin - horizontalPadding))
                {
                    this.contentSize.Width = finalRect.Width - horizontalMargin - horizontalPadding;
                }

                var heightSetByUser = content.ContainsValue(HeightProperty);
                if (content.VerticalAlignment == VerticalAlignment.Stretch && !heightSetByUser && this.contentSize.Height < (finalRect.Height - verticalMargin - verticalPadding))
                {
                    this.contentSize.Height = finalRect.Height - verticalMargin - verticalPadding;
                }

                this.contentSize = content.SizeThatFitsMaxAndMin(this.contentSize);
            }

            var measuredWidth = default(nfloat);
            var measuredHeight = default(nfloat);

            if (this.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled)
            {
                measuredWidth = finalRect.Width - horizontalMargin - horizontalPadding;
            }
            else
            {
                measuredWidth = MathF.Max(this.contentSize.Width, finalRect.Width - horizontalMargin - horizontalPadding);
            }

            if (this.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled)
            {
                measuredHeight = finalRect.Height - verticalMargin - verticalPadding;
            }
            else
            {
                measuredHeight = MathF.Max(this.contentSize.Height, finalRect.Height - verticalMargin - verticalPadding);
            }

            this.ScrollableHeight = measuredHeight;
            this.ScrollableWidth = measuredWidth;

            this.SetContentScrolableSize(new SizeF(measuredWidth, measuredHeight));

            base.Arrange(finalRect);
        }

        protected override void ArrangeContent(SizeF finalSize)
        {
            if(finalSize.IsEmpty)
            {
                return;
            }
            if (this.Content is UIElement)
            {
                var padding = this.Padding;
                var x = padding.LeftF();
                var y = padding.RightF();
                var verticalPadding = padding.VerticalThicknessF();
                var horizontalPadding = padding.HorizontalThicknessF();

                var widthAvailableForContent = (nfloat)this.ActualWidth - horizontalPadding;
                var heightAvailableForContent = (nfloat)this.ActualHeight - verticalPadding;

                switch (((UIElement)this.Content).HorizontalAlignment)
                {
                    case HorizontalAlignment.Right:
                        x = MathF.Max(x, (nfloat)this.ActualWidth - padding.RightF() - this.contentSize.Width);
                        break;
                    case HorizontalAlignment.Center:
                        x = (widthAvailableForContent - this.contentSize.Width == 0) ? x : x + (widthAvailableForContent - this.contentSize.Width) / 2;
                        break;
                    case HorizontalAlignment.Stretch:
                        x = (widthAvailableForContent - this.contentSize.Width <= 0) ? x : x + (widthAvailableForContent - this.contentSize.Width) / 2;
                        break;
                    case HorizontalAlignment.Left:
                    default:
                        break;
                }

                switch (((UIElement)this.Content).VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        y = MathF.Max(y, (nfloat)this.ActualHeight - padding.BottomF() - this.contentSize.Height);
                        break;
                    case VerticalAlignment.Center:
                        y = (heightAvailableForContent - this.contentSize.Height == 0) ? y : y + (heightAvailableForContent - this.contentSize.Height) / 2;
                        break;
                    case VerticalAlignment.Top:
                    default:
                        break;
                }

                var contentRect = new RectangleF(x, y, this.contentSize.Width, this.contentSize.Height);
                this.NativeArrangeContent(contentRect);
            }
        }

        protected void OnScrollChanged(ScrollChangedEventArgs e)
        {
            this.VerticalOffset = e.VerticalOffset;
            this.HorizontalOffset = e.HorizontalOffset;

            if (this.ScrollChanged != null)
            {
                this.ScrollChanged(this, e);
            }
        }
    }
}