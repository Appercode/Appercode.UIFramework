using Appercode.UI.Controls.Media.Imaging;
using CoreGraphics;
using System;
using System.Windows;
using UIKit;
using SWM = System.Windows.Media;

namespace Appercode.UI.Controls
{
    public sealed partial class Image
    {
        public override CGSize MeasureOverride(CGSize availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = CGSize.Empty;
            }

            // TODO: size caching
            this.measuredFor = availableSize;
            availableSize = this.SizeThatFitsMaxAndMin(availableSize);
            this.measuredSize = base.MeasureOverride(availableSize);
            if (this.Source == null)
            {
                return this.measuredSize;
            }

            var image = ((UIImageView)this.NativeUIElement).Image;
            if (image == null)
            {
                return this.measuredSize;
            }

            var imageSize = image.Size;

            bool widthIsUnset = this.ReadLocalValue(Image.WidthProperty) == DependencyProperty.UnsetValue && this.ReadValueFromStyle(Image.WidthProperty) == DependencyProperty.UnsetValue;
            bool heightIsUnset = this.ReadLocalValue(Image.HeightProperty) == DependencyProperty.UnsetValue && this.ReadValueFromStyle(Image.HeightProperty) == DependencyProperty.UnsetValue;

            var margin = this.Margin;
            nfloat aspect, widthForImage, heightForImage;
            switch (this.Stretch)
            {
                case SWM.Stretch.Fill:
                    if (widthIsUnset)
                    {
                        this.measuredSize.Width = double.IsInfinity(availableSize.Width) ? margin.HorizontalThicknessF() + imageSize.Width : availableSize.Width;
                    }
                    if (heightIsUnset)
                    {
                        this.measuredSize.Height = double.IsInfinity(availableSize.Height) ? margin.VerticalThicknessF() + imageSize.Height : availableSize.Height;
                    }
                    break;

                case SWM.Stretch.Uniform:
                    aspect = imageSize.Width / imageSize.Height;

                    widthForImage = (nfloat)this.Width;
                    if (widthIsUnset)
                    {
                        widthForImage = margin.HorizontalThicknessF() + imageSize.Width < availableSize.Width ? imageSize.Width : availableSize.Width - margin.HorizontalThicknessF();
                    }

                    heightForImage = (nfloat)this.Height;
                    if (heightIsUnset)
                    {
                        heightForImage = margin.VerticalThicknessF() + imageSize.Height < availableSize.Height ? imageSize.Height : availableSize.Height - margin.VerticalThicknessF();
                    }

                    if (widthIsUnset && !heightIsUnset)
                    {
                        widthForImage = heightForImage * aspect;
                    }
                    if (!widthIsUnset && heightIsUnset)
                    {
                        heightForImage = widthForImage / aspect;
                    }

                    if (imageSize.Width < imageSize.Height)
                    {
                        this.measuredSize.Height = heightForImage + margin.VerticalThicknessF();
                        this.measuredSize.Width = heightForImage * aspect + margin.HorizontalThicknessF();
                    }
                    else
                    {
                        this.measuredSize.Width = widthForImage + margin.HorizontalThicknessF();
                        this.measuredSize.Height = widthForImage / aspect + margin.VerticalThicknessF();
                    }
                    break;
                case SWM.Stretch.UniformToFill:
                    aspect = imageSize.Width / imageSize.Height;

                    widthForImage = (nfloat)this.Width;
                    if (widthIsUnset)
                    {
                        if (!nfloat.IsInfinity(availableSize.Width))
                        {
                            widthForImage = availableSize.Width - margin.HorizontalThicknessF();
                        }
                        else
                        {
                            widthForImage = imageSize.Width;
                        }
                    }

                    heightForImage = (nfloat)this.Height;
                    if (heightIsUnset)
                    {
                        if (!nfloat.IsInfinity(availableSize.Height))
                        {
                            heightForImage = availableSize.Height - margin.VerticalThicknessF();
                        }
                        else
                        {
                            heightForImage = imageSize.Height;
                        }
                    }

                    if (nfloat.IsInfinity(availableSize.Width) && !nfloat.IsInfinity(availableSize.Height))
                    {
                        widthForImage = heightForImage * aspect;
                    }

                    if (!nfloat.IsInfinity(availableSize.Width) && nfloat.IsInfinity(availableSize.Height))
                    {
                        heightForImage = widthForImage / aspect;
                    }

                    this.measuredSize.Height = heightForImage + margin.VerticalThicknessF();
                    this.measuredSize.Width = widthForImage + margin.HorizontalThicknessF();

                    break;
                default:
                    if (widthIsUnset)
                    {
                        this.measuredSize.Width = MathF.Min(margin.HorizontalThicknessF() + imageSize.Width, availableSize.Width);
                    }
                    if (heightIsUnset)
                    {
                        this.measuredSize.Height = MathF.Min(margin.VerticalThicknessF() + imageSize.Height, availableSize.Height);
                    }
                    break;
            }
            return this.measuredSize;
        }

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new UIImageView();
            }
            this.NativeUIElement.ClipsToBounds = true;
            this.ApplyNativeSource();
            this.ApplyNativeStretch();
            base.NativeInit();
        }

        private void ApplyNativeSource()
        {
            var source = this.Source;
            if (source != null)
            {
                ((BitmapImage)source).ImageFailed += (s, e1) =>
                {
                    if (this.ImageFailed != null)
                    {
                        this.ImageFailed(this, e1);
                    }
                };

                ((BitmapImage)source).ImageOpened += (s, e2) =>
                {
                    this.SetImage();
                    this.OnLayoutUpdated();

                    if (this.ImageOpened != null)
                    {
                        this.ImageOpened(this, e2);
                    }
                };
                this.SetImage();
            }
        }

        private void SetImage()
        {
            if (this.NativeUIElement != null)
            {
                var source = this.Source;
                ((UIImageView)this.NativeUIElement).Image = source == null ? null : source.GetUIImage();
            }
        }

        private void ApplyNativeStretch()
        {
            if (this.NativeUIElement != null)
            {
                switch (this.Stretch)
                {
                    case SWM.Stretch.Fill:
                        this.NativeUIElement.ContentMode = UIViewContentMode.ScaleToFill;
                        break;
                    case SWM.Stretch.None:
                        this.NativeUIElement.ContentMode = UIViewContentMode.Center;
                        break;
                    case SWM.Stretch.Uniform:
                        this.NativeUIElement.ContentMode = UIViewContentMode.ScaleAspectFit;
                        break;
                    case SWM.Stretch.UniformToFill:
                        this.NativeUIElement.ContentMode = UIViewContentMode.ScaleAspectFill;
                        break;
                }
            }
        }
    }
}