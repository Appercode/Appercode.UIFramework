using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ScrollViewer
    {
        private CGPoint lastContentOfset;

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

        protected internal override void NativeInit()
        {
            if (Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    var scroll = new UIScrollView();
                    scroll.BackgroundColor = UIColor.Clear;
                    this.NativeUIElement = scroll;
                    this.NativeUIElement.ClipsToBounds = true;
                    this.lastContentOfset = CGPoint.Empty;
                    scroll.Scrolled += (sender, e) => 
                    {
                        this.OnScrollChanged(new ScrollChangedEventArgs
                        { 
                            VerticalOffset = scroll.ContentOffset.Y,
                            HorizontalOffset = scroll.ContentOffset.X,
                            VerticalChange = scroll.ContentOffset.Y - this.lastContentOfset.Y,
                            HorizontalChange = scroll.ContentOffset.X - this.lastContentOfset.X,
                        });
                        this.lastContentOfset = scroll.ContentOffset;
                    };
                }
                base.NativeInit();
            }
        }

        protected override void NativeArrangeContent(CGRect finalRect)
        {
            if (this.Content is UIElement)
            {
                ((UIElement)this.Content).Arrange(finalRect);
            }
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null && this.Background != null)
            {
                this.NativeUIElement.BackgroundColor = this.Background.ToUIColor(this.RenderSize);
            }
        }

        /// <summary>
        /// Sets the size of the content in wich content wil be scrollable.
        /// It may be less than real size of content
        /// </summary>
        /// <param name="size">Size</param>
        private void SetContentScrolableSize(CGSize size)
        {
            ((UIScrollView)this.NativeUIElement).ContentSize = size;
        }

        private CGSize MeasureContent(CGSize availableSize)
        {
            var size = this.NativeMeasureContent(availableSize);
            return size;
        }

        private void NativeScrollToHorizontalOffset(double offset)
        {
            var scroll = (UIScrollView)this.NativeUIElement;
            scroll.SetContentOffset(new CGPoint(offset, scroll.ContentOffset.Y), true);
        }

        private void NativeScrollToVerticalOffset(double offset)
        {
            var scroll = (UIScrollView)this.NativeUIElement;
            scroll.SetContentOffset(new CGPoint(scroll.ContentOffset.X, offset), true);
        }

        private void NativeMoveToVerticalOffset(double offset)
        {
            var scroll = (UIScrollView)this.NativeUIElement;
            scroll.SetContentOffset(new CGPoint(scroll.ContentOffset.X, offset), false);
        }
    }
}