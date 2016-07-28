using Android.Views;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedViewGroup : ViewGroup
    {
        private static LayoutParams layoutParams;
        private readonly UIElement owner;
        private readonly TapDetector tapDetector;

        public WrappedViewGroup(UIElement owner)
            : base(owner.Context)
        {
            this.Clickable = true;
            this.owner = owner;
            this.tapDetector = new TapDetector(owner);
        }

        public static bool FillNativeUIElement(UIElement owner)
        {
            if (owner.Parent != null && owner.Context != null && owner.NativeUIElement == null)
            {
                owner.NativeUIElement = new WrappedViewGroup(owner)
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
            this.tapDetector.Detect(e);
            return base.OnTouchEvent(e);
        }

        public void AddViewInLayoutOverride(View view)
        {
            this.AddViewInLayout(view, -1, layoutParams ?? (layoutParams = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)), true);
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            return this.owner.ShouldInterceptTouchEvent;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }

        protected override void JavaFinalize()
        {
            this.owner.FreeNativeView(this);
            base.JavaFinalize();
        }
    }
}