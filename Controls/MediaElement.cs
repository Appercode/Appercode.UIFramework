using System;
using System.Windows;
using System.Windows.Media;

#if __IOS__
using PointF = CoreGraphics.CGPoint;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public partial class MediaElement : UIElement
    {
        public static readonly DependencyProperty AudioStreamCountProperty =
            DependencyProperty.Register("AudioStreamCount", typeof(int), typeof(MediaElement), new PropertyMetadata(0));

        public static readonly DependencyProperty AudioStreamIndexProperty =
            DependencyProperty.Register("AudioStreamIndex", typeof(int?), typeof(MediaElement), new PropertyMetadata(new int?(), (d, e) =>
                {
                    ((MediaElement)d).NativeAudioStreamIndex = (int?)e.NewValue;
                }));

        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register("AutoPlay", typeof(bool), typeof(MediaElement), new PropertyMetadata(true, (d, e) =>
                {
                    ((MediaElement)d).NativeAutoPlay = (bool)e.NewValue;
                }));

        public static readonly DependencyProperty BalanceProperty =
            DependencyProperty.Register("Balance", typeof(double), typeof(MediaElement), new PropertyMetadata(.0, (d, e) =>
            {
                ((MediaElement)d).NativeBalance = (double)e.NewValue;
            }));

        public static readonly DependencyProperty BufferingProgressProperty =
            DependencyProperty.Register("BufferingProgress", typeof(double), typeof(MediaElement), new PropertyMetadata(.0));

        public static readonly DependencyProperty CanPauseProperty =
            DependencyProperty.Register("CanPause", typeof(bool), typeof(MediaElement));

        public static readonly DependencyProperty CanSeekProperty =
            DependencyProperty.Register("CanSeek", typeof(bool), typeof(MediaElement));

        public static readonly DependencyProperty CurrentStateProperty =
            DependencyProperty.Register("CurrentState", typeof(MediaElementState), typeof(MediaElement), new PropertyMetadata(0));

        public static readonly DependencyProperty DownloadProgressProperty =
            DependencyProperty.Register("DownloadProgress", typeof(double), typeof(MediaElement), new PropertyMetadata(.0));

        public static readonly DependencyProperty DroppedFramesPerSecondProperty =
            DependencyProperty.Register("DroppedFramesPerSecond", typeof(double), typeof(MediaElement), new PropertyMetadata(.0d));

        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register("IsMuted", typeof(bool), typeof(MediaElement), new PropertyMetadata(false, (d, e) =>
            {
                ((MediaElement)d).NativeIsMuted = (bool)e.NewValue;
            }));

        public static readonly DependencyProperty NaturalDurationProperty =
            DependencyProperty.Register("NaturalDuration", typeof(Duration), typeof(MediaElement));

        public static readonly DependencyProperty NaturalVideoHeightProperty =
            DependencyProperty.Register("NaturalVideoHeight", typeof(int), typeof(MediaElement), new PropertyMetadata(0));

        public static readonly DependencyProperty NaturalVideoWidthProperty =
            DependencyProperty.Register("NaturalVideoWidth", typeof(int), typeof(MediaElement), new PropertyMetadata(0));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(TimeSpan), typeof(MediaElement), new PropertyMetadata(new TimeSpan(), (d, e) =>
            {
                ((MediaElement)d).NativePosition = (TimeSpan)e.NewValue;
            }));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(MediaElement), new PropertyMetadata(null, (d, e) =>
            {
                ((MediaElement)d).NativeSource = (Uri)e.NewValue;
            }));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(MediaElement), new PropertyMetadata(Stretch.Uniform, (d, e) =>
            {
                ((MediaElement)d).NativeStretch = (Stretch)e.NewValue;
            }));

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(MediaElement), new PropertyMetadata(GetNativeVolumeInitialValue(), (d, e) =>
            {
                ((MediaElement)d).NativeVolume = (double)e.NewValue;
            }));

        public MediaElement()
        {
        }

        public event RoutedEventHandler BufferingProgressChanged = delegate { };

        public event RoutedEventHandler CurrentStateChanged = delegate { };

        public event RoutedEventHandler DownloadProgressChanged = delegate { };

        public event RoutedEventHandler MediaEnded = delegate { };

        public event EventHandler<ExceptionRoutedEventArgs> MediaFailed = delegate { };

        public event RoutedEventHandler MediaOpened = delegate { };

        public int AudioStreamCount
        {
            get { return (int)this.GetValue(AudioStreamCountProperty); }
            private set { this.SetValue(AudioStreamCountProperty, value); }
        }

        public int? AudioStreamIndex
        {
            get { return (int?)this.GetValue(AudioStreamIndexProperty); }
            set { this.SetValue(AudioStreamIndexProperty, value); }
        }

        public bool AutoPlay
        {
            get { return (bool)this.GetValue(AutoPlayProperty); }
            set { this.SetValue(AutoPlayProperty, value); }
        }

        public double Balance
        {
            get { return (double)this.GetValue(BalanceProperty); }
            set { this.SetValue(BalanceProperty, value); }
        }

        public double BufferingProgress
        {
            get { return (double)this.GetValue(BufferingProgressProperty); }
            private set { this.SetValue(BufferingProgressProperty, value); }
        }

        public bool CanPause
        {
            get { return (bool)this.GetValue(CanPauseProperty); }
            private set { this.SetValue(CanPauseProperty, value); }
        }

        public bool CanSeek
        {
            get { return (bool)this.GetValue(CanSeekProperty); }
            private set { this.SetValue(CanSeekProperty, value); }
        }

        public MediaElementState CurrentState
        {
            get { return (MediaElementState)this.GetValue(CurrentStateProperty); }
            private set { this.SetValue(CurrentStateProperty, value); }
        }

        public double DownloadProgress
        {
            get { return (double)this.GetValue(DownloadProgressProperty); }
            private set { this.SetValue(DownloadProgressProperty, value); }
        }

        public double DroppedFramesPerSecond
        {
            get { return (double)this.GetValue(DroppedFramesPerSecondProperty); }
            private set { this.SetValue(DroppedFramesPerSecondProperty, value); }
        }

        public bool IsMuted
        {
            get { return (bool)this.GetValue(IsMutedProperty); }
            set { this.SetValue(IsMutedProperty, value); }
        }

        public Duration NaturalDuration
        {
            get { return (Duration)this.GetValue(NaturalDurationProperty); }
            private set { this.SetValue(NaturalDurationProperty, value); }
        }

        public int NaturalVideoHeight
        {
            get { return (int)this.GetValue(NaturalVideoHeightProperty); }
            private set { this.SetValue(NaturalVideoHeightProperty, value); }
        }

        public int NaturalVideoWidth
        {
            get { return (int)this.GetValue(NaturalVideoWidthProperty); }
            private set { this.SetValue(NaturalVideoWidthProperty, value); }
        }

        public TimeSpan Position
        {
            get { return (TimeSpan)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        public Uri Source
        {
            get { return (Uri)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch)this.GetValue(StretchProperty); }
            set { this.SetValue(StretchProperty, value); }
        }

        public double Volume
        {
            get { return (double)this.GetValue(VolumeProperty); }
            set { this.SetValue(VolumeProperty, value); }
        }

        public void Pause()
        {
            if (this.NativeUIElement != null)
            {
                this.NativePause();
            }
        }

        public void Play()
        {
            if (this.NativeUIElement != null)
            {
                this.NativePlay();
            }
        }

        public void Stop()
        {
            if (this.NativeUIElement != null)
            {
                this.NativeStop();
            }
        }

        /*public void SetSource(MediaStreamSource mediaStreamSource)
        {
            if (this.NativeUIElement != null)
            {
                this.NativeSetSource(mediaStreamSource);
            }
        }

        public void SetSource(Stream stream)
        {
            if (this.NativeUIElement != null)
            {
                this.NativeSetSource(stream);
            }
        }*/

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.NaturalVideoHeight == 0 || this.NaturalVideoWidth == 0)
            {
                return base.MeasureOverride(availableSize);
            }

            nfloat height;
            nfloat width;
            if (this.ContainsValue(HeightProperty))
            {
                if (this.ContainsValue(WidthProperty))
                {
                    return base.MeasureOverride(availableSize);
                }

                height = (nfloat)this.Height;
                width = this.NaturalVideoWidth;
            }
            else
            {
                if (this.ContainsValue(WidthProperty))
                {
                    width = (nfloat)this.Width;
                    height = this.NaturalVideoHeight;
                }
                else
                {
                    width = this.NaturalVideoWidth;
                    height = this.NaturalVideoHeight;
                }
            }

            return base.MeasureOverride(new SizeF(width, height));
        }

        public override void Arrange(RectangleF finalRect)
        {
            base.Arrange(finalRect);

            switch (this.Stretch)
            {
                case Stretch.Fill:
                    {
                        this.ArrangeVideoView(new RectangleF(PointF.Empty, finalRect.Size));
                        break;
                    }
                case Stretch.None:
                    {
                        RectangleF videoFinalRect = new RectangleF();
                        videoFinalRect.X = (finalRect.Right - finalRect.Left - this.NaturalVideoWidth) / 2;
                        videoFinalRect.Y = (finalRect.Bottom - finalRect.Top - this.NaturalVideoHeight) / 2;
                        videoFinalRect.Width = this.NaturalVideoWidth;
                        videoFinalRect.Height = this.NaturalVideoHeight;
                        this.ArrangeVideoView(videoFinalRect);

                        break;
                    }
                case Stretch.Uniform:
                    {
                        RectangleF videoFinalRect = new RectangleF();

                        var heightFactor = finalRect.Height / this.NaturalVideoHeight;
                        var widthFactor = finalRect.Width / this.NaturalVideoWidth;

                        if (heightFactor > widthFactor)
                        {
                            videoFinalRect.Width = finalRect.Width;
                            videoFinalRect.Height = this.NaturalVideoHeight * widthFactor;
                            videoFinalRect.X = 0f;
                            videoFinalRect.Y = (finalRect.Bottom - finalRect.Top - videoFinalRect.Height) / 2;
                        }
                        else
                        {
                            videoFinalRect.Height = finalRect.Height;
                            videoFinalRect.Width = this.NaturalVideoWidth * heightFactor;
                            videoFinalRect.X = (finalRect.Right - finalRect.Left - videoFinalRect.Width) / 2;
                            videoFinalRect.Y = 0f;
                        }

                        this.ArrangeVideoView(videoFinalRect);
                        break;
                    }
                case Stretch.UniformToFill:
                    {
                        RectangleF videoFinalRect = new RectangleF();

                        var heightFactor = finalRect.Height / this.NaturalVideoHeight;
                        var widthFactor = finalRect.Width / this.NaturalVideoWidth;

                        if (heightFactor > widthFactor)
                        {
                            videoFinalRect.Height = finalRect.Height;
                            videoFinalRect.Width = this.NaturalVideoWidth * heightFactor;
                            videoFinalRect.X = (finalRect.Right - finalRect.Left - videoFinalRect.Width) / 2;
                            videoFinalRect.Y = 0f;
                        }
                        else
                        {
                            videoFinalRect.Width = finalRect.Width;
                            videoFinalRect.Height = this.NaturalVideoHeight * widthFactor;
                            videoFinalRect.X = 0f;
                            videoFinalRect.Y = (finalRect.Bottom - finalRect.Top - videoFinalRect.Height) / 2;
                        }

                        this.ArrangeVideoView(videoFinalRect);
                        break;
                    }
            }
        }

        private void ArrangeVideoView(RectangleF finalRect)
        {
            this.NativeArrangeVideoView(finalRect);
        }
    }
}