using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.Media;
using Appercode.UI.Controls.NativeControl.Wrapers;
using Appercode.UI.Device;

namespace Appercode.UI.Controls
{
    public class NativeMediaElement : WrapedViewGroup
    {
        public NativeMediaElement(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }
    }

    public partial class MediaElement
    {
        private Uri nativeSource;
        private TimeSpan nativePosition;
        private double nativeVolume;
        private WrapedVideoView videoView;

        private bool isApplyNewSource = false;

        protected Nullable<int> NativeAudioStreamIndex
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
                    this.NativeUIElement = new NativeMediaElement(this.Context);
                    this.videoView = new WrapedVideoView(this.Context);
                    ((NativeMediaElement)this.NativeUIElement).AddView((WrapedVideoView)this.videoView);

                    var layoutParams = new ViewGroup.LayoutParams(0, 0);

                    layoutParams.Width = double.IsNaN(this.NativeWidth) ? ViewGroup.LayoutParams.FillParent : (int)this.NativeWidth;
                    layoutParams.Height = double.IsNaN(this.NativeHeight) ? ViewGroup.LayoutParams.FillParent : (int)this.NativeHeight;
                    this.NativeUIElement.LayoutParameters = layoutParams;
                    ((WrapedVideoView)this.videoView).LayoutParameters = layoutParams;

                    ((WrapedVideoView)this.videoView).InitMediaPlayer();

                    ((WrapedVideoView)this.videoView).VideoSizeChanged -= MediaElement_VideoSizeChanged;
                    ((WrapedVideoView)this.videoView).VideoSizeChanged += MediaElement_VideoSizeChanged;

                    ((WrapedVideoView)this.videoView).BufferingProgressUpdate -= MediaElement_BufferingProgressUpdate;
                    ((WrapedVideoView)this.videoView).BufferingProgressUpdate += MediaElement_BufferingProgressUpdate;

                    if (this.ReadLocalValue(MediaElement.AudioStreamIndexProperty) != DependencyProperty.UnsetValue)
                    {
                        this.ApplyNativeAudioStreamIndex(this.NativeAudioStreamIndex);
                    }

                    if (this.ReadLocalValue(MediaElement.AutoPlayProperty) != DependencyProperty.UnsetValue)
                    {
                        this.ApplyNativeAutoPlay(this.NativeAutoPlay);
                    }

                    if (this.ReadLocalValue(MediaElement.BalanceProperty) != DependencyProperty.UnsetValue)
                    {
                        this.ApplyNativeBalance(this.NativeBalance);
                    }

                    if (this.ReadLocalValue(MediaElement.IsMutedProperty) != DependencyProperty.UnsetValue)
                    {
                        ApplyNativeIsMuted(this.NativeIsMuted);
                    }

                    if (this.ReadLocalValue(MediaElement.PositionProperty) != DependencyProperty.UnsetValue)
                    {
                        this.ApplyNativePosition(this.nativePosition);
                    }

                    if (this.ReadLocalValue(MediaElement.SourceProperty) != DependencyProperty.UnsetValue)
                    {
                        this.ApplyNativeSource(this.NativeSource);
                    }

                    if (this.ReadLocalValue(MediaElement.StretchProperty) != DependencyProperty.UnsetValue)
                    {
                        this.ApplyNativeStretch(this.NativeStretch);
                    }

                    if (this.ReadLocalValue(MediaElement.VolumeProperty) != DependencyProperty.UnsetValue)
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
                return new Duration(new TimeSpan(0, 0, 0, 0, ((WrapedVideoView)this.videoView).Duration));
            }

            if (dp.Name == "NaturalVideoHeight")
            {
                return ((WrapedVideoView)this.videoView).VideoHeight;
            }

            if (dp.Name == "NaturalVideoWidth")
            {
                return ((WrapedVideoView)this.videoView).VideoWidth;
            }

            if (dp.Name == "Position")
            {
                return new TimeSpan(0, 0, 0, 0, ((WrapedVideoView)this.videoView).Position);
            }

            if (dp.Name == "BufferingProgress")
            {
                return ((WrapedVideoView)this.videoView).BufferingProgress;
            }

            if (dp.Name == "CurrentState")
            {
                return this.NativeCurrentState;
            }

            if (dp.Name == "Volume")
            {
                return ((Android.Media.AudioManager)this.Context.GetSystemService(Android.Content.Context.AudioService)).GetStreamVolume(Stream.Music);
            }

