using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedTextView : TextView, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedTextView(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.SetIncludeFontPadding(false);
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