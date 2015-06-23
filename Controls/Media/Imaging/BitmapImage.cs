using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls.Media.Imaging
{
    public sealed partial class BitmapImage : BitmapSource
    {
        public static readonly DependencyProperty CreateOptionsProperty =
            DependencyProperty.Register("CreateOptions", typeof(BitmapCreateOptions), typeof(BitmapImage), new PropertyMetadata(default(BitmapCreateOptions)));

        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(BitmapImage), new PropertyMetadata(default(Uri), (d, e) => 
            {
                ((BitmapImage)d).UriChanged((Uri)e.OldValue, (Uri)e.NewValue);
            }));

        public static readonly DependencyProperty DecodePixelHeightProperty =
            DependencyProperty.Register("DecodePixelHeight", typeof(int), typeof(BitmapImage), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty DecodePixelWidthProperty =
            DependencyProperty.Register("DecodePixelWidth", typeof(int), typeof(BitmapImage), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty DecodePixelTypeProperty =
            DependencyProperty.Register("DecodePixelType", typeof(int), typeof(BitmapImage), new PropertyMetadata(default(int)));

        public BitmapImage(Uri uriSource)
        {
            this.UriSource = uriSource;
            this.ImageLoadStatus = ImageStatus.None;
        }

        public event EventHandler<DownloadProgressEventArgs> DownloadProgress = delegate { };

        public event EventHandler<ExceptionRoutedEventArgs> ImageFailed = delegate { };

        public event EventHandler<RoutedEventArgs> ImageOpened = delegate { };

        public BitmapCreateOptions CreateOptions
        {
            get { return (BitmapCreateOptions)this.GetValue(CreateOptionsProperty); }
            set { this.SetValue(CreateOptionsProperty, value); }
        }

        public Uri UriSource
        {
            get { return (Uri)this.GetValue(UriSourceProperty); }
            set { this.SetValue(UriSourceProperty, value); }
        }

        public int DecodePixelHeight
        {
            get { return (int)this.GetValue(DecodePixelHeightProperty); }
            set { this.SetValue(DecodePixelHeightProperty, value); }
        }

        public int DecodePixelWidth
        {
            get { return (int)this.GetValue(DecodePixelWidthProperty); }
            set { this.SetValue(DecodePixelWidthProperty, value); }
        }

        public int DecodePixelType
        {
            get { return (int)this.GetValue(DecodePixelTypeProperty); }
            set { this.SetValue(DecodePixelTypeProperty, value); }
        }

        public override void SetSource(Stream streamSource)
        {
            this.UriSource = null;
            base.SetSource(streamSource);
        }

        public override string ToString()
        {
            return string.Format("[BitmapImage: UriSource={1}, CreateOptions={0}, DecodePixelHeight={2}, DecodePixelWidth={3}, DecodePixelType={4}]", CreateOptions, UriSource, DecodePixelHeight, DecodePixelWidth, DecodePixelType);
        }
    }
}