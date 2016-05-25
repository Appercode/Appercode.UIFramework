using Android.Views;
using Android.Webkit;
using System;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedWebView : WebView, IJavaFinalizable
    {
        private readonly TapDetector tapDetector;

        public WrappedWebView(UIElement owner)
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