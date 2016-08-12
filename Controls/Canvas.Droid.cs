using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrappers;
using System.Drawing;

namespace Appercode.UI.Controls
{
    public partial class Canvas
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                this.NativeUIElement = new WrappedViewGroup(this);
            }

            base.NativeInit();
        }

        protected override void ArrangeChilds(SizeF size)
        {
            this.UpdateLayout();
        }

        private void NativeReorderChildren()
        {
            var nativePanel = (ViewGroup)this.NativeUIElement;
            nativePanel.RemoveAllViews();
            foreach (var child in this.Children)
            {
                nativePanel.AddView(child.NativeUIElement);
            }
        }
    }
}