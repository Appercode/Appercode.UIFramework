using CoreGraphics;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class RadioButton
    {
        protected internal override void NativeInit()
        {
            if (Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    var b = new UIRadioButton(UIButtonType.Custom);

                    b.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;

                    UIImage selectedImage = null;
                    UIImage unselectedImage = null;

                    if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) >= 7)
                    {
                        selectedImage = UIImage.FromBundle("iOS_Resources/RadioButtonChecked").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                        unselectedImage = UIImage.FromBundle("iOS_Resources/RadioButtonUnchecked").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    }
                    else
                    {
                        selectedImage = UIImage.FromBundle("iOS_Resources/RadioButtonCheckediOS6");
                        unselectedImage = UIImage.FromBundle("iOS_Resources/RadioButtonUncheckediOS6");
                    }

                    b.SetImage(unselectedImage, UIControlState.Normal);
                    b.SetImage(selectedImage, UIControlState.Selected);
                    b.SetImage(selectedImage, UIControlState.Highlighted);
                    b.SetImage(selectedImage, UIControlState.Selected | UIControlState.Highlighted);
                    b.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    b.TitleEdgeInsets = new UIEdgeInsets(0, 6, 0, 0);
                    b.SetTitle("", UIControlState.Normal);

                    this.NativeUIElement = b;
                }

                base.NativeInit();
            }
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var size = base.NativeMeasureOverride(availableSize);
            if (this.ReadLocalValue(RadioButton.HeightProperty) == DependencyProperty.UnsetValue && size.Height < 22)
            {
                size.Height = 22;
            }
            if (this.ReadLocalValue(RadioButton.WidthProperty) == DependencyProperty.UnsetValue)
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

            var radioImageWidth = this.NativeUIElement is UIButton ? ((UIButton)this.NativeUIElement).ImageView.Frame.Width : 0;
            var contentFrame = new CGRect(
                padding.LeftF() + nativePadding.LeftF() + radioImageWidth,
                padding.TopF() + (availableSize.Height - contentSize.Height) / 2,
                contentSize.Width - padding.RightF(),
                contentSize.Height);
            this.NativeArrangeContent(contentFrame);
        }

        protected override Thickness GetNativePadding()
        {
            return new Thickness(6, .5, 0, 1);
        }

        private class UIRadioButton : UIButton
        {
            public UIRadioButton(UIButtonType type)
                : base(type)
            {
            }

            public override string Description
            {
                get
                {
                    return "RadioButton " + base.Description;
                }
            }
        }
    }
}