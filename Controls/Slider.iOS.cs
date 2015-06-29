using System;
using UIKit;

namespace Appercode.UI.Controls
{
    // TODO: (dk) work in progress
    public partial class Slider
    {
        #region Properties

        protected bool NativeIsDirectionReversed
        {
            get 
            { 
                return this.IsDirectionReversed; 
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeIsDirectionReversed(value);
                    this.OnLayoutUpdated();
                }
            }
        }

        protected bool NativeIsFocused
        {
            get
            {
                return this.IsFocused;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeIsFocused(value);
                    this.OnLayoutUpdated();
                }
            }
        }

        protected Orientation NativeOrientation
        {
            get
            {
                return this.Orientation;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeOrientation(value);
                    this.OnLayoutUpdated();
                }
            }
        }

        #endregion

        #region Initialization

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                this.NativeUIElement = new UISlider();

                this.ApplyNativeMinimum(Minimum);
                this.ApplyNativeMaximum(Maximum);
                this.ApplyNativeValue(Value);
                this.ApplyNativeIsDirectionReversed(this.IsDirectionReversed);
                this.ApplyNativeOrientation(this.Orientation);

                ((UISlider)this.NativeUIElement).ValueChanged += this.OnValueChanged;
            }
            base.NativeInit();
        }

        #endregion

        #region Private methods

        protected override void ApplyNativeMaximum(double maximum)
        {
            var floatMaximum = (float)maximum;
            if (float.IsPositiveInfinity(floatMaximum))
            {
                floatMaximum = float.MaxValue;
            }
            if (float.IsNegativeInfinity(floatMaximum))
            {
                floatMaximum = float.MinValue;
            }

            ((UISlider)NativeUIElement).MaxValue = floatMaximum;
        }

        protected override void ApplyNativeMinimum(double minimum)
        {
            var floatMinimum = (float)minimum;
            if (float.IsPositiveInfinity(floatMinimum))
            {
                floatMinimum = float.MaxValue;
            }
            if (float.IsNegativeInfinity(floatMinimum))
            {
                floatMinimum = float.MinValue;
            }

            ((UISlider)NativeUIElement).MinValue = floatMinimum;
        }

        protected override void ApplyNativeValue(double value)
        {
            var floatValue = (float)value;
            if (float.IsPositiveInfinity(floatValue))
            {
                floatValue = float.MaxValue;
            }
            if (float.IsNegativeInfinity(floatValue))
            {
                floatValue = float.MinValue;
            }

            ((UISlider)NativeUIElement).Value = floatValue;
        }

        private void ApplyNativeIsDirectionReversed(bool isDirectionReversed)
        {
        }

        private void ApplyNativeIsFocused(bool isFocused)
        {
        }

        private void ApplyNativeOrientation(Orientation orientation)
        {
        }

        #endregion

        #region Event handlers

        private void OnValueChanged(object sender, EventArgs eventArgs)
        {
            Value = ((UISlider)NativeUIElement).Value;
        }

        #endregion
    }
}