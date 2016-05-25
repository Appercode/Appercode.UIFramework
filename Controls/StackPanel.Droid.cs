using Appercode.UI.Controls.NativeControl.Wrapers;
using System.Drawing;

namespace Appercode.UI.Controls
{
    public partial class StackPanel
    {
        protected internal override void NativeInit()
        {
            WrapedViewGroup.FillNativeUIElement(this);
            base.NativeInit();
        }

        protected override void ArrangeChilds(SizeF size)
        {
            this.UpdateLayout();
        }
    }
}