using Android.Views;
using System;

namespace Appercode.UI.Controls.NativeControl
{
    internal class TapDetector
    {
        private readonly static int tapTimeout = ViewConfiguration.TapTimeout;
        private readonly float slop;
        private readonly UIElement owner;
        private float startX, startY;
        private bool isTapPossible;

        public TapDetector(UIElement owner)
        {
            this.owner = owner;
            this.slop = ViewConfiguration.Get(owner.Context).ScaledTouchSlop;
        }

        public void Detect(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    this.startX = e.GetX();
                    this.startY = e.GetY();
                    this.isTapPossible = true;
                    break;
                case MotionEventActions.Move:
                    if (this.isTapPossible)
                    {
                        if (Math.Abs(e.GetX() - this.startX) > this.slop || Math.Abs(e.GetY() - this.startY) > this.slop)
                        {
                            this.isTapPossible = false;
                        }
                    }

                    break;
                case MotionEventActions.Up:
                    if (this.isTapPossible && e.EventTime - e.DownTime < tapTimeout)
                    {
                        this.owner.OnTap();
                    }

                    if (this.PointInView(e.GetX(), e.GetY()))
                    {
                        this.owner.OnNativeClick();
                    }

                    break;
            }
        }

        private bool PointInView(float localX, float localY)
        {
            var nativeElement = this.owner.NativeUIElement;
            return localX >= -this.slop
                && localY >= -this.slop
                && localX < (nativeElement.Right - nativeElement.Left + this.slop)
                && localY < (nativeElement.Bottom - nativeElement.Top + this.slop);
        }
    }
}