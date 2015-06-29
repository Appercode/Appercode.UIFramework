using System.Drawing;
using Android.Content;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;

namespace Appercode.UI.Controls
{
    public class NativeSeekBar : WrappedSeekBar
    {
        #region Properties

        public Orientation Orientation { get; set; }

        #endregion

        #region Constructors

        public NativeSeekBar(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            this.Orientation = Orientation.Horizontal;
        }

        public NativeSeekBar(Context context)
            : base(context)
        {
            this.Orientation = Orientation.Horizontal;
        }

        #endregion

        #region Lifecycle

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                base.OnSizeChanged(h, w, oldh, oldw);
            }
            else
            {
                base.OnSizeChanged(w, h, oldw, oldh);
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                base.OnMeasure(heightMeasureSpec, widthMeasureSpec);
                this.SetMeasuredDimension(MeasuredHeight, MeasuredWidth);
            }
            else
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                canvas.Rotate(-90);
                canvas.Translate(-Height, 0);
            }

            base.OnDraw(canvas);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                if (!this.Enabled)
                {
                    return false;
                }

                switch (e.Action)
                {
                    case MotionEventActions.Down:
                    case MotionEventActions.Move:
                    case MotionEventActions.Up:
                        this.Progress = Max - (int)(Max * e.GetY() / Height);
                        this.OnSizeChanged(Width, Height, 0, 0);
                        break;
                    default:
                        return base.OnTouchEvent(e);
                }

                return true;
            }

            return base.OnTouchEvent(e);
        }

        #endregion
    }

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

        #region Initialization

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new NativeSeekBar(this.Context);
                    ((NativeSeekBar)this.NativeUIElement).Max = (int)SeekBarMaximum;
                    this.ApplyNativeMinimum(Minimum);
                    this.ApplyNativeMaximum(Maximum);
                    this.ApplyNativeValue(Value);
                    this.ApplyNativeIsDirectionReversed(IsDirectionReversed);
                    this.ApplyNativeOrientation(Orientation);
                    ((NativeSeekBar)this.NativeUIElement).ProgressChanged += SeekBarProgressChanged;
                    this.NativeUIElement.FocusChange += SeekBarFocusChange;
                }
            }
            base.NativeInit();
        }

        #endregion

        #region Private methods

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
            ((NativeSeekBar)this.NativeUIElement).Progress = (int)percValue;
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
                ((NativeSeekBar)this.NativeUIElement).Orientation = orientation;
        }

        #endregion

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