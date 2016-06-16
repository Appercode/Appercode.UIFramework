using Android.Views;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System.Drawing;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class Panel
    {
        public virtual void AddNativeChildView(UIElement view)
        {
            if (this.NativeUIElement != null)
            {
                var wvg = this.NativeUIElement as WrapedViewGroup;
                if (wvg != null)
                {
                    wvg.AddViewInLayoutOverride(view.NativeUIElement);
                    return;
                }
                ((ViewGroup)this.NativeUIElement).AddView(view.NativeUIElement);
            }
        }

        public virtual void RemoveNativeChildView(UIElement view)
        {
            if (this.NativeUIElement != null)
            {
                ((ViewGroup)this.NativeUIElement).RemoveView(view.NativeUIElement);
            }
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();
            if (this.Parent != null && this.Context != null)
            {
                this.SetNativeBackground(this.Background);
                foreach (var child in this.Children)
                {
                    if (child.NativeUIElement != null && child.NativeUIElement.Parent != this.NativeUIElement)
                    {
                        LogicalTreeHelper.AddLogicalChild(this, child);
                        this.AddNativeChildView(child);
                    }
                }
            }
        }

        protected virtual void ArrangeChilds(SizeF size)
        {
        }

        partial void ApplyNativeBackground(Brush newValue)
        {
            this.SetNativeBackground(newValue);
        }
    }
}