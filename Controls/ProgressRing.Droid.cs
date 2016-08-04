using Appercode.UI.Controls.NativeControl.Wrappers;

namespace Appercode.UI.Controls
{
    public partial class ProgressRing : Control
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                this.NativeUIElement = new WrappedProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleLarge);
            }

            base.NativeInit();
        }
    }
}