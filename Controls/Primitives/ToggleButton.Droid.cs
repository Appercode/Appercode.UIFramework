using Appercode.UI.Controls.NativeControl;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ToggleButton
    {
        private bool? nativeIsChecked;

        protected bool? NativeIsChecked
        {
            get
            {
                return this.nativeIsChecked;
            }
            set
            {
                this.nativeIsChecked = value;

                if (this.NativeUIElement != null && this.controlTemplateInstance == null)
                {
                    ((NativeToggleButton)this.NativeUIElement).IsChecked = value.HasValue && value.Value;
                    ((NativeToggleButton)this.NativeUIElement).RefreshDrawableState();
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                this.NativeUIElement = new NativeToggleButton(this);
                this.ApplyNativeContent(null, this.Content);
            }

            base.NativeInit();
            if (this.NativeUIElement != null && !(this is CheckBox) && !(this is RadioButton))
            {
                var nativeView = (NativeToggleButton)this.NativeUIElement;
                nativeView.IsChecked = this.NativeIsChecked == true;
                nativeView.Background = new Android.Widget.ToggleButton(this.Context).Background;
            }
        }
    }
}