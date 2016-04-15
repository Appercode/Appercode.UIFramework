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

        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(ItemsControl), new PropertyMetadata(new ItemsPanelTemplate() { VisualTree = new FrameworkElementFactory(typeof(StackPanel)) },
                (d, e) =>
                {
                    ((ItemsControl)d).GeneratePanel();
                    ((ItemsControl)d).OnLayoutUpdated();
                }));

        /// <summary>
        /// Identifies the <seealso cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ItemsControl), new PropertyMetadata(OnItemTemplatePropertyChanged));

        /// <summary>
        /// Identifies the <seealso cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(ItemsControl), new PropertyMetadata(OnItemTemplatePropertyChanged));

        protected Panel panel;
        private ItemContainerGenerator itemContainerGenerator;
        private ItemCollection items;

        private WeakEventHandler<NotifyCollectionChangedEventArgs> itemsSourceCollectionChangedHandler;

        private bool isItemsCollectionChangeFromCode = false;
        private bool itemsWasSetByUser = false;

        public ItemsControl()
        {
            this.itemsSourceCollectionChangedHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(this.ItemsSourceCollectionChanged);
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
                if (this.items == null)
                {
                    this.InitializeItems();
                }

                return this.items;
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

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate" /> used to display each item. 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector" />, which provides custom logic for choosing the <see cref="DataTemplate" /> for each item.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(ItemTemplateSelectorProperty); }
            set { this.SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets the <see cref="ItemContainerGenerator" /> associated with this <see cref="ItemsControl"/>.
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (this.itemContainerGenerator == null)
                {
                    this.InitializeItems();
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

        internal virtual ItemContainerGenerator CreateItemContainerGenerator()
        {
            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetBinding(ContentPresenter.ContentProperty, new Binding());
            return new ItemContainerGenerator(contentPresenterFactory, this.items);
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
                        this.panel.Children[e.OldStartingIndex] = (UIElement)this.ItemContainerGenerator.ContainerFromIndex(e.OldStartingIndex);
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
                    var container = (UIElement)this.ItemContainerGenerator.ContainerFromIndex(ind);
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

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = (ItemsControl)d;
            var containerFactory = itemsControl.ItemContainerGenerator.ContainerFactory;
            DependencyProperty templateProperty;
            DependencyProperty templateSelectorProperty;
            if (containerFactory.Type == typeof(ContentPresenter))
            {
                templateProperty = ContentPresenter.ContentTemplateProperty;
                templateSelectorProperty = ContentPresenter.ContentTemplateSelectorProperty;
            }
            else
            {
                templateProperty = ContentControl.ContentTemplateProperty;
                templateSelectorProperty = ContentControl.ContentTemplateSelectorProperty;
            }

            // TODO: Investigate if exception should be thrown if both Template and TemplateSelector are set
            var templateSelectorValue = itemsControl.ItemTemplateSelector;
            var templateValue = templateSelectorValue == null ? itemsControl.ItemTemplate : null;

            containerFactory.SetValue(templateProperty, templateValue);
            containerFactory.SetValue(templateSelectorProperty, templateSelectorValue);
            itemsControl.ItemContainerGenerator.ItemTemplateSelector = templateSelectorValue;
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke((Action<NotifyCollectionChangedEventArgs>)this.UpdateItems, e);
        }

        private void InitializeItems()
        {
            this.items = new ItemCollection();
            this.items.CollectionChanged += this.OnItemsCollectionChanged;
            this.itemContainerGenerator = this.CreateItemContainerGenerator();
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.isItemsCollectionChangeFromCode == false)
            {
                if (this.ItemsSource != null)
                {
                    throw new InvalidOperationException("ItemsCollection must be empty before using of ItemsSource");
                }

                this.itemsWasSetByUser = true;
                this.UpdateItems(e);
            }
        }
    }
}