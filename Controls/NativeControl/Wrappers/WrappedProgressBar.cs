using Android.Util;
using Android.Views;
using System;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedProgressBar : Android.Widget.ProgressBar, IJavaFinalizable, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedProgressBar(UIElement owner, IAttributeSet attrs, int defStyle)
            : base(owner.Context, attrs, defStyle)
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