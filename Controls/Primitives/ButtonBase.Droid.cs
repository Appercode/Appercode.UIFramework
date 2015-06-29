using System.Drawing;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;
using System.Windows.Input;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ButtonBase
    {
        protected ClickMode NativeClickMode
        {
            get;
            set;
        }

        protected bool NativeIsPressed
        {
            get { return (this.NativeUIElement.Pressed); }
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();
            if (this.NativeUIElement != null && this.NativeUIElement is IClickableView)
            {
                ((IClickableView)this.NativeUIElement).NativeClick -= this.ButtonBase_NativeClick;
                ((IClickableView)this.NativeUIElement).NativeClick += this.ButtonBase_NativeClick;
            }
        }

        protected override void NativeArrange(RectangleF finalRect)
        {
            base.NativeArrange(finalRect);
            if (controlTemplateInstance != null && this.controlTemplateInstance.NativeUIElement is IClickableView)
            {
                ((IClickableView)this.controlTemplateInstance.NativeUIElement).NativeClick -= this.ButtonBase_NativeClick;
                ((IClickableView)this.controlTemplateInstance.NativeUIElement).NativeClick += this.ButtonBase_NativeClick;
            }
        }

        private void ButtonBase_NativeClick(object sender, EventArgs e)
        {
            if (this.IsEnabled)
            {
                this.OnClick();
            }
        }
    }
}