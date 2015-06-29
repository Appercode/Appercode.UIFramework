using System.Windows.Media;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.Media.Imaging;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Device;
using System.Drawing;

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

        //protected Visibility NativeComputedHorizontalScrollBarVisibility
        //{
        //    get
        //    {
        //        return ((NativeScrollViewer)this.ContentNativeUIElement).VerticalScrollBarEnabled;
        //    }
        //    set
        //    {
        //    }
        //}

        //protected Visibility NativeComputedVerticalScrollBarVisibility
        //{
        //    get
        //    {
        //        return ((NativeScrollViewer)this.ContentNativeUIElement).VerticalScrollBarEnabled;
        //    }
        //    set
        //    {
        //    }
        //}

        protected override View CreateDefaultControl(string value)
        {
            var innerDefaultControl = new NativeScrollViewer(this.Context);
            innerDefaultControl.ScrollChanged += innerDefaultControl_ScrollChanged;
            innerDefaultControl.LayoutParameters = this.CreateLayoutParams();
            if (this.Background != null)
                innerDefaultControl.SetBackgroundDrawable(this.Background.ToDrawable());
            var text = new Android.Widget.TextView(this.Context);
            text.LayoutParameters = this.CreateLayoutParams();
            text.Text = value;
            text.SetSingleLine(true);
            innerDefaultControl.ChildView = text;
            this.ContentNativeUIElement = innerDefaultControl;
            return innerDefaultControl;
        }

        protected override View/*Group*/ CreateLayoutControl(UIElement value)
        {
            LogicalTreeHelper.AddLogicalChild(this, value);
            var innerLayoutControl = new NativeScrollViewer(this.Context);
            innerLayoutControl.ScrollChanged += innerDefaultControl_ScrollChanged;
            innerLayoutControl.LayoutParameters = this.CreateLayoutParams();
            innerLayoutControl.ChildView = value.NativeUIElement;
            SetBackground();
            this.ContentNativeUIElement = innerLayoutControl;

            return this.ContentNativeUIElement;
        }

        protected override void OnBackgroundChanged()
        {
            SetBackground();
        }

        private void SetBackground()
        {
            if (this.Background != null && this.NativeUIElement != null)
            {
                if (IsBackgroundValidImageBrush())
                {
                    ((BitmapImage)(((ImageBrush)this.Background).ImageSource)).ImageOpened += (s, e) =>
                    {
                        if (IsBackgroundValidImageBrush())
                        {
                            this.NativeUIElement.Post(() =>
                            {
                                this.NativeUIElement.SetBackgroundDrawable(this.Background.ToDrawable());
                                this.OnLayoutUpdated();
                            });
                        }
                    };
                }
                else
                    this.NativeUIElement.SetBackgroundDrawable(this.Background.ToDrawable());
            }
        }

        private bool IsBackgroundValidImageBrush()
        {
            return this.Background is ImageBrush
                   && ((ImageBrush)this.Background).ImageSource is BitmapImage
                   && ((BitmapImage)(((ImageBrush)this.Background).ImageSource)).UriSource.IsAbsoluteUri;
        }

        protected override void ApplyNativeContentForDefaultControl(string value)
        {
            ((TextView)((NativeScrollViewer)this.ContentNativeUIElement).ChildView).Text = value;
        }

        protected override void NativeArrangeContent(System.Drawing.RectangleF rectangleF)
        {
            if (this.Content is UIElement)
            {
                ((UIElement)this.Content).Arrange(new System.Drawing.RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height));
            }
            else
            {
                ((View)this.Content).Layout((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
            }
        }

        protected override void NativeArrange(System.Drawing.RectangleF finalRect)
        {
            base.NativeArrange(finalRect);

            int newWidth = Android.Views.View.MeasureSpec.MakeMeasureSpec((int)finalRect.Width, MeasureSpecMode.Exactly);
            int newHeight = Android.Views.View.MeasureSpec.MakeMeasureSpec((int)finalRect.Height, MeasureSpecMode.Exactly);
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

        private void SetContentScrolableSize(System.Drawing.SizeF sizeF)
        {
            ((NativeScrollViewer)this.ContentNativeUIElement).ContentWidth = (int)ScreenProperties.ConvertDPIToPixels(sizeF.Width+(float)(Margin.Left+Margin.Right));
            ((NativeScrollViewer)this.ContentNativeUIElement).ContentHeight = (int)ScreenProperties.ConvertDPIToPixels(sizeF.Height + (float)(Margin.Top + Margin.Bottom));
        }

        private System.Drawing.SizeF MeasureContent(System.Drawing.SizeF sizeF)
        {
            if (this.Content is UIElement)
            {
                System.Drawing.SizeF measuredSize = ((UIElement)this.Content).MeasureOverride(new System.Drawing.SizeF(sizeF.Width, sizeF.Height));
                return measuredSize;
            }
            else
            {
                System.Drawing.SizeF absoluteSizeF = new SizeF();
                absoluteSizeF.Width = ScreenProperties.ConvertDPIToPixels(sizeF.Width);
                absoluteSizeF.Height = ScreenProperties.ConvertDPIToPixels(sizeF.Height);

                ((Android.Views.View)this.ContentNativeUIElement).Measure(Android.Views.View.MeasureSpec.MakeMeasureSpec((int)absoluteSizeF.Width, MeasureSpecMode.AtMost),
                                                                          Android.Views.View.MeasureSpec.MakeMeasureSpec((int)absoluteSizeF.Height, MeasureSpecMode.AtMost));

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