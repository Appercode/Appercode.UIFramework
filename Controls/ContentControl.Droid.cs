using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Device;
using System.Drawing;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class ContentControl
    {
        protected View ContentNativeUIElement
        {
            get;
            set;
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new NativeContentControl(this.Context);

                    var layoutParams = new ViewGroup.LayoutParams(0, 0);
                    layoutParams.Width = double.IsNaN(this.NativeWidth) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeWidth;
                    layoutParams.Height = double.IsNaN(this.NativeHeight) ? ViewGroup.LayoutParams.WrapContent : (int)this.NativeHeight;
                    this.NativeUIElement.LayoutParameters = layoutParams;
                    this.ApplyNativeContent(null, this.Content);
                }
            }

            base.NativeInit();
        }

        protected virtual View CreateDefaultControl(string value)
        {
            var innerDefaultControl = new Android.Widget.TextView(this.Context);
            innerDefaultControl.LayoutParameters = this.CreateLayoutParams();
            innerDefaultControl.SetSingleLine(true);
            innerDefaultControl.Text = value;
            return innerDefaultControl;
        }

        protected virtual View CreateLayoutControl(UIElement value)
        {
            LogicalTreeHelper.AddLogicalChild(this, value);
            return value.NativeUIElement;
        }

        protected void OnNativeContentChanged(object oldContent, object newContent)
        {
            if(this.controlTemplateInstance != null)
            {
                return;
            }

            if(oldContent is UIElement)
            {
                ((ViewGroup)this.NativeUIElement).RemoveAllViews();
                if (((UIElement)oldContent).Parent != null)
                {
                    ((ViewGroup)((UIElement)oldContent).Parent.NativeUIElement).RemoveAllViews();
                }
                this.ContentNativeUIElement = null;
            }
            if (this.NativeUIElement != null)
            {
                this.ApplyNativeContent(oldContent, newContent);
            }
        }

        protected virtual void ApplyNativeContent(object oldContent, object newContent)
        {
            // forward applying to the template
            if (this.controlTemplateInstance != null)
            {
                return;
            }

            if (this.contentTemplateInstance == null && oldContent is string && newContent is string)
            {
                // the view is not changed, apply the new text only
                this.ApplyNativeContentForDefaultControl(newContent.ToString());
            }
            else
            {
                var oldUIElement = this.NativeUIElement;

                // create new views in dependence of the content type
                if (this.Parent != null && this.Context != null)
                {
                    if (this.contentTemplateInstance == null)
                    {
                        if (newContent is UIElement)
                        {
                            var el = (UIElement)newContent;
                            if (this.ContentNativeUIElement == null || !(this.ContentNativeUIElement is ViewGroup) || ((ViewGroup)this.ContentNativeUIElement).GetChildAt(0) != el.NativeUIElement)
                            {
                                this.ContentNativeUIElement = this.CreateLayoutControl(el);
                            }
                        }
                        else
                        {
                            this.ContentNativeUIElement = this.CreateDefaultControl(newContent != null ? newContent.ToString() : string.Empty);
                        }
                    }
                    else
                    {
                        this.ContentNativeUIElement = this.contentTemplateInstance.NativeUIElement;
                    }

                    ((ViewGroup)this.NativeUIElement).RemoveAllViews();
                    if (this.ContentNativeUIElement.Parent != null)
                    {
                        ((ViewGroup)this.ContentNativeUIElement.Parent).RemoveView(this.ContentNativeUIElement);
                    }
                    ((ViewGroup)this.NativeUIElement).AddView(this.ContentNativeUIElement);
                }
            }
        }

        protected virtual void ApplyNativeContentForDefaultControl(string value)
        {
            ((TextView)this.ContentNativeUIElement).Text = value;
        }

        // TODO: Looks like redundant, because LayoutParams is generated in the UIElement
        protected ViewGroup.LayoutParams CreateLayoutParams()
        { 
            var layoutParams = new ViewGroup.LayoutParams(0, 0);
            layoutParams.Width = ViewGroup.LayoutParams.WrapContent;
            layoutParams.Height = ViewGroup.LayoutParams.WrapContent; 
            return layoutParams;
        }

        protected virtual void NativeArrangeContent(System.Drawing.RectangleF rectangleF)
        {
            if (this.contentTemplateInstance != null)
            {
                this.contentTemplateInstance.Arrange(rectangleF);
                return;
            }
            RectangleF rectangleContent = rectangleF;

            if (this.Content is UIElement)
            {
                ((UIElement)this.Content).Arrange(rectangleContent);

                if (this.ContentNativeUIElement != null)
                {
                    this.ContentNativeUIElement.Layout((int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Left),
                                                         (int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Top),
                                                         (int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Right),
                                                         (int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Bottom));
                }
            }
            else
            {
                if (this.ContentNativeUIElement != null)
                {
                    this.ContentNativeUIElement.Layout((int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Left),
                                                         (int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Top),
                                                         (int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Right),
                                                         (int)ScreenProperties.ConvertDPIToPixels(rectangleContent.Bottom));
                }
            }
        }

        protected virtual System.Drawing.SizeF NativeMeasureContent(System.Drawing.SizeF sizeF)
        {
            if(this.contentTemplateInstance != null)
            {
                return this.contentTemplateInstance.MeasureOverride(sizeF);
            }
            if (this.Content is UIElement)
            {
                System.Drawing.SizeF measuredSize = ((UIElement)this.Content).MeasureOverride(sizeF);
                return measuredSize;
            }
            else
            {
                if (this.ContentNativeUIElement == null)
                {
                    return new SizeF();
                }

                SizeF absoluteSizeF = new SizeF();
                absoluteSizeF.Width = ScreenProperties.ConvertDPIToPixels(sizeF.Width);
                absoluteSizeF.Height = ScreenProperties.ConvertDPIToPixels(sizeF.Height);

                int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec((int)absoluteSizeF.Width, MeasureSpecMode.AtMost);
                int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec((int)absoluteSizeF.Height, MeasureSpecMode.AtMost);
                this.ContentNativeUIElement.Measure(widthMeasureSpec, heightMeasureSpec);

                SizeF dpiMeasuredContentSize = new SizeF();
                dpiMeasuredContentSize.Width = ScreenProperties.ConvertPixelsToDPI(this.ContentNativeUIElement.MeasuredWidth);
                dpiMeasuredContentSize.Height = ScreenProperties.ConvertPixelsToDPI(this.ContentNativeUIElement.MeasuredHeight);

                return dpiMeasuredContentSize;
            }
        }

        protected override SizeF NativeMeasureOverride(SizeF availableSize)
        {
            var result = new SizeF((float)this.Width, (float)this.Height);

            if (float.IsNaN(result.Width))
            {
                result.Width = 0;
            }
            if (float.IsNaN(result.Height))
            {
                result.Height = 0;
            }
            return result;
        }

        protected virtual Thickness GetNativePadding()
        {
            if (this.NativeUIElement.Background == null)
            {
                return new Thickness(0);
            }

            Android.Graphics.Rect rect = new Android.Graphics.Rect();
            this.NativeUIElement.Background.GetPadding(rect);

            Thickness absoluteNativePadding = new Thickness();
            absoluteNativePadding.Left = rect.Left + (this.NativeUIElement.Background.MinimumWidth - rect.Left - rect.Right) / 2.0;
            absoluteNativePadding.Top = rect.Top + (this.NativeUIElement.Background.MinimumHeight - rect.Top - rect.Bottom) / 2.0;
            absoluteNativePadding.Right = rect.Right + (this.NativeUIElement.Background.MinimumWidth - rect.Left - rect.Right) / 2.0;
            absoluteNativePadding.Bottom = rect.Bottom + (this.NativeUIElement.Background.MinimumHeight - rect.Top - rect.Bottom) / 2.0;

            Thickness dpiNativePadding = new Thickness();
            dpiNativePadding.Left = ScreenProperties.ConvertPixelsToDPI((float)absoluteNativePadding.Left);
            dpiNativePadding.Top = ScreenProperties.ConvertPixelsToDPI((float)absoluteNativePadding.Top);
            dpiNativePadding.Right = ScreenProperties.ConvertPixelsToDPI((float)absoluteNativePadding.Right);
            dpiNativePadding.Bottom = ScreenProperties.ConvertPixelsToDPI((float)absoluteNativePadding.Bottom);

            return dpiNativePadding;
        }

        partial void RemoveContentNativeView()
        {
            var contentElement = this.Content as UIElement;
            if (contentElement != null)
            {
                ((ViewGroup)this.NativeUIElement).RemoveAllViews();
                ((ViewGroup)contentElement.Parent.NativeUIElement).RemoveAllViews();
                this.RemoveLogicalChild(contentElement);
                this.ContentNativeUIElement = null;
            }
        }
    }
}