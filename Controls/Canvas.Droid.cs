using Android.Content;
using Android.Views;

using System.Windows.Markup;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System.Collections.Generic;

namespace Appercode.UI.Controls
{
    public class NativeCanvas : WrapedViewGroup
    {
        public NativeCanvas(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }
    }

    public partial class Canvas
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeCanvas = new NativeCanvas(this.Context);

                    var layoutParams = new ViewGroup.LayoutParams(0, 0);

                    layoutParams.Width = double.IsNaN(this.NativeWidth) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeWidth;
                    layoutParams.Height = double.IsNaN(this.NativeHeight) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeHeight;
                    nativeCanvas.LayoutParameters = layoutParams;
                    this.NativeUIElement = nativeCanvas;
                }
            }
            base.NativeInit();
        }

        protected override void ArrangeChilds(System.Drawing.SizeF size)
        {
            this.UpdateLayout();
        }

        private void NativeReorderChildren()
        {
            ((NativeCanvas)this.NativeUIElement).RemoveAllViews();

            foreach(UIElement child in this.Children)
            {
                ((NativeCanvas)this.NativeUIElement).AddView(child.NativeUIElement);
            }
        }
    }
}