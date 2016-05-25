using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Device;
using System.Drawing;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class ScrollViewer
    {
        protected ScrollBarVisibility NativeHorizontalScrollBarVisibility
        {
            get
            {
                return this.HorizontalScrollBarVisibility;
            }
            set
            {
            }
        }

        protected ScrollBarVisibility NativeVerticalScrollBarVisibility
        {
            get
            {
                return this.VerticalScrollBarVisibility;
            }
            set
            {
            }
        }

        protected override View CreateDefaultControl(string value)
        {
            var innerDefaultControl = new NativeScrollViewer(this)
            {
                LayoutParameters = this.CreateLayoutParams()
            };
            innerDefaultControl.ScrollChanged += innerDefaultControl_ScrollChanged;
            if (this.ContainsValue(BackgroundProperty))
            {
                innerDefaultControl.Background = this.Background.ToDrawable();
            }

            var textView = new TextView(this.Context)
            {
                LayoutParameters = this.CreateLayoutParams(),
                Text = value
            };
            textView.SetSingleLine(true);
            innerDefaultControl.ChildView = textView;
            this.ContentNativeUIElement = innerDefaultControl;
            return innerDefaultControl;
        }

        protected override View CreateLayoutControl(UIElement value)
        {
            this.AddLogicalChild(value);
            var innerLayoutControl = new NativeScrollViewer(this);
            innerLayoutControl.ScrollChanged += innerDefaultControl_ScrollChanged;
            innerLayoutControl.LayoutParameters = this.CreateLayoutParams();
            innerLayoutControl.ChildView = value.NativeUIElement;
            this.SetNativeBackground(this.Background);
            this.ContentNativeUIElement = innerLayoutControl;
            return this.ContentNativeUIElement;
        }

        protected override void OnBackgroundChanged(Brush oldValue, Brush newValue)
        {
           this.SetNativeBackground(newValue);
        }

        protected override void ApplyNativeContentForDefaultControl(string value)
        {
            ((TextView)((NativeScrollViewer)this.ContentNativeUIElement).ChildView).Text = value;
        }

        protected override void NativeArrangeContent(RectangleF rectangleF)
        {
            if (this.Content is UIElement)
            {
                ((UIElement)this.Content).Arrange(new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height));
            }
            else
            {
                ((View)this.Content).Layout((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
            }
        }

        protected override void NativeArrange(RectangleF finalRect)
        {
            base.NativeArrange(finalRect);

            int newWidth = View.MeasureSpec.MakeMeasureSpec((int)finalRect.Width, MeasureSpecMode.Exactly);
            int newHeight = View.MeasureSpec.MakeMeasureSpec((int)finalRect.Height, MeasureSpecMode.Exactly);
            this.ContentNativeUIElement.Measure(newWidth, newHeight);

            this.ContentNativeUIElement.Layout(0,
                                               0,
                                               (int)ScreenProperties.ConvertDPIToPixels(finalRect.Width),
                                               (int)ScreenProperties.ConvertDPIToPixels(finalRect.Height));

            this.NativeUpdateLayout();
        }

        private void NativeUpdateLayout()
        {
            ((NativeScrollViewer)this.ContentNativeUIElement).UpdateChildrenWhoCanScroll();
        }

        private void innerDefaultControl_ScrollChanged(object sender, NativeScrollChangedEventArgs e)
        {
            ScrollChangedEventArgs args = new ScrollChangedEventArgs()
            {
                HorizontalOffset = ScreenProperties.ConvertPixelsToDPI(e.l),
                HorizontalChange = ScreenProperties.ConvertPixelsToDPI(e.l - e.oldl),
                VerticalOffset = ScreenProperties.ConvertPixelsToDPI(e.t),
                VerticalChange = ScreenProperties.ConvertPixelsToDPI(e.t - e.oldt),
            };
            this.OnScrollChanged(args);
        }

        private void SetContentScrolableSize(SizeF sizeF)
        {
            ((NativeScrollViewer)this.ContentNativeUIElement).ContentWidth = (int)ScreenProperties.ConvertDPIToPixels(sizeF.Width+(float)(Margin.Left+Margin.Right));
            ((NativeScrollViewer)this.ContentNativeUIElement).ContentHeight = (int)ScreenProperties.ConvertDPIToPixels(sizeF.Height + (float)(Margin.Top + Margin.Bottom));
        }

        private SizeF MeasureContent(SizeF sizeF)
        {
            if (this.Content is UIElement)
            {
                var measuredSize = ((UIElement)this.Content).MeasureOverride(new SizeF(sizeF.Width, sizeF.Height));
                return measuredSize;
            }
            else
            {
                var absoluteSizeF = new SizeF(
                    ScreenProperties.ConvertDPIToPixels(sizeF.Width),
                    ScreenProperties.ConvertDPIToPixels(sizeF.Height));

                this.ContentNativeUIElement.Measure(
                    View.MeasureSpec.MakeMeasureSpec((int)absoluteSizeF.Width, MeasureSpecMode.AtMost),
                    View.MeasureSpec.MakeMeasureSpec((int)absoluteSizeF.Height, MeasureSpecMode.AtMost));

                SizeF dpiMeasuredContentSize = new SizeF();
                dpiMeasuredContentSize.Width = ScreenProperties.ConvertPixelsToDPI(this.ContentNativeUIElement.MeasuredWidth);
                dpiMeasuredContentSize.Height = ScreenProperties.ConvertPixelsToDPI(this.ContentNativeUIElement.MeasuredHeight);

                return dpiMeasuredContentSize;
            }
        }

        private void NativeScrollToHorizontalOffset(double offset)
        {
            if (this.ContentNativeUIElement != null)
            {
                ((NativeScrollViewer)this.ContentNativeUIElement).ScrollToHorizontalOffset((int)ScreenProperties.ConvertDPIToPixels((float)offset));
            }
        }

        private void NativeScrollToVerticalOffset(double offset)
        {
            if (this.ContentNativeUIElement != null)
            {
                ((NativeScrollViewer)this.ContentNativeUIElement).ScrollToVerticalOffset((int)ScreenProperties.ConvertDPIToPixels((float)offset));
            }
        }

        private void NativeMoveToVerticalOffset(double offset)
        {
            if (this.ContentNativeUIElement != null)
            {
                ((NativeScrollViewer)this.ContentNativeUIElement).MoveToVerticalOffset((int)ScreenProperties.ConvertDPIToPixels((float)offset));
            }
        }
    }
}