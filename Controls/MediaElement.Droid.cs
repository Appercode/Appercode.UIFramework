using Android.Content;
using Android.Media;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrappers;
using Appercode.UI.Device;
using System;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class MediaElement
    {
        private Uri nativeSource;
        private TimeSpan nativePosition;
        private double nativeVolume;
        private WrappedVideoView videoView;
        private bool isApplyNewSource = false;

        protected int? NativeAudioStreamIndex
        {
            get
            {
                return this.AudioStreamIndex;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeAudioStreamIndex(value);
                }
            }
        }

        protected bool NativeAutoPlay
        {
            get
            {
                return this.AutoPlay;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeAutoPlay(value);
                }
            }
        }

        protected double NativeBalance
        {
            get
            {
                return this.Balance;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeBalance(value);
                }
            }
        }

        protected bool NativeIsMuted
        {
            get
            {
                return this.IsMuted;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeIsMuted(value);
                }
            }
        }

        protected TimeSpan NativePosition
        {
            get
            {
                return this.Position;
            }
            set
            {
                this.nativePosition = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativePosition(value);
                }
            }
        }

        protected Uri NativeSource
        {
            get
            {
                return this.Source;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeSource(value);
                }
            }
        }

        protected Stretch NativeStretch
        {
            get
            {
                return this.Stretch;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeStretch(value);
                }
            }
        }

        protected double NativeVolume
        {
            get
            {
                return this.Volume;
            }
            set
            {
                this.nativeVolume = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeVolume(value);
                }
            }
        }

        protected MediaElementState NativeCurrentState
        {
            get;
            set;
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeView = new WrappedViewGroup(this);
                    this.videoView = new WrappedVideoView(this);
                    nativeView.AddView(this.videoView);
                    this.NativeUIElement = nativeView;

                    var layoutParams = new ViewGroup.LayoutParams(
                        double.IsNaN(this.NativeWidth) ? ViewGroup.LayoutParams.MatchParent : (int)this.NativeWidth,
                        double.IsNaN(this.NativeHeight) ? ViewGroup.LayoutParams.MatchParent : (int)this.NativeHeight);
                    this.NativeUIElement.LayoutParameters = layoutParams;
                    this.videoView.LayoutParameters = layoutParams;

                    this.videoView.InitMediaPlayer();
                    this.videoView.VideoSizeChanged += MediaElement_VideoSizeChanged;
                    this.videoView.BufferingProgressUpdate += MediaElement_BufferingProgressUpdate;

                    if (this.ContainsValue(AudioStreamIndexProperty))
                    {
                        this.ApplyNativeAudioStreamIndex(this.NativeAudioStreamIndex);
                    }

                    if (this.ContainsValue(AutoPlayProperty))
                    {
                        this.ApplyNativeAutoPlay(this.NativeAutoPlay);
                    }

                    if (this.ContainsValue(BalanceProperty))
                    {
                        this.ApplyNativeBalance(this.NativeBalance);
                    }

                    if (this.ContainsValue(IsMutedProperty))
                    {
                        ApplyNativeIsMuted(this.NativeIsMuted);
                    }

                    if (this.ContainsValue(PositionProperty))
                    {
                        this.ApplyNativePosition(this.nativePosition);
                    }

                    if (this.ContainsValue(SourceProperty))
                    {
                        this.ApplyNativeSource(this.NativeSource);
                    }

                    if (this.ContainsValue(StretchProperty))
                    {
                        this.ApplyNativeStretch(this.NativeStretch);
                    }

                    if (this.ContainsValue(VolumeProperty))
                    {
                        this.ApplyNativeVolume(this.nativeVolume);
                    }
                }

                base.NativeInit();
            }
        }

        public override object GetValue(DependencyProperty dp)
        {
            if (dp.Name == "NaturalDuration")
            {
                return new Duration(new TimeSpan(0, 0, 0, 0, this.videoView.Duration));
            }

            if (dp.Name == "NaturalVideoHeight")
            {
                return this.videoView.VideoHeight;
            }

            if (dp.Name == "NaturalVideoWidth")
            {
                return this.videoView.VideoWidth;
            }

            if (dp.Name == "Position")
            {
                return new TimeSpan(0, 0, 0, 0, this.videoView.Position);
            }

            if (dp.Name == "BufferingProgress")
            {
                return this.videoView.BufferingProgress;
            }

            if (dp.Name == "CurrentState")
            {
                return this.NativeCurrentState;
            }

            if (dp.Name == "Volume")
            {
                return ((AudioManager)this.Context.GetSystemService(Context.AudioService)).GetStreamVolume(Stream.Music);
            }

            return base.GetValue(dp);
        }

        private static double GetNativeVolumeInitialValue()
        {
            return 0d;
        }

        private void NativePause()
        {
            this.videoView.Pause();
            this.NativeCurrentState = MediaElementState.Paused;
            this.CurrentStateChanged?.Invoke(this, new RoutedEventArgs());
        }

        private void NativePlay()
        {
            if (this.NativeUIElement != null && !string.IsNullOrWhiteSpace(this.Source.OriginalString))
            {
                if (!this.isApplyNewSource)
                {
                    try
                    {
                        if (this.NativeUIElement != null && !string.IsNullOrWhiteSpace(this.Source.OriginalString))
                        {
                            this.videoView.Prepare();
                            this.isApplyNewSource = true;
                        }
                    }
                    catch (Exception e)
                    {
                        this.MediaFailed?.Invoke(this, new ExceptionRoutedEventArgs(e));
                    }
                }

                this.videoView.Play();
                this.NativeCurrentState = MediaElementState.Playing;
                this.CurrentStateChanged?.Invoke(this, new RoutedEventArgs());
                this.videoView.Completion -= MediaElement_Completion;
                this.videoView.Completion += MediaElement_Completion;
            }
        }

        private void NativeStop()
        {
            this.videoView.Completion -= MediaElement_Completion;
            this.videoView.Stop();
            this.isApplyNewSource = false;

            this.NativeCurrentState = MediaElementState.Stopped;
            this.CurrentStateChanged?.Invoke(this, new RoutedEventArgs());
            this.MediaEnded?.Invoke(this, new RoutedEventArgs());
        }

        private void MediaElement_BufferingProgressUpdate(object sender, EventArgs e)
        {
            this.NativeCurrentState = MediaElementState.Buffering;
            this.CurrentStateChanged?.Invoke(this, new RoutedEventArgs());
            this.BufferingProgressChanged?.Invoke(this, new RoutedEventArgs());
        }

        private void MediaElement_VideoSizeChanged(object sender, EventArgs e)
        {
            this.OnLayoutUpdated();
        }

        private void MediaElement_Completion(object sender, EventArgs e)
        {
            this.isApplyNewSource = false;
            this.NativeCurrentState = MediaElementState.Stopped;
            this.CurrentStateChanged?.Invoke(this, new RoutedEventArgs());
            this.MediaEnded?.Invoke(this, new RoutedEventArgs());
        }

        private void MediaElement_Prepared(object sender, EventArgs e)
        {
            this.MediaOpened?.Invoke(this, new RoutedEventArgs());
            this.videoView.Prepared -= MediaElement_Prepared;
        }

        private void NativeArrangeVideoView(System.Drawing.RectangleF finalRect)
        {
            int left = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Left);
            int top = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Top);
            int right = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Right);
            int bottom = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Bottom);
            this.videoView.Layout(left, top, right, bottom);
        }

        private void ApplyNativeAudioStreamIndex(int? audioStreamIndex)
        {
        }

        private void ApplyNativeAutoPlay(bool autoPlay)
        {
        }

        private void ApplyNativeBalance(double balance)
        {
        }

        private void ApplyNativeIsMuted(bool isMuted)
        {
        }

        private void ApplyNativePosition(TimeSpan position)
        {
            this.videoView.SeekTo((int)position.TotalMilliseconds);
        }

        private void ApplyNativeSource(Uri source)
        {
            this.NativeCurrentState = MediaElementState.Opening;
            this.CurrentStateChanged?.Invoke(this, new RoutedEventArgs());

            int indBegin = source.OriginalString.IndexOf("Assets");
            int indEnd = 5;
            string src = source.OriginalString;

            if (indBegin != -1)
            {
                src = src.Substring(indBegin + 7, indEnd - indBegin - 6);
            }

            if (src[0] == '/')
            {
                src = src.Substring(1, src.Length);
            }

            if (source.IsAbsoluteUri)
            {
                this.nativeSource = new Uri(src, UriKind.Absolute);
            }
            else
            {
                this.nativeSource = new Uri(src, UriKind.Relative);
            }

            if (this.videoView.IsPlaying)
            {
                this.Stop();
            }

            if (!string.IsNullOrWhiteSpace(this.nativeSource.OriginalString))
            {
                try
                {
                    if (this.Source.IsAbsoluteUri)
                    {
                        this.videoView.SetDataSource(this.Source.OriginalString);
                    }
                    else
                    {
                        var afd = this.Context.Assets.OpenFd(this.nativeSource.OriginalString);
                        if (afd != null)
                        {
                            this.videoView.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                        }
                    }

                    this.videoView.Prepared -= MediaElement_Prepared;
                    this.videoView.Prepared += MediaElement_Prepared;
                    this.videoView.Prepare();

                    this.isApplyNewSource = true;

                    // Run playback of the new media
                    if (this.AutoPlay)
                    {
                        this.NativePlay();
                    }
                }
                catch (Exception e)
                {
                    this.MediaFailed?.Invoke(this, new ExceptionRoutedEventArgs(e));
                }
            }
        }

        private void ApplyNativeStretch(Stretch stretch)
        {
        }

        private void ApplyNativeVolume(double volume)
        {
            var t = (AudioManager)this.Context.GetSystemService(Context.AudioService);
            t.SetStreamVolume(Stream.Music, (int)volume, VolumeNotificationFlags.PlaySound);
        }
    }
}