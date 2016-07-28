using Android.Util;
using Android.Views;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedProgressBar : Android.Widget.ProgressBar, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedProgressBar(UIElement owner, IAttributeSet attrs, int defStyle)
            : base(owner.Context, attrs, defStyle)
        {
            this.owner = owner;
            this.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            this.owner.OnTap();
        }

        protected override void JavaFinalize()
        {
            this.owner.FreeNativeView(this);
            base.JavaFinalize();
        }
    }
}