using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl.Wrappers;

namespace Appercode.UI.Controls
{
    public partial class Slider
    {
        #region Consts

        private const double SeekBarMaximum = 10000d;

        #endregion

        #region Properties

        internal bool NativeIsDirectionReversed
        {
            get { return this.IsDirectionReversed; }
            set
            {
                if (this.NativeUIElement != null)
                {
                    ApplyNativeIsDirectionReversed(value);
                }
            }
        }

        internal bool NativeIsFocused
        {
            get { return this.IsFocused; }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeIsFocused(value);
                }
            }
        }

        internal Orientation NativeOrientation
        {
            get { return this.Orientation; }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeOrientation(value);
                }
            }
        }

        #endregion

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                var nativeView = new WrappedSeekBar(this) { Max = (int)SeekBarMaximum };
                this.NativeUIElement = nativeView;
                this.ApplyNativeMinimum(this.Minimum);
                this.ApplyNativeMaximum(this.Maximum);
                this.ApplyNativeValue(this.Value);
                this.ApplyNativeIsDirectionReversed(this.IsDirectionReversed);
                this.ApplyNativeOrientation(this.Orientation);
                nativeView.ProgressChanged += this.SeekBarProgressChanged;
                nativeView.FocusChange += this.SeekBarFocusChange;
            }

            base.NativeInit();
        }

        protected override void ApplyNativeMaximum(double maximum)
        {
            base.ApplyNativeMaximum(maximum);
            this.ApplyNativeValue(NativeValue);
        }

        protected override void ApplyNativeMinimum(double minimum)
        {
            base.ApplyNativeMinimum(minimum);
            this.ApplyNativeValue(NativeValue);
        }

        protected override void ApplyNativeValue(double value)
        {
            base.ApplyNativeValue(value);
            value = IsDirectionReversed ? (Maximum - value) : value;
            var percValue = (value - Minimum) * SeekBarMaximum / (Maximum - Minimum);
            ((SeekBar)this.NativeUIElement).Progress = (int)percValue;
        }

        private void ApplyNativeIsDirectionReversed(bool isDirectionReversed)
        {
            this.ApplyNativeValue(NativeValue);
        }

        private void ApplyNativeIsFocused(bool isFocused)
        {
            if (isFocused)
            {
                this.NativeUIElement.RequestFocus();
            }
            else
            {
                this.NativeUIElement.ClearFocus();
            }
        }

        private void ApplyNativeOrientation(Orientation orientation)
        {
            // TODO: (dk) set default width and height
            if (this.NativeUIElement != null)
            {
                ((WrappedSeekBar)this.NativeUIElement).Orientation = orientation;
            }
        }

        #region Event handlers

        private void SeekBarProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            var value = ((Maximum - Minimum) * e.Progress) / SeekBarMaximum + Minimum;
            this.Value = IsDirectionReversed ? (Maximum - value) : value;
        }

        private void SeekBarFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            this.IsFocused = e.HasFocus;
        }

        #endregion
    }
}