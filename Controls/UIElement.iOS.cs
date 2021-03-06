using CoreGraphics;
using Foundation;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class UIElement
    {
        private double nativeWidth = double.NaN;
        private double nativeHeight = double.NaN;
        private UIView nativeUIElement;
        private UIGestureRecognizer touchGestureRecognizer;

        /// <summary>
        /// Gets or sets the native user interface element.
        /// </summary>
        /// <value>The native user interface element.</value>
        public virtual UIView NativeUIElement
        {
            get { return this.nativeUIElement; }
            protected internal set
            {
                if (this.nativeUIElement != value)
                {
                    if (this.nativeUIElement != null && this.touchGestureRecognizer != null)
                    {
                        this.nativeUIElement.RemoveGestureRecognizer(this.touchGestureRecognizer);
                    }

                    this.nativeUIElement = value;
                    if (this.touchGestureRecognizer != null)
                    {
                        this.nativeUIElement.AddGestureRecognizer(this.touchGestureRecognizer);
                    }
                }
            }
        }

        internal virtual bool ChildrenShouldForwardTouch
        {
            get { return false;}
        }

        internal virtual bool IsFocused
        {
            get
            {
                // NativeUIElement might be null even after initialization,
                // because the control is not presented by a descendant of UIView.
                return this.NativeUIElement != null && this.NativeUIElement.IsFirstResponder;
            }
        }

        protected double NativeWidth
        {
            get
            {
                return this.nativeWidth;
            }
            set
            {
                this.nativeWidth = value;
                if (this.NativeUIElement != null)
                {
                    this.NativeUIElement.InvokeOnMainThread(() =>
                    {
                        this.NativeUIElement.Frame = new CGRect(this.NativeUIElement.Frame.X, this.NativeUIElement.Frame.Y, double.IsNaN(value) ? 0f : value, this.NativeUIElement.Frame.Height);
                    });
                }
            }
        }

        protected double NativeHeight
        {
            get
            {
                return this.nativeHeight;
            }
            set
            {
                this.nativeHeight = value;
                if (this.NativeUIElement != null)
                {
                    this.NativeUIElement.InvokeOnMainThread(() =>
                    {
                        this.NativeUIElement.Frame = new CGRect(this.NativeUIElement.Frame.X, this.NativeUIElement.Frame.Y, this.NativeUIElement.Frame.Width, double.IsNaN(value) ? 0f : value);
                    });
                }
            }
        }

        protected Thickness NativeMargin
        {
            set
            {
            }
        }

        protected Visibility NativeVisibility
        {
            get
            {
                return this.NativeUIElement.Hidden ? Visibility.Collapsed : Visibility.Visible;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.NativeUIElement.Hidden = value != Visibility.Visible;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}", base.ToString(), this.NativeUIElement == null ? "(null)" : this.NativeUIElement.ToString());
        }

        internal virtual void OnTouchDown() { }

        internal virtual void OnTouchUp()
        {
            this.RaiseTap();
        }

        protected internal virtual void NativeInit()
        {
            this.NativeVisibility = this.Visibility;
            if (this.touchGestureRecognizer == null)
            {
                this.touchGestureRecognizer = new TouchRecognizer(this);
                this.NativeUIElement.AddGestureRecognizer(this.touchGestureRecognizer);
                this.NativeUIElement.UserInteractionEnabled = true;
            }
        }

        protected virtual CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var width = this.Width.GetNotNaN();
            var height = this.Height.GetNotNaN();

            var margin = this.Margin;
            width += margin.HorizontalThicknessF();
            height += margin.VerticalThicknessF();

            return new CGSize(width, height);
        }

        protected virtual void NativeArrange(CGRect finalRect)
        {
            var margin = this.Margin;
            this.NativeUIElement.Frame = new CGRect(
                finalRect.X + margin.LeftF(),
                finalRect.Y + margin.TopF(),
                MathF.Max(0, finalRect.Width - margin.HorizontalThicknessF()),
                MathF.Max(0, finalRect.Height - margin.VerticalThicknessF()));
        }

        private class TouchRecognizer : UIGestureRecognizer
        {
            private readonly UIElement owner;

            public TouchRecognizer(UIElement owner)
            {
                this.owner = owner;
                this.CancelsTouchesInView = false;
                this.ShouldReceiveTouch = this.ShouldRecieveTouchHandler;
            }

            public override void TouchesBegan(NSSet touches, UIEvent evt)
            {
                base.TouchesBegan(touches, evt);
                var childrenShouldForwardTouch = this.owner.ChildrenShouldForwardTouch;
                foreach (UITouch touch in touches)
                {
                    if (childrenShouldForwardTouch || touch.View == this.owner.NativeUIElement)
                    {
                        this.owner.OnTouchDown();
                    }
                }

                this.State = UIGestureRecognizerState.Possible;
            }

            public override void TouchesEnded(NSSet touches, UIEvent evt)
            {
                base.TouchesEnded(touches, evt);
                var childrenShouldForwardTouch = this.owner.ChildrenShouldForwardTouch;
                foreach (UITouch touch in touches)
                {
                    if (childrenShouldForwardTouch || touch.View == this.owner.NativeUIElement)
                    {
                        this.owner.OnTouchUp();
                    }
                }

                this.State = UIGestureRecognizerState.Failed;
            }

            private bool ShouldRecieveTouchHandler(UIGestureRecognizer recognizer, UITouch touch)
            {
                if (touch.View != this.owner.NativeUIElement && this.owner.ChildrenShouldForwardTouch == false)
                {
                    return false;
                }

                var parent = this.owner.Parent;
                while (parent != null)
                {
                    if (parent.ChildrenShouldForwardTouch)
                    {
                        return false;
                    }

                    parent = parent.Parent;
                }

                return true;
            }
        }
    }
}