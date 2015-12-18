using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class PivotVirtualizingPanel
    {
        private static readonly object[] EmptyArray = new object[0];
        private const int ChildrenCount = 3;
        private Dictionary<int, PivotItem> children;
        private int currentPage = -1;

        public event EventHandler CurrentPageChanged;

        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                if (this.currentPage != value)
                {
                    var oldValue = this.currentPage;
                    this.currentPage = value;

                    if (oldValue < 0)
                    {
                        // invalidate whole pivot on loading of the first item
                        // otherwise it will have zero size, which was measured without elements
                        this.InvalidateMeasure();
                    }

                    // refresh child position for new page
                    this.RefreshChildren();
                    this.ScrollToCurrentPage();

                    if (this.CurrentPageChanged != null)
                    {
                        this.CurrentPageChanged(this, EventArgs.Empty);
                    }

                    this.OnSelectionChanged(oldValue, value);
                }
            }
        }

        public void ItemsUpdated(NotifyCollectionChangedEventArgs e)
        {
            this.GenerateChildren();
            if (this.CurrentPage < 0 && this.DataItems != null && this.DataItems.Count > 0)
            {
                this.CurrentPage = 0;
            }

            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                // refresh updated item if need
                this.RefreshChild(e.NewStartingIndex);
            }
        }

        public void ArrangeItemAtPosition(UIElement child, int index)
        {
            child.MeasureOverride(this.arrangedSize);
            child.Arrange(this.GetChildRect(index));
        }

        public void RefreshNativeAdapter()
        {
            if (this.GenerateChildren() == false)
            {
                // if no new items generated, refresh children and arrange them
                this.RefreshChildren();
            }
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.ScrollOwner = new ScrollViewer();
                    this.AddLogicalChild(this.ScrollOwner);
                    var scrollView = this.ScrollOwner.NativeUIElement as UIScrollView;
                    if (scrollView != null)
                    {
                        scrollView.ShowsHorizontalScrollIndicator = false;
                        scrollView.ShowsVerticalScrollIndicator = false;
                        scrollView.PagingEnabled = true;
                        scrollView.DecelerationEnded += this.ScrollViewDecelerationEnded;
                        this.NativeUIElement = scrollView;

                        this.children = new Dictionary<int, PivotItem>(ChildrenCount);
                        this.GenerateChildren();
                    }
                }

                base.NativeInit();
            }
        }

        protected override void NativeArrange(CGRect finalRect)
        {
            base.NativeArrange(finalRect);
            var scrollView = this.NativeUIElement as UIScrollView;
            if (scrollView != null)
            {
                // set scroll content size to display all items
                scrollView.ContentSize = new CGSize(finalRect.Width * this.Count, finalRect.Height);
            }
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            PivotItem currentChild;
            if (this.children.TryGetValue(this.CurrentPage, out currentChild))
            {
                return currentChild.MeasureOverride(availableSize);
            }
            else
            {
                return base.NativeMeasureOverride(availableSize);
            }
        }

        private static int CalculateCurrentPage(UIScrollView scrollView)
        {
            return (int)Math.Floor(scrollView.ContentOffset.X / scrollView.Frame.Width + 0.5f);
        }

        private void ScrollViewDecelerationEnded(object sender, EventArgs e)
        {
            var scrollView = sender as UIScrollView;
            if (scrollView != null)
            {
                this.CurrentPage = CalculateCurrentPage(scrollView);
            }
        }

        private void ScrollToCurrentPage()
        {
            var scrollView = this.NativeUIElement as UIScrollView;
            if (scrollView != null)
            {
                if (this.CurrentPage != CalculateCurrentPage(scrollView))
                {
                    scrollView.ScrollRectToVisible(this.GetChildRect(this.CurrentPage), true);
                }
            }
        }

        private CGRect GetChildRect(int index)
        {
            return new CGRect(new CGPoint(index * this.arrangedSize.Width, 0), this.arrangedSize);
        }

        /// <summary>
        /// Generates children if required.
        /// </summary>
        /// <returns>If generation and refresh occurred, returns true, otherwise false.</returns>
        private bool GenerateChildren()
        {
            // we cannot generate child, until there are no item to display by this child
            var count = Math.Min(ChildrenCount, this.Count);
            var needGenerate = this.children.Count < count;
            if (needGenerate)
            {
                for (int i = this.children.Count; i < count; i++)
                {
                    var child = (PivotItem)this.Generator.ContainerFromIndex(i);
                    this.AddLogicalChild(child);
                    this.NativeUIElement.AddSubview(child.NativeUIElement);
                    this.children[-i] = child;      // generate item as hidden, correct position will be determined in RefreshChildren method
                }

                this.RefreshChildren();
            }

            return needGenerate;
        }

        /// <summary>
        /// Calculates position movements and calls RefreshChild for each child.
        /// </summary>
        private void RefreshChildren()
        {
            if (this.currentPage < 0)
            {
                return;
            }

            var movements = new PositionMovement[ChildrenCount];
            for (int i = 0; i < ChildrenCount; i++)
            {
                // define new positions as [currentPage - 1, currentPage, currentPage + 1]
                var newPosition = this.currentPage + i - 1;
                if (newPosition == this.Count)
                {
                    // if Current Page is last, last child will have position = -2 (invisible)
                    newPosition = -2;
                }

                movements[i] = new PositionMovement(newPosition);
            }

            // mark not moved children
            for (int i = 0; i < ChildrenCount; i++)
            {
                if (this.children.ContainsKey(movements[i].New))
                {
                    movements[i].MarkAsNotMoved();
                }
            }

            // set old position for moved child (usually it only one)
            foreach (var oldPosition in this.children.Keys)
            {
                // ensure there are no movement for this child
                if (movements.Any(m => m.Old == oldPosition) == false)
                {
                    for (int i = 0; i < ChildrenCount; i++)
                    {
                        // search "free" movement for the child
                        if (movements[i].Old < 0 && movements[i].IsMoved)
                        {
                            movements[i].Old = oldPosition;
                            break;
                        }
                    }
                }
            }

            // apply movements
            foreach (var movement in movements)
            {
                this.RefreshChild(movement);
            }
        }

        /// <summary>
        /// Places a child to it position.
        /// </summary>
        /// <param name="movement"></param>
        private void RefreshChild(PositionMovement movement)
        {
            PivotItem child;
            if (this.children.TryGetValue(movement.Old, out child) == false)
            {
                // if the child has not been generated yet, it cannot be placed anywhere
                return;
            }

            // change the position of the child
            if (movement.IsMoved)
            {
                this.children.Remove(movement.Old);
                this.children.Add(movement.New, child);
            }

            if (movement.New < 0)
            {
                // hide child with negative position
                child.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (movement.IsMoved)
                {
                    // refresh and show moved child
                    this.Generator.Reuse(movement.New, child);
                    child.Visibility = Visibility.Visible;
                }

                // arrange visible child, even it has not been moved
                this.ArrangeItemAtPosition(child, movement.New);
            }
        }

        /// <summary>
        /// Calls Reuse and Invalidate for updated item, if it displayed by any child.
        /// </summary>
        private void RefreshChild(int position)
        {
            PivotItem child;
            if (this.children.TryGetValue(position, out child))
            {
                this.Generator.Reuse(position, child);
                child.InvalidateMeasure();
            }
        }

        private void ArrangeCurrentlyInstantiatedElements()
        {
        }

        private void OnSelectionChanged(int oldPageIndex, int newPageIndex)
        {
            var oldItems = oldPageIndex >= 0 ? new[] { this.Generator.Items[oldPageIndex] } : EmptyArray;
            var newItems = newPageIndex >= 0 ? new[] { this.Generator.Items[newPageIndex] } : EmptyArray;
            this.SelectionChanged(this, new SelectionChangedEventArgs(oldItems, newItems));
        }

        private struct PositionMovement
        {
            public int New;
            public int Old;

            public PositionMovement(int newPosition)
            {
                this.New = newPosition;
                this.Old = -1;
            }

            public bool IsMoved
            {
                get { return this.New != this.Old; }
            }

            public void MarkAsNotMoved()
            {
                this.Old = this.New;
            }
        }
    }
}