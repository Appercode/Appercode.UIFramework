using System;
using System.Linq;
using Appercode.UI.Controls.Primitives;
using Appercode.UI.Data;
using System.Windows;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
#endif

namespace Appercode.UI.Controls
{
    public partial class Pivot : ItemsControl
    {
        #region Dependency properties
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Pivot), new PropertyMetadata(-1, SelectedIndexChanged));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(Pivot),
                new PropertyMetadata(
                    null,
                    delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
                    {
                        //var headerControl = ((PivotNew) d).pivotHeader;
                    }));

        public PivotHeaderMode HeaderMode
        {
            get { return (PivotHeaderMode)GetValue(HeaderModeProperty); }
            set { SetValue(HeaderModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderModeProperty =
            DependencyProperty.Register(
                "HeaderMode",
                typeof(PivotHeaderMode),
                typeof(Pivot),
                new PropertyMetadata(
                    PivotHeaderMode.Top,
                    delegate(DependencyObject d, DependencyPropertyChangedEventArgs args)
                    {
                        ((Pivot)d).ApplyHeaderMode();
                    }));

        #endregion //Dependency properties

        #region Constructors

        static Pivot()
        {
            /* TODO: Can we add header and panel using control template?
            PivotNew.TemplateProperty.AddOwner(
                typeof (PivotNew),
                new PropertyMetadata(
                    TODO: Create control template,
                    (d, e) =>
                    ((PivotNew) d).OnTemplateChanged((ControlTemplate) e.OldValue, (ControlTemplate) e.NewValue)));
            */


            Pivot.ItemsPanelProperty.AddOwner(typeof(Pivot), new PropertyMetadata(new ItemsPanelTemplate() { VisualTree = new FrameworkElementFactory(typeof(PivotVirtualizingPanel)) },
                (d, e) =>
                {
                    ((Pivot)d).GeneratePanel();
                    ((Pivot)d).OnLayoutUpdated();
                }));
        }

        public Pivot()
        {
            ApplyHeaderMode();
        }

        #endregion //Constructors

        #region Events
        public event SelectionChangedEventHandler SelectionChanged = delegate { };
        #endregion

        #region Fields


        private ItemContainerGenerator itemContainerGenerator;

        #endregion //Fields

        #region Properties

        public IPivotHeaderControl PivotHeader { get; private set; }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { this.SetValue(SelectedIndexProperty, value); }
        }

        protected internal override ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (this.itemContainerGenerator == null)
                {
                    var listBoxItemFactory = new FrameworkElementFactory(typeof(PivotItem));

                    listBoxItemFactory.SetValue(PivotItem.ContentTemplateProperty, this.ItemTemplate);
                    listBoxItemFactory.SetBinding(PivotItem.StyleProperty, new Binding("ItemContainerStyle") { Source = this });
                    listBoxItemFactory.SetBinding(PivotItem.ContentProperty, new Binding());
                    // listBoxItemFactory.AddHandler(ListBoxItem.TapEvent, new EventHandler<GestureEventArgs>(this.ListBoxItem_Tap));
                    this.itemContainerGenerator = new PivotItemContainerGenerator(listBoxItemFactory, this.Items);
                }

                return this.itemContainerGenerator;
            }
        }

        #endregion // Properties

        private SizeF pivotHeaderSize = SizeF.Empty;
        public override SizeF MeasureOverride(SizeF availableSize)
        {
            pivotHeaderSize = SizeF.Empty;
            if (this.PivotHeader != null)
            {
                switch (this.HeaderMode)
                {
                    case PivotHeaderMode.None:
                        ((UIElement)this.PivotHeader).MeasureOverride(pivotHeaderSize);
                        break;
                    case PivotHeaderMode.BottomCircles:
                        pivotHeaderSize = ((UIElement)this.PivotHeader).MeasureOverride(availableSize);
                        break;
                    default:
                        pivotHeaderSize = ((UIElement)this.PivotHeader).MeasureOverride(availableSize);
                        availableSize.Height -= pivotHeaderSize.Height;
                        break;
                }
            }

            var size = base.MeasureOverride(availableSize);
            if (pivotHeaderSize.Height > 0)
            {
                if (this.HeaderMode == PivotHeaderMode.BottomCircles)
                {
                    pivotHeaderSize.Width = MathF.Max(pivotHeaderSize.Width, size.Width);
                }
                else
                {
                    size.Height += pivotHeaderSize.Height;
                }
            }

            return size;
        }

        public override void Arrange(RectangleF finalRect)
        {
            if (PivotHeader != null)
            {
                RectangleF headerRectangle = RectangleF.Empty;

                switch (this.HeaderMode)
                {
                    case PivotHeaderMode.Top:
                    case PivotHeaderMode.TopTabs:
                        headerRectangle = new RectangleF(0, 0, pivotHeaderSize.Width, pivotHeaderSize.Height);
                        finalRect = new RectangleF(finalRect.Left, finalRect.Top + pivotHeaderSize.Height, finalRect.Width, finalRect.Height);
                        break;
                    case PivotHeaderMode.BottomCentered:
                    case PivotHeaderMode.BottomCircles:
                        var headerLeft = (finalRect.Width - pivotHeaderSize.Width) / 2;
                        headerRectangle = new RectangleF(headerLeft, finalRect.Height - pivotHeaderSize.Height, pivotHeaderSize.Width, pivotHeaderSize.Height);
                        // finalRect = new RectangleF(finalRect.Left, finalRect.Top, finalRect.Width, finalRect.Height - pivotHeaderSize.Height);
                        break;
                    case PivotHeaderMode.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ((UIElement)this.PivotHeader).Arrange(headerRectangle);
            }

            base.Arrange(finalRect);
        }

        public override void UpdateLayout()
        {
            this.Parent.UpdateLayout();
        }

        protected override void FillPanel(System.Collections.IEnumerable newItems, int newStartingIndex = -1)
        {
            base.FillPanel(newItems, newStartingIndex);

            var pivotPanel = (PivotVirtualizingPanel)this.panel;

            pivotPanel.DataItems = this.Items;
            pivotPanel.RefreshNativeAdapter();

            if (PivotHeader != null)
            {
                PivotHeader.ItemsSource = this.Items.ToList();
                PivotHeader.SelectedIndex = this.Items.Count > 0 ? 0 : -1;
            }
        }

        protected override void GeneratePanel()
        {
            if (this.panel != null)
            {
                this.panel.LayoutUpdated -= this.PanelLayoutUpdated;
            }

            var oldVirtualizingPanel = this.panel as PivotVirtualizingPanel;
            if (oldVirtualizingPanel != null)
            {
                oldVirtualizingPanel.SelectionChanged -= this.Pivot_SelectionChanged;
            }

            base.GeneratePanel();

            this.panel.LayoutUpdated += this.PanelLayoutUpdated;
            ((PivotVirtualizingPanel)this.panel).SelectionChanged += this.Pivot_SelectionChanged;
            SetNativeViewPagerToHeaderControl();
        }

        private static void SelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pivot = d as Pivot;
            if (pivot != null)
            {
                ((PivotVirtualizingPanel)pivot.panel).CurrentPage = e.NewValue is int ? (int)e.NewValue : 0;
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedIndex = (sender as PivotVirtualizingPanel).CurrentPage;
            this.SelectionChanged(this, e);
        }

        private void PanelLayoutUpdated(object sender, EventArgs e)
        {
            this.OnLayoutUpdated();
        }

        private void SetHeaderControl(IPivotHeaderControl header)
        {
            if (PivotHeader != null)
            {
                RemoveNativeViewPagerFromHeaderControl();
                RemoveLogicalChild((UIElement)PivotHeader);
            }
            PivotHeader = header;

            if (PivotHeader != null)
            {
                if (!(PivotHeader is UIElement))
                {
                    throw new ArgumentException("header must be UIElement", "header");
                }

                if (PivotHeader is ItemsControl)
                {
                    ((ItemsControl)PivotHeader).SetBinding(ItemsControl.ItemTemplateProperty,
                                                            new Binding("HeaderTemplate")
                                                                {
                                                                    Source = this
                                                                });
                }

                AddLogicalChild((UIElement)PivotHeader);
                SetNativeViewPagerToHeaderControl();
            }
        }

        private void ApplyHeaderMode()
        {
            if (this.HeaderMode == PivotHeaderMode.BottomCircles)
            {
                SetHeaderControl(new CirclePivotHeaderControl());
            }
#if __ANDROID__
            else if (this.HeaderMode == PivotHeaderMode.TopTabs)
            {
                SetHeaderControl(new TabPivotHeaderControl());
            }
#endif
            else
            {
                SetHeaderControl(new PivotHeadersControl());
            }

            this.OnLayoutUpdated();
        }
    }
}