using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Appercode.UI.Controls.NativeControl;
using System;

namespace Appercode.UI.Controls
{
    internal class NativeAppercodeFragment : Fragment
    {
        private readonly AppercodePage page;

        public NativeAppercodeFragment(AppercodePage page)
        {
            this.page = page;
        }

        public event EventHandler<BundleEventArgs> Create;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return this.page.NativeUIElement;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            this.Create?.Invoke(this, new BundleEventArgs(savedInstanceState));
            base.OnCreate(savedInstanceState);
        }

        public override void OnStop()
        {
            this.HideKeyboard();
            base.OnStop();
        }

        private void HideKeyboard()
        {
            var context = page.Context as Activity;
            var focused = context?.CurrentFocus;
            if (focused != null)
            {
                var inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(focused.WindowToken, HideSoftInputFlags.None);
            }
        }
    }
}