using Android.App;
using Appercode.UI.Device;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Appercode.UI.Controls.Navigation.Primitives
{
    internal class ViewPagerPage : AppercodePage
    {

        public Android.Support.V4.View.ViewPager ViewPager { get; private set; }

        public FragmentManager FragmentManager { get; set; }

        protected internal override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return ((AppercodeFragmentPageAdapter)this.ViewPager.Adapter).Pages.GetEnumerator();
            }
        }

        public AppercodePage CurrentPage
        {
            get
            {
                return ((AppercodeFragmentPageAdapter)this.ViewPager.Adapter).Pages[this.ViewPager.CurrentItem];
            }
        }

        protected internal override void NativeInit()
        {
            if (this.ViewPager == null)
            {
                this.ViewPager = new Android.Support.V4.View.ViewPager(this.Context);
                this.ViewPager.Adapter = new AppercodeFragmentPageAdapter(this.FragmentManager);
                this.ViewPager.Id = 666;
            }
            base.NativeInit();
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            foreach (var page in ((AppercodeFragmentPageAdapter)this.ViewPager.Adapter).Pages)
            {
                page.MeasureOverride(availableSize);
            }
            return availableSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            base.Arrange(finalRect);
            foreach (var page in ((AppercodeFragmentPageAdapter)this.ViewPager.Adapter).Pages)
            {
                if(page.measuredFor == null)
                {
                    page.MeasureOverride(finalRect.Size);
                }
                page.Arrange(new RectangleF(finalRect.Left, finalRect.Top, finalRect.Width, finalRect.Height - ScreenProperties.ConvertPixelsToDPI(this.ViewPager.PaddingTop)));
            }
        }

        internal void AddPage(AppercodePage appercodePage, object p)
        {
            appercodePage.InternalOnNavigatedTo(new NavigationEventArgs(appercodePage.GetType(), p));
            this.AddLogicalChild(appercodePage);
            appercodePage.LayoutUpdated += appercodePage_LayoutUpdated;
            (this.ViewPager.Adapter as AppercodeFragmentPageAdapter).Pages.Add(appercodePage);
            (this.ViewPager.Adapter as AppercodeFragmentPageAdapter).NotifyDataSetChanged();
        }

        void appercodePage_LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.arrangedSize.IsEmpty)
            {
                ((UIElement)sender).MeasureOverride(this.arrangedSize);
                ((UIElement)sender).Arrange(new RectangleF(PointF.Empty, this.arrangedSize));
            }
        }

        private class AppercodeFragmentPageAdapter : Android.Support.V13.App.FragmentPagerAdapter
        {
            public AppercodeFragmentPageAdapter(FragmentManager fragmentManager)
                : base(fragmentManager)
            {
                this.Pages = new List<AppercodePage>();
            }

            public List<AppercodePage> Pages { get; private set; }

            public override Fragment GetItem(int position)
            {
                return Pages[position].NativeFragment;
            }

            public override int Count
            {
                get { return Pages.Count; }
            }
        }
    }
}