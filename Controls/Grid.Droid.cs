using Android.Content;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appercode.UI.Controls
{
    public class NativeGrid : WrapedViewGroup
    {
        public NativeGrid(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }
    }

    public partial class Grid
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeGrid = new NativeGrid(this.Context);
                    var layoutParams = new ViewGroup.LayoutParams(0, 0);
                    layoutParams.Width = double.IsNaN(this.NativeWidth) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeWidth;
                    layoutParams.Height = double.IsNaN(this.NativeHeight) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeHeight;
                    nativeGrid.LayoutParameters = layoutParams;

                    this.NativeUIElement = nativeGrid;
                }
            }

            base.NativeInit();
        }

        protected override void ArrangeChilds(System.Drawing.SizeF size)
        {
            this.ArrangeCells(size);
        }
    }
}
