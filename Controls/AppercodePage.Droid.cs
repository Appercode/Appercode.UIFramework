using Android.App;
using Android.OS;
using Android.Views.InputMethods;
using Appercode.UI.Controls.Native;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Controls.Navigation;
using Appercode.UI.Input;
using System;

namespace Appercode.UI.Controls
{
    public partial class AppercodePage
    {
        private NavigationType navigationType;

        internal event EventHandler<BundleEventArgs> Create;

        internal Fragment NativeFragment { get; private set; }

        internal void OnCreate(Bundle savedInstanceState)
        {
            this.Create?.Invoke(this, new BundleEventArgs(savedInstanceState));
        }

        internal void HideKeyboard()
        {
            var activity = this.Context as Activity;
            var focused = activity?.CurrentFocus;
            if (focused != null)
            {
                var inputManager = (InputMethodManager)this.Context.GetSystemService(Android.Content.Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(focused.WindowToken, HideSoftInputFlags.None);
            }
        }

        internal override ViewGroup.LayoutParams GetDefaultLayoutParameters()
        {
            return new FrameLayout.LayoutParams(0, 0);
        }

        internal override void OnTap(GestureEventArgs args)
        {
            base.OnTap(args);
            if (args.OriginalSource is TextBox == false
                && args.OriginalSource is PasswordBox == false)
            {
                this.HideKeyboard();
            }
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();
            if (this.Context != null && this.Parent != null && this.NativeFragment == null)
            {
                this.NativeFragment = this.navigationType == NavigationType.Modal
                    ? (Fragment)new NativeDialogFragment(this) : new NativeFragment(this);
            }
        }

        partial void ApplyNavigationType(NavigationType navigationType)
        {
            this.navigationType = navigationType;
        }
    }
}