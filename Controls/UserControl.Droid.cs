using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Controls.NativeControl.Wrapers;
  
namespace Appercode.UI.Controls
{
    public class NativeUserControl : WrapedViewGroup
    {
        public NativeUserControl(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }

    public partial class UserControl
    {
        protected View ContentNativeUIElemtnt
        {
            get;
            set;
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();

            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new NativeUserControl(this.Context);
                }
                
                this.ApplyNativeContent(this.Content);
                this.ApplyNativePadding(this.Padding);
            }
        }

        protected virtual void OnNativeUIElementChanged(View oldUIElement, View newUIElement)
        {
        }

        protected void OnNativeContentChanged(UIElement oldContent, UIElement newContent)
        {
            this.ApplyNativeContent(newContent);
        }

        protected ViewGroup.LayoutParams CreateLayoutParams(UIElement element)
        {
            var layoutParams = new ViewGroup.LayoutParams(0, 0);
            layoutParams.Width = double.IsNaN(element.Width) ? ViewGroup.LayoutParams.FillParent : (int)element.Width;
            layoutParams.Height = double.IsNaN(element.Height) ? ViewGroup.LayoutParams.FillParent : (int)element.Height;
            return layoutParams;
        }

        private void ApplyNativeContent(UIElement newContent)
        {
            if (this.NativeUIElement != null && newContent != null)
            {
                this.ContentNativeUIElemtnt = newContent.NativeUIElement;
                ((NativeUserControl)this.NativeUIElement).RemoveAllViews();
                ((NativeUserControl)this.NativeUIElement).AddView(this.ContentNativeUIElemtnt);
            }
        }

        private void NativeArrangeContent(System.Drawing.RectangleF rectangleF)
        {
            ((UIElement)this.Content).Arrange(rectangleF);
        }
    }
}