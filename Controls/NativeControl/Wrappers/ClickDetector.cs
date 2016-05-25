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

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    internal class ClickDetector
    {
        private View owner;

        public ClickDetector(View owner)
        {
            this.owner = owner;
        }

        public bool Detect(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Up && this.PointInView(e.GetX(), e.GetY()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PointInView(float localX, float localY)
        {
            float slop = ViewConfiguration.Get(this.owner.Context).ScaledTouchSlop;
            return localX >= -slop && localY >= -slop && localX < ((this.owner.Right - this.owner.Left) + slop) &&
                localY < ((this.owner.Bottom - this.owner.Top) + slop);
        }
    }
}