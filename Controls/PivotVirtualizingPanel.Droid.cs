using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Appercode.UI.Controls;
using Appercode.UI.Controls.NativeControl.Wrapers;
using Appercode.UI.Controls.Primitives;
using Java.Lang;
using ViewPagerIndicator;

namespace Appercode.UI.Controls
{
    #region Implementation classes

    internal class PivotAdapter : PagerAdapter, TitleProvider
    {
        public class UiElementTuple
        {
            public UiElementTuple(UIElement element, int index)
            {
                this.Element = element;
                this.Index = index;
            }

            public UIElement Element { get; private set; }
            public int Index { get; private set; }
        }

        public IList<UiElementTuple> CurrentlyInstantiatedElements { get; private set; }



        private UiElementTuple GetElementTuple(View nativeView)
        {
            return this.CurrentlyInstantiatedElements.FirstOrDefault(x => x.Element.NativeUIElement == nativeView);
        }

        private IPivotItemProvider _ItemProvider;

        public PivotAdapter(IPivotItemProvider itemProvider)
            : base()
        {
            _ItemProvider = itemProvider;
            this.CurrentlyInstantiatedElements = new List<UiElementTuple>(3);
        }

        public override int Count
        {
            get
            {
                return _ItemProvider.Count;
            }
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object @object)
        {
            var nativeViewToRemove = (View)@object;

            var vp = (ViewPager)container;
            vp.RemoveView(nativeViewToRemove);

            var uiElementToRemove = GetElementTuple(nativeViewToRemove);
            if (uiElementToRemove != null)
            {
                this.CurrentlyInstantiatedElements.Remove(uiElementToRemove);
            }
        }

        public override void StartUpdate(View container)
        {
            base.StartUpdate(container);
        }

        public override void StartUpdate(ViewGroup container)
        {
            base.StartUpdate(container);
        }

        public override void FinishUpdate(View container)
        {
            base.FinishUpdate(container);
        }

        public override void FinishUpdate(ViewGroup container)
        {
            base.FinishUpdate(container);
        }

        public override float GetPageWidth(int position)
        {
            var w = base.GetPageWidth(position);
            return w;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            var a = container;
            base.DestroyItem(container, position, @object);
        }

        public override int GetItemPosition(Java.Lang.Object @object)
        {
            var nativeView = (View)@object;

            var uiElementTuple = GetElementTuple(nativeView);
            if (uiElementTuple == null) return -1;
            return uiElementTuple.Index;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            var itemHeader = _ItemProvider.GetHeader(position);
            return new Java.Lang.String(GetTitle(position));
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            return this.InstantiateItem((View)container, position);
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            var itemElement = _ItemProvider.CreateItemElement(position);
            var res = itemElement.NativeUIElement;

            var uiElementTuple = new UiElementTuple(itemElement, position);
            this.CurrentlyInstantiatedElements.Add(uiElementTuple);

            var vp = (ViewPager)container;
            vp.AddView(res);

            return res;
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            var res = view == @object;
            return res;
        }

        public string GetTitle(int position)
        {
            var itemHeader = _ItemProvider.GetHeader(position);
            return itemHeader == null ? "" : itemHeader.ToString();
        }        
    }

    internal class WrappedViewPager : ViewPager, ITapableView, IJavaFinalizable
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
        /// <param name="javaReference"></param>
        /// <param name="transfer"></param>
        protected WrappedViewPager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            _TapDetector = new TapDetector(this);
        }

        /// <summary>
        /// Initializes the wrapped web view
        /// </summary>
        /// <param name="context"></param>
        public WrappedViewPager(Context context)
            : base(context)
        {
            _TapDetector = new TapDetector(this);
        }

        /// <summary>
        /// Initializes the wrapped web view
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public WrappedViewPager(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            _TapDetector = new TapDetector(this);
        }

        #endregion //Constructors

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
        }

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

    #endregion //Implementation classes

    public partial class PivotVirtualizingPanel
    {
        public void ItemsUpdated(NotifyCollectionChangedEventArgs e)
        {
        }

        private PivotAdapter _PivotAdapter;

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeStackPanel = new WrappedViewPager(this.Context);
                    _PivotAdapter = new PivotAdapter(this);
                    nativeStackPanel.Adapter = _PivotAdapter;
                    nativeStackPanel.CurrentItem = 0;

                    nativeStackPanel.SetOnPageChangeListener(new PageChangeListener(this));

                    this.NativeUIElement = nativeStackPanel;
                }
                base.NativeInit();
            }
        }

        private void ArrangeCurrentlyInstantiatedElements()
        {
            if (_PivotAdapter == null) return;

            foreach (var instantiatedElement in _PivotAdapter.CurrentlyInstantiatedElements)
            {
                instantiatedElement.Element.InvalidateMeasure();

                ArrangeItemAtPosition(instantiatedElement.Element, instantiatedElement.Index);
            }
        }

        public void ArrangeItemAtPosition(UIElement child, int index)
        {
            child.MeasureOverride(this.arrangedSize);
            var rect = new RectangleF(new PointF(index * this.arrangedSize.Width, 0), this.arrangedSize);
            child.Arrange(rect);
        }

        public void RefreshNativeAdapter()
        {
            if (_PivotAdapter != null)
            {
                _PivotAdapter.NotifyDataSetChanged();
            }
        }

        public int CurrentPage
        {
            get
            {
                return ((WrappedViewPager)this.NativeUIElement).CurrentItem;
            }
            set
            {
                var pager = (WrappedViewPager)this.NativeUIElement;
                if (pager.CurrentItem != value)
                {
                    pager.CurrentItem = value;
                }
            }
        }

        internal void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            this.SelectionChanged(this, e);
        }

        private class PageChangeListener : ViewPager.SimpleOnPageChangeListener
        {
            private static readonly object[] EmptyArray = new object[0];
            private readonly PivotVirtualizingPanel owner;
            private int currentSelectedIndex = 0;

            public PageChangeListener(PivotVirtualizingPanel owner)
            {
                this.owner = owner;
            }

            public override void OnPageSelected(int position)
            {
                var oldItems = this.currentSelectedIndex >= 0 ? new[] { this.owner.Generator.Items[this.currentSelectedIndex] } : EmptyArray;
                var newItems = position >= 0 ? new[] { this.owner.Generator.Items[position] } : EmptyArray;
                this.owner.OnSelectionChanged(new SelectionChangedEventArgs(oldItems, newItems));
                this.currentSelectedIndex = position;
            }
        }
    }
}