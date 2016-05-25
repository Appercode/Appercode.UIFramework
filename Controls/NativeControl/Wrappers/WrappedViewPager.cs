using Android.Support.V4.View;
using Android.Views;
using System;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedViewPager : ViewPager, IJavaFinalizable
    {
        private readonly TapDetector tapDetector;

        public WrappedViewPager(UIElement owner)
            : base(owner.Context)
        {
            this.tapDetector = new TapDetector(owner);
        }

        public event EventHandler JavaFinalized;

        public override bool OnTouchEvent(MotionEvent e)
        {
            this.tapDetector.Detect(e);
            return base.OnTouchEvent(e);
        }

        protected override void JavaFinalize()
        {
            this.JavaFinalized?.Invoke(null, null);
            base.JavaFinalize();
        }
    }
}