using CoreGraphics;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ContentControl
    {
        protected TextBlock textContent;
        private UIElement uiContent;

        protected internal override void NativeInit()
        {
            if (Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UIView();
                }
                this.OnNativeContentChanged(null, this.Content);
                base.NativeInit();
            }
        }

        protected virtual void SetTextContent(string text)
        {
            if (text == string.Empty)
            {
                if (this.textContent != null && this.textContent.NativeUIElement != null)
                {
                    this.RemoveLogicalChild(this.textContent);
                    this.textContent.NativeUIElement.RemoveFromSuperview();
                }
            }
            else
            {
                this.textContent = new TextBlock { Text = text };
                this.AddLogicalChild(textContent);
                this.NativeUIElement.AddSubview(this.textContent.NativeUIElement); 
            }
        }

        protected virtual Thickness GetNativePadding()
        {
            return new Thickness();
        }

        protected virtual CGSize NativeMeasureContent(CGSize availableSize)
        {
            if (this.contentTemplateInstance != null)
            {
                return this.contentTemplateInstance.MeasureOverride(availableSize);
            }

            var uiContent = this.Content as UIElement;
            if (uiContent != null)
            {
                return uiContent.MeasureOverride(availableSize);
            }

            if (this.textContent != null)
            {
                return this.textContent.MeasureOverride(availableSize);
            }
            var stringContent = this.Content == null ? "" : this.Content.ToString();
            var label = new UILabel();
            label.Text = stringContent;
            return label.SizeThatFits(availableSize);
        }

        protected virtual void NativeArrangeContent(CGRect contentFrame)
        {
            if (this.contentTemplateInstance != null)
            {
                this.contentTemplateInstance.Arrange(contentFrame);
            }
            else if (this.uiContent != null)
            {
                this.uiContent.Arrange(contentFrame);
            }
            else if (this.textContent != null)
            {
                this.textContent.Arrange(contentFrame);
            }
        }

        private void OnNativeContentChanged(object oldContent, object newContent)
        {
            if (this.controlTemplateInstance != null)
            {
                return;
            }

            if (oldContent is UIElement && ((UIElement)oldContent).NativeUIElement != null)
            {
                ((UIElement)oldContent).NativeUIElement.RemoveFromSuperview();
            }

            if (this.NativeUIElement == null)
            {
                return;
            }

            this.SetTextContent(string.Empty);
            if (this.uiContent != null && this.uiContent.NativeUIElement != null)
            {
                this.uiContent.NativeUIElement.RemoveFromSuperview();
            }

            var c = this.NativeUIElement.Subviews.Length;

            if (this.contentTemplateInstance != null)
            {
                this.contentTemplateInstance.NativeUIElement.RemoveFromSuperview();
                this.NativeUIElement.AddSubview(this.contentTemplateInstance.NativeUIElement);
                return;
            }

            this.uiContent = newContent as UIElement;
            if (this.uiContent != null)
            {
                this.NativeUIElement.AddSubview(this.uiContent.NativeUIElement);
                return;
            }
            this.SetTextContent(this.Content == null ? "" : this.Content.ToString());
        }

        private void RemoveContentTemplateInstance()
        {
            this.contentTemplateInstance.NativeUIElement.RemoveFromSuperview();
        }

        private void AddContentTemplateInstance()
        {
            this.contentTemplateInstance.NativeUIElement.RemoveFromSuperview();
        }

        partial void RemoveContentNativeView()
        {
            var contentElement = (UIElement)this.Content;
            contentElement.NativeUIElement.RemoveFromSuperview();
            this.RemoveLogicalChild(contentElement);
        }
    }
}