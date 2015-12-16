using System;
using System.Collections;
using System.Windows;
using System.Windows.Markup;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public partial class ContentControl : Control, IAddChild
    {
        /// <summary>
        /// Identifies the <see cref="Content" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Content" /> dependency property.</returns>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content), typeof(object), typeof(ContentControl), new PropertyMetadata(OnContentChanged));

        /// <summary>
        /// Identifies the <see cref="ContentTemplate" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentTemplate" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
            nameof(ContentTemplate), typeof(DataTemplate), typeof(ContentControl), new PropertyMetadata(OnContentTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ContentTemplateSelector" /> dependency property.</returns>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(ContentControl), new PropertyMetadata(OnContentTemplateSelectorChanged));

        protected SizeF contentSize;
        protected UIElement contentTemplateInstance;

        /// <summary>
        /// Gets or sets the content of a <see cref="ContentControl" />. This is a dependency property.
        /// </summary>
        /// <returns>An object that contains the control's content. The default is null.</returns>
        public object Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentTemplateProperty); }
            set { this.SetValue(ContentTemplateProperty, value); }
        }

        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(ContentTemplateSelectorProperty); }
            set { this.SetValue(ContentTemplateSelectorProperty, value); }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Content != null)
                {
                    yield return this.Content;
                }

                if (this.controlTemplateInstance != null)
                {
                    yield return this.controlTemplateInstance;
                }

                if (this.contentTemplateInstance != null)
                {
                    yield return this.contentTemplateInstance;
                }
            }
        }

        public override void UpdateLayout()
        {
            if (this.NativeUIElement != null)
            {
                base.UpdateLayout();

                if (this.Content != null)
                {
                    this.contentSize = this.MessureContent(this.RenderSize);
                }
            }
        }

        public override void Arrange(RectangleF finalRect)
        {
            base.Arrange(finalRect);

            if (this.controlTemplateInstance == null)
            {
                this.ArrangeContent(finalRect.Size);
            }
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = SizeF.Empty;
            }

            var isMeasureNotActual = !this.IsMeasureValid;

            isMeasureNotActual |= this.measuredFor == null || this.measuredFor.Value != availableSize;

            if (!isMeasureNotActual)
            {
                return this.measuredSize;
            }

            availableSize = this.SizeThatFitsMaxAndMin(availableSize);

            var size = base.MeasureOverride(availableSize);
            this.measuredFor = availableSize;

            if (this.Template == null)
            {
                var nativePadding = this.GetNativePadding();
                var totalHorizontalOffsets = this.Padding.HorizontalThicknessF() + nativePadding.HorizontalThicknessF();
                var totalVerticalOffsets = this.Padding.VerticalThicknessF() + nativePadding.VerticalThicknessF();

                var isWidthNotSet = double.IsNaN(this.Width);
                var isHeightNotSet = double.IsNaN(this.Height);

                var widthForContent = isWidthNotSet
                    ? availableSize.Width - totalHorizontalOffsets - this.Margin.HorizontalThicknessF()
                    : (nfloat)this.Width - totalHorizontalOffsets;
                var heightForContent = isHeightNotSet
                    ? availableSize.Height - totalVerticalOffsets - this.Margin.VerticalThicknessF()
                    : (nfloat)this.Height - totalVerticalOffsets;
                this.contentSize = this.MessureContent(new SizeF(widthForContent, heightForContent));

                if (isWidthNotSet)
                {
                    size.Width += this.contentSize.Width + totalHorizontalOffsets;
                }

                if (isHeightNotSet)
                {
                    size.Height += this.contentSize.Height + totalVerticalOffsets;
                }
            }

            this.IsMeasureValid = true;
            this.measuredSize = size;
            return size;
        }

        public void AddChild(object value)
        {
            this.Content = value;
        }

        public void AddText(string text)
        {
            this.Content = text;
        }

        /// <summary>
        /// Invoked when the <see cref="Content" /> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="Content" /> property.</param>
        /// <param name="newContent">The new value of the <see cref="Content" /> property.</param>
        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            var oldElement = oldContent as UIElement;
            if (oldElement != null)
            {
                oldElement.LayoutUpdated -= this.OnContentLayoutUpdated;
            }

            var newContentDO = newContent as DependencyObject;
            if (newContentDO != null)
            {
                var newElement = newContent as UIElement;
                if (newElement != null)
                {
                    newElement.LayoutUpdated += this.OnContentLayoutUpdated;
                }

                var parent = LogicalTreeHelper.GetParent(newContentDO);
                if (parent != null)
                {
                    LogicalTreeHelper.RemoveLogicalChild(parent, newContent);
                }
            }

            DataTemplateSelector templateSelector;
            if (this.contentTemplateInstance != null)
            {
                this.contentTemplateInstance.DataContext = newContent;
            }
            else if (newContent != null && (templateSelector = this.ContentTemplateSelector) != null)
            {
                this.ApplyContentTemplate(templateSelector.SelectTemplate(newContent, this));
            }
            else
            {
                this.AddLogicalChild(newContent);
            }

            this.OnNativeContentChanged(oldContent, newContent);
            this.RemoveLogicalChild(oldContent);
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Invoked when the <see cref="ContentTemplate" /> property changes.
        /// </summary>
        /// <param name="oldContentTemplate">The old value of the <see cref="ContentTemplate" /> property.</param>
        /// <param name="newContentTemplate">The new value of the <see cref="ContentTemplate" /> property.</param>
        protected virtual void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
        {
            this.ApplyContentTemplate(newContentTemplate);
        }

        protected virtual void ArrangeContent(SizeF finalSize)
        {
            Thickness nativePadding = this.GetNativePadding();

            var totalLeftOffset = this.Padding.LeftF() + nativePadding.LeftF();
            var totalTopOffset = this.Padding.TopF() + nativePadding.TopF();
            var totalRightOffset = this.Padding.RightF() + nativePadding.RightF();
            var totalBottomOffset = this.Padding.BottomF() + nativePadding.BottomF();
            var rawWidthOfRect = finalSize.Width - this.Margin.HorizontalThicknessF();
            var rawHeigthOfRect = finalSize.Height - this.Margin.VerticalThicknessF();

            var x = (nfloat)totalLeftOffset;
            var y = (nfloat)totalTopOffset;
            var width = this.contentSize.Width;
            var heigth = this.contentSize.Height;

            if ((rawWidthOfRect - totalLeftOffset - totalRightOffset - this.contentSize.Width) > 0)
            {
                x += (rawWidthOfRect - totalLeftOffset - totalRightOffset - this.contentSize.Width) / 2;
                width = this.contentSize.Width;
            }

            if ((rawHeigthOfRect - totalTopOffset - totalBottomOffset - this.contentSize.Height) > 0)
            {
                y += (rawHeigthOfRect - totalTopOffset - totalBottomOffset - this.contentSize.Height) / 2;
                heigth = this.contentSize.Height;
            }

            var contentFrame = new RectangleF(x, y, width, heigth);

            if (this.contentTemplateInstance == null)
            {
                this.NativeArrangeContent(contentFrame);
            }
            else
            {
                this.contentTemplateInstance.Arrange(contentFrame);
            }
        }

        protected SizeF MessureContent(SizeF availableSize)
        {
            if (availableSize.Width <= 0 || availableSize.Height <= 0)
            {
                return this.contentSize = SizeF.Empty;
            }

            if (this.contentTemplateInstance != null)
            {
                return this.contentTemplateInstance.MeasureOverride(availableSize);
            }

            if (this.contentTemplateInstance == null)
            {
                this.contentSize = this.NativeMeasureContent(availableSize);
            }

            return this.contentSize;
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControl contentControl = (ContentControl)d;
            contentControl.OnContentChanged(e.OldValue, e.NewValue);
        }

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var contentControl = (ContentControl)d;
            contentControl.OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        private static void OnContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var contentControl = (ContentControl)d;
            var content = contentControl.Content;
            var newValue = e.NewValue as DataTemplateSelector;
            if (newValue != null && content != null)
            {
                var newTemplateValue = newValue.SelectTemplate(content, contentControl);
                contentControl.ApplyContentTemplate(newTemplateValue);
            }
        }

        private void ApplyContentTemplate(DataTemplate value)
        {
            if (this.contentTemplateInstance != null)
            {
                this.contentTemplateInstance.LayoutUpdated -= this.OnContentLayoutUpdated;
                this.RemoveContentTemplateInstance();
                this.RemoveLogicalChild(this.contentTemplateInstance);
            }

            if (value != null)
            {
                this.contentTemplateInstance = (UIElement)value.LoadContent();
                this.contentTemplateInstance.LayoutUpdated += this.OnContentLayoutUpdated;
                this.AddLogicalChild(this.contentTemplateInstance);
                this.RemoveContentNativeView();
                this.contentTemplateInstance.DataContext = this.Content;
                this.AddContentTemplateInstance();
            }
            else
            {
                this.RemoveContentNativeView();
                this.contentTemplateInstance = null;
            }

            this.OnNativeContentChanged(null, this.Content);
            this.OnLayoutUpdated();
        }

        private void OnContentLayoutUpdated(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                var element = sender as UIElement;
                if (element.measuredFor != null)
                {
                    var measuredSize = element.MeasureOverride(element.measuredFor.Value);
                    if (element.arrangedSize.Height == measuredSize.Height && element.arrangedSize.Width == measuredSize.Width)
                    {
                        element.Arrange(new RectangleF(element.TranslatePoint, element.arrangedSize));
                        return;
                    }
                }
                this.InvalidateMeasure();
            }
        }

        partial void RemoveContentNativeView();
    }
}