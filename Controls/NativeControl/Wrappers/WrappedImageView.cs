using Android.Views;
using Android.Widget;
using System;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedImageView : ImageView, IJavaFinalizable, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedImageView(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.SetOnClickListener(this);
        }

        public event EventHandler JavaFinalized;

        public void OnClick(View v)
        {
            this.owner.OnTap();
        }

        protected override void JavaFinalize()
        {
            this.JavaFinalized?.Invoke(null, null);
            base.JavaFinalize();
        }
    }
}