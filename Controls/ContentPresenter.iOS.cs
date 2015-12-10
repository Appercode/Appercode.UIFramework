using CoreGraphics;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ContentPresenter
    {
        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new UIView();
            }
            base.NativeInit();

            if (this.Template != null)
            {
                this.templateInstance.NativeUIElement.Frame = this.NativeUIElement.Frame;
                this.NativeUIElement.AddSubview(this.templateInstance.NativeUIElement);
            }
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var size = base.NativeMeasureOverride(availableSize);
            bool needToMesureContent = false;

            if (this.ReadLocalValue(UIElement.WidthProperty) == DependencyProperty.UnsetValue)
            {
                size.Width = availableSize.Width - this.Margin.HorizontalThicknessF();
                needToMesureContent = true;
            }

            if (this.ReadLocalValue(UIElement.HeightProperty) == DependencyProperty.UnsetValue)
            {
                size.Height = availableSize.Height - this.Margin.VerticalThicknessF();
                needToMesureContent = true;
            }

            if (!needToMesureContent)
            {
                return size;
            }

            var needContentSize = this.templateInstance.MeasureOverride(size);

            if (this.ReadLocalValue(UIElement.WidthProperty) == DependencyProperty.UnsetValue)
            {
                size.Width = MathF.Min(needContentSize.Width + Margin.HorizontalThicknessF(), availableSize.Width);
            }

            if (this.ReadLocalValue(UIElement.HeightProperty) == DependencyProperty.UnsetValue)
            {
                size.Height = MathF.Min(needContentSize.Height + this.Margin.VerticalThicknessF(), availableSize.Height);
            }

            return size;
        }

        protected override void NativeArrange(CGRect finalRect)
        {
            base.NativeArrange(finalRect);
            if (this.templateInstance.NativeUIElement.Superview == null)
            {
                this.NativeUIElement.AddSubview(this.templateInstance.NativeUIElement);
            }

            this.templateInstance.Arrange(new CGRect(CGPoint.Empty, this.RenderSize));
        }

        private void NativeTemplateUpdate(UIElement oldValue, UIElement newValue)
        {
            if (this.NativeUIElement != null)
            {
                if (oldValue != null)
                {
                    oldValue.NativeUIElement.RemoveFromSuperview();
                }
                newValue.NativeUIElement.Frame = this.NativeUIElement.Frame;
                this.NativeUIElement.AddSubview(newValue.NativeUIElement);
            }
        }
    }
}
