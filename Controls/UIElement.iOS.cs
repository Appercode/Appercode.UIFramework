using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class UIElement
    {
        private double nativeWidth = double.NaN;
        private double nativeHeight = double.NaN;
        private TapRecognizer tapGestureRecognizer;

        /// <summary>
        /// Gets or sets the native user interface element.
        /// </summary>
        /// <value>The native user interface element.</value>
        public virtual UIView NativeUIElement
        {
            get;
            protected internal set;
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

        protected internal virtual void NativeInit()
        {
            this.NativeVisibility = this.Visibility;
            if (this.tapGestureRecognizer != null && this.NativeUIElement.GestureRecognizers != null && this.NativeUIElement.GestureRecognizers.Contains(this.tapGestureRecognizer))
            {
                return;
            }

            WeakReference wr = new WeakReference(this);
            this.tapGestureRecognizer = new TapRecognizer(() =>
            {
                if (wr.IsAlive)
                {
                    ((UIElement)wr.Target).RaiseTap();
                }
            }, this.NativeUIElement);
            this.tapGestureRecognizer.CancelsTouchesInView = false;
            this.NativeUIElement.AddGestureRecognizer(this.tapGestureRecognizer);
            this.NativeUIElement.UserInteractionEnabled = true;
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

        private class TapRecognizer : UIGestureRecognizer
        {
            public TapRecognizer(Action tapAction, UIView ownerNativeView)
            {
                this.TapAction = tapAction;
                this.OvnerNativeView = ownerNativeView;
            }

            public Action TapAction
            {
                get;
                set;
            }

            public UIView OvnerNativeView
            {
                get;
                set;
            }

            public override void TouchesBegan(NSSet touches, UIEvent evt)
            {
                base.TouchesBegan(touches, evt);
                this.State = UIGestureRecognizerState.Possible;
            }

            public override void TouchesEnded(NSSet touches, UIEvent evt)
            {
                base.TouchesEnded(touches, evt);
                foreach (UITouch t in touches)
                {
                    var l = t.LocationInView(t.View);
                    if (t.View != null && t.View == this.OvnerNativeView && l.X >= 0 && l.Y >= 0 && l.X <= t.View.Frame.Width && l.Y <= t.View.Frame.Height)
                    {
                        this.TapAction.Invoke();
                    }
                }
                this.State = UIGestureRecognizerState.Failed;
            }
        }
    }
}