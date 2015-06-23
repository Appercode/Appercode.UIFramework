using Appercode.UI.Controls.Media;
using Appercode.UI.Controls.Primitives;
using Appercode.UI.Data;
using System.Collections;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial interface IPivotHeaderControl
    {
        IEnumerable ItemsSource { get; set; }
        int SelectedIndex { get; set; }
    }

    public partial class PivotHeadersControl : ListBox, IPivotHeaderControl
    {
        static PivotHeadersControl()
        {
            PivotHeadersControl.ItemsPanelProperty.AddOwner(
                typeof(PivotHeadersControl),
                new PropertyMetadata(CreateDefaultItemsPanelTemplate(),
                                        (d, e) =>
                                            {
                                                ((PivotHeadersControl) d).GeneratePanel();
                                                ((PivotHeadersControl) d).OnLayoutUpdated();
                                            }));
        }

        private static ItemsPanelTemplate CreateDefaultItemsPanelTemplate()
        {   
            var panelTemplateFactory = new FrameworkElementFactory(typeof (StackPanel));
            panelTemplateFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            var res = new ItemsPanelTemplate()
                {
                    VisualTree = panelTemplateFactory
                };

            return res;
        }

        public PivotHeadersControl()
        {
            this.SelectionMode = SelectionMode.Single;
        }

        internal void NotifyItemSelectionChanged(PivotHeaderItem item, bool oldValue, bool newValue)
        {
            if (this.panel == null || this.panel.Children == null) return;
            if (!newValue) return;
            if (oldValue) return;
            var index = this.panel.Children.IndexOf(item);
            if (index < 0) return;

            if (this.SelectedIndex != index)
            {
                this.SelectedIndex = index;
            }
        }

        protected override void SetupScrollViewer(ScrollViewer scrollViewer)
        {
            base.SetupScrollViewer(scrollViewer);
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        protected override void OnSelectionChanged(System.Collections.IList removedItems, System.Collections.IList addedItems)
        {
            base.OnSelectionChanged(removedItems, addedItems);
            
            if (this.SelectedItem != null)
            {
                ScrollIntoView(this.SelectedItem);
            }
        }

        // TODO: Move to resources
        static Brush _SelectedItemBackgroundBrush = new SolidColorBrush(AppercodeColors.LightGray);

        protected internal override ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (this.itemContainerGenerator == null)
                {
                    var listBoxItemFactory = new FrameworkElementFactory(typeof(PivotHeaderItem));
                    listBoxItemFactory.SetValue(ListBoxItem.ContentTemplateProperty, this.ItemTemplate);
                    listBoxItemFactory.SetBinding(ListBoxItem.StyleProperty, new Binding("ItemContainerStyle") { Source = this });
                    if (this.ItemTemplate == null)
                    {
                        listBoxItemFactory.SetBinding(
                            ListBoxItem.BackgroundProperty,
                            new Binding("IsSelected")
                                {
                                    RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                                    Converter = ValueConverterFactory.Make<bool, Brush>(x => x ? _SelectedItemBackgroundBrush : null),
                                });
                        listBoxItemFactory.SetBinding(ListBoxItem.ContentProperty, new Binding("Header"));
                        // TODO: listBoxItemFactory.SetValue(TextBlock.FontSizeProperty, 20);
                    }
                    this.itemContainerGenerator = new ItemContainerGenerator(listBoxItemFactory, this.Items);
                }

                return this.itemContainerGenerator;
            }
        }
    }

    public partial class CirclePivotHeaderControl : Control, IPivotHeaderControl
    {
        public static readonly DependencyProperty CircleColorProperty =
            DependencyProperty.RegisterAttached("CircleColor", typeof(Color), typeof(CirclePivotHeaderControl),
                new PropertyMetadata(Color.FromWhiteAlpha(128, byte.MaxValue), OnColorChanged));

        public static readonly DependencyProperty SelectedCircleColorProperty =
            DependencyProperty.RegisterAttached("SelectedCircleColor", typeof(Color), typeof(CirclePivotHeaderControl),
                new PropertyMetadata(AppercodeColors.White, OnColorChanged));

        public static Color GetCircleColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(CircleColorProperty);
        }

        public static void SetCircleColor(DependencyObject obj, Color value)
        {
            obj.SetValue(CircleColorProperty, value);
        }

        public static Color GetSelectedCircleColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(SelectedCircleColorProperty);
        }

        public static void SetSelectedCircleColor(DependencyObject obj, Color value)
        {
            obj.SetValue(SelectedCircleColorProperty, value);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pivot = d as Pivot;
            if (pivot != null)
            {
                var headerControl = pivot.PivotHeader as CirclePivotHeaderControl;
                if (headerControl != null)
                {
                    headerControl.OnColorChanged(e);
                }
            }
        }

        partial void OnColorChanged(DependencyPropertyChangedEventArgs e);
    }
}
