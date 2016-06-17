using Android.Content;
using Android.Content.Res;
using Android.Views;
using Appercode.UI.Device;
using System;
using System.Collections.Generic;

namespace Appercode.UI.Controls.NativeControl
{
    public class NativeScrollChangedEventArgs : EventArgs
    {
        public int l { get; set; }
        public int t { get; set; }
        public int oldl { get; set; }
        public int oldt { get; set; }
    }

    public class MyOverScroller
    {
        private const int ModulA = -4000;

        private int minX;
        private int maxX;
        private int minY;
        private int maxY;

        private int aX, aY;

        private int startX, startY;
        private int startVelocityX, startVelocityY;
        private DateTime startTime;

        private bool isFirstFlingFromCode = false;

        private TimeSpan currentTime;

        public bool IsFinishedY
        {
            get
            {
                return Math.Sign(this.startVelocityY) * Math.Sign(this.VY(this.currentTime)) < 0;
            }
        }

        public bool IsFinished { get; set; }

        public bool IsFinishedX
        {
            get
            {
                return Math.Sign(this.startVelocityX) * Math.Sign(this.VX(this.currentTime)) < 0;
            }
        }

        public int CurrX { get; set; }
        public int CurrY { get; set; }

        internal void AbortAnimation()
        {
            this.startTime = DateTime.MinValue;
        }

        internal bool SpringBack(int p1, int p2, int p3, int p4, int p5, int p6)
        {
            return true;
        }

        internal bool ComputeScrollOffset()
        {
            if (this.isFirstFlingFromCode)
            {
                this.CurrX = 1;
                this.CurrY = 1;

                this.isFirstFlingFromCode = false;
                return true;
            }

            this.currentTime = DateTime.Now - this.startTime;

            bool changed = false;

            if (!this.IsFinishedX)
            {
                var newValue = this.SX(this.currentTime);
                if (newValue < this.minX)
                {
                    newValue = this.minX;
                }
                if (newValue > this.maxX)
                {
                    newValue = this.maxX;
                }
                this.CurrX = newValue;
                changed = true;
            }

            if (!this.IsFinishedY)
            {
                var newValue = this.SY(this.currentTime);
                if (newValue < this.minY)
                {
                    newValue = this.minY;
                }
                if (newValue > this.maxY)
                {
                    newValue = this.maxY;
                }
                this.CurrY = newValue;
                changed = true;
            }
            this.IsFinished = changed;

            return this.IsFinished;
        }

        internal void Fling(int x, int y, int velocityX, int velocityY, int minX, int maxX, int minY, int maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;

            this.aX = ModulA * Math.Sign(velocityX);
            this.aY = ModulA * Math.Sign(velocityY);
            this.startX = x;
            this.startY = y;
            this.startVelocityX = velocityX;
            this.startVelocityY = velocityY;
            this.startTime = DateTime.Now;
        }

        internal void FlingFromCode(int targetX, int targetY, int currentX, int currentY, int minX, int maxX, int minY, int maxY)
        {
            double t = 0.3;
            double velocityX = 0.0;
            double velocityY = 0.0;

            if (targetX != currentX)
            {
                velocityX = 2.0 * (targetX - currentX) / t;
            }
            if (targetY != currentY)
            {
                velocityY = 2.0 * (targetY - currentY) / t;
            }

            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;

            this.aX = (int)(-1 * velocityX / t);
            this.aY = (int)(-1 * velocityY / t);

            this.startX = currentX;
            this.startY = currentY;

            this.startVelocityX = (int)velocityX;
            this.startVelocityY = (int)velocityY;

            this.startTime = DateTime.Now;

            this.isFirstFlingFromCode = true;
        }

        private int SY(TimeSpan dt)
        {
            double t = dt.TotalMilliseconds * 0.001;

            var res = this.startY + this.startVelocityY * t + (this.aY * t * t) / 2;
            return (int)res;
        }

        private int SX(TimeSpan dt)
        {
            double t = dt.TotalMilliseconds * 0.001;

            var res = this.startX + this.startVelocityX * t + (this.aX * t * t) / 2;
            return (int)res;
        }

