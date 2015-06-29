using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls.Primitives
{
    public abstract partial class RangeBase
    {
        protected double NativeLargeChange
        {
            get
            {
                return this.LargeChange;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeLargeChange(value);
                }
            }
        }

        protected double NativeMaximum
        {
            get
            {
                return this.Maximum;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeMaximum(value);
                }
            }
        }

        protected double NativeMinimum
        {
            get
            {
                return this.Minimum;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeMinimum(value);
                }
            }
        }

        protected double NativeSmallChange
        {
            get
            {
                return this.SmallChange;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeSmallChange(value);
                }
            }
        }

        protected double NativeValue
        {
            get
            {
                return this.Value;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeValue(value);
                }
            }
        }

        protected internal override void NativeInit()
        {
            this.ApplyNativeLargeChange(this.NativeLargeChange);
            this.ApplyNativeMaximum(this.NativeMaximum);
            this.ApplyNativeMinimum(this.NativeMinimum);
            this.ApplyNativeSmallChange(this.NativeSmallChange);
            this.ApplyNativeValue(this.NativeValue);

            base.NativeInit();
        }

        protected virtual void ApplyNativeLargeChange(double largeChange)
        {
        }

        protected virtual void ApplyNativeMaximum(double maximum)
        {
        }

        protected virtual void ApplyNativeMinimum(double minimum)
        {
        }

        protected virtual void ApplyNativeSmallChange(double smallChange)
        {
        }

        protected virtual void ApplyNativeValue(double value)
        {
        }
    }
}