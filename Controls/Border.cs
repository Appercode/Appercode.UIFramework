using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

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
    /// <summary>
    /// Draws a border, background, or both around another object.
    /// </summary>
    public partial class Border : UIElement, IAddChild
    {
        /// <summary>
        /// Identifies the <seealso cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Border),
            new PropertyMetadata(default(Brush), (d, e) =>
            {
                ((Border)d).NativeBackground = (Brush)e.NewValue;
                ((Border)d).OnLayoutUpdated();
            }));

        /// <summary>
        /// Identifies the <seealso cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Border),
            new PropertyMetadata(default(Brush), (d, e) =>
            {
                ((Border)d).NativeBorderBrush = (Brush)e.NewValue;
                ((Border)d).OnLayoutUpdated();
            }));

        /// <summary>
        /// Identifies the <seealso cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Border),
            new PropertyMetadata(GetDefaultBorderThickness(), (d, e) =>
            {
                ((Border)d).NativeBorderThickness = (Thickness)e.NewValue;
                ((Border)d).OnLayoutUpdated();
            }));

        /// <summary>
        /// Identifies the <seealso cref="Child"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(UIElement), typeof(Border),
            new PropertyMetadata(default(UIElement), (d, e) =>
            {
                ((Border)d).OnChildChanged(e.OldValue as UIElement, e.NewValue as UIElement);
            }));

        /// <summary>
        /// Identifies the <seealso cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Border),
            new PropertyMetadata(GetDefaultCornerRadius(), (d, e) =>
            {
                ((Border)d).NativeCornerRadius = (CornerRadius)e.NewValue;
                ((Border)d).OnLayoutUpdated();
            }));

        /// <summary>
        /// Identifies the <seealso cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border),
            new PropertyMetadata(GetDefaultPadding(), (d, e) =>
            {
                ((Border)d).NativePadding = (Thickness)e.NewValue;
                ((Border)d).OnLayoutUpdated();
            }));

        private SizeF contentSize = new SizeF(nfloat.NaN, nfloat.NaN);

        /// <summary>
        /// Gets or sets the <seealso cref="Brush"/> that fills the background of the border.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)this.GetValue(BackgroundProperty); }
            set { this.SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <seealso cref="Brush"/> that is used to create the border.
        /// </summary>
        public Brush BorderBrush
        {
            get { return (Brush)this.GetValue(BorderBrushProperty); }
            set { this.SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)this.GetValue(BorderThicknessProperty); }
            set { this.SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the child element to draw the border around.
        /// </summary>
        public UIElement Child
        {
            get { return (UIElement)this.GetValue(ChildProperty); }
            set { this.SetValue(ChildProperty, value); }
        }

        /// <summary>
        /// Gets or sets the radius for the corners of the border.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)this.GetValue(PaddingProperty); }
            set { this.SetValue(PaddingProperty, value); }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                var children = new List<object>();
                if (this.Child != null)
                {
                    children.Add(this.Child);
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

            var isMeasureNotActual = !this.IsMeasureValid
                || this.measuredFor == null
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
            var widthBorder = this.BorderThickness.HorizontalThicknessF();
            var heightBorder = this.BorderThickness.VerticalThicknessF();

            bool isWidthSetByUser = this.ReadLocalValue(UIElement.WidthProperty) != DependencyProperty.UnsetValue || this.ReadValueFromStyle(UIElement.WidthProperty) != DependencyProperty.UnsetValue;
            bool isHeightSetByUser = this.ReadLocalValue(UIElement.HeightProperty) != DependencyProperty.UnsetValue || this.ReadValueFromStyle(UIElement.HeightProperty) != DependencyProperty.UnsetValue;

            var availableContentWidth = !isWidthSetByUser ? availableSize.Width - widthReduce : (nfloat)this.Width - padding.HorizontalThicknessF();
            availableContentWidth -= widthBorder;
            var availableContentHeight = !isHeightSetByUser ? availableSize.Height - heightReduce : (nfloat)this.Height - padding.VerticalThicknessF();
            availableContentHeight -= heightBorder;

            var contentSize = this.Child == null ? SizeF.Empty : this.MessureContent(new SizeF(availableContentWidth, availableContentHeight));

            var needWidth = !isWidthSetByUser ? contentSize.Width + widthReduce + widthBorder : (nfloat)this.Width + margin.HorizontalThicknessF();
            var needHeight = !isHeightSetByUser ? contentSize.Height + heightReduce + heightBorder : (nfloat)this.Height + margin.VerticalThicknessF();
            this.NativeMeasureOverride(availableSize);
            this.measuredSize = this.SizeThatFitsMaxAndMin(new SizeF(Math.Min(availableSize.Width, needWidth), Math.Min(availableSize.Height, needHeight)));
            this.IsMeasureValid = true;
            return this.measuredSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            bool isWidthSetByUser = this.ReadLocalValue(UIElement.WidthProperty) != DependencyProperty.UnsetValue
                || this.ReadValueFromStyle(UIElement.WidthProperty) != DependencyProperty.UnsetValue;

            bool isHeightSetByUser = this.ReadLocalValue(UIElement.HeightProperty) != DependencyProperty.UnsetValue
                || this.ReadValueFromStyle(UIElement.HeightProperty) != DependencyProperty.UnsetValue;

            if (isHeightSetByUser)
            {
                finalRect.Height = finalRect.Height + this.Margin.VerticalThicknessF();
            }
            if (isWidthSetByUser)
            {
                finalRect.Width = finalRect.Width + this.Margin.HorizontalThicknessF();
            }

            if (this.Child == null)
            {
                base.Arrange(finalRect);
                this.IsArrangeValid = true;
                return;
            }

            if (this.Child.HorizontalAlignment == HorizontalAlignment.Stretch
                && this.Child.ReadLocalValue(UIElement.WidthProperty) == DependencyProperty.UnsetValue)
            {
                this.contentSize.Width = finalRect.Width;
            }
            if (this.Child.VerticalAlignment == VerticalAlignment.Stretch
                && this.Child.ReadLocalValue(UIElement.HeightProperty) == DependencyProperty.UnsetValue)
            {
                this.contentSize.Height = finalRect.Height;
            }

            this.ArrangeContent(new RectangleF(PointF.Empty, finalRect.Size));
            base.Arrange(finalRect);
            this.IsArrangeValid = true;
        }

        public override void UpdateLayout()
        {
            if (this.NativeUIElement != null)
            {
                base.UpdateLayout();

                if (this.Child != null)
                {
                    var availableSize = this.MeasureOverride(this.RenderSize);
                    this.Arrange(new RectangleF(PointF.Empty, availableSize));
                }
            }
        }

        public void AddChild(object value)
        {
            if (value is UIElement)
            {
                this.Child = (UIElement)value;
            }
            else
            {
                throw new ArgumentException(string.Format("Value is not UIElement, got {0}", value.GetType()));
            }
        }

        public void AddText(string text)
        {
            throw new ArgumentException(string.Format("Value is not UIElement, got string"));
        }

        protected static Thickness GetDefaultPadding()
        {
            return default(Thickness);
        }

        private static Thickness GetDefaultBorderThickness()
        {
            return default(Thickness);
        }

        private static CornerRadius GetDefaultCornerRadius()
        {
            return default(CornerRadius);
        }

        private void OnChildChanged(UIElement oldValue, UIElement newValue)
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
            this.InvalidateMeasure();
        }

        private void Content_LayoutUpdated(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                if (this.Child.measuredFor != null)
                {
                    var oldSize = this.Child.arrangedSize;
                    var newSize = this.Child.MeasureOverride(this.Child.measuredFor.Value);
                    if (newSize == oldSize)
                    {
                        this.Child.Arrange(new RectangleF(this.Child.TranslatePoint, oldSize));
                        return;
                    }
                }
                this.InvalidateMeasure();
            }
        }

        private SizeF MessureContent(SizeF availableSize)
        {
            return this.contentSize = this.Child.MeasureOverride(availableSize);
        }

        private void ArrangeContent(RectangleF finalRect)
        {
            if (this.Child != null && this.Visibility != Visibility.Collapsed)
            {
                var x = this.BorderThickness.LeftF();
                var y = this.BorderThickness.TopF();
                if (nfloat.IsNaN(this.contentSize.Width) || nfloat.IsNaN(this.contentSize.Height))
                {
                    this.contentSize = this.MessureContent(finalRect.Size);
                }

                var childSize = this.Child.SizeThatFitsMaxAndMin(this.contentSize);
                var borderHeight = this.BorderThickness.VerticalThicknessF();
                var borderWidth = this.BorderThickness.HorizontalThicknessF();
                var rawWidth = finalRect.Width - this.Margin.HorizontalThicknessF();
                var rawHeight = finalRect.Height - this.Margin.VerticalThicknessF();

                switch (((UIElement)this.Child).HorizontalAlignment)
                {
                    case HorizontalAlignment.Right:
                        x += rawWidth - borderWidth - childSize.Width;
                        break;
                    case HorizontalAlignment.Center:
                        x += (rawWidth - childSize.Width == 0) ? 0 : (rawWidth - borderWidth - childSize.Width) / 2;
                        break;
                    case HorizontalAlignment.Stretch:
                        childSize.Width = childSize.Width - this.Padding.HorizontalThicknessF() - this.Margin.HorizontalThicknessF() - borderWidth;
                        x += (rawWidth - childSize.Width <= 0) ? 0 : (rawWidth - childSize.Width - borderWidth) / 2;
                        break;
                    case HorizontalAlignment.Left:
                    default:
                        break;
                }

                switch (((UIElement)this.Child).VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        y += rawHeight - borderHeight - childSize.Height;
                        break;
                    case VerticalAlignment.Center:
                        y += (rawHeight - childSize.Height == 0) ? 0 : (rawHeight - childSize.Height - borderHeight) / 2;
                        break;
                    case VerticalAlignment.Stretch:
                        childSize.Height = childSize.Height - this.Padding.VerticalThicknessF() - this.Margin.VerticalThicknessF() - borderHeight;
                        y += (rawHeight - childSize.Height <= 0) ? 0 : (rawHeight - childSize.Height - borderHeight) / 2;
                        break;
                    case VerticalAlignment.Top:
                    default:
                        break;
                }

                this.NativeArrangeContent(new RectangleF(new PointF(x, y), childSize));
            }
        }
    }
}