        private int VY(TimeSpan dt)
        {
            double t = dt.TotalMilliseconds * 0.001;
            var res = this.startVelocityY + this.aY * t;
            return (int)res;
        }

        private int VX(TimeSpan dt)
        {
            double t = dt.TotalMilliseconds * 0.001;
            var res = this.startVelocityX + this.aX * t;
            return (int)res;
        }
    }

    public class NativeScrollViewer : ContentViewGroup
    {
        /// <summary>
        /// Children of the current view, which able to scroll their content. It will be updated on UpdateLayout call of the ScrollViewer.
        /// </summary>
        private List<View> childrenWhoCanScroll;

        /// <summary>
        /// Child view, which currently scrolls it's content. It will be assigned on a touch event.
        /// </summary>
        private View childWhoScrolling = null;

        private const int InvalidPointerId = -1;

        private MyOverScroller scroller;

        private VelocityTracker velocityTracker;

        private bool isBeingDragged;
        private int lastMotionX, lastMotionY;
        private int activePointerId;

        private int touchSlop;
        private int minimumVelocity;
        private int maximumVelocity;

        private int overflingDistance;

        public NativeScrollViewer(Context context)
            : base(context)
        {
            this.InitConstructor(context);
        }

        public event EventHandler<NativeScrollChangedEventArgs> ScrollChanged;

        public int ContentWidth
        {
            get;
            set;
        }
        public int ContentHeight
        {
            get;
            set;
        }

