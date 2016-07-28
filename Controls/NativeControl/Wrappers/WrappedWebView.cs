using Android.Views;
using Android.Webkit;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedWebView : WebView
    {
        private readonly UIElement owner;
        private readonly TapDetector tapDetector;

        public WrappedWebView(UIElement owner)
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