using Android.App;
using Android.Content;
using Android.Views;
using Appercode.UI.Device;
using System;
using System.Drawing;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class UIElement
    {
        private Visibility nativeVisibility;
        public static Activity StaticContext;

        public virtual View NativeUIElement { get; protected internal set; }

        public Context Context => StaticContext;

        internal virtual bool IsFocused => false;

        internal virtual bool ShouldInterceptTouchEvent => false;

        protected Visibility NativeVisibility
        {
            get
            {
                return this.nativeVisibility;
            }
            set
            {
                this.nativeVisibility = value;

                if (this.NativeUIElement != null)
                {
                    if (value == Visibility.Collapsed)
                    {
                        this.NativeUIElement.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        this.NativeUIElement.Visibility = ViewStates.Visible;
                    }
                }
            }
        }

        internal virtual ViewGroup.LayoutParams GetDefaultLayoutParameters()
        {
            return new ViewGroup.LayoutParams(0, 0);
        }

        internal virtual void OnTap()
        {
            RaiseTap();
        }

        internal virtual void OnNativeClick() { }

        internal void FreeNativeView(View nativeView)
        {
            if (this.NativeUIElement == nativeView)
            {
                this.NativeUIElement = null;
            }
        }

        protected virtual SizeF NativeMeasureOverride(SizeF availableSize)
        {
            var absoluteAvailableSize = new SizeF(
                ScreenProperties.ConvertDPIToPixels(availableSize.Width),
                ScreenProperties.ConvertDPIToPixels(availableSize.Height));
            var margin = this.Margin;

            var absoluteMargin = new Thickness(
                (int)ScreenProperties.ConvertDPIToPixels(margin.LeftF()),
                (int)ScreenProperties.ConvertDPIToPixels(margin.TopF()),
                (int)ScreenProperties.ConvertDPIToPixels(margin.RightF()),
                (int)ScreenProperties.ConvertDPIToPixels(margin.BottomF()));

            int availableWidth;
            int availableHeight;
            // TODO: Unclear behavior. Incorrect on API 18
            if (double.IsPositiveInfinity(absoluteAvailableSize.Width))
            {
                availableWidth = int.MaxValue;
            }
            else
            {
                availableWidth = (int)(absoluteAvailableSize.Width - absoluteMargin.HorizontalThicknessF());
            }
            if (availableWidth < 0)
            {
                availableWidth = 0;
            }

            if (double.IsPositiveInfinity(absoluteAvailableSize.Height))
            {
                availableHeight = int.MaxValue;
            }
            else
            {
                availableHeight = (int)(absoluteAvailableSize.Height - absoluteMargin.VerticalThicknessF());
            }
            if (availableHeight < 0)
            {
                availableHeight = 0;
            }

            int measuredWidth;
            int measuredHeight;
            switch (this.NativeUIElement.LayoutParameters.Width)
            {
                case ViewGroup.LayoutParams.WrapContent:
                    measuredWidth = View.MeasureSpec.MakeMeasureSpec(availableWidth, MeasureSpecMode.AtMost);
                    break;
                case ViewGroup.LayoutParams.MatchParent:
                    measuredWidth = View.MeasureSpec.MakeMeasureSpec(availableWidth, MeasureSpecMode.Exactly);
                    break;
                default:
                    measuredWidth = View.MeasureSpec.MakeMeasureSpec(availableWidth, MeasureSpecMode.Exactly);
                    break;
            }
            switch (this.NativeUIElement.LayoutParameters.Height)
            {
                case ViewGroup.LayoutParams.WrapContent:
                    measuredHeight = View.MeasureSpec.MakeMeasureSpec(availableHeight, MeasureSpecMode.AtMost);
                    break;
                case ViewGroup.LayoutParams.MatchParent:
                    measuredHeight = View.MeasureSpec.MakeMeasureSpec(availableHeight, MeasureSpecMode.Exactly);
                    break;
                default:
                    measuredHeight = View.MeasureSpec.MakeMeasureSpec(availableHeight, MeasureSpecMode.Exactly);
                    break;
            }

            this.NativeUIElement.Measure(measuredWidth, measuredHeight);

            var absoluteMeasuredSize = new SizeF(
                this.NativeUIElement.MeasuredWidth + absoluteMargin.HorizontalThicknessF(),
                this.NativeUIElement.MeasuredHeight + absoluteMargin.VerticalThicknessF());

            var dpiMeasuredSize = new SizeF(
                Math.Min(ScreenProperties.ConvertPixelsToDPI(absoluteMeasuredSize.Width), availableSize.Width),
                Math.Min(ScreenProperties.ConvertPixelsToDPI(absoluteMeasuredSize.Height), availableSize.Height));

            return dpiMeasuredSize;
        }

        protected virtual void NativeArrange(RectangleF finalRect)
        {
            var margin = this.Margin;
            var newFinalRect = new RectangleF(
                finalRect.X + margin.LeftF(),
                finalRect.Y + margin.TopF(),
                Math.Max(0, finalRect.Width - margin.HorizontalThicknessF()),
                Math.Max(0, finalRect.Height - margin.VerticalThicknessF()));

            var absoluteNewFinalRect = new RectangleF(
                ScreenProperties.ConvertDPIToPixels(newFinalRect.X),
                ScreenProperties.ConvertDPIToPixels(newFinalRect.Y),
                ScreenProperties.ConvertDPIToPixels(newFinalRect.Width),
                ScreenProperties.ConvertDPIToPixels(newFinalRect.Height));

            //int newWidth = Android.Views.View.MeasureSpec.MakeMeasureSpec((int)(absoluteNewFinalRect.Right - absoluteNewFinalRect.Left), MeasureSpecMode.Exactly);
            //int newHeight = Android.Views.View.MeasureSpec.MakeMeasureSpec((int)(absoluteNewFinalRect.Bottom - absoluteNewFinalRect.Top), MeasureSpecMode.Exactly);
            //this.NativeUIElement.Measure(newWidth, newHeight);

            this.NativeUIElement.Layout((int)absoluteNewFinalRect.Left,
                                        (int)absoluteNewFinalRect.Top,
                                        (int)absoluteNewFinalRect.Right,
                                        (int)absoluteNewFinalRect.Bottom);
        }

        protected internal virtual void NativeInit()
        {
            this.ApplySize(this.Width, this.Height);
            this.NativeVisibility = this.Visibility;
        }

        partial void ApplySize(double? width, double? height)
        {
            if (this.NativeUIElement != null)
            {
                var oldParams = this.NativeUIElement.LayoutParameters;
                var isChanged = oldParams == null;
                var currentWidth = default(int);
                var currentHeight = default(int);
                if (oldParams != null)
                {
                    currentWidth = oldParams.Width;
                    currentHeight = oldParams.Height;
                }

                if (width.HasValue)
                {
                    var newWidth = double.IsNaN(width.Value)
                        ? ViewGroup.LayoutParams.WrapContent
                        : (int)ScreenProperties.ConvertDPIToPixels((float)width);
                    if (currentWidth != newWidth)
                    {
                        currentWidth = newWidth;
                        isChanged = true;
                    }
                }

                if (height.HasValue)
                {
                    var newHeight = double.IsNaN(height.Value)
                       ? ViewGroup.LayoutParams.WrapContent
                       : (int)ScreenProperties.ConvertDPIToPixels((float)height);
                    if (currentHeight != newHeight)
                    {
                        currentHeight = newHeight;
                        isChanged = true;
                    }
                }

                if (isChanged)
                {
                    var newParams = this.GetDefaultLayoutParameters();
                    newParams.Width = currentWidth;
                    newParams.Height = currentHeight;
                    this.NativeUIElement.LayoutParameters = newParams;
                }
            }
        }
    }
}