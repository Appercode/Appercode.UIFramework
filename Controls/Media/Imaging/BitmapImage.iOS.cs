using Foundation;
using System;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls.Media.Imaging
{
    public partial class BitmapImage
    {
        private UIImage image;

        public override UIImage GetUIImage()
        {
            if (this.image == null)
            {
                this.ImageLoadStatus = ImageStatus.Loading;
                if (!this.UriSource.IsAbsoluteUri)
                {
                    this.image = UIImage.FromBundle(string.Format("iOS/{0}", this.UriSource.OriginalString));
                    if (this.image == null)
                    {
                        this.image = UIImage.FromBundle("Assets/" + this.UriSource.OriginalString);
                    }
                }
                else if (this.UriSource.IsFile)
                {
                    this.image = UIImage.FromBundle(this.UriSource.OriginalString);
                }
                else if (this.UriSource.Scheme == Uri.UriSchemeHttp || this.UriSource.Scheme == Uri.UriSchemeHttps)
                {
                    NSUrlRequest req = new NSMutableUrlRequest(this.UriSource);
                    NSOperationQueue queue = new NSOperationQueue();
                    NSUrlConnection.SendAsynchronousRequest(req, queue, (pesponse, data, error) =>
                    {
                        try 
                        {
                            if (error == null && data != null && data.Length != 0)
                            {
                                Dispatcher.BeginInvoke(() =>
                                    {
                                        this.image = UIImage.LoadFromData(data);
                                        if (this.ImageOpened != null)
                                        {
                                            this.ImageOpened(this, new RoutedEventArgs());
                                        }
                                    });
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    if (this.ImageFailed != null)
                                    {
                                        this.ImageFailed(this, new ExceptionRoutedEventArgs());
                                    }
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                this.image = UIImage.LoadFromData(data);
                                if (this.ImageFailed != null)
                                {
                                    this.ImageFailed(this, new ExceptionRoutedEventArgs(e));
                                }
                            });
                        }
                    });
                    return this.image;
                }
                if (this.image == null)
                {
                    this.image = UIImage.FromFile(this.UriSource.OriginalString);
                }
            }
            return this.image;
        }

        private void UriChanged(Uri oldValue, Uri newValue)
        {
        }
    }
}