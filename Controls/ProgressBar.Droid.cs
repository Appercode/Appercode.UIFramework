using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl.Wrapers;
using Appercode.UI.Controls.Primitives;

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
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new WrapedProgressBar(this.Context, null, Android.Resource.Attribute.ProgressBarStyleHorizontal);

                    this.ApplyNativeIsIndeterminate(this.NativeIsIndeterminate);
                    this.ApplyNativeValue(this.NativeValue);
                }
            }

            base.NativeInit();
        }

        protected override void ApplyNativeMaximum(double maximum)
        {
            if (this.NativeUIElement != null)
            {
                ((WrapedProgressBar)this.NativeUIElement).Max = (int)maximum;
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

                ((WrapedProgressBar)this.NativeUIElement).Progress = progress;
            }
        }

        private void ApplyNativeIsIndeterminate(bool isIndeterminate)
        {
            ((WrapedProgressBar)this.NativeUIElement).Indeterminate = isIndeterminate;
        }
    }
}