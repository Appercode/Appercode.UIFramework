using Android.Media;
using Android.Views;
using Android.Widget;
using Appercode.UI.Device;
using System;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedVideoView : VideoView, ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnCompletionListener, View.IOnClickListener
    {
        private readonly UIElement owner;

        private MediaPlayer player;
        private ISurfaceHolder holder;
        private double bufferingProgress = 0.0;

        public WrappedVideoView(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.SetOnClickListener(this);
        }

        public event EventHandler VideoSizeChanged;
        public new event EventHandler Completion;
        public new event EventHandler Prepared;
        public event EventHandler BufferingProgressUpdate;

        public override bool IsPlaying
        {
            get
            {
                return this.player.IsPlaying;
            }
        }

        public int VideoHeight
        {
            get
            {
                return (int)ScreenProperties.ConvertPixelsToDPI(this.player.VideoHeight);
            }
        }

        public int VideoWidth
        {
            get
            {
                return (int)ScreenProperties.ConvertPixelsToDPI(this.player.VideoWidth);
            }
        }

        public override int Duration
        {
            get
            {
                return this.player.Duration;
            }
        }

        public int Position
        {
            get
            {
                return this.player.CurrentPosition;
            }
        }

        public double BufferingProgress
        {
            get
            {
                return this.bufferingProgress;
            }
        }

        public void InitMediaPlayer()
        {
            this.holder = this.Holder;
            this.holder.AddCallback(this);

            this.player = new MediaPlayer();
            this.player.VideoSizeChanged += this.OnVideoSizeChanged;
            this.player.BufferingUpdate += this.OnBufferingUpdate;
            this.player.SetOnCompletionListener(this);
            this.player.SetOnPreparedListener(this);
        }

        public void SetDataSource(string path)
        {
            if(!string.IsNullOrWhiteSpace(path))
            {
                this.player.SetDataSource(path);
            }
        }

        public void SetDataSource(Java.IO.FileDescriptor fd, long offset, long length)
        {
            if (fd != null)
            {
                this.player.Reset();
                this.player.SetDataSource(fd, offset, length);
            }
        }

        public void Prepare()
        {
            this.player.Prepare();
        }

        public void Play()
        {
            this.player.Start();
        }

        public override void Pause()
        {
            if (this.player.IsPlaying)
            {
                this.player.Pause();
            }
        }

        public void Stop()
        {
            this.player.SeekTo(this.player.Duration);
        }

        public override void SeekTo(int msec)
        {
            this.player.SeekTo(msec);
        }

        protected override void JavaFinalize()
        {
            this.owner.FreeNativeView(this);
            base.JavaFinalize();
        }

        public void OnPrepared(MediaPlayer mp)
        {
            this.Prepared?.Invoke(this, EventArgs.Empty);
        }

        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            this.player.SetDisplay(holder);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            
        }

        private void OnBufferingUpdate(object sender, MediaPlayer.BufferingUpdateEventArgs e)
        {
            this.bufferingProgress = e.Percent;
            this.BufferingProgressUpdate?.Invoke(this, e);
        }

        private void OnVideoSizeChanged(object sender, MediaPlayer.VideoSizeChangedEventArgs e)
        {
            this.VideoSizeChanged?.Invoke(this, e);
        }

        public void OnCompletion(MediaPlayer mp)
        {
            this.Completion?.Invoke(this, EventArgs.Empty);
        }

        public void OnClick(View v)
        {
            this.owner.OnTap();
        }
    }
}