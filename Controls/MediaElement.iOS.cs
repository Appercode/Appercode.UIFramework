using AVFoundation;
using CoreGraphics;
using Foundation;
using MediaPlayer;
using System;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class MediaElement
    {
        private const double RefreshInterval = 250.0;

        private readonly Timer invalidationTimer = new Timer() { Interval = RefreshInterval };
        private MPMoviePlayerController mpc;
        private double? volumeBeforeMuted;

        protected bool NativeAutoPlay
        {
            get
            {
                return this.mpc.ShouldAutoplay;
            }
            set
            {
                this.mpc.ShouldAutoplay = value;
            }
        }

        protected Duration NativeDuration
        {
            get
            {
                var time = this.mpc.Duration;
                if (double.IsNaN(time))
                {
                    return TimeSpan.Zero;
                }
                return new Duration(ConvertPosition(time));
            }
        }

        protected double NativeBufferingProgress
        {
            get
            {
                if (double.IsNaN(this.mpc.PlayableDuration) || this.mpc.PlayableDuration < float.Epsilon
                    || double.IsNaN(this.mpc.Duration) || this.mpc.Duration < float.Epsilon)
                {
                    return .0;
                }
                return this.mpc.PlayableDuration / this.mpc.Duration;
            }
        }

        protected int? NativeAudioStreamIndex { get; set; }
        protected double NativeBalance { get; set; }

        protected Stretch NativeStretch
        {
            get;
            set;
        }

        protected bool NativeCanSeek
        {
            get
            {
                // todo implementation
                return false;
            }
        }

        protected bool NativeCanPause
        {
            get
            {
                // todo implementation
                return false;
            }
        }

        protected int NativeNaturalVideoHeight
        {
            get
            {
                return (int)this.mpc.NaturalSize.Height;
            }
        }

        protected int NativeNaturalVideoWidth
        {
            get
            {
                return (int)this.mpc.NaturalSize.Width;
            }
        }

        protected bool NativeIsMuted
        {
            get
            {
                return this.NativeVolume <= float.Epsilon;
            }
            set
            {
                if (!this.NativeIsMuted)
                {
                    this.volumeBeforeMuted = this.NativeVolume;
                }

                this.NativeVolume = value ? .0 :
                    (this.volumeBeforeMuted.HasValue ? (double)this.volumeBeforeMuted : this.NativeVolume);
            }
        }

        protected double NativeVolume
        {
            get
            {
                return AVAudioSession.SharedInstance().OutputVolume;
            }
            set
            {
                ((UISlider)new MPVolumeView().Subviews.First(s => s is UISlider && s.Description.Contains("MPVolumeSlider"))).Value = (float)value;
            }
        }

        protected TimeSpan NativePosition
        {
            get
            {
                var time = this.mpc.CurrentPlaybackTime;
                if (double.IsNaN(time))
                {
                    return new TimeSpan();
                }
                return ConvertPosition(time);
            }
            set
            {
                if (Math.Abs(ConvertPosition(value) - this.mpc.CurrentPlaybackTime) > .20)
                {
                    this.mpc.CurrentPlaybackTime = ConvertPosition(value);
                }
            }
        }

        protected Uri NativeSource
        {
            get
            {
                return this.mpc.ContentUrl;
            }
            set
            {
                this.mpc.ContentUrl = value;
            }
        }

        private MediaElementState NativeCurrentState
        {
            get
            {
                switch (this.mpc.PlaybackState)
                {
                    case MPMoviePlaybackState.Playing:
                        return MediaElementState.Playing;
                    case MPMoviePlaybackState.Paused:
                        return MediaElementState.Paused;
                    case MPMoviePlaybackState.Interrupted:
                        return MediaElementState.Closed;
                    case MPMoviePlaybackState.Stopped:
                        return MediaElementState.Stopped;
                    case MPMoviePlaybackState.SeekingBackward:
                        return MediaElementState.Playing;
                    case MPMoviePlaybackState.SeekingForward:
                        return MediaElementState.Playing;
                    default:
                        throw new InvalidEnumArgumentException("PlaybackState");
                }
            }
        }

        public override object GetValue(DependencyProperty dp)
        {
            switch (dp.Name)
            {
                case "CurrentState":
                    return this.NativeCurrentState;
                case "AutoPlay":
                    return this.NativeAutoPlay;
                case "BufferingProgress":
                    return this.NativeBufferingProgress;
                case "CanPause":
                    return this.NativeCanPause;
                case "CanSeek":
                    return this.NativeCanSeek;
                case "DownloadProgress":
                    return this.NativeBufferingProgress;
                case "IsMuted":
                    return this.NativeIsMuted;
                case "NaturalDuration":
                    return this.NativeDuration;
                case "NaturalVideoHeight":
                    return this.NativeNaturalVideoHeight;
                case "NaturalVideoWidth":
                    return this.NativeNaturalVideoWidth;
                case "Position":
                    return this.NativePosition;
                case "Volume":
                    return this.NativeVolume;
                default:
                    return base.GetValue(dp);
            }
        }

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.mpc = new MPMoviePlayerController();
                this.mpc.ControlStyle = MPMovieControlStyle.None;
                this.mpc.AllowsAirPlay = true;
                this.mpc.ScalingMode = MPMovieScalingMode.AspectFill;
                this.AddEventHandlers();
                this.NativeUIElement = this.mpc.View;
                this.invalidationTimer.Elapsed += (sender, e) =>
                {
                    if (this.mpc.PlaybackState != MPMoviePlaybackState.Stopped && this.mpc.PlaybackState != MPMoviePlaybackState.Interrupted)
                    {
                        this.Position = this.NativePosition;

                        // TODO: check conditions for the next assignments
                        this.BufferingProgress = this.NativeBufferingProgress;
                        this.DownloadProgress = this.NativeBufferingProgress;
                    }
                    this.AutoPlay = this.NativeAutoPlay;
                    this.IsMuted = this.NativeIsMuted;
                    this.Volume = this.NativeVolume;
                };
                this.invalidationTimer.Start();
            }
            base.NativeInit();
        }

        protected static double GetNativeVolumeInitialValue()
        {
            return AVAudioSession.SharedInstance().OutputVolume;
        }

        protected void NativePause()
        {
            this.mpc.Pause();
        }

        protected void NativePlay()
        {
            this.mpc.Play();
        }

        protected void NativeStop()
        {
            this.mpc.Stop();
        }

        protected void NativeArrangeVideoView(CGRect finalRect) 
        {
        }

        private static TimeSpan ConvertPosition(double time)
        {
            return TimeSpan.FromMilliseconds(time * 1000d);
        }

        private static double ConvertPosition(TimeSpan time)
        {
            return time.TotalMilliseconds * .001;
        }

        private void InvalidateDuration()
        {
            if (this.mpc.LoadState != MPMovieLoadState.Unknown)
            {
                this.NaturalDuration = this.NativeDuration;
            }
        }
        private void AddEventHandlers()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.DurationAvailableNotification, _ => this.BufferingProgressChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.DurationAvailableNotification, _ => this.DownloadProgressChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.DurationAvailableNotification, _ => this.InvalidateDuration());
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.MediaPlaybackIsPreparedToPlayDidChangeNotification, _ => this.CurrentStateChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.MoviePlayerReadyForDisplayDidChangeNotification, _ => this.InvalidateDuration());
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.MoviePlayerReadyForDisplayDidChangeNotification, _ => this.CurrentStateChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.NowPlayingMovieDidChangeNotification, _ => this.CurrentStateChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.ScalingModeDidChangeNotification, _ => this.CurrentStateChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.DidExitFullscreenNotification, _ => this.CurrentStateChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.DidEnterFullscreenNotification, _ => this.CurrentStateChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.PlaybackStateDidChangeNotification, _ => this.CurrentStateChanged(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.PlaybackDidFinishNotification, _ => this.MediaEnded(this, new RoutedEventArgs()));
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.LoadStateDidChangeNotification, _ => this.InvalidateDuration());
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.PlaybackStateDidChangeNotification, _ => this.InvalidateDuration());
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.NowPlayingMovieDidChangeNotification, _ => this.InvalidateDuration());
            NSNotificationCenter.DefaultCenter.AddObserver(MPMoviePlayerController.MediaPlaybackIsPreparedToPlayDidChangeNotification, _ => this.InvalidateDuration());
        }
    }
}