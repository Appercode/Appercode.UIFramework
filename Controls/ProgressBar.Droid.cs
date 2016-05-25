using Appercode.UI.Controls.NativeControl.Wrappers;
using static Android.Resource;

namespace Appercode.UI.Controls
{
    public partial class ProgressBar
    {
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
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                this.NativeUIElement = new WrappedProgressBar(this, null, Attribute.ProgressBarStyleHorizontal);
                this.ApplyNativeIsIndeterminate(this.NativeIsIndeterminate);
                this.ApplyNativeValue(this.NativeValue);
            }

            base.NativeInit();
        }

        protected override void ApplyNativeMaximum(double maximum)
        {
            if (this.NativeUIElement != null)
            {
                ((WrappedProgressBar)this.NativeUIElement).Max = (int)maximum;
                this.ApplyNativeValue(this.NativeValue);
            }
        }

        protected override void ApplyNativeMinimum(double minimum)
        {
            this.ApplyNativeValue(this.NativeValue);
        }

        protected override void ApplyNativeValue(double value)
        {
            if (this.NativeUIElement != null)
            {
                double minimum = this.Minimum;
                double maximum = this.Maximum;

                double range = maximum - minimum;
                int progress = (int)(100.0 * value / range);

                ((WrappedProgressBar)this.NativeUIElement).Progress = progress;
            }
        }

        private void ApplyNativeIsIndeterminate(bool isIndeterminate)
        {
            ((WrappedProgressBar)this.NativeUIElement).Indeterminate = isIndeterminate;
        }
    }
}