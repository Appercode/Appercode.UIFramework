using Android.Views;
using Appercode.UI.Controls.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using Appercode.UI.Controls.NativeControl.Wrapers;

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

        protected void NativeOnbackgroundChange()
        {
            SetBackground();
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();

            if (this.Parent != null && this.Context != null)
            {
                this.NativeOnbackgroundChange();
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

        private void SetBackground()
        {
            if (this.Background != null && this.NativeUIElement != null)
            {
                if (IsBackgroundValidImageBrush())
                {
                    ((BitmapImage)(((ImageBrush)this.Background).ImageSource)).ImageOpened += (s, e) =>
                    {
                        if (IsBackgroundValidImageBrush())
                        {
                            this.NativeUIElement.Post(() =>
                            {
                                this.NativeUIElement.SetBackgroundDrawable(this.Background.ToDrawable());
                                this.OnLayoutUpdated();
                            });
                        }
                    };
                }
                else
                    this.NativeUIElement.SetBackgroundDrawable(this.Background.ToDrawable());
            }
        }

        private bool IsBackgroundValidImageBrush()
        {
            return this.Background is ImageBrush
                   && ((ImageBrush)this.Background).ImageSource is BitmapImage
                   && ((BitmapImage)(((ImageBrush)this.Background).ImageSource)).UriSource.IsAbsoluteUri;
        }
    }
}