        public int AvailableContentHeight
        {
            get;
            set;
        }
        public int AvailableContentWidth
        {
            get;
            set;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            int currenrMotionX;
            int currentMotionY;

            if (ev.Action == MotionEventActions.Move && this.isBeingDragged)
            {
                return true;
            }

            if (this.ScrollX == 0 && !this.CanScrollVertically(1) &&
                this.ScrollY == 0 && !this.CanScrollHorizontally(1))
            {
                return false;
            }

            switch (ev.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Move:
                    {
                        this.FindViewWhoScrolling((int)ev.GetX(), (int)ev.GetY(), 1, 1);
                        if (this.childWhoScrolling != null)
                        {
                            return false;
                        }

                        if (this.activePointerId == InvalidPointerId)
                        {
                            break;
                        }

                        int pointerIndex = ev.FindPointerIndex(this.activePointerId);
                        if (pointerIndex == -1)
                        {
                            break;
                        }

                        currenrMotionX = (int)ev.GetX(pointerIndex);
                        currentMotionY = (int)ev.GetY(pointerIndex);
                        int xDiff = Math.Abs(currenrMotionX - this.lastMotionX);
                        int yDiff = Math.Abs(currentMotionY - this.lastMotionY);
                        if (xDiff > this.touchSlop || yDiff > this.touchSlop)
                        {
                            this.isBeingDragged = true;
                            this.lastMotionX = currenrMotionX;
                            this.lastMotionY = currentMotionY;
                            this.InitVelocityTrackerIfNotExists();
                            this.velocityTracker.AddMovement(ev);

                            if (this.Parent != null)
                            {
                                this.Parent.RequestDisallowInterceptTouchEvent(true);
                            }
                        }
                        break;
                    }
                case MotionEventActions.Down:
                    {
                        this.FindViewWhoScrolling((int)ev.GetX(), (int)ev.GetY(), 1, 1);

                        if (this.childWhoScrolling != null)
                        {
                            return false;
                        }

                        // Stop, if touched during movement on inertia
                        if (!this.scroller.IsFinishedY)
                        {
                            this.scroller.AbortAnimation();
                            ////FlingStrictSpan
                        }

                        currenrMotionX = (int)ev.GetX();
                        currentMotionY = (int)ev.GetY();

                        this.lastMotionX = currenrMotionX;
                        this.lastMotionY = currentMotionY;
                        this.activePointerId = ev.GetPointerId(0);

                        ////if (this.inChild(x, y))
                        {
                            this.isBeingDragged = false;
                            this.RecycleVelocityTracker();
                            ////break;
                        }

                        this.InitOrResetVelocityTracker();
                        this.velocityTracker.AddMovement(ev);

                        ////this.isBeingDragged = this.scroller.IsFinished;

                        break;
                    }
                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    if (this.childWhoScrolling != null)
                    {
                        this.childWhoScrolling = null;
                        return false;
                    }
                    this.isBeingDragged = false;
                    this.activePointerId = InvalidPointerId;
                    this.RecycleVelocityTracker();

                    //if (this.scroller.SpringBack(this.ScrollX, this.ScrollY, 0, 0, this.GetScrollRangeX(), this.GetScrollRangeY()))
                    //{
                        // TODO: possible something like postInvalidate (postInvalidateOnAnimation)
                    //}
                    break;
                case MotionEventActions.PointerUp:
                    if (this.childWhoScrolling != null)
                    {
                        return false;
                    }
                    this.OnSecondaryPointerUp(ev);
                    break;
            }

            return this.isBeingDragged;
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            base.OnTouchEvent(ev);

            this.InitVelocityTrackerIfNotExists();
            this.velocityTracker.AddMovement(ev);

            switch (ev.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    {
                        if (this.childWhoScrolling != null)
                        {
                            this.childWhoScrolling.OnTouchEvent(ev);
                            return false;
                        }

                        if (this.ChildCount == 0)
                        {
                            return false;
                        }

                        this.isBeingDragged = !this.scroller.IsFinishedY;
                        if (this.isBeingDragged)
                        {
                            if (this.Parent != null)
                            {
                                this.Parent.RequestDisallowInterceptTouchEvent(true);
                            }
                        }

                        // Stop, if touched during movement on inertia
                        if (!this.scroller.IsFinishedY)
                        {
                            this.scroller.AbortAnimation();
                            ////FlingStrictSpan
                        }

                        this.lastMotionX = (int)ev.GetX();
                        this.lastMotionY = (int)ev.GetY();
                        this.activePointerId = ev.GetPointerId(0);
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        if (this.childWhoScrolling != null)
                        {
                            this.childWhoScrolling.OnTouchEvent(ev);
                            return false;
                        }

                        //int activePointerIndex = ev.FindPointerIndex(this.activePointerId);
                        //if (activePointerIndex == -1)
                        //{
                        //    break;
                        //}

                        int x = (int)ev.GetX();
                        int y = (int)ev.GetY();
                        int deltaX = this.lastMotionX - x;
                        int deltaY = this.lastMotionY - y;

                        if (!this.isBeingDragged && (Math.Abs(deltaX) > this.touchSlop || Math.Abs(deltaY) > this.touchSlop))
                        {
                            if (this.Parent != null)
                            {
                                this.Parent.RequestDisallowInterceptTouchEvent(true);
                            }

                            this.isBeingDragged = true;
                        }

                        if (this.isBeingDragged)
                        {
                            // scroll after touch motions
                            this.lastMotionX = x;
                            this.lastMotionY = y;

                            int oldX = this.ScrollX;
                            int oldY = this.ScrollY;
                            int rangeX = this.GetScrollRangeX();
                            int rangeY = this.GetScrollRangeY();

                            // OverScroll is ignored for now
                            // bool canOverscrollX = this.OverScrollMode == Android.Views.OverScrollMode.Always ||
                            //     (this.OverScrollMode == Android.Views.OverScrollMode.IfContentScrolls && rangeX > 0);
                            // bool canOverscrollY = this.OverScrollMode == Android.Views.OverScrollMode.Always ||
                            //    (this.OverScrollMode == Android.Views.OverScrollMode.IfContentScrolls && rangeY > 0);

                            if (this.OverScrollBy(deltaX, deltaY, this.ScrollX, this.ScrollY, rangeX, rangeY, this.overflingDistance, this.overflingDistance, true))
                            {
                                ////this.velocityTracker.Clear();
                            }
                            //this.OnScrollChanged(this.ScrollX, this.ScrollY, oldX, oldY);
                        }

                        break;
                    }
                case MotionEventActions.Up:
                    if (this.childWhoScrolling != null)
                    {
                        this.childWhoScrolling.OnTouchEvent(ev);
                        return false;
                    }

                    if (this.isBeingDragged)
                    {
                        this.velocityTracker.ComputeCurrentVelocity(1000, this.maximumVelocity);

                        int initialVelocityX = (int)this.velocityTracker.GetXVelocity(this.activePointerId);
                        int initialVelocityY = (int)this.velocityTracker.GetYVelocity(this.activePointerId);
                        ////int initialVelocityX = this.velocityTracker.XVelocity;
                        ////int initialVelocityY 

                        if (this.ChildCount > 0)
                        {
                            initialVelocityX = (int)this.velocityTracker.XVelocity;  ////Math.Abs(initialVelocityX) > this.minimumVelocity ? initialVelocityX : 0;
                            initialVelocityY = (int)this.velocityTracker.YVelocity; ////Math.Abs(initialVelocityY) > this.minimumVelocity ? initialVelocityY : 0;

                            if (initialVelocityX != 0 || initialVelocityY != 0)
                            {
                                this.Fling(-initialVelocityX, -initialVelocityY);
                            }
                            // else if (this.scroller.SpringBack(this.ScrollX, this.ScrollY, 0, 0, this.GetScrollRangeX(), this.GetScrollRangeY()))
                            // {
                                // TODO: possible something like postInvalidate (postInvalidateOnAnimation)
                            // }
                        }

                        this.activePointerId = InvalidPointerId;
                        this.EndDrag();
                    }
                    break;
                case MotionEventActions.Cancel:
                    if (this.childWhoScrolling != null)
                    {
                        this.childWhoScrolling.OnTouchEvent(ev);
                        return false;
                    }

                    if (this.isBeingDragged && this.ChildCount > 0)
                    {
                        if (this.scroller.SpringBack(this.ScrollX, this.ScrollY, 0, 0, this.GetScrollRangeX(), this.GetScrollRangeY()))
                        {
                            // TODO: possible something like postInvalidate (postInvalidateOnAnimation)
                        }
                        this.activePointerId = InvalidPointerId;
                    }
                    break;
                case MotionEventActions.PointerDown:
                    if (this.childWhoScrolling != null)
                    {
                        this.childWhoScrolling.OnTouchEvent(ev);
                        return false;
                    }

                    int index = ev.ActionIndex;
                    this.lastMotionX = (int)ev.GetX(index);
                    this.lastMotionY = (int)ev.GetY(index);
                    this.activePointerId = ev.GetPointerId(index);
                    break;
                case MotionEventActions.PointerUp:
                    if (this.childWhoScrolling != null)
                    {
                        this.childWhoScrolling.OnTouchEvent(ev);
                        return false;
                    }

                    this.OnSecondaryPointerUp(ev);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Defines view, which currently scrolls it's content and assign the result to childWhoScrolling.
        /// </summary>
        private void FindViewWhoScrolling(int x, int y, int deltaX, int deltaY)
        {
            int offsetY = (int)ScreenProperties.DisplaySize.Height - (int)AppercodeVisualRoot.Instance.ActualHeight; // StackNavigationFrame.Instance.PageHeight;
            int offsetX = (int)ScreenProperties.DisplaySize.Width - (int)AppercodeVisualRoot.Instance.ActualHeight; // StackNavigationFrame.Instance.PageWidth;
            
            this.childWhoScrolling = null;
            bool result = false;

            for (int i = 0; i < this.childrenWhoCanScroll.Count && !result; i++)
            {
                View child = this.childrenWhoCanScroll[i];
                var location = new int[2];
                child.GetLocationInWindow(location);
                int childX = location[0] - offsetX;
                int childY = location[1] - offsetY;

                if ((x > childX && x < (childX + child.Width)) &&
                    (y > childY && y < (childY + child.Height)))
                {
                    this.childWhoScrolling = child;
                    result = true;
                }
            }
        }

        public void UpdateChildrenWhoCanScroll()
        {
            this.childrenWhoCanScroll = new List<View>();

            List<ViewGroup> children = new List<ViewGroup>();
            children.Add(this);

            ViewGroup view = this;

            while (children.Count > 0)
            {
                view = children[0];

                for (int i = 0; i < view.ChildCount; i++)
                {
                    View child = view.GetChildAt(i);

                    if (child is Appercode.UI.Controls.WebBrowser.WrappedWebView)
                    {
                        this.childrenWhoCanScroll.Add(child);
                    }

                    if (child is ViewGroup && ((ViewGroup)child).ChildCount > 0)
                    {
                        children.Add((ViewGroup)child);

                        if (child is NativeScrollViewer)
                        {
                            this.childrenWhoCanScroll.Add(child);
                        }
                    }
                }

                children.Remove(view);
            }
        }

        public override bool CanScrollVertically(int direction)
        {
            int offset = this.ComputeVerticalScrollOffset();
            int range = this.ComputeVerticalScrollRange() - this.ComputeVerticalScrollExtent();
            if (range == 0)
            {
                return false;
            }

            if (direction < 0)
            {
                return offset > 0;
            }
            else
            {
                return offset < range - 1;
            }
        }

        public override bool CanScrollHorizontally(int direction)
        {
            int offset = this.ComputeHorizontalScrollOffset();
            int range = this.ComputeHorizontalScrollRange() - this.ComputeHorizontalScrollExtent();
            if (range == 0)
            {
                return false;
            }

            if (direction < 0)
            {
                return offset > 0;
            }
            else
            {
                return offset < range - 1;
            }
        }

        public override void ComputeScroll()
        {
            bool val = this.scroller.ComputeScrollOffset();
            
            if (val)
            {
                int oldX = this.ScrollX;
                int oldY = this.ScrollY;
                int x = this.scroller.CurrX;
                int y = this.scroller.CurrY;

                ////this.prevY = y;

                /*int dy = -20;
                if (this.ScrollY + dy < 0)
                {
                    dy = this.ScrollY;
                    isFlinging = false;
                }*/

                if (oldX != x || oldY != y)
                {
                    int rangeX = this.GetScrollRangeX();
                    int rangeY = this.GetScrollRangeY();

                    ////this.ScrollBy(x - oldX, y - oldY);

                    this.OverScrollBy(x - oldX, y - oldY, oldX, oldY, rangeX, rangeY, this.overflingDistance, this.overflingDistance, false);
                    ////this.OverScrollBy(0, dy, oldX, oldY, rangeX, rangeY, overflingDistance, overflingDistance, false);
                    //this.OnScrollChanged(this.ScrollX, this.ScrollY, oldX, oldY);
                }
            }
            else
            {
                /*FlingStrictSpan*/
            }
        }

        public void ScrollToHorizontalOffset(int offset)
        {
            int width = this.Width - this.PaddingLeft - this.PaddingRight;
            int height = this.Height - this.PaddingBottom - this.PaddingTop;
            
            int bottom = this.ContentHeight;
            int right = this.ContentWidth;

            this.scroller.FlingFromCode(offset, this.ScrollY, this.ScrollX, this.ScrollY, 0, Math.Max(0, right - width), 0, Math.Max(0, bottom - height));
            this.ComputeScroll();
        }

        public void ScrollToVerticalOffset(int offset)
        {
            int width = this.Width - this.PaddingLeft - this.PaddingRight;
            int height = this.Height - this.PaddingBottom - this.PaddingTop;

            int bottom = this.ContentHeight;
            int right = this.ContentWidth;

            this.scroller.FlingFromCode(this.ScrollX, offset, this.ScrollX, this.ScrollY, 0, Math.Max(0, right - width), 0, Math.Max(0, bottom - height));
            this.ComputeScroll();
        }

        public void MoveToVerticalOffset(int offset)
        {
            int rangeX = this.GetScrollRangeX();
            int rangeY = this.GetScrollRangeY();

            this.scroller.AbortAnimation();
            this.ComputeScroll();

            this.OverScrollBy(0, offset, this.ScrollX, this.ScrollY, rangeX, rangeY, this.overflingDistance, this.overflingDistance, false);
        }

        protected override void OnOverScrolled(int scrollX, int scrollY, bool clampedX, bool clampedY)
        {
            this.ScrollTo(scrollX, scrollY);
        }

        protected override int ComputeHorizontalScrollExtent() 
        {
            int contentWidth = this.ComputeHorizontalScrollRange();
            if (this.Width != 0 && contentWidth / this.Width > 0)
            {
                int w = (int)((double)this.Width * (double)this.Width / (double)contentWidth);

                if (w > 30)
                {
                    return w;
                }

                return 30;
            }

            return 0;
        }

        protected override int ComputeVerticalScrollExtent()
        {
            int contentHeight = this.ComputeVerticalScrollRange();
            if (this.Height != 0 && contentHeight / this.Height > 0)
            {
                int h = (int)((double)this.Height * (double)this.Height / (double)contentHeight);

                if (h > 30)
                {
                    return h;
                }

                return 30;
            }

            return 0;
        }

        protected override int ComputeHorizontalScrollOffset()
        {
            double d1 = this.ComputeHorizontalScrollRange();
            double d2 = this.Width;
            double l1 = base.ComputeHorizontalScrollOffset();

            double l2 = (d1 - this.ComputeHorizontalScrollExtent()) * l1 / (d1 - d2);

            return (int)l2;
        }
        protected override int ComputeVerticalScrollOffset()
        {
            double d1 = this.ComputeVerticalScrollRange();
            double d2 = this.Height;
            double l1 = base.ComputeVerticalScrollOffset();

            double l2 = (d1 - this.ComputeVerticalScrollExtent()) * l1 / (d1 - d2);

            return (int)l2;
        }

        protected override int ComputeHorizontalScrollRange() 
        {
            return this.ContentWidth;
        }
        protected override int ComputeVerticalScrollRange()
        {
            return this.ContentHeight;
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            if (changed)
            {
                if (this.ChildView != null)
                {
                    ChildView.Layout(left, top, this.ContentWidth, this.ContentHeight);
                }
            }
        } 

        private void OnSecondaryPointerUp(MotionEvent e)
        {
            int pointerIndex = (int)((object)(e.Action & MotionEventActions.PointerIndexMask)) >>
                (int)((object)MotionEventActions.PointerIndexShift);
            int pointerId = e.GetPointerId(pointerIndex);
            if (pointerId == this.activePointerId)
            {
                int newPointerIndex = pointerIndex == 0 ? 1 : 0;
                this.lastMotionX = (int)e.GetX(newPointerIndex);
                this.lastMotionY = (int)e.GetY(newPointerIndex);
                this.activePointerId = e.GetPointerId(newPointerIndex);
                if (this.velocityTracker != null)
                {
                    this.velocityTracker.Clear();
                }
            }
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            if (this.ScrollChanged != null)
            {
                var args = new NativeScrollChangedEventArgs()
                {
                    l = l,
                    t = t,
                    oldl = oldl,
                    oldt = oldt
                };
                this.ScrollChanged(this, args);
            }
            base.OnScrollChanged(l, t, oldl, oldt);
        }

        private void EndDrag()
        {
            this.isBeingDragged = false;
            this.RecycleVelocityTracker();
        }

        private void Fling(int velocityX, int velocityY)
        {
            ////velocityX = 0;
            if (this.ChildCount > 0)
            {
                int width = this.Width - this.PaddingLeft - this.PaddingRight;
                int height = this.Height - this.PaddingBottom - this.PaddingTop;

                int bottom = this.ContentHeight;
                int right = this.ContentWidth;

                ////this.prevY = this.ScrollX;

                this.scroller.Fling(this.ScrollX, this.ScrollY, velocityX, velocityY, 0, Math.Max(0, right - width), 0, Math.Max(0, bottom - height));
                this.ComputeScroll();
                ////this.isFlinging = true;
            }
        }

        private bool InChild(int x, int y)
        {
            if (this.ChildCount > 0)
            {
                View child = this.GetChildAt(0);
                return !(y < child.Top - this.ScrollY
                        || y >= child.Bottom - this.ScrollY
                        || x < child.Left
                        || x >= child.Right);
            }
            return false;
        }

        private int GetScrollRangeX()
        {
            return Math.Max(0,
                        this.ContentWidth - (this.Width - this.PaddingLeft - this.PaddingRight));
        }

        private int GetScrollRangeY()
        {
            return Math.Max(0,
                        this.ContentHeight - (this.Height - this.PaddingBottom - this.PaddingTop));
        }

        private void InitScrollView()
        {
            this.scroller = new MyOverScroller();
            this.Focusable = true;
            this.DescendantFocusability = Android.Views.DescendantFocusability.AfterDescendants;
            this.SetWillNotDraw(false);

            ViewConfiguration configuration = ViewConfiguration.Get(this.Context);
            this.touchSlop = configuration.ScaledTouchSlop;
            this.minimumVelocity = configuration.ScaledMinimumFlingVelocity;
            this.maximumVelocity = configuration.ScaledMaximumFlingVelocity;
            this.overflingDistance = 0;
        }

        private void InitConstructor(Context context)
        {
            this.InitScrollView();

            this.VerticalScrollBarEnabled = true;
            this.HorizontalScrollBarEnabled = true;
    
            var resourceStyleableView = this.GetScrollViewResourceStyleableView();
            TypedArray a = context.ObtainStyledAttributes(resourceStyleableView);
            this.InitializeScrollbars(a);
            a.Recycle();
        }

        private int[] GetScrollViewResourceStyleableView()
        {
            return new int[]
            {
                    Android.Resource.Attribute.ScrollbarSize,
                    Android.Resource.Attribute.ScrollbarThumbHorizontal,
                    Android.Resource.Attribute.ScrollbarThumbVertical,
                    Android.Resource.Attribute.ScrollbarTrackHorizontal,
                    Android.Resource.Attribute.ScrollbarTrackVertical,
                    Android.Resource.Attribute.ScrollbarAlwaysDrawHorizontalTrack,
                    Android.Resource.Attribute.ScrollbarAlwaysDrawVerticalTrack,
                    Android.Resource.Attribute.ScrollbarStyle,
                    Android.Resource.Attribute.Id,
                    Android.Resource.Attribute.Tag,
                    Android.Resource.Attribute.ScrollX,
                    Android.Resource.Attribute.ScrollY,
                    Android.Resource.Attribute.Background,
                    Android.Resource.Attribute.Padding,
                    Android.Resource.Attribute.PaddingLeft,
                    Android.Resource.Attribute.PaddingTop,
                    Android.Resource.Attribute.PaddingRight,
                    Android.Resource.Attribute.PaddingBottom,
                    Android.Resource.Attribute.Focusable,
                    Android.Resource.Attribute.FocusableInTouchMode,
                    Android.Resource.Attribute.Visibility,
                    Android.Resource.Attribute.FitsSystemWindows,
                    Android.Resource.Attribute.Scrollbars,
                    Android.Resource.Attribute.FadingEdge,
                    Android.Resource.Attribute.FadingEdgeLength,
                    Android.Resource.Attribute.NextFocusLeft,
                    Android.Resource.Attribute.NextFocusRight,
                    Android.Resource.Attribute.NextFocusUp,
                    Android.Resource.Attribute.NextFocusDown,
                    Android.Resource.Attribute.Clickable,
                    Android.Resource.Attribute.LongClickable,
                    Android.Resource.Attribute.SaveEnabled,
                    Android.Resource.Attribute.DrawingCacheQuality,
                    Android.Resource.Attribute.DuplicateParentState,
                    Android.Resource.Attribute.MinWidth,
                    Android.Resource.Attribute.MinHeight,
                    Android.Resource.Attribute.SoundEffectsEnabled,
                    Android.Resource.Attribute.KeepScreenOn,
                    Android.Resource.Attribute.IsScrollContainer,
                    Android.Resource.Attribute.HapticFeedbackEnabled,
                    Android.Resource.Attribute.OnClick,
                    Android.Resource.Attribute.ContentDescription,
                    Android.Resource.Attribute.ScrollbarFadeDuration,
                    Android.Resource.Attribute.ScrollbarDefaultDelayBeforeFade,
                    Android.Resource.Attribute.FadeScrollbars
            };
        }

        private void InitOrResetVelocityTracker()
        {
            if (this.velocityTracker == null)
            {
                this.velocityTracker = VelocityTracker.Obtain();
            }
            else
            {
                this.velocityTracker.Clear();
            }
        }

        private void InitVelocityTrackerIfNotExists()
        {
            if (this.velocityTracker == null)
            {
                this.velocityTracker = VelocityTracker.Obtain();
            }
        }

        private void RecycleVelocityTracker()
        {
            if (this.velocityTracker != null)
            {
                this.velocityTracker.Recycle();
                this.velocityTracker = null;
            }
        }
    }
}