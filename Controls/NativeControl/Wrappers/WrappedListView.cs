using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedListView : ListView, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedListView(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.DividerHeight = 0;
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