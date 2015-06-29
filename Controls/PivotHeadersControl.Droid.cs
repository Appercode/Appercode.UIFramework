using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Appercode.UI.Controls;
using Appercode.UI.Controls.NativeControl.Wrapers;
using Java.Interop;
using Java.Lang;

namespace Appercode.UI.Controls
{
    public partial interface IPivotHeaderControl
    {
        void SetNativeViewPager(ViewPager viewPagerr);
        void FreeNativeViewPager();
    }

    public partial class PivotHeadersControl
    {
        protected override void ApplySelectedIndex(int newSelectedIndex, int oldSelectedIndex)
        {
            base.ApplySelectedIndex(newSelectedIndex, oldSelectedIndex);

            if (_ViewPager.CurrentItem != this.SelectedIndex)
            {
                _ViewPager.CurrentItem = newSelectedIndex;
            }
        }

        private ViewPager _ViewPager;
        public void SetNativeViewPager(ViewPager viewPager)
        {
            if (_ViewPager == viewPager) return;

            if (_ViewPager != null)
            {
                //_ViewPager.SetOnPageChangeListener(this);
                _ViewPager.PageScrollStateChanged -= ViewPager_PageScrollStateChanged;

                _ViewPager.PageScrolled -= ViewPager_PageScrolled;

                _ViewPager.PageSelected -= ViewPager_PageSelected;
            }

            _ViewPager = viewPager;

            if (_ViewPager != null)
            {
                //_ViewPager.SetOnPageChangeListener(this);
                _ViewPager.PageScrollStateChanged += ViewPager_PageScrollStateChanged;

                _ViewPager.PageScrolled += ViewPager_PageScrolled;

                _ViewPager.PageSelected += ViewPager_PageSelected;
            }
        }

        public void FreeNativeViewPager()
        {
            if (_ViewPager != null)
            {
                //_ViewPager.SetOnPageChangeListener(this);
                _ViewPager.PageScrollStateChanged -= ViewPager_PageScrollStateChanged;

                _ViewPager.PageScrolled -= ViewPager_PageScrolled;

                _ViewPager.PageSelected -= ViewPager_PageSelected;
            }

            _ViewPager = null;
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs args)
        {
            this.SelectedIndex = args.Position;
        }

        private void ViewPager_PageScrolled(object sender, ViewPager.PageScrolledEventArgs args)
        {        
        }

        private void ViewPager_PageScrollStateChanged(object sender, ViewPager.PageScrollStateChangedEventArgs args)
        {
        }
    }

    #region ViewPagerIndicator-based headers

    #region Tabs

    public class WrappedTabPageIndicator : ViewPagerIndicator.TabPageIndicator, ITapableView, IJavaFinalizable
    {
        #region Fields

        /// <summary>
        /// Holds tap detector object instance
        /// </summary>
        private TapDetector _TapDetector;

        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Initializes the wrapped web view
        /// </summary>
        /// <param name="context"></param>
        public WrappedTabPageIndicator(Context context)
            : base(context, null)
        {
            _TapDetector = new TapDetector(this);
        }

        /// <summary>
        /// Initializes the wrapped web view
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public WrappedTabPageIndicator(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            _TapDetector = new TapDetector(this);
        }

        #endregion //Constructors

        #region Interfaces implementations

        public override bool OnTouchEvent(Android.Views.MotionEvent e)
        {
            _TapDetector.Detect(e);
            return base.OnTouchEvent(e);
        }

        public event EventHandler NativeTap;
        public event EventHandler JavaFinalized;

        public void WrapedNativeRaiseTap()
        {
            if (this.NativeTap != null)
            {
                this.NativeTap(this, null);
            }
        }

        protected override void JavaFinalize()
        {
            if (this.JavaFinalized != null)
            {
                this.JavaFinalized(null, null);
            }
            base.JavaFinalize();
        }

        #endregion //Interfaces implementations
    }

    public class TabPivotHeaderControl : Control, IPivotHeaderControl
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new WrappedTabPageIndicator(this.Context);
                    if (_ViewPager != null)
                    {
                        this.NativePageIndicator.SetViewPager(_ViewPager);
                    }
                }
            }
            base.NativeInit();
        }

        protected WrappedTabPageIndicator NativePageIndicator
        {
            get { return (WrappedTabPageIndicator)this.NativeUIElement; }
        }

        private System.Collections.IEnumerable _ItemsSource;
        public System.Collections.IEnumerable ItemsSource
        {
            get { return _ItemsSource; }
            set
            {
                _ItemsSource = value;
                this.NativePageIndicator.NotifyDataSetChanged();
            }
        }

        public int SelectedIndex { get; set; }

        private ViewPager _ViewPager;
        public void SetNativeViewPager(ViewPager viewPager)
        {
            if (_ViewPager == viewPager) return;
            _ViewPager = viewPager;
            this.NativePageIndicator.SetViewPager(_ViewPager);
        }

        public void FreeNativeViewPager()
        {
        }

        public override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        {
            var res = base.MeasureOverride(availableSize);
            return res;
        }
    }

    #endregion //Tabs

    #region Circles

    public class WrappedCirclePageIndicator : ViewPagerIndicator.CirclePageIndicator, ITapableView, IJavaFinalizable
    {
        #region Fields

        /// <summary>
        /// Holds tap detector object instance
        /// </summary>
        private TapDetector _TapDetector;

        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Initializes the wrapped web view
        /// </summary>
        /// <param name="context"></param>
        public WrappedCirclePageIndicator(Context context)
            : base(context)
        {
            _TapDetector = new TapDetector(this);
        }

        /// <summary>
        /// Initializes the wrapped web view
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public WrappedCirclePageIndicator(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            _TapDetector = new TapDetector(this);
        }

        #endregion //Constructors

        #region Interfaces implementations

        public override bool OnTouchEvent(Android.Views.MotionEvent e)
        {
            _TapDetector.Detect(e);
            return base.OnTouchEvent(e);
        }

        public event EventHandler NativeTap;
        public event EventHandler JavaFinalized;

        public void WrapedNativeRaiseTap()
        {
            if (this.NativeTap != null)
            {
                this.NativeTap(this, null);
            }
        }

        protected override void JavaFinalize()
        {
            if (this.JavaFinalized != null)
            {
                this.JavaFinalized(null, null);
            }
            base.JavaFinalize();
        }

        #endregion //Interfaces implementations
    }

    public partial class CirclePivotHeaderControl
    {
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new WrappedCirclePageIndicator(this.Context);
                    if (_ViewPager != null)
                    {
                        this.NativePageIndicator.SetViewPager(_ViewPager);
                    }
                }
            }
            base.NativeInit();
        }

        protected WrappedCirclePageIndicator NativePageIndicator
        {
            get { return (WrappedCirclePageIndicator) this.NativeUIElement; }
        }

        public System.Collections.IEnumerable ItemsSource { get; set; }

        public int SelectedIndex { get; set; }

        private ViewPager _ViewPager;

        public void SetNativeViewPager(ViewPager viewPager)
        {
            if (_ViewPager == viewPager) return;
            _ViewPager = viewPager;
            this.NativePageIndicator.SetViewPager(_ViewPager);
        }

        public void FreeNativeViewPager()
        {
        }

        public override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        {
            var res = base.MeasureOverride(availableSize);
            return res;
        }
    }

    #endregion //Circles

    #endregion //ViewPagerIndicator-based headers

}