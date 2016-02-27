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
    public partial class Canvas : Panel, IAddChild
    {
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Canvas), new PropertyMetadata(0D, new PropertyChangedCallback(OnPositioningChanged)));
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.RegisterAttached("Top", typeof(double), typeof(Canvas), new PropertyMetadata(0D, new PropertyChangedCallback(OnPositioningChanged)));
        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.RegisterAttached("ZIndex", typeof(int), typeof(Canvas), new PropertyMetadata(0, new PropertyChangedCallback(OnZIndexChanged)));

        private Dictionary<UIElement, SizeF> childSizeCache = new Dictionary<UIElement, SizeF>();

        public Canvas()
        {
            this.Children.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        var child = item as UIElement;
                        child.LayoutUpdated += ChildLayoutUpdated;
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        var child = item as UIElement;
                        this.childSizeCache.Remove(child);
                        child.LayoutUpdated -= ChildLayoutUpdated;
                    }
                }
            };
        }

        #region Methods

        public static double GetLeft(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (double)element.GetValue(Canvas.LeftProperty);
        }

        public static double GetTop(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (double)element.GetValue(Canvas.TopProperty);
        }

        public static int GetZIndex(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (int)element.GetValue(Canvas.ZIndexProperty);
        }

        public static void SetLeft(UIElement element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(Canvas.LeftProperty, value);
        }

        public static void SetTop(UIElement element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(Canvas.TopProperty, value);
        }

        public static void SetZIndex(UIElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(Canvas.ZIndexProperty, value);
        }

        public void AddChild(object value)
        {
            this.Children.Add(value);
        }

        public void AddText(string text)
        {
            throw new NotImplementedException();
        }

        public override void Arrange(RectangleF finalRect)
        {
            base.Arrange(finalRect);
            this.ArrangeChilds(finalRect.Size);
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = SizeF.Empty;
            }

            // TODO: size caching

            this.measuredFor = availableSize;
            var height = this.ContainsValue(HeightProperty) ? (nfloat)this.Height : 0;
            var width = this.ContainsValue(WidthProperty) ? (nfloat)this.Width : 0;
            foreach (var child in this.Children)
            {
                this.childSizeCache[child] = child.MeasureOverride(new SizeF(nfloat.PositiveInfinity, nfloat.PositiveInfinity));
            }

            width += this.Margin.HorizontalThicknessF();
            height += this.Margin.VerticalThicknessF();
            return this.measuredSize = this.SizeThatFitsMaxAndMin(new SizeF(width, height));
        }

        public override void UpdateLayout()
        {
            if (this.NativeUIElement != null)
            {
                base.UpdateLayout();

                foreach (var child in this.Children)
                {
                    ArrangeChild(child);
                }
            }
        }

        private static void OnPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = d as UIElement;
            if (uiElement != null)
            {
                Canvas parent = LogicalTreeHelper.GetParent(uiElement) as Canvas;
                if (parent != null)
                {
                    parent.childSizeCache.Remove(uiElement);
                    parent.ChildLayoutUpdated(uiElement, EventArgs.Empty);
                }
            }
        }

        private static void OnZIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = d as UIElement;
            if (uiElement != null)
            {
                Canvas parent = LogicalTreeHelper.GetParent(uiElement) as Canvas;
                if (parent != null)
                {
                    parent.Children.Sort((x, y) => ((int)x.GetValue(Canvas.ZIndexProperty)).CompareTo((int)y.GetValue(Canvas.ZIndexProperty)));
                    parent.NativeReorderChildren();
                    parent.ChildLayoutUpdated(uiElement, EventArgs.Empty);
                }
            }
        }

        private void ArrangeChild(UIElement child)
        {
            if (child.NativeUIElement != null)
            {
                SizeF needSize = new SizeF();
                if (this.childSizeCache.ContainsKey(child))
                {
                    needSize = this.childSizeCache[child];
                }
                else
                {
                    needSize = child.MeasureOverride(new SizeF(nfloat.PositiveInfinity, nfloat.PositiveInfinity));
                    this.childSizeCache[child] = needSize;
                }

                child.Arrange(new RectangleF((nfloat)Canvas.GetLeft(child), (nfloat)Canvas.GetTop(child), needSize.Width, needSize.Height));
            }
        }

        private void ChildLayoutUpdated(object sender, EventArgs e)
        {
            this.childSizeCache.Remove((UIElement)sender);
            this.ArrangeChild((UIElement)sender);
        }

        #endregion
    }
}