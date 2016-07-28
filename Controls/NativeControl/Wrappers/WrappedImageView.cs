using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedImageView : ImageView, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedImageView(UIElement owner)
            : base(owner.Context)
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