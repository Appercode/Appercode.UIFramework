using CoreGraphics;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class CheckBox
    {
        protected internal override void NativeInit()
        {
            if (Parent == null)
            {
                return;
            }
            if (this.NativeUIElement == null)
            {
                var b = new UIButton();

                b.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                b.VerticalAlignment = UIControlContentVerticalAlignment.Top;

                UIImage selectedImage = null;
                UIImage unselectedImage = null;

                if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) >= 7)
                {
                    selectedImage = UIImage.FromBundle("iOS_Resources/CheckBoxChecked").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    unselectedImage = UIImage.FromBundle("iOS_Resources/CheckBoxUnchecked").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                }
                else
                {
                    selectedImage = UIImage.FromBundle("iOS_Resources/CheckBoxCheckediOS6");
                    unselectedImage = UIImage.FromBundle("iOS_Resources/CheckBoxUncheckediOS6");
                }

                b.SetImage(unselectedImage, UIControlState.Normal);
                b.SetImage(selectedImage, UIControlState.Selected);
                b.SetImage(selectedImage, UIControlState.Highlighted);
                b.SetImage(selectedImage, UIControlState.Selected | UIControlState.Highlighted);
                b.SetTitleColor(UIColor.Black, UIControlState.Normal);
                b.TitleEdgeInsets = new UIEdgeInsets(0, 6, 0, 0);

                this.NativeUIElement = b;
            }

            base.NativeInit();
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var size = base.NativeMeasureOverride(availableSize);
            if (this.ReadLocalValue(CheckBox.HeightProperty) == DependencyProperty.UnsetValue && size.Height < 22)
            {
                size.Height = 22;
            }
            if (this.ReadLocalValue(CheckBox.WidthProperty) == DependencyProperty.UnsetValue)
            {
                size.Width += 34;
            }
            return size;
        }

        protected override void ArrangeContent(CGSize finalSize)
        {
            Thickness nativePadding = this.GetNativePadding();

            var padding = this.Padding;
            var availableSize = new CGSize(
                finalSize.Width - padding.HorizontalThicknessF() - this.Margin.HorizontalThicknessF(),
                finalSize.Height - padding.VerticalThicknessF() - this.Margin.VerticalThicknessF());
            var contentSize = this.MessureContent(availableSize);

            var checkImageWidth = this.NativeUIElement is UIButton ? ((UIButton)this.NativeUIElement).ImageView.Frame.Width : 0;
            var contentFrame = new CGRect(padding.Left + nativePadding.Left + checkImageWidth,
                padding.Top + (availableSize.Height - contentSize.Height) / 2,
                contentSize.Width, contentSize.Height);
            this.NativeArrangeContent(contentFrame);
        }

        protected override Thickness GetNativePadding()
        {
            return new Thickness(6, 0, 0, 0);
        }
    }
}
