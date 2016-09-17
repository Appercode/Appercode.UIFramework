using Android.App;
using Android.OS;
using Android.Views;

namespace Appercode.UI.Controls.Native
{
    internal class NativeFragment : Fragment
    {
        private readonly AppercodePage page;

        public NativeFragment(AppercodePage page)
        {
            this.page = page;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return this.page.NativeUIElement;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            this.page.OnCreate(savedInstanceState);
            base.OnCreate(savedInstanceState);
        }

        public override void OnStop()
        {
            this.page.HideKeyboard();
            base.OnStop();
        }
    }
}