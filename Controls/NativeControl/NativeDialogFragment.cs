using Android.App;
using Android.OS;
using Android.Views;

namespace Appercode.UI.Controls.Native
{
    internal class NativeDialogFragment : DialogFragment
    {
        private readonly AppercodePage page;

        public NativeDialogFragment(AppercodePage page)
        {
            this.page = page;
            this.Cancelable = false;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.Dialog.Window.DecorView.LayoutChange += this.OnLayoutChanged;
            return this.page.NativeUIElement;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            this.page.OnCreate(savedInstanceState);
            this.SetStyle(DialogFragmentStyle.NoTitle, 0);
            base.OnCreate(savedInstanceState);
        }

        public override void Dismiss()
        {
            this.Dialog.Window.DecorView.LayoutChange -= this.OnLayoutChanged;
            base.Dismiss();
        }

        public override void OnStop()
        {
            this.page.HideKeyboard();
            base.OnStop();
        }

        private void OnLayoutChanged(object sender, View.LayoutChangeEventArgs e)
        {
            var width = e.Right - e.Left;
            var height = e.Bottom - e.Top;
            if (width != e.OldRight - e.OldLeft
                || height != e.OldBottom - e.OldTop)
            {
                this.page.ApplyPageSize(width, height);
            }
        }
    }
}