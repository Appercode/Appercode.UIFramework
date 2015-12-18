using Appercode.UI.Controls.Primitives;
using Appercode.UI.Data;
using Appercode.UI.Input;
using Appercode.UI.StylesAndResources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public partial class ListBox : Selector
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(ListBox), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ListBox), new PropertyMetadata(SelectionMode.Single, (d, e) =>
                {
                    ((ListBox)d).ApplySelectionMode((SelectionMode)e.NewValue, (SelectionMode)e.OldValue);
                }));

        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(ListBox), new PropertyMetadata(null));

        private ScrollViewer scrollViewer;
        private bool isSelectedItemsChange = false;

        static ListBox()
        {
            ListBox.ItemsPanelProperty.AddOwner(typeof(ListBox), new PropertyMetadata(new ItemsPanelTemplate() { VisualTree = new FrameworkElementFactory(typeof(VirtualizingStackPanel)) },
                (d, e) =>
                {
                    ((ListBox)d).GeneratePanel();
                    ((ListBox)d).OnLayoutUpdated();
                }));
        }

        public ListBox()
        {
            this.SelectedItems = new ObservableCollection<object>();

            ((ObservableCollection<object>)this.SelectedItems).CollectionChanged += this.ListBox_CollectionChanged;
        }

        public IList SelectedItems
        {
            get
            {
                return (ObservableCollection<object>)GetValue(SelectedItemsProperty);
            }
            private set
            {
                this.SetValue(SelectedItemsProperty, value);
            }
        }

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)this.GetValue(SelectionModeProperty); }
            set { this.SetValue(SelectionModeProperty, value); }
        }

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { this.SetValue(ItemContainerStyleProperty, value); }
        }

        public void SelectAll()
        {
            if (this.SelectionMode == Controls.SelectionMode.Multiple)
            {
                this.IsSelectionActive = true;

                this.SelectedIndex = 0;
                this.SelectedItem = this.Items[0];
                this.SelectedValue = this.Items[0];

                this.IsSelectionActive = false;

                var addedItems = new List<object>();

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.SelectItem(i))
                    {
                        addedItems.Add(this.Items[i]);
                    }
                }

                this.OnSelectionChanged(new List<object>(), addedItems);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void ScrollIntoView(object item)
        {
            int index = this.Items.IndexOf(item);

            if (index == -1)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (this.panel is IVirtualizingPanel || this.scrollViewer == null)
            {
                return;
            }

            UIElement child = panel.Children[index];
            double horizontalOffset = 0;
            double verticalOffset = 0;

            var visibleRectangle = new RectangleF(
                (nfloat)this.scrollViewer.HorizontalOffset,
                (nfloat)this.scrollViewer.VerticalOffset,
                (nfloat)this.scrollViewer.ExtentWidth,
                (nfloat)this.scrollViewer.ExtentHeight);
            var itemRectangle = new RectangleF(child.TranslatePoint, child.RenderSize);

            if (this.scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                if (visibleRectangle.Width <= itemRectangle.Width || visibleRectangle.Left > itemRectangle.Left)
                {
                    horizontalOffset = itemRectangle.Left;
                    this.scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
                }
                else
                {
                    if (visibleRectangle.Right < itemRectangle.Right)
                    {
                        horizontalOffset = itemRectangle.Left - visibleRectangle.Width + itemRectangle.Width;
                        this.scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
                    }
                }
            }

            if (this.scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                verticalOffset = Math.Min(itemRectangle.Top, this.scrollViewer.ScrollableHeight - this.scrollViewer.ActualHeight); // - visibleRectangle.Height + itemRectangle.Height;
                this.scrollViewer.ScrollToVerticalOffset(verticalOffset);
            }
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.scrollViewer == null)
            {
                this.measuredFor = availableSize;
                return base.MeasureOverride(availableSize);
            }
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;

                this.scrollViewer.MeasureOverride(SizeF.Empty);
                return this.measuredSize = SizeF.Empty;
            }

            var isMeasureNotActual = !this.IsMeasureValid || !this.scrollViewer.IsMeasureValid;

            isMeasureNotActual |= this.measuredFor == null
                || (availableSize.Height < this.measuredFor.Value.Height && this.measuredSize.Height > availableSize.Height)
                || (availableSize.Width < this.measuredFor.Value.Width && this.measuredSize.Width > availableSize.Width);

            if (!isMeasureNotActual)
            {
                return this.measuredSize;
            }

            this.measuredFor = availableSize;
            var margin = this.Margin;
            availableSize = new SizeF(
                MathF.Max(0, availableSize.Width - margin.HorizontalThicknessF()),
                MathF.Max(0, availableSize.Height - margin.VerticalThicknessF()));

            var size = this.SizeThatFitsMaxAndMin(this.scrollViewer.MeasureOverride(this.SizeThatFitsMaxAndMin(availableSize)));

            this.measuredSize = new SizeF(size.Width + margin.HorizontalThicknessF(), size.Height + margin.VerticalThicknessF());
            this.IsMeasureValid = true;
            return this.measuredSize;
        }

        public override void Arrange(RectangleF finalRect)
        {
            if (this.scrollViewer == null)
            {
                base.Arrange(finalRect);
                return;
            }

            var margin = Margin;
            finalRect = new RectangleF(
                finalRect.Left + margin.LeftF(),
                finalRect.Top + margin.TopF(),
                finalRect.Width - margin.HorizontalThicknessF(),
                finalRect.Height - margin.VerticalThicknessF());
            this.scrollViewer.Arrange(finalRect);
            this.IsArrangeValid = true;
        }

        internal override ItemContainerGenerator CreateItemContainerGenerator()
        {
            var listBoxItemFactory = new FrameworkElementFactory(typeof(ListBoxItem));
            listBoxItemFactory.SetBinding(StyleProperty, new Binding(nameof(ItemContainerStyle)) { Source = this });
            listBoxItemFactory.SetBinding(ContentControl.ContentProperty, new Binding());
            listBoxItemFactory.AddHandler(TapEvent, new EventHandler<GestureEventArgs>(this.ListBoxItem_Tap));
            return new ItemContainerGenerator(listBoxItemFactory, this.Items);
        }

        protected virtual void SetupScrollViewer(ScrollViewer scrollViewer)
        {
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.LayoutUpdated += this.ScrollViewer_LayoutUpdated;
        }

        protected override void GeneratePanel()
        {
            if (this.panel != null)
            {
                if (this.panel is IVirtualizingPanel)
                {
                    ((IVirtualizingPanel)panel).ScrollOwner = null;
                }
                this.RemoveLogicalChild(this.panel);
                this.panel.Children.Clear();
                this.RemovePanelFromNativeContainer();
            }

            this.panel = (Panel)this.ItemsPanel.LoadContent();

            if (this.panel is IVirtualizingPanel)
            {
                ((IVirtualizingPanel)this.panel).Generator = this.ItemContainerGenerator;
                if (((IVirtualizingPanel)panel).HasNativeScroll)
                {
                    this.scrollViewer = null;
                }
                else
                {
                    if (this.scrollViewer == null)
                    {
                        this.scrollViewer = new ScrollViewer();
                        this.AddLogicalChild(this.scrollViewer);
                        this.SetupScrollViewer(this.scrollViewer);
                    }
                    ((IVirtualizingPanel)panel).ScrollOwner = this.scrollViewer;
                }
            }
            if (this.panel.Parent == null)
            {
                this.AddLogicalChild(this.panel);
                this.AddPanelToNativeContainer();
                this.UpdateItems(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected override void AddPanelToNativeContainer()
        {
            if (this.scrollViewer == null)
            {
                base.AddPanelToNativeContainer();
                return;
            }
            this.scrollViewer.Content = this.panel;
        }

        protected override void RemovePanelFromNativeContainer()
        {
            if (this.scrollViewer == null)
            {
                base.RemovePanelFromNativeContainer();
                return;
            }
            this.scrollViewer.Content = null;
        }

        protected override void UpdateItems(NotifyCollectionChangedEventArgs e)
        {
            base.UpdateItems(e);

            if (this.Items != null)
            {
                if (this.SelectedItems != null)
                {
                    for (var i = 0; i < this.SelectedItems.Count; i++)
                    {
                        var item = this.SelectedItems;
                        if (this.Items.Contains(item))
                        {
                            this.ApplySelectedItem(item);
                        }
                    }
                }

                foreach (var listBoxItem in this.panel.Children)
                {
                    listBoxItem.Tap -= this.ListBoxItem_Tap;
                    listBoxItem.Tap += this.ListBoxItem_Tap;
                }
            }
        }

        protected override void UnselectAllItems()
        {
            base.UnselectAllItems();

            this.isSelectedItemsChange = true;
            this.IsSelectionActive = true;

            var oldItems = this.SelectedItems;

            foreach (var item in this.SelectedItems)
            {
                var index = this.Items.IndexOf(item);
                if (index == -1)
                {
                    continue;
                }
                if (this.panel is IVirtualizingPanel)
                {
                    ((IVirtualizingPanel)this.panel).SetIsSelectedOnRealizedItem(index, false);
                }
                else
                {
                    var childItem = this.panel.Children[index] as ListBoxItem;
                    if (childItem != null)
                    {
                        childItem.IsSelected = false;
                    }
                }
            }
            this.SelectedItems.Clear();
            this.ItemContainerGenerator.SelectedIndexes.Clear();
            this.isSelectedItemsChange = false;
            this.IsSelectionActive = false;

            this.OnSelectionChanged(oldItems, new List<object>());
        }

        protected override bool SelectItem(int index)
        {
            base.SelectItem(index);

            this.isSelectedItemsChange = true;
            var wasItemSelected = this.SelectedItems.Add(this.Items[index]) != -1;
            this.isSelectedItemsChange = false;
            this.ItemContainerGenerator.SelectedIndexes.Add(index);

            if (this.panel is IVirtualizingPanel)
            {
                ((IVirtualizingPanel)this.panel).SetIsSelectedOnRealizedItem(index, true);
            }
            else
            {
                var childItem = this.panel.Children[index] as ListBoxItem;
                if (childItem != null)
                {
                    childItem.IsSelected = true;
                }
            }

            return wasItemSelected;
        }

        protected override IList GetSelectedItems()
        {
            return this.SelectedItems;
        }

        protected override void OnPaddingUpdated(System.Windows.Thickness padding)
        {
            base.OnPaddingUpdated(padding);
            var virtualizingPanel = this.panel as IVirtualizingPanel;
            if (virtualizingPanel != null)
            {
                virtualizingPanel.SetPadding(padding);
            }
        }

        private void ScrollViewer_LayoutUpdated(object sender, EventArgs e)
        {
            this.OnLayoutUpdated();
        }

        private void ListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.SelectionMode == Controls.SelectionMode.Single && !this.isSelectedItemsChange && e.Action != NotifyCollectionChangedAction.Remove && e.Action != NotifyCollectionChangedAction.Reset)
            {
                throw new InvalidOperationException();
            }
            if (!this.isSelectedItemsChange)
            {
                this.OnSelectionChanged(e.OldItems ?? new List<object>(), e.NewItems ?? new List<object>());
            }
        }

        private void ApplySelectionMode(SelectionMode newSelectionMode, SelectionMode oldSelectionMode)
        {
            if (this.SelectedItems.Count > 1 &&
                oldSelectionMode == Controls.SelectionMode.Multiple &&
                newSelectionMode == Controls.SelectionMode.Single)
            {
                this.isSelectedItemsChange = true;

                var newSelectedItem = this.SelectedItems[0];

                var oldItems = this.SelectedItems;
                oldItems.Remove(newSelectedItem);

                foreach (var item in this.SelectedItems)
                {
                    var index = this.Items.IndexOf(item);
                    this.ItemContainerGenerator.SelectedIndexes.Remove(index);

                    if (this.panel is IVirtualizingPanel)
                    {
                        ((IVirtualizingPanel)this.panel).SetIsSelectedOnRealizedItem(index, false);
                    }
                    else
                    {
                        var childItem = this.panel.Children[index] as ListBoxItem;
                        if (childItem != null)
                        {
                            childItem.IsSelected = false;
                        }
                    }
                }
                this.SelectedItems.Clear();
                this.SelectedItems.Add(newSelectedItem);

                this.IsSelectionActive = true;

                this.SelectedItem = newSelectedItem;
                this.SelectedValue = newSelectedItem;

                this.SelectedIndex = this.Items.IndexOf(this.SelectedItem);

                this.IsSelectionActive = false;
                this.isSelectedItemsChange = false;

                this.OnSelectionChanged(oldItems, new List<object>());
            }
        }

        protected void ListBoxItem_Tap(object sender, GestureEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null)
            {
                this.ListBoxItemSetSelected(listBoxItem);
            }
        }

        private void ListBoxItemSetSelected(ListBoxItem listBoxItem)
        {
            if (listBoxItem.IsSelected)
            {
                return;
            }
            var item = listBoxItem.DataContext;
            int index = this.Items.IndexOf(item);

            IList removedItems = new List<object>();

            if (this.SelectionMode == SelectionMode.Single)
            {
                this.isSelectedItemsChange = true;

                removedItems = this.SelectedItems;

                var ind = this.Items.IndexOf(this.SelectedItem);
                if (ind >= 0)
                {
                    this.ItemContainerGenerator.SelectedIndexes.Clear();
                    if (this.panel is IVirtualizingPanel)
                    {
                        ((IVirtualizingPanel)this.panel).SetIsSelectedOnRealizedItem(ind, false);
                    }
                    else
                    {
                        var childItem = this.panel.Children[ind] as ListBoxItem;
                        if (childItem != null)
                        {
                            childItem.IsSelected = false;
                        }
                    }
                }

                this.SelectedItems.Clear();
                this.ItemContainerGenerator.SelectedIndexes.Clear();
                this.isSelectedItemsChange = false;
            }

            this.IsSelectionActive = true;

            listBoxItem.IsSelected = true;

            this.isSelectedItemsChange = true;
            this.SelectedItems.Add(item);
            this.ItemContainerGenerator.SelectedIndexes.Add(index);
            this.isSelectedItemsChange = false;

            this.SelectedIndex = index;
            this.SelectedItem = item;
            this.SelectedValue = item;

            this.OnSelectionChanged(removedItems, new List<object> { this.SelectedItem });

            this.IsSelectionActive = false;
        }

        protected override void OnItemsSourceChanged(object oldValue, object newValue)
        {
            this.ItemContainerGenerator.SelectedIndexes.Clear();

            IEnumerable newCollection = (IEnumerable)newValue;
            if (newCollection != null)
            {
                var enumerator = newCollection.GetEnumerator();

                for (int index = 0; enumerator.MoveNext(); index++)
                {
                    if (this.SelectedItems.Contains(enumerator.Current))
                    {
                        this.ItemContainerGenerator.SelectedIndexes.Add(index);
                    }
                }
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }
    }
}