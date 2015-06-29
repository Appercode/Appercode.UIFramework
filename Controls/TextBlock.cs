using System.Windows;
using System.Windows.Media;

#if __IOS__
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
#endif

namespace Appercode.UI.Controls
{
    public partial class TextBlock : UIElement
    {
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(TextBlock), new PropertyMetadata(GetDefaultFontSize(), (d, e) =>
                {
                    ((TextBlock)d).NativeFontSize = (double)e.NewValue;
                    ((TextBlock)d).InvalidateMeasure();
                }));

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(TextBlock), new PropertyMetadata(null, (d, e) =>
            {
                ((TextBlock)d).NativeFontFamily = (FontFamily)e.NewValue;
                ((TextBlock)d).InvalidateMeasure();
            }));

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(TextBlock), new PropertyMetadata(FontWeights.Normal, (d, e) =>
            {
                ((TextBlock)d).NativeFontWeight = (FontWeight)e.NewValue;
                ((TextBlock)d).InvalidateMeasure();
            }));

        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(TextBlock), new PropertyMetadata(FontStyles.Normal, (d, e) =>
            {
                ((TextBlock)d).NativeFontStyle = (FontStyle)e.NewValue;
                ((TextBlock)d).InvalidateMeasure();
            }));

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(TextBlock), new PropertyMetadata(null, (d, e) =>
                {
                    ((TextBlock)d).NativeForeground = (Brush)e.NewValue;
                }));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBlock), new PropertyMetadata(TextWrapping.NoWrap, (d, e) =>
                {
                    ((TextBlock)d).NativeTextWrapping = (TextWrapping)e.NewValue;
                    ((TextBlock)d).InvalidateMeasure();
                }));

        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(TextBlock), new PropertyMetadata(TextTrimming.None, (d, e) =>
                {
                    ((TextBlock)d).NativeTextTrimming = (TextTrimming)e.NewValue;
                    ((TextBlock)d).InvalidateMeasure();
                }));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextBlock), new PropertyMetadata(default(TextAlignment), (d, e) =>
                {
                    ((TextBlock)d).NativeTextAlignment = (TextAlignment)e.NewValue;
                    ((TextBlock)d).InvalidateMeasure();
                }));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBlock), new PropertyMetadata(string.Empty, (d, e) =>
                {
                    ((TextBlock)d).NativeText = (string)e.NewValue;
                    ((TextBlock)d).InvalidateMeasure();
                }));

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(TextBlock), new PropertyMetadata(new Thickness(), (d, e) =>
                {
                    ((TextBlock)d).NativePadding = (Thickness)e.NewValue;
                    ((TextBlock)d).InvalidateMeasure();
                }));

        public TextBlock()
        {
        }

        public double FontSize
        {
            get { return (double)this.GetValue(TextBlock.FontSizeProperty); }
            set { this.SetValue(TextBlock.FontSizeProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)this.GetValue(FontFamilyProperty); }
            set { this.SetValue(FontFamilyProperty, value); }
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

        public Brush Foreground
        {
            get { return (Brush)this.GetValue(ForegroundProperty); }
            set { this.SetValue(ForegroundProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)this.GetValue(TextWrappingProperty); }
            set { this.SetValue(TextWrappingProperty, value); }
        }

        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)this.GetValue(TextTrimmingProperty); }
            set { this.SetValue(TextTrimmingProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)this.GetValue(PaddingProperty); }
            set { this.SetValue(PaddingProperty, value); }
        }

        internal override SizeF MeasureContentViewPort(SizeF availableSize)
        {
            availableSize.Width -= this.Padding.HorizontalThicknessF();
            availableSize.Height -= this.Padding.VerticalThicknessF();
            return base.MeasureContentViewPort(availableSize);
        }
    }
}
