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
            return this.page.NativeUIElement;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            this.page.OnCreate(savedInstanceState);
            this.SetStyle(DialogFragmentStyle.NoTitle, 0);
            base.OnCreate(savedInstanceState);
        }

        public override void OnStop()
        {
            this.page.HideKeyboard();
            base.OnStop();
        }
    }
}