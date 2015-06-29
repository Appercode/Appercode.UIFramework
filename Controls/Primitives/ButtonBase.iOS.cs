using Foundation;
using System;
using System.Linq;
using UIKit;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ButtonBase
    {
        private ClickRecognizer clickGestureRecognizer;

        protected bool NativeIsPressed
        {
            set { }
        }

        protected Controls.ClickMode NativeClickMode
        {
            set { }
        }

        protected internal override void NativeInit()
        {
            this.SubscribeToClicks();
            base.NativeInit();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            var element = newContent as UIElement;
            if (element != null)
            {
                element.NativeUIElement.UserInteractionEnabled = false;
            }
        }

        protected override void AddControlTemplateInstance()
        {
            base.AddControlTemplateInstance();
            this.SubscribeToClicks();
        }

        private void SubscribeToClicks()
        {
            if (this.clickGestureRecognizer != null && this.NativeUIElement.GestureRecognizers != null && this.NativeUIElement.GestureRecognizers.Contains(this.clickGestureRecognizer))
            {
                return;
            }
            this.clickGestureRecognizer = new ClickRecognizer(() =>
            {
                this.OnClick();
            }, this);
            this.clickGestureRecognizer.CancelsTouchesInView = false;
            this.NativeUIElement.AddGestureRecognizer(this.clickGestureRecognizer);
        }

        private void HandleTouchDown(object sender, EventArgs e)
        {
            if (this.IsEnabled && this.ClickMode == ClickMode.Press)
            {
                this.OnClick();
            }
        }

        private void HandleTouchUpInside(object sender, EventArgs e)
        {
            if (this.IsEnabled && this.ClickMode == ClickMode.Release)
            {
                this.OnClick();
            }
        }

        private class ClickRecognizer : UIGestureRecognizer
        {
            public ClickRecognizer(Action clickAction, ButtonBase owner)
            {
                this.ClickAction = clickAction;
                this.Owner = owner;
            }

            public Action ClickAction
            {
                get;
                set;
            }

            public UIView OvnerNativeView
            {
                get
                {
                    return this.Owner.NativeUIElement;
                }
            }

            public ButtonBase Owner
            {
                get;
                set;
            }

            public override void TouchesBegan(NSSet touches, UIEvent evt)
            {
                base.TouchesBegan(touches, evt);
                if (this.Owner.ClickMode == ClickMode.Press)
                {
                    this.State = UIGestureRecognizerState.Failed;
                    this.ClickActionIfInView(touches);
                }
                else
                {
                    this.State = UIGestureRecognizerState.Possible;
                }
            }

            public override void TouchesEnded(NSSet touches, UIEvent evt)
            {
                base.TouchesEnded(touches, evt);
                if (this.Owner.ClickMode == ClickMode.Release)
                {
                    this.ClickActionIfInView(touches);
                }
                this.State = UIGestureRecognizerState.Failed;
            }

            private void ClickActionIfInView(NSSet touches)
            {
                foreach (UITouch t in touches)
                {
                    var l = t.LocationInView(t.View);
                    if (t.View != null && l.X >= 0 && l.Y >= 0 && l.X <= t.View.Frame.Width && l.Y <= t.View.Frame.Height)
                    {
                        this.ClickAction.Invoke();
                    }
                }
            }
        }
    }
}