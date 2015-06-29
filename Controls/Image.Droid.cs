using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.Media.Imaging;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;
using System.Windows;
using Color = Android.Graphics.Color;
using Appercode.UI.Controls.Media;

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
                    WrapedImageView imageView = new WrapedImageView(this.Context);
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
                    case System.Windows.Media.Stretch.None:
                        imageView.SetScaleType(ImageView.ScaleType.Center);
                        break;
                    case System.Windows.Media.Stretch.Uniform:
                        imageView.SetScaleType(ImageView.ScaleType.FitCenter);
                        break;
                    case System.Windows.Media.Stretch.UniformToFill:
                        imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                        break;
                    case System.Windows.Media.Stretch.Fill:
                        imageView.SetScaleType(ImageView.ScaleType.FitXy);
                        break;
                }
            }
        }

        protected override System.Drawing.SizeF NativeMeasureOverride(System.Drawing.SizeF availableSize)
        {
            var isWidthSetByUser = this.ReadLocalValue(UIElement.WidthProperty) != DependencyProperty.UnsetValue ||
                                   this.ReadValueFromStyle(UIElement.WidthProperty) != DependencyProperty.UnsetValue;
            var isHeightSetByUser = this.ReadLocalValue(UIElement.HeightProperty) != DependencyProperty.UnsetValue ||
                                    this.ReadValueFromStyle(UIElement.HeightProperty) != DependencyProperty.UnsetValue;
            if (isHeightSetByUser)
            {
                availableSize.Height = (float)(this.Height + this.Margin.Bottom + this.Margin.Top);
            }

            if (isWidthSetByUser)
            {
                availableSize.Width = (float)(this.Width + this.Margin.Right + this.Margin.Left);
            }

            var measuredSize = base.NativeMeasureOverride(availableSize);

            if (this.Stretch == System.Windows.Media.Stretch.UniformToFill)
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
            if (this.ReadLocalValue(UIElement.HeightProperty) != DependencyProperty.UnsetValue || this.ReadValueFromStyle(UIElement.HeightProperty) != DependencyProperty.UnsetValue)
            {
                var height = (float)(finalRect.Height - this.Margin.Bottom - this.Margin.Top);
                finalRect.Height = height < 0 ? (float)(finalRect.Height + this.Margin.Bottom + this.Margin.Top) : finalRect.Height;
            }
            if (this.ReadLocalValue(UIElement.WidthProperty) != DependencyProperty.UnsetValue || this.ReadValueFromStyle(UIElement.WidthProperty) != DependencyProperty.UnsetValue)
            {
                var width = (float)(finalRect.Width - this.Margin.Right - this.Margin.Left);
                finalRect.Width = width < 0 ? (float)(finalRect.Width + this.Margin.Right + this.Margin.Left) : finalRect.Width;
            }
            base.NativeArrange(finalRect);
        }
    }
}