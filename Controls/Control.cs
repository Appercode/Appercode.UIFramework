using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public abstract partial class Control : UIElement
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(Control),
                                        new PropertyMetadata(true, (d, e) => { ((Control)d).OnIsEnabledChanged(); }));

        public static readonly DependencyProperty PaddingProperty =
                    DependencyProperty.Register("Padding", typeof(Thickness), typeof(Control), new PropertyMetadata(GetDefaultPadding(), OnPaddingChanged));

        public static readonly DependencyProperty FontSizeProperty =
                    DependencyProperty.Register("FontSize", typeof(double), typeof(Control),
                                                new PropertyMetadata(GetDefaultFontSize(), (d, e) =>
                                                {
                                                    ((Control)d).NativeFontSize = (double)e.NewValue;
                                                    ((Control)d).InvalidateMeasure();
                                                }));

        public static readonly DependencyProperty FontWeightProperty =
                    DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Control),
                                                new PropertyMetadata(default(FontWeight), (d, e) => { ((Control)d).NativeFontWeight = (FontWeight)e.NewValue; }));

        public static readonly DependencyProperty FontStyleProperty =
                    DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Control),
                                                new PropertyMetadata(default(FontStyle), (d, e) => { ((Control)d).NativeFontStyle = (FontStyle)e.NewValue; }));

        public static readonly DependencyProperty FontFamilyProperty =
                    DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Control),
                                                new PropertyMetadata(default(FontFamily), (d, e) => { ((Control)d).NativeFontFamily = (FontFamily)e.NewValue; }));

        public static readonly DependencyProperty ForegroundProperty =
                    DependencyProperty.Register("Foreground", typeof(Brush), typeof(Control),
                                                new PropertyMetadata(null, (d, e) => { ((Control)d).NativeForeground = (Brush)e.NewValue; }));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Control), new PropertyMetadata(null, (d, e) => ((Control)d).OnBackgroundChanged()));

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(Control), new PropertyMetadata(null, (d, e) => ((Control)d).OnTemplateChanged((ControlTemplate)e.OldValue, (ControlTemplate)e.NewValue)));

        protected UIElement controlTemplateInstance;
        protected SizeF measuredSizeCashe;

        public Brush Background
        {
            get { return (Brush)this.GetValue(BackgroundProperty); }
            set { this.SetValue(BackgroundProperty, value); }
        }

        public bool IsEnabled
        {
            get { return (bool)this.GetValue(IsEnabledProperty); }
            set { this.SetValue(IsEnabledProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)this.GetValue(PaddingProperty); }
            set { this.SetValue(PaddingProperty, value); }
        }

        public double FontSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)this.GetValue(FontWeightProperty); }
            set { this.SetValue(FontWeightProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)this.GetValue(FontStyleProperty); }
            set { this.SetValue(FontStyleProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)this.GetValue(FontFamilyProperty); }
            set { this.SetValue(FontFamilyProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush)this.GetValue(ForegroundProperty); }
            set { this.SetValue(ForegroundProperty, value); }
        }

        public ControlTemplate Template
        {
            get { return (ControlTemplate)this.GetValue(TemplateProperty); }
            set 
            { 
                this.SetValue(TemplateProperty, value); 
            }
        }

        internal override FrameworkTemplate InternalTemplate
        {
            get { return this.Template; }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                var children = new List<object>();
                if (this.controlTemplateInstance != null)
                {
                    children.Add(this.controlTemplateInstance);
                }
                return children.GetEnumerator();
            }
        }

        private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Control;
            if (control != null)
            {
                control.OnPaddingUpdated((Thickness)e.NewValue);
            }
        }

        public bool Focus()
        {
            return this.InternalFocus();
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.controlTemplateInstance != null)
            {
                var isMeasureActual = this.IsMeasureValid && this.measuredFor == availableSize;
                if (isMeasureActual)
                {
                    return this.measuredSize;
                }

                this.ApplyTemplate();
                this.measuredFor = availableSize;

                if (this.Visibility == Visibility.Collapsed)
                {
                    return SizeF.Empty;
                }

                var margin = this.Margin;
                var padding = this.Padding;
                if (this.ContainsValue(HeightProperty))
                {
                    availableSize.Height = (nfloat)this.Height;
                }
                else
                {
                    availableSize.Height -= margin.VerticalThicknessF() + padding.VerticalThicknessF();
                }

                if (this.ContainsValue(WidthProperty))
                {
                    availableSize.Width = (nfloat)this.Width;
                }
                else
                {
                    availableSize.Width -= margin.HorizontalThicknessF() + padding.HorizontalThicknessF();
                }

                this.measuredSize = this.controlTemplateInstance.MeasureOverride(availableSize);

                if (!double.IsNaN(this.Height))
                {
                    measuredSize.Height = (nfloat)this.Height;
                }

                if (!double.IsNaN(this.Width))
                {
                    measuredSize.Width = (nfloat)this.Width;
                }

                this.measuredSize.Height += margin.VerticalThicknessF() + padding.VerticalThicknessF();
                this.measuredSize.Width += margin.HorizontalThicknessF() + padding.HorizontalThicknessF();
                this.measuredSizeCashe = this.measuredSize;
                this.IsMeasureValid = true;
            }
            else
            {
                this.measuredSize = base.MeasureOverride(availableSize);
            }

            return this.measuredSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            if (finalRect.Size == this.arrangedSize && this.IsArrangeValid)
            {
                base.Arrange(finalRect);
                return;
            }

            if (this.controlTemplateInstance != null)
            {
                this.NativeArrange(finalRect);

                var margin = this.Margin;
                var padding = this.Padding;
                var rawWidthOfRect = finalRect.Width - margin.HorizontalThicknessF() - padding.HorizontalThicknessF();
                var rawHeigthOfRect = finalRect.Height - margin.VerticalThicknessF() - padding.VerticalThicknessF();

                var contentFrame = new RectangleF(padding.LeftF(), padding.TopF(), rawWidthOfRect, rawHeigthOfRect);
                this.TranslatePoint = finalRect.Location;

                if (this.controlTemplateInstance.IsArrangeValid && this.controlTemplateInstance.arrangedSize == contentFrame.Size)
                {
                    return;
                }

                this.arrangedSize = finalRect.Size;
                this.controlTemplateInstance.Arrange(contentFrame);
                this.IsArrangeValid = true;
                this.RenderSize = this.controlTemplateInstance.RenderSize;
            }
            else
            {
                base.Arrange(finalRect);
            }
        }

        internal void ShowValidationError()
        {
            // TODO: show validation error
        }

        internal void HideValidationError()
        {
            // TODO: hide validation error
        }

        internal override SizeF MeasureContentViewPort(SizeF availableSize)
        {
            var padding = this.Padding;
            availableSize.Width -= padding.HorizontalThicknessF();
            availableSize.Height -= padding.VerticalThicknessF();
            return base.MeasureContentViewPort(availableSize);
        }

        protected static Thickness GetDefaultPadding()
        {
            return default(Thickness);
        }

        protected virtual void OnPaddingUpdated(Thickness padding)
        {
            this.ApplyNativePadding(padding);
            this.InvalidateMeasure();
        }

        protected virtual void OnBackgroundChanged()
        {
            this.NativeOnbackgroundChange();
        }

        protected virtual void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            if (newTemplate.TargetType != this.GetType())
            {
                throw new ArgumentException(string.Format("Invalid TargetType expected {0} got {1}", this.GetType(), newTemplate.TargetType));
            }

            if (this.controlTemplateInstance != null)
            {
                this.controlTemplateInstance.LayoutUpdated -= this.OnTemplateInstanceLayoutUpdated;
                this.RemoveControlTemplateInstance();
                this.RemoveLogicalChild(this.controlTemplateInstance);
            }

            if (newTemplate != null)
            {
                this.controlTemplateInstance = (UIElement)newTemplate.LoadContent();
                this.controlTemplateInstance.LayoutUpdated += this.OnTemplateInstanceLayoutUpdated;
                this.AddLogicalChild(this.controlTemplateInstance);
                this.AddControlTemplateInstance();
            }
            else
            {
                this.controlTemplateInstance = null;
                this.NativeUIElement = null;
                this.NativeInit();
            }

            this.OnLayoutUpdated();
        }

        private void OnTemplateInstanceLayoutUpdated(object sender, EventArgs e)
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
    }
}