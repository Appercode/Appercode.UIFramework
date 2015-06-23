using System.Windows;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
#endif

namespace Appercode.UI.Controls
{
    public partial class ListBoxItem : ContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ListBoxItem), new PropertyMetadata(false, (d, e) =>
                {
                    ((ListBoxItem)d).NativeIsSelected = (bool)e.NewValue;
                }));

        public bool IsSelected
        {
            get { return (bool)this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        protected override void ArrangeContent(SizeF finalSize)
        {
            var padding = this.Padding;
            var availableSize = new SizeF(
                finalSize.Width - padding.HorizontalThicknessF() - this.Margin.HorizontalThicknessF(),
                finalSize.Height - padding.VerticalThicknessF() - this.Margin.VerticalThicknessF());
            var contentSize = this.MessureContent(availableSize);

            var contentFrame = new RectangleF(padding.LeftF(), padding.TopF(), contentSize.Width, contentSize.Height);

            this.NativeArrangeContent(contentFrame);
        }
    }
}