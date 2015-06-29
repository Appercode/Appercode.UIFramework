using System.Drawing;
using System.Windows;
using Appercode.UI.Controls.NativeControl;

namespace Appercode.UI.Controls
{
    public partial class Button
    {
        protected internal override void NativeInit()
        {
            base.NativeInit();

            if (this.NativeUIElement != null && this.controlTemplateInstance == null)
            {
                ((NativeContentControl)this.NativeUIElement).SetBackgroundDrawable(new Android.Widget.Button(this.Context).Background);
            }
        }
    }
}