using Appercode.UI.Controls.Media.Imaging;
using System;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class BitmapIcon
    {
        private BitmapImage bitmapImage;
        private UIBarButtonItem item;

        protected internal override UIBarButtonItem GetNativeItem()
        {
            this.item = new UIBarButtonItem();
            this.SetUIImage(this.UriSource);
            return this.item;
        }

        private void SetUIImage(Uri source)
        {
            if (source != null)
            {
                if (this.bitmapImage == null)
                {
                    this.bitmapImage = new BitmapImage(source);
                    this.bitmapImage.ImageOpened += this.ImageOpened;
                }
                else
                {
                    this.bitmapImage.UriSource = source;
                }

                this.item.Image = this.bitmapImage.GetUIImage();
            }
            else
            {
                this.item.Image = null;
            }
        }

        private void ImageOpened(object sender, RoutedEventArgs e)
        {
            var bitmapImage = sender as BitmapImage;
            if (bitmapImage != null)
            {
                bitmapImage.ImageOpened -= this.ImageOpened;
                if (this.item != null)
                {
                    this.item.Image = bitmapImage.GetUIImage();
                }
            }
        }

        partial void UriSourceChanged(Uri newValue)
        {
            if (this.item != null)
            {
                this.SetUIImage(newValue);
            }
        }
    }
}