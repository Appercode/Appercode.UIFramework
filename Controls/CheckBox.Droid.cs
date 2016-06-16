using Android.Graphics.Drawables;
using Android.Views;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Device;
using System;
using System.Drawing;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class CheckBox
    {
        private View androidCheckBox;
        private Drawable androidCheckBoxBackground;
        private float originalContentSize;

        protected override void ApplyNativeContent(object oldContent, object newContent)
        {
            base.ApplyNativeContent(oldContent, newContent);
            if (this.Parent != null && this.Context != null)
            {
                if (this.androidCheckBox == null)
                {
                    this.androidCheckBox = new View(this.Context);
                    this.androidCheckBox.DuplicateParentStateEnabled = true;
                    this.androidCheckBoxBackground = CheckBoxDrawableHelper.GetDrawable(this.Context);
                    this.NativeIsChecked = this.IsChecked;
                }

                var nativeView = (ViewGroup)this.NativeUIElement;
                if (nativeView.IndexOfChild(this.androidCheckBox) < 0)
                {
                    nativeView.AddView(this.androidCheckBox);
                    this.androidCheckBox.Background = this.androidCheckBoxBackground;
                }
            }
        }

        protected override void NativeArrangeContent(RectangleF finalRect)
        {
            var rectContent = new Rectangle();
            var checkBoxWidhPx = this.androidCheckBoxBackground!=null?ScreenProperties.ConvertPixelsToDPI(this.androidCheckBoxBackground.IntrinsicWidth):0;
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

            if (this.controlTemplateInstance == null)
            {
                // Rendering of the checkmark
                RectangleF rectCheckBox = new RectangleF();
                rectCheckBox.X = 0.0f;
                rectCheckBox.Y = 0.0f;

                rectCheckBox.Width = Math.Min(finalRect.Width, ScreenProperties.ConvertPixelsToDPI(this.androidCheckBoxBackground.IntrinsicWidth));
                rectCheckBox.Height = Math.Min(finalRect.Height, ScreenProperties.ConvertPixelsToDPI(this.androidCheckBoxBackground.IntrinsicHeight));

                this.androidCheckBox.Layout((int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Left),
                                            (int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Top),
                                            (int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Right),
                                            (int)ScreenProperties.ConvertDPIToPixels(rectCheckBox.Bottom));
            }
        }

        protected override SizeF NativeMeasureOverride(SizeF availableSize)
        {
            SizeF measuredSize = base.NativeMeasureOverride(availableSize);

            measuredSize.Width += ScreenProperties.ConvertPixelsToDPI(this.androidCheckBoxBackground.IntrinsicWidth);
            measuredSize.Height = Math.Max(measuredSize.Height, ScreenProperties.ConvertPixelsToDPI(this.androidCheckBoxBackground.IntrinsicHeight));

            return new SizeF(measuredSize.Width, measuredSize.Height);
        }

        protected override SizeF NativeMeasureContent(SizeF availableSize)
        {
            var checkBoxHeightPx = ScreenProperties.ConvertPixelsToDPI(this.androidCheckBoxBackground.IntrinsicHeight);
            SizeF measuredContent = base.NativeMeasureContent(availableSize);
            var width = Math.Min(measuredContent.Width, availableSize.Width);
            this.originalContentSize = Math.Min(measuredContent.Height, availableSize.Height);
            var height = Math.Max(this.originalContentSize, checkBoxHeightPx);
            return new SizeF(width, height);
        }

        protected override Thickness GetNativePadding()
        {
            return default(Thickness);
        }
    }
}