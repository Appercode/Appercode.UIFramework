using Android.Views;
using Android.Widget;
using System;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedListView : ListView, IJavaFinalizable, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedListView(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.DividerHeight = 0;
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