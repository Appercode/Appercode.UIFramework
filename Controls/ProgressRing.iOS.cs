using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ProgressRing
    {
        private const float DefaultSize = 35f;
        private const float ShiftCorrection = 1f;

        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UIActivityIndicatorView
                    {
                        ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge,
                        HidesWhenStopped = true
                    };
                }

                base.NativeInit();
            }
        }

        protected override void UpdateForeground()
        {
            base.UpdateForeground();
            var activityIndicator = this.NativeUIElement as UIActivityIndicatorView;
            var newValue = this.Foreground;
            if (activityIndicator != null && newValue != null)
            {
                activityIndicator.Color = newValue.ToUIColor(activityIndicator.Frame.Size);
            }
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var size = base.NativeMeasureOverride(availableSize);
            size.Height = MathF.Max(size.Height, DefaultSize + this.Margin.VerticalThicknessF());
            size.Width = MathF.Max(size.Width, DefaultSize + this.Margin.HorizontalThicknessF());
            return size;
        }

        protected override void NativeArrange(CGRect finalRect)
        {
            finalRect.X += ShiftCorrection;
            finalRect.Y += ShiftCorrection;
            base.NativeArrange(finalRect);
        }

        partial void IsActiveChanged(bool newValue)
        {
            var activityIndicator = this.NativeUIElement as UIActivityIndicatorView;
            if (activityIndicator != null && newValue != activityIndicator.IsAnimating)
            {
                if (newValue)
                {
                    activityIndicator.StartAnimating();
                }
                else
                {
                    activityIndicator.StopAnimating();
                }
            }
        }
    }
}