using Android.Graphics.Drawables;
using Android.Widget;
using Appercode.UI.Controls.Media.Imaging;
using Appercode.UI.Controls.NativeControl.Wrappers;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public sealed partial class Image : UIElement
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var imageView = new WrappedImageView(this);
                    imageView.SetAdjustViewBounds(true);
                    this.NativeUIElement = imageView;
                }

                this.ApplyNativeStretch();
                this.ApplyNativeSource();
                base.NativeInit();
            }
        }

        private void ApplyNativeSource()
        {
            if (this.Source != null)
            {
                ((BitmapImage)this.Source).ImageFailed += this.Image_ImageFailed;

                ((BitmapImage)this.Source).ImageOpened += this.Image_ImageOpened;
                SetImage();
            }
        }

        void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            (sender as BitmapImage).ImageFailed -= this.Image_ImageFailed;
            (sender as BitmapImage).ImageOpened -= this.Image_ImageOpened;
            this.Dispatcher.BeginInvoke(() =>
            {
                this.ImageFailed(this, e);
            });
        }

        void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            (sender as BitmapImage).ImageFailed -= this.Image_ImageFailed;
            (sender as BitmapImage).ImageOpened -= this.Image_ImageOpened;
            this.Dispatcher.BeginInvoke(() =>
            {
                this.SetImage();
                this.OnLayoutUpdated();
                this.ImageOpened(this, e);
            });
        }

        private void SetImage()
        {
            var imageView = this.NativeUIElement as ImageView;

            if (imageView != null && this.Source != null)
            {
                try
                {
                    var drawable = new BitmapDrawable(this.Source.GetBitmap());
                    drawable.SetTargetDensity(this.Context.Resources.DisplayMetrics);
                    imageView.SetImageDrawable(drawable);
                    drawable.Dispose();

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.ImageOpened(this, new RoutedEventArgs() { OriginalSource = this });
                    });
                }
                catch (Exception e)
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.ImageFailed(this, new ExceptionRoutedEventArgs(e) { ErrorMessage = "Image can not be opened." });
                    });
                }
            }
        }

        private void ApplyNativeStretch()
        {
            var imageView = this.NativeUIElement as ImageView;
            if (imageView != null)
            {
                switch (this.Stretch)
                {
                    case Stretch.None:
                        imageView.SetScaleType(ImageView.ScaleType.Center);
                        break;
                    case Stretch.Uniform:
                        imageView.SetScaleType(ImageView.ScaleType.FitCenter);
                        break;
                    case Stretch.UniformToFill:
                        imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                        break;
                    case Stretch.Fill:
                        imageView.SetScaleType(ImageView.ScaleType.FitXy);
                        break;
                }
            }
        }

        protected override SizeF NativeMeasureOverride(SizeF availableSize)
        {
            if (this.ContainsValue(HeightProperty))
            {
                availableSize.Height = (float)this.Height + this.Margin.VerticalThicknessF();
            }

            if (this.ContainsValue(WidthProperty))
            {
                availableSize.Width = (float)this.Width + this.Margin.HorizontalThicknessF();
            }

            var measuredSize = base.NativeMeasureOverride(availableSize);
            if (this.Stretch == Stretch.UniformToFill)
            {
                float k, k1 = float.MaxValue, k2 = float.MaxValue;

                if (measuredSize.Width != 0)
                {
                    k1 = availableSize.Width / measuredSize.Width;
                }
                if (measuredSize.Height != 0)
                {
                    k2 = availableSize.Height / measuredSize.Height;
                }

                k = Math.Min(k1, k2);

                if (k != float.MaxValue)
                {
                    measuredSize.Height *= k;
                    measuredSize.Width *= k;
                }
            }
            return measuredSize;
        }

        protected override void NativeArrange(RectangleF finalRect)
        {
            var margin = this.Margin;
            if (this.ContainsValue(HeightProperty))
            {
                var height = finalRect.Height - margin.VerticalThicknessF();
                finalRect.Height = height < 0 ? finalRect.Height + margin.VerticalThicknessF() : finalRect.Height;
            }

            if (this.ContainsValue(WidthProperty))
            {
                var width = finalRect.Width - margin.HorizontalThicknessF();
                finalRect.Width = width < 0 ? finalRect.Width + margin.HorizontalThicknessF() : finalRect.Width;
            }

            base.NativeArrange(finalRect);
        }
    }
}