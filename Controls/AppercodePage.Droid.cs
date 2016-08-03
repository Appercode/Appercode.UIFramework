using Appercode.UI.Input;

namespace Appercode.UI.Controls
{
    public partial class AppercodePage
    {
        internal NativeAppercodeFragment NativeFragment { get; private set; }

        internal override void OnTap(GestureEventArgs args)
        {
            base.OnTap(args);
            if (args.OriginalSource is TextBox == false
                && args.OriginalSource is PasswordBox == false)
            {
                this.NativeFragment.HideKeyboard();
            }
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();
            if (this.Context != null && this.Parent != null && this.NativeFragment == null)
            {
                this.NativeFragment = new NativeAppercodeFragment(this);
            }
        }
    }
}