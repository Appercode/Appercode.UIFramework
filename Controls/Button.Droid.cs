namespace Appercode.UI.Controls
{
    public partial class Button
    {
        protected internal override void NativeInit()
        {
            base.NativeInit();
            if (this.NativeUIElement != null && this.controlTemplateInstance == null)
            {
                this.NativeUIElement.Background = new Android.Widget.Button(this.Context).Background;
            }
        }
    }
}