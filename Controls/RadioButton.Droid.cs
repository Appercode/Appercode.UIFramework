using Android.Graphics.Drawables;
using Android.Views;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Device;
using System;
using System.Drawing;

namespace Appercode.UI.Controls
{
    public partial class RadioButton
    {
        private View androidRadioButton;
        private Drawable androidRadioButtonBackground;
        private float originalContentSize;

        protected override void ApplyNativeContent(object oldContent, object newContent)
        {
            base.ApplyNativeContent(oldContent, newContent);
            if (this.Parent != null && this.Context != null)
            {
                this.androidRadioButton = new View(this.Context);
                this.androidRadioButton.DuplicateParentStateEnabled = true;
                this.androidRadioButtonBackground = RadioButtonDrawableHelper.GetDrawable(this.Context);

                if (((ViewGroup)this.NativeUIElement).IndexOfChild(this.androidRadioButton) < 0)
                {
                    ((ViewGroup)this.NativeUIElement).AddView(this.androidRadioButton);
                    this.androidRadioButton.SetBackgroundDrawable(this.androidRadioButtonBackground);
                }
            }
        }

        protected override void NativeArrangeContent(System.Drawing.RectangleF finalRect)
        {
            var rectContent = new Rectangle();
            var checkBoxWidhPx = ScreenProperties.ConvertPixelsToDPI(this.androidRadioButtonBackground.IntrinsicWidth);
            var y = (finalRect.Height - this.Padding.Top - this.Padding.Bottom - this.originalContentSize) / 2d;
            if (y < 0)
                y = finalRect.Y;
            else
                y += this.Padding.Top;
            rectContent.X = (int)(checkBoxWidhPx + this.Padding.Left);
            rectContent.Y = (int)y;
            rectContent.Width = (int)finalRect.Width;
            rectContent.Height = (int)finalRect.Height;
            base.NativeArrangeContent(rectContent);
        }

        protected override void NativeArrange(RectangleF finalRect)
        {
            base.NativeArrange(finalRect);

            // rendering of the checkmark
            if (this.controlTemplateInstance == null)
            {
                RectangleF rectCheckBox = new RectangleF();
                rectCheckBox.X = 0.0f;
                rectCheckBox.Y = 0.0f;

                rectCheckBox.Width = Math.Min(finalRect.Width, ScreenProperties.ConvertPixelsToDPI(this.androidRadioButtonBackground.IntrinsicWidth));
                rectCheckBox.Height = Math.Min(finalRect.Height, ScreenProperties.ConvertPixelsToDPI(this.androidRadioButtonBackground.IntrinsicHeight));

                this.androidRadioButton.Layout((int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Left),
                                                   (int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Top),
                                                   (int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Right),
                                                   (int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Bottom));
            }
        }

        protected override SizeF NativeMeasureOverride(SizeF availableSize)
        {
            SizeF measuredSize = base.NativeMeasureOverride(availableSize);

            measuredSize.Width += ScreenProperties.ConvertPixelsToDPI(this.androidRadioButtonBackground.IntrinsicWidth);
            measuredSize.Height = Math.Max(measuredSize.Height, ScreenProperties.ConvertPixelsToDPI(this.androidRadioButtonBackground.IntrinsicHeight));

            return new SizeF(measuredSize.Width, measuredSize.Height);
        }

        protected override SizeF NativeMeasureContent(System.Drawing.SizeF availableSize)
        {
            var checkBoxHeightPx = ScreenProperties.ConvertPixelsToDPI(this.androidRadioButtonBackground.IntrinsicHeight);
            SizeF measuredContent = base.NativeMeasureContent(availableSize);
            var width = Math.Min(measuredContent.Width, availableSize.Width);
            this.originalContentSize = Math.Min(measuredContent.Height, availableSize.Height);
            var height = Math.Max(this.originalContentSize, checkBoxHeightPx);
            return new SizeF(width, height);
        }

        protected override System.Windows.Thickness GetNativePadding()
        {
            return new System.Windows.Thickness(0);
        }
    }
}
