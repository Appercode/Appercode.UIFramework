using Android.Content;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrapers;

namespace Appercode.UI.Controls
{
    public class NativeStackPanel : WrapedViewGroup
    {
        public NativeStackPanel(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }
    }

    public partial class StackPanel
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeStackPanel = new NativeStackPanel(this.Context);

                    this.NativeUIElement = nativeStackPanel;
                }

                var layoutParams = new ViewGroup.LayoutParams(0, 0);
                layoutParams.Width = double.IsNaN(this.NativeWidth) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeWidth;
                layoutParams.Height = double.IsNaN(this.NativeHeight) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeHeight;
                this.NativeUIElement.LayoutParameters = layoutParams;
            }

            base.NativeInit();
        }

        protected override void ArrangeChilds(System.Drawing.SizeF size)
        {
            this.UpdateLayout();
        }
    }
}