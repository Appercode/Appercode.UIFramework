using Android.Support.V4.View;
using Android.Views;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedViewPager : ViewPager
    {
        private readonly UIElement owner;
        private readonly TapDetector tapDetector;

        public WrappedViewPager(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.tapDetector = new TapDetector(owner);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            this.tapDetector.Detect(e);
            return base.OnTouchEvent(e);
        }

        protected override void JavaFinalize()
        {
            this.owner.FreeNativeView(this);
            base.JavaFinalize();
        }
    }
}