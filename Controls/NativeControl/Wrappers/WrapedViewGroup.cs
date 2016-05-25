using Android.Content;
using Android.Views;
using System;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    public class WrapedViewGroup : ViewGroup, ITapableView, IClickableView, IJavaFinalizable
    {
        private TapDetector tapDetector;
        private ClickDetector clickDetector;

        public WrapedViewGroup(Context context)
            : base(context)
        {
            this.Clickable = true;
            this.tapDetector = new TapDetector(this);
            this.clickDetector = new ClickDetector(this);
        }

        public event EventHandler NativeTap;
        public event EventHandler NativeClick;
        public event EventHandler JavaFinalized;

        public static bool FillNativeUIElement(UIElement owner)
        {
            if (owner.Parent != null && owner.Context != null && owner.NativeUIElement == null)
            {
                owner.NativeUIElement = new WrapedViewGroup(owner.Context)
                {
                    LayoutParameters = new LayoutParams(0, 0)
                    {
                        Width = double.IsNaN(owner.NativeWidth) ? LayoutParams.WrapContent : (int)owner.NativeWidth,
                        Height = double.IsNaN(owner.NativeHeight) ? LayoutParams.WrapContent : (int)owner.NativeHeight
                    }
                };
                return true;
            }

            return false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (clickDetector.Detect(e))
            {
                if (this.NativeClick != null)
                {
                    this.NativeClick(this, null);
                }
            }

            tapDetector.Detect(e);

            return base.OnTouchEvent(e);
        }

        private static LayoutParams layoutParams;
        public void AddViewInLayoutOverride(View view)
        {
            this.AddViewInLayout(view, -1, layoutParams ?? (layoutParams = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)), true);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (clickDetector.Detect(ev))
            {
                if (this.NativeClick != null)
                {
                    this.NativeClick(this, null);
                }
            }

            base.OnTouchEvent(ev);
            return base.OnInterceptTouchEvent(ev);
        }

        public void WrapedNativeRaiseTap()
        {
            if (this.NativeTap != null)
            {
                this.NativeTap(this, null);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }

        protected override void JavaFinalize()
        {
            if (this.JavaFinalized != null)
            {
                this.JavaFinalized(null, null);
            }
            base.JavaFinalize();
        }
    }
}