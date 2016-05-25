namespace Appercode.UI.Controls.Primitives
{
    public partial class ButtonBase
    {
        internal override bool ShouldInterceptTouchEvent
        {
            get { return IsEnabled; }
        }

        protected ClickMode NativeClickMode
        {
            get;
            set;
        }

        protected bool NativeIsPressed
        {
            get { return (this.NativeUIElement.Pressed); }
        }

        internal override void OnNativeClick()
        {
            if (this.IsEnabled)
            {
                this.OnClick();
            }
        }
    }
}