using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedSeekBar : SeekBar, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedSeekBar(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.SetOnClickListener(this);
            this.Orientation = Orientation.Horizontal;
        }

        public Orientation Orientation { get; set; }

        public void OnClick(View v)
        {
            this.owner.OnTap();
        }

        protected override void JavaFinalize()
        {
            this.owner.FreeNativeView(this);
            base.JavaFinalize();
        }

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
                canvas.Translate(-this.Height, 0);
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
                        this.Progress = this.Max - (int)(this.Max * e.GetY() / this.Height);
                        this.OnSizeChanged(this.Width, this.Height, 0, 0);
                        break;
                    default:
                        return base.OnTouchEvent(e);
                }

                return true;
            }

            return base.OnTouchEvent(e);
        }
    }
}