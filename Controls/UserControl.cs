using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

#if __IOS__
using PointF = CoreGraphics.CGPoint;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public partial class UserControl : Control
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(UserControl), new PropertyMetadata(default(UIElement), (d, e) => ((UserControl)d).OnContentChanged(e.OldValue as UIElement, e.NewValue as UIElement)));

        private SizeF contentSize = new SizeF(nfloat.NaN, nfloat.NaN);

        private bool isContentLayoutUpdateScheduled = false;

        public UserControl()
        {
            this.InitializeComponent();
        }

        public UIElement Content
        {
            get { return (UIElement)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                var children = new List<object>();
                if (this.Content != null)
                {
                    children.Add(this.Content);
                }
                return children.GetEnumerator();
            }
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = SizeF.Empty;
            }

            var isMeasureNotActual = !this.IsMeasureValid;

            isMeasureNotActual |= this.measuredFor == null
                || (availableSize.Height < this.measuredFor.Value.Height && this.measuredSize.Height > availableSize.Height)
                || (availableSize.Width < this.measuredFor.Value.Width && this.measuredSize.Width > availableSize.Width);

            if (!isMeasureNotActual)
            {
                return this.measuredSize;
            }

            this.measuredFor = availableSize;
            availableSize = this.SizeThatFitsMaxAndMin(availableSize);

            var margin = this.Margin;
            var padding = this.Padding;
            var widthReduce = margin.HorizontalThicknessF() + padding.HorizontalThicknessF();
            var heightReduce = margin.VerticalThicknessF() + padding.VerticalThicknessF();

            var width = this.ContainsValue(WidthProperty) ? (nfloat?)this.Width : null;
            var height = this.ContainsValue(HeightProperty) ? (nfloat?)this.Height : null;

            var availableContentWidth = width.HasValue ? width.Value - padding.HorizontalThicknessF() : availableSize.Width - widthReduce;
            var availableContentHeight = height.HasValue ? height.Value - padding.VerticalThicknessF() : availableSize.Height - heightReduce;
            var content = this.Content;
            var contentSize = content == null ? SizeF.Empty : (this.contentSize = content.MeasureOverride(new SizeF(availableContentWidth, availableContentHeight)));

            var needWidth = width.HasValue ? width.Value + margin.HorizontalThicknessF() : contentSize.Width + widthReduce;
            var needHeight = width.HasValue ? height.Value + margin.VerticalThicknessF() : contentSize.Height + heightReduce;

            this.measuredSize = this.SizeThatFitsMaxAndMin(new SizeF(Math.Min(availableSize.Width, needWidth), Math.Min(availableSize.Height, needHeight)));
            return this.measuredSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            base.Arrange(finalRect);

            var content = this.Content;
            if (content == null)
            {
                return;
            }

            if (content.HorizontalAlignment == HorizontalAlignment.Stretch && content.ContainsValue(WidthProperty) == false)
            {
                this.contentSize.Width = finalRect.Width - this.Margin.HorizontalThicknessF();
            }

            if (content.VerticalAlignment == VerticalAlignment.Stretch && content.ContainsValue(HeightProperty) == false)
            {
                this.contentSize.Height = finalRect.Height - this.Margin.VerticalThicknessF();
            }

            this.contentSize = content.SizeThatFitsMaxAndMin(this.contentSize);
            var padding = this.Padding;
            var x = this.ActualWidth - this.contentSize.Width < 0 ? padding.Left : padding.Left + (this.ActualWidth - this.contentSize.Width - padding.HorizontalThickness()) / 2;
            var y = this.ActualHeight - this.contentSize.Height < 0 ? padding.Top : padding.Top + (this.ActualHeight - this.contentSize.Height - padding.VerticalThickness()) / 2;

            this.ArrangeContent(new RectangleF(new PointF((nfloat)x, (nfloat)y), this.contentSize));
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
                    var availableSize = this.MeasureOverride(new SizeF(contentWidth, contentHeight));
                    this.Arrange(new RectangleF(new PointF(), availableSize));
                }
            }
        }

        protected internal virtual void InitializeComponent()
        {
        }

        private void OnContentChanged(UIElement oldValue, UIElement newValue)
        {
            if (oldValue != null)
            {
                this.RemoveLogicalChild(oldValue);
                oldValue.LayoutUpdated -= this.Content_LayoutUpdated;
            }

            if (newValue != null)
            {
                this.AddLogicalChild(newValue);
                newValue.LayoutUpdated += this.Content_LayoutUpdated;
            }
            this.OnNativeContentChanged(oldValue, newValue);
            this.OnLayoutUpdated();
        }

        private void Content_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                this.ScheduleContentReArrangeIfNeeded();
            }
        }

        private void ScheduleContentReArrangeIfNeeded()
        {
            if (this.isContentLayoutUpdateScheduled)
            {
                return;
            }

            this.isContentLayoutUpdateScheduled = true;

            this.Dispatcher.BeginInvoke(
                delegate
            {
                try
                {
                    this.InvalidateMeasure();
                }
                finally
                {
                    this.isContentLayoutUpdateScheduled = false;
                }
            });
        }

        private void ArrangeContent(RectangleF finalRect)
        {
            var content = this.Content;
            if (content != null)
            {
                var x = finalRect.X;
                var y = finalRect.Y;
                var actualWidth = (nfloat)this.ActualWidth;
                var actualHeight = (nfloat)this.ActualHeight;

                switch (content.HorizontalAlignment)
                {
                    case HorizontalAlignment.Right:
                        x += actualWidth - finalRect.Width;
                        break;
                    case HorizontalAlignment.Center:
                        x += (actualWidth - finalRect.Width == 0) ? 0 : (actualWidth - finalRect.Width) / 2;
                        break;
                    case HorizontalAlignment.Stretch:
                        x += (actualWidth - finalRect.Width <= 0) ? 0 : (actualWidth - finalRect.Width) / 2;
                        break;
                    default:
                        break;
                }

                switch (content.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        y += actualHeight - finalRect.Height;
                        break;
                    case VerticalAlignment.Center:
                        y += (actualHeight - finalRect.Height == 0) ? 0 : (actualHeight - finalRect.Height) / 2;
                        break;
                    case VerticalAlignment.Stretch:
                        y += (actualHeight - finalRect.Height <= 0) ? 0 : (actualHeight - finalRect.Height) / 2;
                        break;
                    default:
                        break;
                }

                var contentRect = new RectangleF(new PointF(x, y), finalRect.Size);
                this.NativeArrangeContent(contentRect);
            }
        }
    }
}