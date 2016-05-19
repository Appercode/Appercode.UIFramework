using Android.Content;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;
using System.Drawing;

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

                    if (this.Template != null)
                    {
                        ((NativeContentPresenter)this.NativeUIElement).AddView(this.TemplateInstance.NativeUIElement);
                    }
                }
            }

            base.NativeInit();
        }

        protected override SizeF NativeMeasureOverride(SizeF availableSize)
        {
            var margin = this.Margin;
            var size = base.NativeMeasureOverride(availableSize);
            var widthIsNotSet = this.ContainsValue(WidthProperty) == false;
            var heightIsNotSet = this.ContainsValue(HeightProperty) == false;
            bool needToMeasureContent = false;

            if (widthIsNotSet)
            {
                size.Width = availableSize.Width - margin.HorizontalThicknessF();
                needToMeasureContent = true;
            }

            if (heightIsNotSet)
            {
                size.Height = availableSize.Height - margin.VerticalThicknessF();
                needToMeasureContent = true;
            }

            if (!needToMeasureContent)
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
                needContentSize = SizeF.Empty;
            }

            if (widthIsNotSet)
            {
                size.Width = Math.Min(needContentSize.Width + margin.HorizontalThicknessF(), availableSize.Width);
            }

            if (heightIsNotSet)
            {
                size.Height = Math.Min(needContentSize.Height + margin.VerticalThicknessF(), availableSize.Height);
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