            return base.GetValue(dp);
        }

        private static double GetNativeVolumeInitialValue()
        {
            return 0d;
        }

        private void NativePause()
        {
            ((WrapedVideoView)this.videoView).Pause();

            this.NativeCurrentState = MediaElementState.Paused;
            if (this.CurrentStateChanged != null)
            {
                this.CurrentStateChanged(this, new RoutedEventArgs());
            }
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
                            ((WrapedVideoView)this.videoView).Prepare();
                            this.isApplyNewSource = true;
                        }
                    }
                    catch (Exception e)
                    {
                        if (this.MediaFailed != null)
                        {
                            this.MediaFailed(this, new System.Windows.ExceptionRoutedEventArgs(e));
                        }
                    }
                }

                ((WrapedVideoView)this.videoView).Play();

                this.NativeCurrentState = MediaElementState.Playing;
                if (this.CurrentStateChanged != null)
                {
                    this.CurrentStateChanged(this, new RoutedEventArgs());
                }

                ((WrapedVideoView)this.videoView).Completion -= MediaElement_Completion;
                ((WrapedVideoView)this.videoView).Completion += MediaElement_Completion;
            }
        }

        private void NativeStop()
        {
            ((WrapedVideoView)this.videoView).Completion -= MediaElement_Completion;
            ((WrapedVideoView)this.videoView).Stop();
            this.isApplyNewSource = false;

            this.NativeCurrentState = MediaElementState.Stopped;
            if (this.CurrentStateChanged != null)
            {
                this.CurrentStateChanged(this, new RoutedEventArgs());
            }

            if (this.MediaEnded != null)
            {
                this.MediaEnded(this, new RoutedEventArgs());
            }
        }

        private void MediaElement_BufferingProgressUpdate(object sender, EventArgs e)
        {
            this.NativeCurrentState = MediaElementState.Buffering;
            if (this.CurrentStateChanged != null)
            {
                this.CurrentStateChanged(this, new RoutedEventArgs());
            }

            if (this.BufferingProgressChanged != null)
            {
                this.BufferingProgressChanged(this, new RoutedEventArgs());
            }
        }

        private void MediaElement_VideoSizeChanged(object sender, EventArgs e)
        {
            this.OnLayoutUpdated();
        }

        private void MediaElement_Completion(object sender, EventArgs e)
        {
            this.isApplyNewSource = false;

            this.NativeCurrentState = MediaElementState.Stopped;
            if (this.CurrentStateChanged != null)
            {
                this.CurrentStateChanged(this, new RoutedEventArgs());
            }

            if (this.MediaEnded != null)
            {
                this.MediaEnded(this, new RoutedEventArgs());
            }
        }

        private void MediaElement_Prepared(object sender, EventArgs e)
        {
            if (this.MediaOpened != null)
            {
                this.MediaOpened(this, new RoutedEventArgs());
            }

            ((WrapedVideoView)this.videoView).Prepared -= MediaElement_Prepared;
        }

        private void NativeArrangeVideoView(System.Drawing.RectangleF finalRect)
        {
            int left = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Left);
            int top = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Top);
            int right = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Right);
            int bottom = (int)ScreenProperties.ConvertDPIToPixels(finalRect.Bottom);

            ((WrapedVideoView)this.videoView).Layout(left, top, right, bottom);
        }

        private void ApplyNativeAudioStreamIndex(Nullable<int> audioStreamIndex)
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
            ((WrapedVideoView)this.videoView).SeekTo((int)position.TotalMilliseconds);
        }

        private void ApplyNativeSource(Uri source)
        {
            this.NativeCurrentState = MediaElementState.Opening;
            if (this.CurrentStateChanged != null)
            {
                this.CurrentStateChanged(this, new RoutedEventArgs());
            }

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

            if (((WrapedVideoView)this.videoView).IsPlaying)
            {
                this.Stop();
            }

            if (!string.IsNullOrWhiteSpace(this.nativeSource.OriginalString))
            {
                try
                {
                    if (this.Source.IsAbsoluteUri)
                    {
                        ((WrapedVideoView)this.videoView).SetDataSource(this.Source.OriginalString);
                    }
                    else
                    {
                        Android.Content.Res.AssetFileDescriptor afd = this.Context.Assets.OpenFd(this.nativeSource.OriginalString);
                        if (afd != null)
                        {
                            ((WrapedVideoView)this.videoView).SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                        }
                    }

                    ((WrapedVideoView)this.videoView).Prepared -= MediaElement_Prepared;
                    ((WrapedVideoView)this.videoView).Prepared += MediaElement_Prepared;
                    ((WrapedVideoView)this.videoView).Prepare();

                    this.isApplyNewSource = true;

                    // Run playback of the new media
                    if (this.AutoPlay)
                    {
                        this.NativePlay();
                    }
                }
                catch (Exception e)
                {
                    if (this.MediaFailed != null)
                    {
                        this.MediaFailed(this, new System.Windows.ExceptionRoutedEventArgs(e));
                    }
                }
            }
        }

        private void ApplyNativeStretch(Stretch stretch)
        {
        }

        private void ApplyNativeVolume(double volume)
        {
            Android.Media.AudioManager t = (Android.Media.AudioManager)this.Context.GetSystemService(Android.Content.Context.AudioService);
            t.SetStreamVolume(Stream.Music, (int)volume, VolumeNotificationFlags.PlaySound);
        }
    }
}