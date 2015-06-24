using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class Border
    {
        protected Brush NativeBackground
        {
            get
            {
                return this.Background;
            }
            set
            {
                this.InvalidateNativeView();
            }
        }

        protected Brush NativeBorderBrush
        {
            get
            {
                return this.BorderBrush;
            }
            set
            {
                this.InvalidateNativeView();
            }
        }

        protected Thickness NativeBorderThickness
        {
            get
            {
                return this.BorderThickness;
            }
            set
            {
                this.InvalidateNativeView();
            }
        }

        protected Thickness NativePadding
        {
            get
            {
                return this.Padding;
            }
            set
            {
                this.InvalidateNativeView();
            }
        }

        protected CornerRadius NativeCornerRadius
        {
            get
            {
                return this.CornerRadius;
            }
            set
            {
                this.InvalidateNativeView();
            }
        }

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                var nativeBorder = new NativeRectangle(this);
                this.NativeUIElement = nativeBorder;
                if (this.Child != null && this.Child.NativeUIElement != null)
                {
                    nativeBorder.AddSubview(this.Child.NativeUIElement);
                }
            }

            // force re-draw in case the Style is set
            this.NativeUIElement.SetNeedsDisplay();
            base.NativeInit();
        }

        protected void OnNativeContentChanged(UIElement oldValue, UIElement newValue)
        {
            if (oldValue != null && oldValue.NativeUIElement != null)
            {
                oldValue.NativeUIElement.RemoveFromSuperview();
            }
            if (this.NativeUIElement != null && newValue.NativeUIElement != null)
            {
                this.NativeUIElement.AddSubview(newValue.NativeUIElement);
            }
        }

        protected void NativeArrangeContent(CGRect contentRect)
        {
            this.Child.Arrange(contentRect);
        }

        private void InvalidateNativeView()
        {
            if (this.NativeUIElement != null)
            {
                this.NativeUIElement.SetNeedsDisplay();
            }
        }

        private class NativeRectangle : UIView
        {
            private readonly Border border;

            internal NativeRectangle(Border border)
            {
                this.border = border;
                this.BackgroundColor = UIColor.Clear;
            }

            public override CGRect Frame
            {
                get
                {
                    return base.Frame;
                }
                set
                {
                    base.Frame = value;
                    this.SetNeedsDisplay();
                }
            }

            public override void Draw(CGRect rect)
            {
                using (CGContext g = UIGraphics.GetCurrentContext())
                {
                    g.ClearRect(rect);
                    this.DrawRect(g, rect, this.border.BorderThickness, this.border.BorderBrush, this.border.Background, this.border.CornerRadius);
                }
            }

            private void DrawRect(CGContext g, CGRect rect, Thickness borderThickness, Brush borderBrush, Brush fill, CornerRadius cornerRadius)
            {
                var center = new CGPoint(rect.Left + cornerRadius.TopLeft + borderThickness.Left / 2, rect.Top + cornerRadius.TopLeft + borderThickness.Top / 2);
                var strokeColor = borderBrush != null ? borderBrush.ToUIColor(rect.Size).CGColor : UIColor.Black.CGColor;
                g.SetStrokeColor(strokeColor);
                g.SetFillColor(strokeColor);
                List<Arc> innerArcs = new List<Arc>();
                if (cornerRadius.TopLeft != 0)
                {
                    innerArcs.Add(this.DrawArc(g, cornerRadius.TopLeft, center, borderThickness.LeftF(), borderThickness.TopF(), 180f, 270f));
                }
                else
                {
                    innerArcs.Add(new Arc() { Center = center });
                }
                g.BeginPath();
                g.SetLineWidth(borderThickness.TopF());
                g.MoveTo((nfloat)cornerRadius.TopLeft, borderThickness.TopF() / 2);
                g.AddLineToPoint(rect.Right - (nfloat)cornerRadius.TopRight - borderThickness.RightF() / 2, borderThickness.TopF() / 2);
                g.StrokePath();
                center = new CGPoint(rect.Right - cornerRadius.TopRight - borderThickness.Right / 2, rect.Top + cornerRadius.TopRight + borderThickness.Top / 2);
                if (cornerRadius.TopRight != 0)
                {
                    innerArcs.Add(this.DrawArc(g, cornerRadius.TopRight, center, borderThickness.TopF(), borderThickness.RightF(), -90f, 0f));
                }
                else
                {
                    innerArcs.Add(new Arc { Center = center });
                }
                g.BeginPath();
                g.SetLineWidth(borderThickness.RightF());
                g.MoveTo(rect.Right - borderThickness.RightF() / 2, (nfloat)cornerRadius.TopRight);
                g.AddLineToPoint(rect.Right - borderThickness.RightF() / 2, rect.Height - (nfloat)cornerRadius.BottomRight);
                g.StrokePath();
                center = new CGPoint(rect.Right - cornerRadius.BottomRight - borderThickness.Right / 2, rect.Bottom - cornerRadius.BottomRight - borderThickness.Bottom / 2);
                if (cornerRadius.BottomRight != 0)
                {
                    innerArcs.Add(this.DrawArc(g, cornerRadius.BottomRight, center, borderThickness.RightF(), borderThickness.BottomF(), 0f, 90f));
                }
                else
                {
                    innerArcs.Add(new Arc() { Center = center });
                }
                g.BeginPath();
                g.SetLineWidth(borderThickness.BottomF());
                g.MoveTo(rect.Right - (nfloat)cornerRadius.BottomRight, rect.Bottom - borderThickness.BottomF() / 2);
                g.AddLineToPoint(rect.Left + (nfloat)cornerRadius.BottomLeft, rect.Bottom - borderThickness.BottomF() / 2);
                g.StrokePath();
                center = new CGPoint((rect.Left + cornerRadius.BottomLeft + borderThickness.Left / 2), (rect.Bottom - cornerRadius.BottomLeft - borderThickness.Bottom / 2));
                if (cornerRadius.BottomLeft != 0)
                {
                    innerArcs.Add(this.DrawArc(g, cornerRadius.BottomLeft, center, borderThickness.BottomF(), borderThickness.LeftF(), 90f, 180f));
                }
                else
                {
                    innerArcs.Add(new Arc() { Center = center });
                }
                g.SetLineWidth((nfloat)borderThickness.Left);
                g.MoveTo(rect.Left + borderThickness.LeftF() / 2, rect.Bottom - (nfloat)cornerRadius.BottomLeft);
                g.AddLineToPoint(rect.Left + borderThickness.LeftF() / 2, rect.Top + (nfloat)cornerRadius.TopLeft);
                g.StrokePath();
                if (fill != null)
                {
                    var fillColor = fill.ToUIColor(rect.Size).CGColor;
                    g.SetFillColor(fillColor);
                    g.SetLineWidth(0);
                    using (CGPath path = new CGPath())
                    {
                        path.AddArc(
                            innerArcs[0].Center.X,
                            innerArcs[0].Center.Y,
                            innerArcs[0].Radius,
                            innerArcs[0].EndingAngle,
                            innerArcs[0].StartingAngle,
                            false);

                        path.AddArc(
                            innerArcs[1].Center.X,
                            innerArcs[1].Center.Y,
                            innerArcs[1].Radius,
                            innerArcs[1].EndingAngle,
                            innerArcs[1].StartingAngle,
                            false);

                        path.AddArc(
                            innerArcs[2].Center.X,
                            innerArcs[2].Center.Y,
                            innerArcs[2].Radius,
                            innerArcs[2].EndingAngle,
                            innerArcs[2].StartingAngle,
                            false);

                        path.AddArc(
                            innerArcs[3].Center.X,
                            innerArcs[3].Center.Y,
                            innerArcs[3].Radius,
                            innerArcs[3].EndingAngle,
                            innerArcs[3].StartingAngle,
                            false);

                        g.AddPath(path);
                        g.DrawPath(CGPathDrawingMode.Fill);
                    }
                }
            }

            private Arc DrawArc(CGContext context, double radius, CGPoint center, nfloat startingThickness, nfloat endingThickness, nfloat startingAngle, nfloat endingAngle)
            {
                using (CGPath path = new CGPath())
                {
                    context.SetLineWidth(0);
                    var deltaAngle = MathF.Abs(endingAngle - startingAngle);

                    // projectedEndingThickness is the ending thickness we would have if the two arcs
                    // subtended an angle of 180 degrees at their respective centers instead of deltaAngle
                    var projectedEndingThickness = startingThickness + (endingThickness - startingThickness) * (180.0f / deltaAngle);

                    var centerOffset = (projectedEndingThickness - startingThickness) / 4.0f;
                    var centerForInnerArc = new CGPoint(center.X + centerOffset * Math.Cos(startingAngle * Math.PI / 180.0f),
                        center.Y + centerOffset * Math.Sin(startingAngle * Math.PI / 180.0f));
                    var centerForOuterArc = new CGPoint(center.X - centerOffset * Math.Cos(startingAngle * Math.PI / 180.0f),
                        center.Y - centerOffset * Math.Sin(startingAngle * Math.PI / 180.0f));

                    var radiusForInnerArc = (nfloat)radius - (startingThickness + projectedEndingThickness) / 4;
                    var radiusForOuterArc = (nfloat)radius + (startingThickness + projectedEndingThickness) / 4;

                    path.AddArc(
                        centerForInnerArc.X,
                        centerForInnerArc.Y,
                        radiusForInnerArc,
                        endingAngle * MathF.PI / 180.0f,
                        startingAngle * MathF.PI / 180.0f,
                        true);

                    var arc = new Arc()
                    {
                        Center = centerForInnerArc,
                        Radius = radiusForInnerArc,
                        StartingAngle = endingAngle * MathF.PI / 180.0f,
                        EndingAngle = startingAngle * MathF.PI / 180.0f
                    };

                    path.AddArc(
                        centerForOuterArc.X,
                        centerForOuterArc.Y,
                        radiusForOuterArc,
                        startingAngle * MathF.PI / 180.0f,
                        endingAngle * MathF.PI / 180.0f,
                        false);

                    context.AddPath(path);

                    context.FillPath();

                    return arc;
                }
            }

            private struct Arc
            {
                public CGPoint Center;
                public nfloat Radius;
                public nfloat StartingAngle;
                public nfloat EndingAngle;
            }
        }
    }
}