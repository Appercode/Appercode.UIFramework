using Android.App;
using Android.Graphics;
using Java.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Appercode.UI.Controls.Media.Imaging
{
    public sealed partial class BitmapImage
    {
        private Bitmap Image
        {
            get;
            set;
        }

        private void DownloadImage()
        {
            try
            {
                this.ImageLoadStatus = ImageStatus.Loading;
                var uriSource = this.UriSource;
                if (uriSource.IsAbsoluteUri
                    && (string.Equals(uriSource.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(uriSource.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)))
                {
                    // download the image
                    var connection = (HttpURLConnection)new URL(uriSource.ToString()).OpenConnection();
                    connection.Connect();
                    Stream dataStream = connection.InputStream;

                    byte[] buf = new byte[1024];
                    byte[] outData;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        long length = connection.ContentLength;
                        int percent = 0;
                        int oldPercent = 0;
                        int completeByte = 0;

                        while ((read = dataStream.Read(buf, 0, buf.Length)) > 0)
                        {
                            oldPercent = percent;
                            ms.Write(buf, 0, read);
                            completeByte += buf.Length;
                            percent = (int)((double)completeByte / (double)length * 100.0);

                            if (oldPercent != percent)
                            {
                                this.DownloadProgress(this, new DownloadProgressEventArgs(percent));
                            }
                        }

                        percent = 100;
                        this.DownloadProgress(this, new DownloadProgressEventArgs(percent));

                        outData = ms.ToArray();
                    }

                    this.Image = BitmapFactory.DecodeByteArray(outData, 0, outData.Length, CreateBitmapOptions());
                }
                else
                {
                    var path = uriSource.IsAbsoluteUri ? uriSource.AbsolutePath : uriSource.ToString();
                    if (File.Exists(path))
                    {
                        // load the image from the local storage
                        this.Image = BitmapFactory.DecodeFile(path, CreateBitmapOptions());
                    }
                    else
                    {
                        if (System.IO.Path.IsPathRooted(path))
                        {
                            path = path.Substring(1);
                        }

                        // try to load the image from Resources
                        var context = Application.Context;
                        var resourceName = System.IO.Path.ChangeExtension(path, string.Empty).TrimEnd('.').ToLower();
                        var id = context.Resources.GetIdentifier(resourceName, "drawable", context.PackageName);
                        if (id != 0)
                        {
                            var contentPath = string.Format("android.resource://{0}/drawable/{1}", context.PackageName, id);
                            var androidUrl = Android.Net.Uri.Parse(contentPath);
                            using (var imageStream = context.ContentResolver.OpenInputStream(androidUrl))
                            {
                                this.Image = BitmapFactory.DecodeStream(imageStream, new Rect(), this.CreateBitmapOptions());
                            }
                        }
                        else
                        {
                            // load the image from Assets
                            using (var imageStream = context.Assets.Open(path))
                            {
                                this.Image = BitmapFactory.DecodeStream(imageStream, new Rect(), this.CreateBitmapOptions());
                            }
                        }
                    }
                }

                this.ImageLoadStatus = ImageStatus.Loaded;
                this.ImageOpened?.Invoke(this, new RoutedEventArgs { OriginalSource = this });
            }
            catch (Exception e)
            {
                this.ImageLoadStatus = ImageStatus.Failed;
                this.ImageFailed?.Invoke(this, new ExceptionRoutedEventArgs(e) { ErrorMessage = "Image can not be opened." });
            }
        }

        private BitmapFactory.Options CreateBitmapOptions()
        {
            return new BitmapFactory.Options
            {
                InDither = false,
                InPurgeable = true,
                InInputShareable = true,
                InTempStorage = new byte[32 * 1024]
            };
        }

        public override Bitmap GetBitmap()
        {
            return this.Image;
        }

        private void UriChanged(Uri oldValue, Uri newValue)
        {
            if (this.ImageLoadStatus != ImageStatus.Loading)
            {
                // TODO: change loading logic to take in account CreateOptions property
                Task.Run((Action)this.DownloadImage);
            }
        }
    }
}