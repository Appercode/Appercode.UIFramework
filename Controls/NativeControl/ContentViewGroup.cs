using Android.Content;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;

namespace Appercode.UI.Controls.NativeControl
{
    public class ContentViewGroup : WrapedViewGroup
    {
        private View childView;

        public ContentViewGroup(Context context)
            : base(context)
        {
        }

        public View ChildView
        {
            get
            {
                return this.childView;
            }
            set
            {
                if (this.childView != value)
                {
                    this.RemoveAllViews();
                    this.childView = value;
                    this.AddView(value, new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent));
                }
            }
        }

        public View AdditionalView { get; set; }

        public GravityFlags ChildGravity { get; set; }

        public GravityFlags AdditionalChildGravity { get; set; }

        public LayoutParams ChildLayoutParams { get; set; }

        public LayoutParams AdditionalChildLyaoutParams { get; set; }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            if (changed)
            {
                right = right - left;
                left = 0;
                bottom = bottom - top;
                top = 0;

                if (this.childView != null)
                {
                    int childLeft, childTop, childRight, childBottom;
                    int tempChildLeft, tempChildTop, tempChildRight, tempChildBottom;
                    int additionalChildLeft, additionalChildTop, additionalChildRight, additionalChildBottom;

                    if (this.AdditionalView != null)
                    {
                        this.AdditionalView.Measure(
                            MeasureSpec.MakeMeasureSpec(right - left, MeasureSpecMode.Exactly),
                            MeasureSpec.MakeMeasureSpec(bottom - top, MeasureSpecMode.Exactly));
                        switch (this.AdditionalChildGravity & GravityFlags.HorizontalGravityMask)
                        {
                            case GravityFlags.NoGravity:
                            case GravityFlags.Left:
                                additionalChildLeft = left;
                                additionalChildRight = left + this.AdditionalView.MeasuredWidth;
                                tempChildLeft = left + this.AdditionalView.MeasuredWidth;
                                tempChildRight = right;
                                break;
                            case GravityFlags.Right:
                                additionalChildLeft = right - this.AdditionalView.MeasuredWidth;
                                additionalChildRight = right;
                                tempChildLeft = left;
                                tempChildRight = right - this.AdditionalView.MeasuredWidth;
                                break;
                            case GravityFlags.CenterHorizontal:
                                additionalChildLeft = (int)(0.5 * (left + right - this.AdditionalView.MeasuredWidth));
                                additionalChildRight = (int)(0.5 * (left + right + this.AdditionalView.MeasuredWidth));
                                tempChildLeft = left;
                                tempChildRight = right;
                                break;
                            default:
                                throw new NotSupportedException("Unsupported gravity flag");
                        }
                        switch (this.AdditionalChildGravity & GravityFlags.VerticalGravityMask)
                        {
                            case GravityFlags.NoGravity:
                            case GravityFlags.Top:
                                additionalChildTop = top;
                                additionalChildBottom = top + this.AdditionalView.MeasuredHeight;
                                tempChildTop = top + this.AdditionalView.MeasuredHeight;
                                tempChildBottom = bottom;
                                break;
                            case GravityFlags.Bottom:
                                additionalChildTop = bottom - this.AdditionalView.MeasuredHeight;
                                additionalChildBottom = bottom;
                                tempChildTop = top;
                                tempChildBottom = bottom - this.AdditionalView.MeasuredHeight;
                                break;
                            case GravityFlags.CenterVertical:
                                additionalChildTop = (int)(0.5 * (top + bottom - this.AdditionalView.MeasuredHeight));
                                additionalChildBottom = (int)(0.5 * (top + bottom + this.AdditionalView.MeasuredHeight));
                                tempChildTop = top;
                                tempChildBottom = bottom;
                                break;
                            default:
                                throw new NotSupportedException("Unsupported gravity flag");
                        }
                        this.AdditionalView.Layout(tempChildLeft, tempChildTop, tempChildRight, tempChildBottom);

                        this.ChildView.Measure(
                            MeasureSpec.MakeMeasureSpec(additionalChildRight - additionalChildLeft, MeasureSpecMode.Exactly),
                            MeasureSpec.MakeMeasureSpec(additionalChildBottom - additionalChildTop, MeasureSpecMode.Exactly));
                    }
                    else
                    {
                        tempChildLeft = left;
                        tempChildTop = top;
                        tempChildRight = right;
                        tempChildBottom = bottom;

                        int requaredWidth = tempChildRight - tempChildLeft;
                        int requaredHeight = tempChildBottom - tempChildTop;

                        if (this.ChildLayoutParams != null)
                        {
                            if (this.ChildLayoutParams.Width != LayoutParams.MatchParent
                                && this.ChildLayoutParams.Width != LayoutParams.WrapContent
                                && this.ChildLayoutParams.Width < tempChildRight - tempChildLeft)
                            {
                                requaredWidth = this.ChildLayoutParams.Width;
                            }

                            if (this.ChildLayoutParams.Height != LayoutParams.MatchParent
                                && this.ChildLayoutParams.Height != LayoutParams.WrapContent
                                && this.ChildLayoutParams.Height < tempChildBottom - tempChildTop)
                            {
                                requaredHeight = this.ChildLayoutParams.Height;
                            }
                        }

                        this.ChildView.Measure(
                            MeasureSpec.MakeMeasureSpec(requaredWidth, MeasureSpecMode.Exactly),
                            MeasureSpec.MakeMeasureSpec(requaredHeight, MeasureSpecMode.Exactly));
                    }

                    switch (this.ChildGravity & GravityFlags.HorizontalGravityMask)
                    {
                        case GravityFlags.NoGravity:
                        case GravityFlags.Left:
                            childLeft = tempChildLeft;
                            childRight = this.ChildView.MeasuredWidth;
                            break;
                        case GravityFlags.Right:
                            childLeft = tempChildRight - this.ChildView.MeasuredWidth;
                            childRight = tempChildRight;
                            break;
                        case GravityFlags.CenterHorizontal:
                            childLeft = (int)(0.5 * (tempChildLeft + tempChildRight - this.ChildView.MeasuredWidth));
                            childRight = (int)(0.5 * (tempChildLeft + tempChildRight + this.ChildView.MeasuredWidth));
                            break;
                        default:
                            throw new NotSupportedException("Unsupported gravity flag");
                    }
                    switch (this.ChildGravity & GravityFlags.VerticalGravityMask)
                    {
                        case GravityFlags.NoGravity:
                        case GravityFlags.Top:
                            childTop = tempChildTop;
                            childBottom = this.ChildView.MeasuredHeight;
                            break;
                        case GravityFlags.Bottom:
                            childTop = tempChildBottom - this.ChildView.MeasuredHeight;
                            childBottom = tempChildBottom;
                            break;
                        case GravityFlags.CenterVertical:
                            childTop = (int)(0.5 * (tempChildTop + tempChildBottom - this.ChildView.MeasuredHeight));
                            childBottom = (int)(0.5 * (tempChildTop + tempChildBottom + this.ChildView.MeasuredHeight));
                            break;
                        default:
                            throw new NotSupportedException("Unsupported gravity flag");
                    }
                    ////ChildView.Layout(childLeft, childTop, childRight, childBottom);
                }
            }
        }
    }
}