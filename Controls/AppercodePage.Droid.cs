namespace Appercode.UI.Controls
{
    public partial class AppercodePage
    {
        internal NativeAppercodeFragment NativeFragment { get; private set; }

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