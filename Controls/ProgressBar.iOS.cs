using Appercode.UI.Controls.NativeControl;
using CoreGraphics;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ProgressBar
    {
        private const float DefaultHeight = 9;
        protected bool NativeIsIndeterminate
        {
            get
            {
                return this.IsIndeterminate;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeIsIndeterminate(value);
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new IndeterminateUIProgressView(UIProgressViewStyle.Default);
            }
            this.ApplyNativeIsIndeterminate(this.IsIndeterminate);
            base.NativeInit();
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var size = base.NativeMeasureOverride(availableSize);
            size.Height = MathF.Max(size.Height, DefaultHeight + this.Margin.VerticalThicknessF());
            return size;
        }

        protected override void ApplyNativeMaximum(double maximum)
        {
            this.ApplyNativeValue(this.NativeValue);
        }

        protected override void ApplyNativeMinimum(double minimum)
        {
            this.ApplyNativeValue(this.NativeValue);
        }

        protected override void ApplyNativeValue(double value)
        {
            double minimum = this.Minimum;
            double maximum = this.Maximum;

            double range = maximum - minimum;
            float progress = (float)((value - minimum) / range);

            ((UIProgressView)this.NativeUIElement).Progress = progress;
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null && this.Background != null)
            {
                ((IndeterminateUIProgressView)this.NativeUIElement).TrackTintColor = this.Background.ToUIColor(this.RenderSize);
            }
        }

        protected override void UpdateForeground()
        {
            base.UpdateForeground();
            if (this.NativeUIElement != null && this.Foreground != null)
            {
                ((IndeterminateUIProgressView)this.NativeUIElement).ProgressTintColor = this.Foreground.ToUIColor(this.RenderSize);
            }
        }

        private void ApplyNativeIsIndeterminate(bool isIndeterminate)
        {
            ((IndeterminateUIProgressView)this.NativeUIElement).IsIndeterminate = isIndeterminate;
        }
    }
}