using Android.App;
using Android.Content;
using Android.Views;
using Appercode.UI.Controls.NativeControl;
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

        internal virtual void OnTap()
        {
            RaiseTap();
        }

        internal virtual void OnNativeClick() { }

        protected internal virtual double NativeWidth
        {
            get
            {
                return this.Width;
            }
            protected set
            {
                if (this.NativeUIElement != null)
                {
                    var oldParams = this.NativeUIElement.LayoutParameters ?? new ViewGroup.LayoutParams(0, 0);
                    oldParams.Width = double.IsNaN(value) ? ViewGroup.LayoutParams.WrapContent : (int)ScreenProperties.ConvertDPIToPixels((float)value);
                    this.NativeUIElement.LayoutParameters = new ViewGroup.LayoutParams(oldParams);
                }
            }
        }

        protected internal virtual double NativeHeight
        {
            get
            {
                return this.Height;
            }
            protected set
            {
                if (this.NativeUIElement != null)
                {
                    var oldParams = this.NativeUIElement.LayoutParameters ?? new ViewGroup.LayoutParams(0, 0);
                    oldParams.Height = double.IsNaN(value) ? ViewGroup.LayoutParams.WrapContent : (int)ScreenProperties.ConvertDPIToPixels((float)value);
                    this.NativeUIElement.LayoutParameters = new ViewGroup.LayoutParams(oldParams);
                }
            }
        }

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
            this.NativeWidth = this.Width;
            this.NativeHeight = this.Height;
            this.NativeVisibility = this.Visibility;

            if (this.NativeUIElement != null)
            {
                ((IJavaFinalizable)this.NativeUIElement).JavaFinalized -= this.UIElement_JavaFinalized;
                ((IJavaFinalizable)this.NativeUIElement).JavaFinalized += this.UIElement_JavaFinalized;
            }
        }

        protected void NativeUIElement_Touch(object sender, View.TouchEventArgs e)
        {
            e.Handled = false;
        }

        private void UIElement_JavaFinalized(object sender, EventArgs e)
        {
            if (this.NativeUIElement != null)
            {
                ((IJavaFinalizable)this.NativeUIElement).JavaFinalized -= this.UIElement_JavaFinalized;
                this.NativeUIElement = null;
            }
        }
    }
}