using Appercode.UI.Helpers;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Appercode.UI.Controls.NativeControl
{
    internal class IndeterminateUIProgressView : UIProgressView
    {
        private static readonly nfloat SingleElementWidth = 28;
        private bool updating;
        private double animationSpeed;
        private float cachedProgress;
        private bool isIndeterminate;
        private UIImage animationImage;
        private UIImageView theImageView;
        private UIView host;

        public IndeterminateUIProgressView(NSCoder coder)
            : base(coder)
        {
            this.ClipsToBounds = true;
            this.animationSpeed = 0.5;
        }

        public IndeterminateUIProgressView(CGRect frame)
            : base(frame)
        {
            this.ClipsToBounds = true;
            this.animationSpeed = 0.5;
        }

        public IndeterminateUIProgressView(UIProgressViewStyle style)
            : base(style)
        {
            this.ClipsToBounds = true;
            this.animationSpeed = 0.5;
        }

        public UIImage AnimationImage
        {
            get
            {
                if (this.animationImage == null)
                {
                    if (this.Style == UIProgressViewStyle.Default)
                    {
                        this.animationImage = UIImage.FromBundle("iOS_Resources/ProgressIndicatorDefault");
                    }
                    else
                    {
                        this.animationImage = UIImage.FromBundle("iOS_Resources/ProgressIndicatorBar");
                    }
                }
                return this.animationImage;
            }
        }

        public List<UIImage> AnimationImages { get; set; }

        public UIImage MasterImage { get; set; }

        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminate;
            }
            set
            {
                if (this.isIndeterminate == value)
                {
                    if (value)
                    {
                        this.ReloopForInterfaceChange();
                    }
                    else
                    {
                        if (this.theImageView != null)
                        {
                            this.theImageView.RemoveFromSuperview();
                            this.theImageView = null;
                        }
                    }
                    return;
                }

                if (value)
                {
                    this.cachedProgress = this.Progress;
                    this.Progress = 0.0f;
                }

                this.isIndeterminate = value;

                if (this.isIndeterminate)
                {
                    this.ReloopForInterfaceChange();
                }
                else
                {
                    this.Progress = this.cachedProgress;
                    this.theImageView.StopAnimating();
                    this.theImageView.RemoveFromSuperview();
                }
            }
        }

        public override UIProgressViewStyle Style
        {
            get
            {
                return base.Style;
            }
            set
            {
                base.Style = value;
                if (value != this.Style && this.IsIndeterminate)
                {
                    this.ReloopForInterfaceChange();
                }
            }
        }

        public override float Progress
        {
            get
            {
                if (this.IsIndeterminate)
                {
                    return this.cachedProgress;
                }
                return base.Progress;
            }
            set
            {
                if (this.IsIndeterminate)
                {
                    this.cachedProgress = value;
                }
                else
                {
                    base.Progress = value;
                }
            }
        }

        public override bool ClipsToBounds
        {
            get
            {
                return base.ClipsToBounds;
            }
            set
            {
                base.ClipsToBounds = true;
            }
        }

        public override UIView[] Subviews
        {
            get
            {
                var l = base.Subviews.ToList();
                l.Remove(this.host);
                return l.ToArray();
            }
        }
        public override CGRect Frame
        {
            get
            {
                return base.Frame;
            }
            set
            {
                var needUpdate = base.Frame.Size != value.Size;
                base.Frame = value;
                if (needUpdate && this.IsIndeterminate)
                {
                    this.ReloopForInterfaceChange();
                }
            }
        }

        public override void SetProgress(float progress, bool animated)
        {
            if (this.IsIndeterminate)
            {
                this.cachedProgress = progress;
            }
            else
            {
                base.SetProgress(progress, animated);
            }
        }

        public override void TintColorDidChange()
        {
            base.TintColorDidChange();
            this.MasterImage = null;
            if (this.isIndeterminate)
            {
                this.ReloopForInterfaceChange();
            }
        }

        private static void LinkerHack()
        {
            UIImage i = null;
            i.Copy();
        }

        private void BeginUpdates()
        {
            this.updating = true;
        }

        private void EndUpdates()
        {
            this.updating = false;
            this.ReloopForInterfaceChange();
        }

        private void LayoutImageView()
        {
            this.theImageView.SizeToFit();
            var border = 9 - this.theImageView.Frame.Size.Height;

            this.theImageView.Center = new CGPoint(this.theImageView.Center.X, (this.Bounds.Top + this.Bounds.Bottom) / 2 - border / 2);

            this.theImageView.Frame = new CGRect(border, this.theImageView.Frame.Y, this.theImageView.Frame.Width - border * 2, this.theImageView.Frame.Height);

            if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7)
            {
                this.theImageView.Layer.CornerRadius = this.theImageView.Frame.Height / 2;
            }

            this.host.Layer.CornerRadius = this.theImageView.Layer.CornerRadius;

            this.host.Frame = this.Bounds;
        }

        private void ReloopForInterfaceChange()
        {
            if (this.updating)
            {
                return;
            }
            UIImage single = int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) >= 7 ? this.TintedImage(this.AnimationImage) : this.AnimationImage;
            var imgs = this.AnimationImages;
            var masterImage = this.MasterImage;
            if (this.MasterImage == null || imgs == null || imgs.Count == 0 || imgs.First().Size.Width != this.Frame.Width)
            {
                var expectedWidth = this.Frame.Width + SingleElementWidth;
                bool completeReloop = this.MasterImage == null;

                if (completeReloop)
                {
                    masterImage = new UIImage(single.CGImage);
                    while (masterImage.Size.Width - SingleElementWidth < expectedWidth)
                    {
                        masterImage = masterImage.AttachImageRight(single);
                    }
                }
                else
                {
                    if (masterImage.Size.Width - SingleElementWidth < expectedWidth)
                    {
                        while (masterImage.Size.Width - SingleElementWidth < expectedWidth)
                        {
                            masterImage = masterImage.AttachImageRight(single);
                        }
                    }
                    else
                    {
                        while (masterImage.Size.Width - SingleElementWidth > expectedWidth + SingleElementWidth)
                        {
                            masterImage = masterImage.CropByX(SingleElementWidth);
                        }
                    }
                }

                this.MasterImage = masterImage;

                if (imgs == null)
                {
                    imgs = new List<UIImage>();
                }
                else
                {
                    imgs.Clear();
                }

                var size = new CGSize(this.Frame.Width, masterImage.Size.Height);
                var pixels = single.Size.Width * single.CurrentScale;
                var anchorX = -Math.Abs(masterImage.Size.Width - size.Width);
                for (int i = 0; i <= pixels; i++)
                {
                    UIGraphics.BeginImageContextWithOptions(size, false, single.CurrentScale);
                    CGContext context = UIGraphics.GetCurrentContext();
                    if (context != null)
                    {
                        context.TranslateCTM(0, masterImage.Size.Height);
                        context.ScaleCTM(1, -1);

                        context.DrawImage(new CGRect(anchorX + i, 0.0, masterImage.Size.Width, masterImage.Size.Height), masterImage.CGImage);

                        UIImage result = UIGraphics.GetImageFromCurrentImageContext();

                        imgs.Add(result);
                    }

                    UIGraphics.EndImageContext();
                }
            }

            this.AnimationImages = imgs;

            if (this.theImageView == null)
            {
                this.theImageView = new UIImageView();
            }
            if (this.host == null)
            {
                this.host = new UIView(this.Bounds);
                this.host.BackgroundColor = UIColor.Clear;

                if (int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) >= 7)
                {
                    // this.host.Layer.CornerRadius = this.Frame.Size.Height / 2.0f;
                }
                else
                {
                    this.host.Layer.CornerRadius = this.theImageView.Frame.Size.Height / 2;
                }
                this.host.ClipsToBounds = true;
            }

            this.theImageView.Layer.MasksToBounds = true;

            if (this.host.Superview != this)
            {
                this.AddSubview(this.host);
            }

            if (this.theImageView.Superview != this.host)
            {
                this.host.AddSubview(this.theImageView);
            }

            this.theImageView.AnimationImages = imgs.ToArray();

            if (this.theImageView.AnimationDuration != this.animationSpeed)
            {
                this.theImageView.AnimationDuration = this.animationSpeed;
            }

            this.LayoutImageView();

            if (!this.theImageView.IsAnimating)
            {
                this.theImageView.StartAnimating();
            }
        }

        private UIImage TintedImage(UIImage img)
        {
            // begin a new image context, to draw our colored image onto
            UIGraphics.BeginImageContextWithOptions(img.Size, false, img.CurrentScale);

            // get a reference to that context we created
            var context = UIGraphics.GetCurrentContext();

            // set the fill color
            this.TintColor.SetFill();

            // translate/flip the graphics context (for transforming from CG* coords to UI* coords
            context.TranslateCTM(0, img.Size.Height);
            context.ScaleCTM(1.0f, -1.0f);

            // set the blend mode to color burn, and the original image
            context.SetBlendMode(CGBlendMode.Multiply);
            var rect = new CGRect(CGPoint.Empty, img.Size);
            context.DrawImage(rect, img.CGImage);

            // set a mask that matches the shape of the image, then draw (color burn) a colored rectangle
            context.ClipToMask(rect, img.CGImage);
            context.AddRect(rect);
            context.DrawPath(CGPathDrawingMode.Fill);

            // generate a new UIImage from the graphics context we drew onto
            UIImage coloredImg = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // return the color-burned image
            return coloredImg;
        }
    }
}