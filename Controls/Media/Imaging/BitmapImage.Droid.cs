using Android.App;
using Android.Graphics;
using Java.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using File = Java.IO.File;

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
            if (this.UriSource.IsAbsoluteUri && (this.UriSource.Scheme.ToLower() == "http" || this.UriSource.Scheme.ToLower() == "https"))
            {
                // download the image
                try
                {
                    this.ImageLoadStatus = ImageStatus.Loading;

                    HttpURLConnection connection = (HttpURLConnection)new Java.Net.URL(this.UriSource.ToString()).OpenConnection();
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
                    this.ImageLoadStatus = ImageStatus.Loaded;
                    this.ImageOpened(this, new RoutedEventArgs() { OriginalSource = this });
                }
                catch (Exception e)
                {
                    this.ImageLoadStatus = ImageStatus.Failed;
                    this.ImageFailed(this, new ExceptionRoutedEventArgs(e) { ErrorMessage = "Image can not be opened." });
                }
            }
            else
            {
                string path = this.UriSource.IsAbsoluteUri ? this.UriSource.AbsolutePath : this.UriSource.ToString();

                if(System.IO.File.Exists(path))
                {
                    // load the image from the local storage
                    try
                    {
                        this.ImageLoadStatus = ImageStatus.Loading;
                        this.Image = BitmapFactory.DecodeFile(path, CreateBitmapOptions());
                        this.ImageLoadStatus = ImageStatus.Loaded;
                        this.ImageOpened(this, new RoutedEventArgs() { OriginalSource = this });
                    }
                    catch (Exception e)
                    {
                        this.ImageLoadStatus = ImageStatus.Failed;
                            this.ImageFailed(this, new ExceptionRoutedEventArgs(e) { ErrorMessage = "Image can not be opened." });
                    }

                }
                else
                {
                    this.ImageLoadStatus = ImageStatus.Loading;
                    if (path[0] == '/')
                    {
                        path = path.Substring(1, path.Length - 1);
                    }

                    // try to load the image from Resources
                    var packageName = Application.Context.PackageName.ToLower();
                    var resourceName = string.Format("{0}:drawable/{1}", packageName, path.Split('.')[0].ToLower());
                    var id = Application.Context.Resources.GetIdentifier(resourceName, null, null);
                    if (id != 0)
                    {
                        try
                        {
                            var contentPath = string.Format("android.resource://{0}/drawable/{1}", packageName, id);
                            var androidUrl = Android.Net.Uri.Parse(contentPath);
                            using (var imageStream = Application.Context.ContentResolver.OpenInputStream(androidUrl))
                            {
                                this.Image = BitmapFactory.DecodeStream(imageStream, new Rect(), this.CreateBitmapOptions());
                            }

                            this.ImageLoadStatus = ImageStatus.Loaded;
                            this.ImageOpened(this, new RoutedEventArgs() { OriginalSource = this });
                            return;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.ToString());
                        }
                    }

                    // load the image from Assets
                    try
                    {
                        using (var imageStream = Application.Context.Assets.Open(path))
                        {
                            this.Image = BitmapFactory.DecodeStream(imageStream, new Rect(), this.CreateBitmapOptions());
                        }

                        this.ImageLoadStatus = ImageStatus.Loaded;
                        this.ImageOpened(this, new RoutedEventArgs() { OriginalSource = this });
                    }
                    catch (Exception e)
                    {
                        this.ImageLoadStatus = ImageStatus.Failed;
                        this.ImageFailed(this, new ExceptionRoutedEventArgs(e) { ErrorMessage = "Image can not be opened." });
                    }
                }
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
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    this.DownloadImage();
                });
            }
        }
    }
}