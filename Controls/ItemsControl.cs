using Appercode.Helpers;
using Appercode.UI.Controls.Primitives;
using Appercode.UI.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

#if __IOS__
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    public partial class ItemsControl : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsControl), new PropertyMetadata(null, (d, e) =>
                {
                    ((ItemsControl)d).OnItemsSourceChanged(e.OldValue, e.NewValue);
                }));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ItemsControl), new PropertyMetadata(null, (d, e) => ((ItemsControl)d).OnItemTemplateChanged()));

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ItemCollection), typeof(ItemsControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(ItemsControl), new PropertyMetadata(new ItemsPanelTemplate() { VisualTree = new FrameworkElementFactory(typeof(StackPanel)) },
                (d, e) =>
                {
                    ((ItemsControl)d).GeneratePanel();
                    ((ItemsControl)d).OnLayoutUpdated();
                }));

        protected Panel panel;
        private ItemContainerGenerator itemContainerGenerator;

        private WeakEventHandler<NotifyCollectionChangedEventArgs> itemsSourceCollectionChangedHandler;

        protected bool isItemsCollectionChangeFromCode = false;
        private bool itemsWasSetByUser = false;

        public ItemsControl()
        {
            this.itemsSourceCollectionChangedHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(this.ItemsSourceCollectionChanged);
            this.Items.CollectionChanged += (s, e) =>
                {
                    if (!this.isItemsCollectionChangeFromCode)
                    {
                        if (this.ItemsSource != null)
                        {
                            throw new InvalidOperationException("ItemsCollectionMustBeEmptyBeforeUsingItemsSource");
                        }
                        this.itemsWasSetByUser = true;
                        this.UpdateItems(e);
                    }
                };
            this.GeneratePanel();
        }

        /// <summary>
        /// Gets or sets a collection used to generate the content of the <see cref="ItemsControl"/>.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.GetValue(ItemsControl.ItemsSourceProperty) as IEnumerable;
            }
            set
            {
                this.SetValue(ItemsControl.ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets collection of Items
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                var items = (ItemCollection)this.GetValue(ItemsControl.ItemsProperty);
                if (items == null)
                {
                    items = new ItemCollection();
                    this.SetValue(ItemsControl.ItemsProperty, items);
                }
                return items;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display each item. 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ItemsControl.ItemTemplateProperty);
            }
            set
            {
                this.SetValue(ItemsControl.ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template that defines the panel that controls the layout of items. 
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)this.GetValue(ItemsPanelProperty); }
            set { this.SetValue(ItemsPanelProperty, value); }
        }

        protected internal virtual ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (this.itemContainerGenerator == null)
                {
                    var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                    contentPresenterFactory.SetValue(ContentPresenter.ContentTemplateProperty, this.ItemTemplate);
                    contentPresenterFactory.SetBinding(ContentPresenter.ContentProperty, new Binding());
                    this.itemContainerGenerator = new ItemContainerGenerator(contentPresenterFactory, this.Items);
                }
                return this.itemContainerGenerator;
            }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get { return this.Items == null ? null : this.Items.GetEnumerator(); }
        }

        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = SizeF.Empty;
            }

            // TODO: size caching

            this.measuredFor = availableSize;
            availableSize = new SizeF(
                MathF.Max(0, availableSize.Width - Margin.HorizontalThicknessF()),
                MathF.Max(0, availableSize.Height - Margin.VerticalThicknessF()));

            if (!double.IsNaN(this.Height))
            {
                availableSize.Height = (nfloat)this.Height;
            }

            if (!double.IsNaN(this.Width))
            {
                availableSize.Width = (nfloat)this.Width;
            }

            this.measuredSize = this.panel.MeasureOverride(this.SizeThatFitsMaxAndMin(availableSize));

            this.measuredSize.Width += this.Margin.HorizontalThicknessF();
            this.measuredSize.Height += this.Margin.VerticalThicknessF();

            return this.measuredSize;
        }

        protected virtual void GeneratePanel()
        {
            if (this.panel != null)
            {
                this.RemoveLogicalChild(this.panel);
                this.panel.Children.Clear();
                this.RemovePanelFromNativeContainer();
            }

            this.panel = (Panel)this.ItemsPanel.LoadContent();

            if (this.panel is IVirtualizingPanel)
            {
                ((IVirtualizingPanel)this.panel).Generator = this.ItemContainerGenerator;
            }

            if (this.panel.Parent == null)
            {
                this.AddLogicalChild(this.panel);
                this.AddPanelToNativeContainer();
                if (this.ItemsSource != null || this.Items.Count > 0)
                {
                    this.UpdateItems(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        protected override void OnBackgroundChanged()
        {
            this.panel.Background = this.Background;
        }

        protected virtual void ApplyItemsSource(IEnumerable itemsSource)
        {
        }

        protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual void UpdateItems(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (this.ItemsSource != null)
                    {
                        this.isItemsCollectionChangeFromCode = true;
                        this.Items.Clear();
                        this.isItemsCollectionChangeFromCode = false;
                    }
                    if (!(this.panel is IVirtualizingPanel))
                    {
                        this.panel.Children.Clear();
                    }

                    this.FillPanel(this.ItemsSource ?? this.Items);
                    break;

                case NotifyCollectionChangedAction.Add:
                    this.FillPanel(e.NewItems, e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = e.OldItems.Count; i > 0; i--)
                    {
                        this.isItemsCollectionChangeFromCode = true;
                        this.Items.RemoveAt(e.OldStartingIndex);
                        this.isItemsCollectionChangeFromCode = false;
                        if (!(this.panel is IVirtualizingPanel))
                        {
                            this.panel.Children.RemoveAt(e.OldStartingIndex);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    this.isItemsCollectionChangeFromCode = true;
                    this.Items[e.OldStartingIndex] = e.NewItems[0];
                    this.isItemsCollectionChangeFromCode = false;
                    if (!(this.panel is IVirtualizingPanel))
                    {
                        this.panel.Children[e.OldStartingIndex] = (UIElement)this.ItemContainerGenerator.Generate(e.OldStartingIndex);
                    }
                    break;

                // move does not exist in SL
                default:
                    break;
            }

            if (this.panel is IVirtualizingPanel)
            {
                ((IVirtualizingPanel)this.panel).ItemsUpdated(e);
            }
            this.OnLayoutUpdated();
        }

        protected virtual void ClearContainerForItemOverride(DependencyObject element, object item)
        {
        }

        private void OnItemTemplateChanged()
        {
            if (this.ItemContainerGenerator.ContainerFactory.Type == typeof(ContentPresenter))
            {
                this.ItemContainerGenerator.ContainerFactory.SetValue(ContentPresenter.ContentTemplateProperty, this.ItemTemplate);
            }
            else
            {
                this.ItemContainerGenerator.ContainerFactory.SetValue(ContentControl.ContentTemplateProperty, this.ItemTemplate);
            }
        }

        protected virtual void OnItemsSourceChanged(object oldValue, object newValue)
        {
            if (this.itemsWasSetByUser && this.Items.Count > 0)
            {
                throw new InvalidOperationException("ItemsCollectionMustBeEmptyBeforeUsingItemsSource");
            }

            this.itemsWasSetByUser = false;

            if (newValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)newValue).CollectionChanged += this.itemsSourceCollectionChangedHandler.Handler;
            }
            this.UpdateItems(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            this.ApplyItemsSource((IEnumerable)newValue);

            this.InvalidateMeasure();
        }

        protected virtual void FillPanel(IEnumerable newItems, int newStartingIndex = -1)
        {
            if (newStartingIndex < 0)
            {
                newStartingIndex = this.ItemsSource == null ? this.Items.Count - ((IList)newItems).Count : this.Items.Count;
            }

            var ind = newStartingIndex;
            var childrens = new List<UIElement>();
            foreach (var item in newItems)
            {
                this.isItemsCollectionChangeFromCode = true;
                if (this.ItemsSource != null)
                {
                    this.Items.Insert(ind, item);
                }

                this.isItemsCollectionChangeFromCode = false;
                if (!(this.panel is IVirtualizingPanel))
                {
                    var container = (UIElement)this.ItemContainerGenerator.Generate(ind);
                    childrens.Add(container);
                    //this.panel.Children.Insert(newStartingIndex, container);
                }

                ind++;
            }

            if(childrens.Count > 0)
            {
                this.panel.Children.InsertRange(newStartingIndex, childrens);
            }
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateItems(e);
        }
    }
}