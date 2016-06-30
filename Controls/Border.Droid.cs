using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrappers;
using Appercode.UI.Device;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Color = Android.Graphics.Color;

namespace Appercode.UI.Controls
{
    public partial class Border
    {
        protected View ChildNativeUIElemtnt
        {
            get;
            set;
        }

        private Thickness nativeBorderThickness;
        private CornerRadius nativeCornerRadius;
        private Thickness nativePadding;
        private Brush nativeBackground;
        private Brush nativeBorderBrush;

        protected Thickness NativeBorderThickness
        {
            get
            {
                return this.nativeBorderThickness;
            }
            set
            {
                var borderLeft = ScreenProperties.ConvertDPIToPixels((float)value.Left);
                var borderTop = ScreenProperties.ConvertDPIToPixels((float)value.Top);
                var borderRight = ScreenProperties.ConvertDPIToPixels((float)value.Right);
                var borderBottom = ScreenProperties.ConvertDPIToPixels((float)value.Bottom);
                this.nativeBorderThickness = new Thickness(borderLeft, borderTop, borderRight, borderBottom);
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeBorderThickness(this.nativeBorderThickness);
                }
            }
        }

        protected CornerRadius NativeCornerRadius
        {
            get
            {
                return this.nativeCornerRadius;
            }
            set
            {
                this.nativeCornerRadius = value;
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeCornerRadius(value);
                }
            }
        }

        protected Thickness NativePadding
        {
            get
            {
                return this.nativePadding;
            }
            set
            {
                this.nativePadding = value;
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativePadding(value);
                }
            }
        }

        protected Brush NativeBackground
        {
            get
            {
                return this.nativeBackground;
            }
            set
            {
                this.nativeBackground = value;
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeBackground(value);
                }
            }
        }
        protected Brush NativeBorderBrush
        {
            get
            {
                return this.nativeBorderBrush;
            }
            set
            {
                this.nativeBorderBrush = value;
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeBorderBrush(value);
                }
            }
        }

        protected internal override void NativeInit()
        {
            base.NativeInit();
            if (this.Parent != null && this.Context != null && this.NativeUIElement == null)
            {
                this.NativeUIElement = new WrappedViewGroup(this);
                this.ApplyNativeContent(this.Child);
                this.ApplyNativeBorderThickness(this.BorderThickness);
            }
        }

        protected override void NativeArrange(RectangleF finalRect)
        {
            base.NativeArrange(finalRect);
            UpdateBackgroundDrawable();
        }

        protected void OnNativeContentChanged(UIElement oldContent, UIElement newContent)
        {
            this.ApplyNativeContent(newContent);
        }

        protected ViewGroup.LayoutParams CreateLayoutParams(UIElement element)
        {
            return new ViewGroup.LayoutParams(
                element.ContainsValue(WidthProperty) ? (int)element.Width : ViewGroup.LayoutParams.MatchParent,
                element.ContainsValue(HeightProperty) ? (int)element.Height : ViewGroup.LayoutParams.MatchParent);
        }

        private void ApplyNativeContent(UIElement newContent)
        {
            if (this.NativeUIElement != null && newContent != null)
            {
                this.ChildNativeUIElemtnt = newContent.NativeUIElement;
                var nativeView = (WrappedViewGroup)this.NativeUIElement;
                nativeView.RemoveAllViews();
                nativeView.AddViewInLayoutOverride(this.ChildNativeUIElemtnt);
                this.ApplyNativeBorderThickness(this.BorderThickness);
            }
        }

        private void ApplyNativeBorderBrush(Brush borderBrush)
        {
            this.UpdateBackgroundDrawable();
        }

        private void ApplyNativeBackground(Brush backgroundBrush)
        {
            this.UpdateBackgroundDrawable();
        }

        private void ApplyNativeBorderThickness(Thickness thickness)
        {
            this.UpdateBackgroundDrawable();
        }

        private void ApplyNativeCornerRadius(CornerRadius cornerRadius)
        {
            this.UpdateBackgroundDrawable();
        }

        private void ApplyNativePadding(Thickness padding)
        {
            //this.NativeUIElement.SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
        }

        private void UpdateBackgroundDrawable()
        {
            var background = this.NativeUIElement.Background;
            this.NativeUIElement.Background = 
                this.CreateNativeBorder(this.Background, this.BorderBrush, this.BorderThickness, this.CornerRadius);
            if (background != null)
            {
                background.SetCallback(null);
            }
        }

        private void NativeArrangeContent(RectangleF rectangleF)
        {
            this.Child.Arrange(rectangleF);
        }

        private Drawable CreateNativeBorder(Brush background, Brush borderBrush, Thickness borderThickness,
            CornerRadius cornerRadius)
        {
            var width = (int)ScreenProperties.ConvertDPIToPixels((float)this.ActualWidth);
            var height = (int)ScreenProperties.ConvertDPIToPixels((float)this.ActualHeight);
            if (height <= 0 || width <= 0)
            {
                return new BitmapDrawable();
            }

            var cornerRadiusArray = new[]
            {
                (float) cornerRadius.TopLeft, (float) cornerRadius.TopLeft, 
                (float) cornerRadius.TopRight, (float) cornerRadius.TopRight, 
                (float) cornerRadius.BottomRight, (float) cornerRadius.BottomRight, 
                (float) cornerRadius.BottomLeft, (float) cornerRadius.BottomLeft
            };
            Paint backgroundPaint = new Paint();
            Paint borderPaint = new Paint();
            backgroundPaint.SetStyle(Paint.Style.Fill);
            borderPaint.SetStyle(Paint.Style.Fill);
            if (borderBrush == null)
            {
                borderPaint.Color = Color.Transparent;
            }
            else
            {
                if (borderBrush is SolidColorBrush)
                {
                    var color = ((SolidColorBrush)borderBrush).Color;
                    borderPaint.Color = new Color(color.R, color.G, color.B, color.A);
                }
                else
                {
                    throw new Exception("SolidColorBrush BorderBrush is supported.");
                }
            }
            if (background == null)
            {
                backgroundPaint.Color = Color.Transparent;
            }
            else
            {
                if (background is SolidColorBrush)
                {
                    var color = ((SolidColorBrush)background).Color;
                    backgroundPaint.Color = new Color(color.R, color.G, color.B, color.A);
                }
                else
                {
                    throw new Exception("SolidColorBrush Background is supported.");
                }
            }
            var rs = new RoundRectShape(cornerRadiusArray, null, null);
            var noChild =
                (this.Child == null || this.Child.Height.Equals(0) || this.Child.Width.Equals(0))
                && (this.ActualHeight.Equals(0) || this.ActualWidth.Equals(0));
            var left = ScreenProperties.ConvertDPIToPixels((float)(borderThickness.Left));
            var top = ScreenProperties.ConvertDPIToPixels((float)(borderThickness.Top));
            var right = ScreenProperties.ConvertDPIToPixels((float)(borderThickness.Right));
            var bottom = ScreenProperties.ConvertDPIToPixels((float)(borderThickness.Bottom));
            var pixelBorderThickness = new Thickness(left, top, right, bottom);
            using (var sd = new CustomShapeDrawable(rs, backgroundPaint, borderPaint, cornerRadiusArray, pixelBorderThickness, noChild))
            {
                var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                var canvas = new Android.Graphics.Canvas(bitmap);
                sd.SetBounds(0, 0, canvas.Width, canvas.Height);
                sd.Draw(canvas);
                return new BitmapDrawable(bitmap);
            }
        }
    }

    public sealed class CustomShapeDrawable : ShapeDrawable
    {
        private Paint fillpaint, strokepaint;
        private float[] cornerRadiusArray;
        private Thickness borderThickness;
        private bool noChild;

        public CustomShapeDrawable(Shape s, Paint background, Paint border, float[] cornerRadiusArray, Thickness borderThickness, bool noChild = false)
            : base(s)
        {
            this.fillpaint = background;
            this.strokepaint = border;
            this.cornerRadiusArray = cornerRadiusArray;
            this.borderThickness = borderThickness;
            this.noChild = noChild;
        }

        protected override void OnDraw(Shape shape, Android.Graphics.Canvas canvas, Paint paint)
        {
            var height = canvas.ClipBounds.Bottom;
            var width = canvas.ClipBounds.Right;
            if (noChild)
            {
                var borderHeight = (int)(this.borderThickness.Top + this.borderThickness.Bottom);
                var borderWidth = (int)(this.borderThickness.Left + this.borderThickness.Right);
                height = borderHeight > 0 ? borderHeight : canvas.ClipBounds.Bottom;
                width = borderWidth > 0 ? borderWidth : canvas.ClipBounds.Right;
            }
            shape.Resize(width, height);
            shape.Draw(canvas, strokepaint);

            var pathInner = new Path();
            var rect = new RectF(
                (float)(borderThickness.Left),
                (float)(borderThickness.Top),
                (float)(canvas.ClipBounds.Right - borderThickness.Right),
                (float)(canvas.ClipBounds.Bottom - borderThickness.Bottom));
            pathInner.AddRoundRect(rect, cornerRadiusArray, Path.Direction.Cw);
            if (!noChild)
            {
                var clearPaint = new Paint();
                clearPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
                canvas.DrawPath(pathInner, clearPaint);
            }
            canvas.DrawPath(pathInner, fillpaint);
        }
    }
}