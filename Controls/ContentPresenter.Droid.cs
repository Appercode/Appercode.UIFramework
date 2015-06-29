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
using Appercode.UI.Controls.NativeControl.Wrapers;

namespace Appercode.UI.Controls
{
    public class NativeContentPresenter : WrapedViewGroup, ITapableView
    {
        public NativeContentPresenter(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }
    }

    public partial class ContentPresenter
    {
        protected internal override void NativeInit()
        {
            if (this.Context != null && this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new NativeContentPresenter(this.Context);

                    ((NativeContentPresenter)this.NativeUIElement).AddView(this.TemplateInstance.NativeUIElement);
                }
            }

            base.NativeInit();
        }

        protected override SizeF NativeMeasureOverride(SizeF availableSize)
        {
            var size = base.NativeMeasureOverride(availableSize);

            bool needToMesureContent = false;

            if (this.ReadLocalValue(UIElement.WidthProperty) == DependencyProperty.UnsetValue)
            {
                size.Width = (float)(availableSize.Width - this.Margin.Left - this.Margin.Right);
                needToMesureContent = true;
            }

            if (this.ReadLocalValue(UIElement.HeightProperty) == DependencyProperty.UnsetValue)
            {
                size.Height = (float)(availableSize.Height - this.Margin.Top - this.Margin.Bottom);
                needToMesureContent = true;
            }

            if (!needToMesureContent)
            {
                return size;
            }

            SizeF needContentSize;
            if (this.templateInstance != null)
            {
                needContentSize = this.templateInstance.MeasureOverride(size);
            }
            else
            {
                needContentSize = new SizeF(0, 0);
            }          

            if (this.ReadLocalValue(UIElement.WidthProperty) == DependencyProperty.UnsetValue)
            {
                size.Width = (float)Math.Min(needContentSize.Width + Margin.Left + Margin.Right, availableSize.Width);
            }

            if (this.ReadLocalValue(UIElement.HeightProperty) == DependencyProperty.UnsetValue)
            {
                size.Height = (float)Math.Min(needContentSize.Height + this.Margin.Top + this.Margin.Bottom, availableSize.Height);
            }

            return size;
        }

        protected override void NativeArrange(RectangleF finalRect)
        {
            /*if (((ViewGroup)(this.NativeUIElement)).ChildCount == 0)
            {
                ((ViewGroup)(this.NativeUIElement)).AddView(this.templateInstance.NativeUIElement);
            }*/
            base.NativeArrange(finalRect);
            if (this.templateInstance != null)
            {
                this.templateInstance.Arrange(new RectangleF(0, 0, finalRect.Width, finalRect.Height));
            }
        }

        private void NativeTemplateUpdate(UIElement oldValue, UIElement newValue)
        {
            if (this.NativeUIElement != null)
            {
                if (oldValue != null)
                {
                    ((ViewGroup)this.NativeUIElement).RemoveAllViews();
                }

                ((NativeContentPresenter)this.NativeUIElement).AddView(newValue.NativeUIElement);
            }
        }
    }
}