using Android.Views;
using System;
using ViewPagerIndicator;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedCirclePageIndicator : CirclePageIndicator, IJavaFinalizable
    {
        private readonly TapDetector tapDetector;

        public WrappedCirclePageIndicator(UIElement owner)
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