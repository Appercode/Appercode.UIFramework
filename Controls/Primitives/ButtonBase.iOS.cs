using System;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ButtonBase
    {
        internal override bool ChildrenShouldForwardTouch
        {
            get { return this.IsEnabled; }
        }

        protected bool NativeIsPressed
        {
            set { }
        }

        protected ClickMode NativeClickMode
        {
            set { }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            var element = newContent as UIElement;
            if (element != null)
            {
                element.NativeUIElement.UserInteractionEnabled = false;
            }
        }

        internal override void OnTouchDown()
        {
            base.OnTouchDown();
            if (this.IsEnabled && this.ClickMode == ClickMode.Press)
            {
                this.OnClick();
            }
        }

        internal override void OnTouchUp()
        {
            base.OnTouchUp();
            if (this.IsEnabled && this.ClickMode == ClickMode.Release)
            {
                this.OnClick();
            }
        }
    }
}