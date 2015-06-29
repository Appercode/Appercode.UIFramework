using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Windows.Input;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    internal class TapDetector
    {
        private static int tapTimeout = ViewConfiguration.TapTimeout;
        private float slop;
        private View owner;
        private float startX, startY;
        private bool tapPossiable;

        public TapDetector(View owner)
        {
            this.owner = owner;
            this.slop = ViewConfiguration.Get(this.owner.Context).ScaledTouchSlop;
        }

        public bool Detect(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    this.startX = e.GetX();
                    this.startY = e.GetY();
                    this.tapPossiable = true;
                    break;
                case MotionEventActions.Move:
                    if (this.tapPossiable)
                    {
                        if (Math.Abs(e.GetX() - this.startX) > this.slop || Math.Abs(e.GetY() - this.startY) > this.slop)
                        {
                            this.tapPossiable = false;
                        }
                    }
                    break;
                case MotionEventActions.Up:
                    if (this.tapPossiable && e.EventTime - e.DownTime < tapTimeout)
                    {
                        ((ITapableView)this.owner).WrapedNativeRaiseTap();
                    }
                    break;
            }
            return false;
        }
    }
}