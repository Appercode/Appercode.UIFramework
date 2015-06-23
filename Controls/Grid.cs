using Appercode.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

#if __IOS__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Defines a flexible grid area that consists of columns and rows.
    /// </summary>
    public partial class Grid : Panel, IAddChild
    {
        #region Dependency properties definitions

        /*
        /// <summary>
        /// Identifies the Grid.ShowGridLines dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register(
                "ShowGridLines",
                typeof(bool),
                typeof(Grid),
                new PropertyMetadata(false, Grid.OnShowGridLinesPropertyChanged));
         * */

        #endregion //Dependency properties definitions

        #region Attached properties

        /// <summary>
        /// Identifies the Grid.Column attached property.
        /// </summary>
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached(
                "Column",
                typeof(int),
                typeof(Grid),
                new PropertyMetadata(0, Grid.OnCellAttachedPropertyChanged) /*,
                Grid.IsIntValueNotNegative*/);

        /// <summary>
        /// Identifies the Grid.ColumnSpan attached property.
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached(
                "ColumnSpan",
                typeof(int),
                typeof(Grid),
                new PropertyMetadata(1, Grid.OnCellAttachedPropertyChanged) /*,
            Grid.IsIntValueGreaterThanZero*/);

        /// <summary>
        /// Identifies the Grid.Row attached property.
        /// </summary>
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached(
                "Row",
                typeof(int),
                typeof(Grid),
                new PropertyMetadata(0, Grid.OnCellAttachedPropertyChanged) /*,
                Grid.IsIntValueNotNegative*/);

        /// <summary>
        /// Identifies the Grid.RowSpan attached property.
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached(
                "RowSpan",
                typeof(int),
                typeof(Grid),
                new PropertyMetadata(1, Grid.OnCellAttachedPropertyChanged) /*,
                Grid.IsIntValueGreaterThanZero*/);

        #endregion // Attached properties

        #region Fields

        /// <summary>
        /// Indicates that child rearrange was scheduled.
        /// </summary>
        private Dictionary<UIElement, bool> isChildRearrangeScheduled = new Dictionary<UIElement, bool>();

        /// <summary>
        /// Holds a fake column definiton list with sigle "*" column
        /// </summary>
        private IList<ColumnDefinition> internalColumnDefinitionCollection;

        /// <summary>
        /// Holds a fake row definiton list with sigle "*" row
        /// </summary>
        private IList<RowDefinition> internalRowDefinitionCollection;

        /// <summary>
        /// Holds column definitions collection
        /// </summary>
        private ColumnDefinitionCollection columnDefinitions;

        /// <summary>
        /// Holds row definitions collection
        /// </summary>
        private RowDefinitionCollection rowDefinitions;

        /// <summary>
        /// Holds last measured size
        /// </summary>
        private SizeF lastMeasuredSize;

        //// <summary>
        //// Holds a grid lines renderer instance
        //// </summary>
        ////private GridLinesRenderer _GridLinesRenderer;

        /// <summary>
        /// Holds cells cache list
        /// </summary>
        private List<CellCache> cells;

        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of Grid.
        /// </summary>
        public Grid()
        {
            this.columnDefinitions = new ColumnDefinitionCollection(this);
            this.rowDefinitions = new RowDefinitionCollection(this);
            this.Children.CollectionChanged += this.Children_CollectionChanged;
        }

        #endregion //Constructors

        #region Properties

        /// <summary>
        /// Gets column definitions collection
        /// </summary>
        public ColumnDefinitionCollection ColumnDefinitions
        {
            get { return this.columnDefinitions; }
        }

        /// <summary>
        /// Gets row definitions collection
        /// </summary>
        public RowDefinitionCollection RowDefinitions
        {
            get { return this.rowDefinitions; }
        }

        /*
        /// <summary>
        /// Gets or sets a value that indicates whether grid lines are visible
        /// </summary>
        public bool ShowGridLines
        {
            get
            {
                return (bool)this.GetValue(ShowGridLinesProperty);
            }
            set
            {
                base.SetValue(ShowGridLinesProperty, value);
            }
        }
         * */

        /// <summary>
        /// Indicates that cell structure is durty and should be rebuilt
        /// </summary>
        protected bool CellsStructureDirty { get; set; }

        /*
        /// <summary>
        /// Gets an enumerator that can iterate the logical children
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                var en = base.LogicalChildren;
                while (en.MoveNext())
                {
                    yield return en.Current;
                }
                if (EnsureGridLinesRenderer() != null)
                {
                    yield return _GridLinesRenderer;
                }
            }
        }
        
        /// <summary>
        /// Gets a value that represents the total number of children within this instance of Grid
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return (base.VisualChildrenCount + ((_GridLinesRenderer != null) ? 1 : 0));
            }
        }
        */

        /// <summary>
        /// Gets internal children list
        /// </summary>
        private IEnumerable<UIElement> InternalChildren
        {
            get
            {
                ////if (!this.ShowGridLines)
                ////{
                return this.Children;
                ////}

                ////var glr = EnsureGridLinesRenderer();
                ////return this.Children.Where(x => x != glr);
            }
        }

        /// <summary>
        /// Gets non empty row definitions list
        /// </summary>
        private IList<RowDefinition> RowDefinitionsInternal
        {
            get
            {
                if (this.RowDefinitions.Count == 0)
                {
                    if (this.internalRowDefinitionCollection == null)
                    {
                        var fakeRowDefinition = new RowDefinition();
                        fakeRowDefinition.ParentGrid = this;

                        this.internalRowDefinitionCollection = new List<RowDefinition>()
                                                               {
                                                                   fakeRowDefinition
                                                               };
                    }
                    return this.internalRowDefinitionCollection;
                }
                else
                {
                    if (this.internalRowDefinitionCollection != null)
                    {
                        this.internalRowDefinitionCollection = null;
                    }
                    return this.RowDefinitions;
                }
            }
        }

        /// <summary>
        /// Gets non empty columns definition list
        /// </summary>
        private IList<ColumnDefinition> ColumnDefinitionsInternal
        {
            get
            {
                if (this.ColumnDefinitions.Count == 0)
                {
                    if (this.internalColumnDefinitionCollection == null)
                    {
                        var fakeColumnDefinition = new ColumnDefinition();
                        fakeColumnDefinition.ParentGrid = this;

                        this.internalColumnDefinitionCollection = new List<ColumnDefinition>()
                                                                  {
                                                                      fakeColumnDefinition
                                                                  };
                    }
                    return this.internalColumnDefinitionCollection;
                }
                else
                {
                    if (this.internalColumnDefinitionCollection != null)
                    {
                        this.internalColumnDefinitionCollection = null;
                    }
                    return this.ColumnDefinitions;
                }
            }
        }

        #endregion //Properties

        #region Getters and setters

        /// <summary>
        /// Gets the value of Grid.Column attached property from a specified element
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Grid.Column attached property.</returns>
        public static int GetColumn(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (int)element.GetValue(ColumnProperty);
        }

        /// <summary>
        /// Sets the value of Grid.Column attached property to a specified element
        /// </summary>
        /// <param name="element">The element on which to set the property</param>
        /// <param name="value">The property value to set</param>
        public static void SetColumn(UIElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ColumnProperty, value);
        }

        /// <summary>
        /// Gets the value of Grid.ColumnSpan attached property from a specified element
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Grid.ColumnSpan attached property.</returns>
        public static int GetColumnSpan(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (int)element.GetValue(ColumnSpanProperty);
        }

        /// <summary>
        /// Sets the value of Grid.ColumnSpan attached property to a specified element
        /// </summary>
        /// <param name="element">The element on which to set the property</param>
        /// <param name="value">The property value to set</param>
        public static void SetColumnSpan(UIElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(ColumnSpanProperty, value);
        }

        /// <summary>
        /// Gets the value of Grid.Row attached property from a specified element
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Grid.Row attached property.</returns>
        public static int GetRow(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (int)element.GetValue(RowProperty);
        }

        /// <summary>
        /// Sets the value of Grid.Row attached property to a specified element
        /// </summary>
        /// <param name="element">The element on which to set the property</param>
        /// <param name="value">The property value to set</param>
        public static void SetRow(UIElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(RowProperty, value);
        }

        /// <summary>
        /// Gets the value of Grid.RowSpan attached property from a specified element
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the Grid.RowSpan attached property.</returns>
        public static int GetRowSpan(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (int)element.GetValue(RowSpanProperty);
        }

        /// <summary>
        /// Sets the value of Grid.RowSpan attached property to a specified element
        /// </summary>
        /// <param name="element">The element on which to set the property</param>
        /// <param name="value">The property value to set</param>
        public static void SetRowSpan(UIElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(RowSpanProperty, value);
        }

        #endregion // Getters and setters

        #region Overriden methods

        /// <summary>
        /// Measures the children of a System.Windows.Controls.Grid in anticipation of
        ///     arranging them during the System.Windows.Controls.Grid.ArrangeOverride(SizeF)
        ///     pass.
        /// </summary>
        /// <param name="availableSize">Indicates an upper limit size that should not be exceeded.</param>
        /// <returns>SizeF that represents the required size to arrange child content.</returns>
        public override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                return this.measuredSize = SizeF.Empty;
            }

            var isMeasureNotActual = !this.IsMeasureValid;

            isMeasureNotActual |= (this.measuredFor == null
                || (availableSize.Height < this.measuredFor.Value.Height && this.measuredSize.Height > availableSize.Height)
                || (availableSize.Width < this.measuredFor.Value.Width && this.measuredSize.Width > availableSize.Width));

            if (!isMeasureNotActual)
            {
                return this.measuredSize;
            }

            this.measuredFor = availableSize;
            this.CellsStructureDirty = true;
            availableSize = this.SizeThatFitsMaxAndMin(availableSize);

            var height = this.ReadLocalValue(Grid.HeightProperty) != DependencyProperty.UnsetValue ? (nfloat)this.Height : 0;
            var width = this.ReadLocalValue(Grid.WidthProperty) != DependencyProperty.UnsetValue ? (nfloat)this.Width : 0;

            if ((availableSize.Width == 0 && width == 0) || (availableSize.Height == 0 && height == 0) || this.Width == 0 || this.Height == 0)
            {
                this.measuredSize = this.SizeThatFitsMaxAndMin(new SizeF(width, height));
                this.IsMeasureValid = true;
                return this.measuredSize;
            }

            // Why is it here?
            ////var res = base.MeasureOverride(availableSize);

            SizeF availableContentSize = new SizeF(availableSize);
            if (!nfloat.IsPositiveInfinity(availableSize.Width))
            {
                availableContentSize.Width -= this.Margin.HorizontalThicknessF();
            }
            if (!nfloat.IsPositiveInfinity(availableSize.Height))
            {
                availableContentSize.Height -= this.Margin.VerticalThicknessF();
            }

            this.InitCellsCacheIfNeeded(ref availableContentSize);

            if (this.ReadLocalValue(Grid.HeightProperty) != DependencyProperty.UnsetValue)
            {
                availableContentSize.Height = (nfloat)this.Height;
            }
            if (this.ReadLocalValue(Grid.WidthProperty) != DependencyProperty.UnsetValue)
            {
                availableContentSize.Width = (nfloat)this.Width;
            }

            availableContentSize.Width += this.Margin.HorizontalThicknessF();
            availableContentSize.Height += this.Margin.VerticalThicknessF();

            this.measuredSize = availableContentSize;
            this.IsMeasureValid = true;
            return availableContentSize;
        }

        /// <summary>
        /// Arranges the content of a System.Windows.Controls.Grid element.
        /// </summary>
        /// <param name="finalRect"></param>
        /// <returns></returns>
        public override void Arrange(RectangleF finalRect)
        {
            var size = finalRect.Size;

            if(size == this.arrangedSize && this.IsArrangeValid)
            {
                base.Arrange(finalRect);
                return;
            }

            this.ArrangeChilds(size);

            base.Arrange(finalRect);

            // this.CellsStructureDirty = false;
            // return base.ArrangeOverride(finalSize);
            // return finalSize;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion //Overriden methods

        #region IAddChild interface implementation

        /// <summary>
        /// Implements IAddChild.AddChild(object)
        /// Adds a child ui element
        /// </summary>
        /// <param name="value"></param>
        void IAddChild.AddChild(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value is RowDefinition)
            {
                this.RowDefinitions.Add(value);
                return;
            }

            if (value is ColumnDefinition)
            {
                this.ColumnDefinitions.Add(value);
                return;
            }

            UIElement element = value as UIElement;
            if (element == null)
            {
                throw new ArgumentException(string.Format("Can't add element of type {0}", element.GetType()));
            }

            this.Children.Add(element);
        }

        /// <summary>
        /// Implements IAddChild.AddText(string)
        /// Adds a child ui element
        /// </summary>
        /// <param name="value"></param>
        void IAddChild.AddText(string value)
        {
            throw new NotImplementedException();
        }

        #endregion //IAddChild interface implementation

        #region Internal methods

        /// <summary>
        /// Invalidates the grid
        /// </summary>
        internal void Invalidate()
        {
            this.CellsStructureDirty = true;
            this.InvalidateMeasure();
        }

        #endregion // Internal methods

        #region Attached properties change handlers

        /// <summary>
        /// Invoked when one of column/column-span/ro/row-span attachecd properties changed
        /// Invalidates the parent grid
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private static void OnCellAttachedPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            Grid parent = LogicalTreeHelper.GetParent(target) as Grid; // TODO: Can we use VisualTreeHelper here?
            if (parent != null)
            {
                parent.Invalidate();
            }
        }

        /// <summary>
        /// Invoked on Grid.ShowGridLines dependency property change
        /// Invalidates the target grid
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnShowGridLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = (Grid)d;

            grid.OnLayoutUpdated();
        }

        #endregion // Attached properties change handlers

        #region Validation methods

        /// <summary>
        /// Validates that specified value is greater than zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsIntValueGreaterThanZero(object value)
        {
            return ((int)value) > 0;
        }

        /// <summary>
        /// Validates that specified value is greater than or equals to zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsIntValueNotNegative(object value)
        {
            return ((int)value) >= 0;
        }

        #endregion // Validation methods

        #region Private methods

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ////using (new PerformanceChecker(this, new object[] {sender, e}))
            ////{
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var child = item as UIElement;
                    this.isChildRearrangeScheduled[child] = false;
                    child.LayoutUpdated += this.Child_LayoutUpdated;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var child = item as UIElement;
                    child.LayoutUpdated -= this.Child_LayoutUpdated;
                    this.isChildRearrangeScheduled.Remove(child);
                }
            }

            this.OnLayoutUpdated();
            ////}
        }

        private void Child_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                this.ScheduleReArrangeIfNeeded((UIElement)sender);
            }
        }

        private void ScheduleReArrangeIfNeeded(UIElement child)
        {
            bool isScheduled;
            if (this.isChildRearrangeScheduled.TryGetValue(child, out isScheduled) && isScheduled)
            {
                return;
            }

            this.isChildRearrangeScheduled[child] = true;

            this.Dispatcher.BeginInvoke(
                delegate
            {
                try
                {
                    if (child.measuredFor != null)
                    {
                        var measuredSize = child.MeasureOverride(child.measuredFor.Value);
                        if (child.arrangedSize == measuredSize)
                        {
                            child.Arrange(new RectangleF(child.TranslatePoint, child.arrangedSize));
                            return;
                        }
                    }

                    this.InvalidateMeasure();
                }
                finally
                {
                    this.isChildRearrangeScheduled[child] = false;
                }
            });
        }

        /*
        /// <summary>
        /// Returns an instance of grid line renderer when we need it, or null - otherwise
        /// </summary>
        /// <returns></returns>
        private GridLinesRenderer EnsureGridLinesRenderer()
        {
            if (this.ShowGridLines && (_GridLinesRenderer == null))
            {
                _GridLinesRenderer = new GridLinesRenderer(this);
                //AddVisualChild(_GridLinesRenderer);
                this.Children.Add(_GridLinesRenderer);
            }
            if (!this.ShowGridLines && (_GridLinesRenderer != null))
            {
                //RemoveVisualChild(_GridLinesRenderer);
                this.Children.Remove(_GridLinesRenderer);
                _GridLinesRenderer = null;
            }
            return _GridLinesRenderer;
        }
         * */

        /// <summary>
        /// Initializes cells cache when it is empty or obsolete
        /// </summary>
        /// <param name="finalSize"></param>
        private void InitCellsCacheIfNeeded(ref SizeF finalSize)
        {
            if (this.cells == null || this.CellsStructureDirty || this.lastMeasuredSize != finalSize)
            {
                this.InitCellsCache(ref finalSize);
                this.lastMeasuredSize = finalSize;
            }
        }

        /// <summary>
        /// Arranges cells
        /// </summary>
        /// <param name="finalSize"></param>
        private void ArrangeCells(SizeF finalSize)
        {
            ////if (_InitCellsInProgress) return;

            ////try
            ////{
            ////_InitCellsInProgress = true;

            var margin = this.Margin;
            var availableContentSize = new SizeF(finalSize.Width - margin.HorizontalThicknessF(), finalSize.Height - margin.VerticalThicknessF());
            this.InitCellsCacheIfNeeded(ref availableContentSize);
            if (this.cells != null)
            {
                foreach (var cell in this.cells)
                {
                    this.ArrangeCell(cell, availableContentSize);
                }
            }

            ////if (EnsureGridLinesRenderer() != null)
            ////{
            ////    _GridLinesRenderer.Measure(finalSize);
            ////    _GridLinesRenderer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            ////}
            ////}
            ////finally
            ////{
            ////    _InitCellsInProgress = false;
            ////}
        }

        /// <summary>
        /// Arranges specified cell.
        /// </summary>
        /// <param name="cell">Target cell to arrange.</param>
        /// <param name="finalSize">Size of area for the cell.</param>
        private void ArrangeCell(CellCache cell, SizeF finalSize)
        {
            var rows = this.RowDefinitionsInternal.ListRange(cell.RowStart, cell.RowSpan);
            var columns = this.ColumnDefinitionsInternal.ListRange(cell.ColumnStart, cell.ColumnSpan);

            if (rows.Count == 0 || columns.Count == 0)
            {
                return;
            }

            var left = (nfloat)columns[0].Offset;
            var top = (nfloat)rows[0].Offset;
            var width = (nfloat)columns.Sum(x => x.ActualWidth);
            var height = (nfloat)rows.Sum(x => x.ActualHeight);

            if (left + width > finalSize.Width)
            {
                width = finalSize.Width - left;
            }
            if (width < 0)
            {
                width = 0;
            }

            if (top + height > finalSize.Height)
            {
                height = finalSize.Height - top;
            }
            if (height < 0)
            {
                height = 0;
            }

            RectangleF contentRectangle = new RectangleF(left, top, width, height);

            if (cell.Element.ReadLocalValue(UIElement.VerticalAlignmentProperty) != DependencyProperty.UnsetValue)
            {
                switch (cell.Element.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        contentRectangle.Y = top + height - cell.MeasuredSize.Height;
                        contentRectangle.Height = cell.MeasuredSize.Height;
                        break;
                    case VerticalAlignment.Center:
                        contentRectangle.Y = top + height / 2f - cell.MeasuredSize.Height / 2f;
                        contentRectangle.Height = cell.MeasuredSize.Height;
                        break;
                    case VerticalAlignment.Stretch:
                        contentRectangle.Y = top;
                        contentRectangle.Height = height;
                        break;
                    case VerticalAlignment.Top:
                        contentRectangle.Y = top;
                        contentRectangle.Height = cell.MeasuredSize.Height;
                        break;
                }
            }
            else
            {
                if (cell.Element.ReadLocalValue(UIElement.HeightProperty) != DependencyProperty.UnsetValue || cell.Element.ReadValueFromStyle(UIElement.HeightProperty) != DependencyProperty.UnsetValue)
                {
                    contentRectangle.Y = top + height / 2f - (nfloat)cell.Element.Height / 2f;
                    contentRectangle.Height = (nfloat)cell.Element.Height;
                }
                else
                {
                    contentRectangle.Y = top;
                    contentRectangle.Height = height;
                }
            }

            if (cell.Element.ReadLocalValue(UIElement.HorizontalAlignmentProperty) != DependencyProperty.UnsetValue)
            {
                switch (cell.Element.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        contentRectangle.X = left + width / 2f - cell.MeasuredSize.Width / 2f;
                        contentRectangle.Width = cell.MeasuredSize.Width;
                        break;
                    case HorizontalAlignment.Left:
                        contentRectangle.X = left;
                        contentRectangle.Width = cell.MeasuredSize.Width;
                        break;
                    case HorizontalAlignment.Right:
                        contentRectangle.X = left + width - cell.MeasuredSize.Width;
                        contentRectangle.Width = cell.MeasuredSize.Width;
                        break;
                    case HorizontalAlignment.Stretch:
                        contentRectangle.X = left;
                        contentRectangle.Width = width;
                        break;
                }
            }
            else
            {
                if (cell.Element.ReadLocalValue(UIElement.WidthProperty) != DependencyProperty.UnsetValue || cell.Element.ReadValueFromStyle(UIElement.WidthProperty) != DependencyProperty.UnsetValue)
                {
                    contentRectangle.X = left + width / 2f - (nfloat)cell.Element.Width / 2f;
                    contentRectangle.Width = (nfloat)cell.Element.Width;
                }
                else
                {
                    contentRectangle.X = left;
                    contentRectangle.Width = width;
                }
            }

            cell.Element.Arrange(contentRectangle);
        }

        private List<CellCache> PrepareCells(SizeF avaliableSize, bool treatColumnStarAsAuto, bool treatRowStarAsAuto)
        {
            var res = this.InternalChildren.Cast<UIElement>().Select(x => new CellCache()
            {
                Element = x,
                ColumnStart = GetColumn(x),
                ColumnSpan = GetColumnSpan(x),
                RowStart = GetRow(x),
                RowSpan = GetRowSpan(x),
            }).ToList();

            res.ForEach(x =>
            {
                if (x.ColumnStart >= this.ColumnDefinitionsInternal.Count)
                {
                    x.ColumnStart = this.ColumnDefinitionsInternal.Count - 1;
                }
                if (x.ColumnStart + x.ColumnSpan - 1 >= this.ColumnDefinitionsInternal.Count)
                {
                    x.ColumnSpan = this.ColumnDefinitionsInternal.Count - x.ColumnStart;
                }

                if (x.RowStart >= this.RowDefinitionsInternal.Count)
                {
                    x.RowStart = this.RowDefinitionsInternal.Count - 1;
                }
                if (x.RowStart + x.RowSpan - 1 >= this.RowDefinitionsInternal.Count)
                {
                    x.RowSpan = this.RowDefinitionsInternal.Count - x.RowStart;
                }

                var columns = this.ColumnDefinitionsInternal.EnumerableRange(x.ColumnStart, x.ColumnSpan);
                var rows = this.RowDefinitionsInternal.EnumerableRange(x.RowStart, x.RowSpan);

                x.IsColumnsAuto = columns.Any(y => y.Width.IsAuto);
                x.IsColumnsStar = columns.Any(y => y.Width.IsStar);

                x.IsRowsAuto = rows.Any(y => y.Height.IsAuto);
                x.IsRowsStar = rows.Any(y => y.Height.IsStar);

                if (treatColumnStarAsAuto)
                {
                    x.IsColumnsAuto |= x.IsColumnsStar;
                    x.IsColumnsStar = false;
                }
                if (treatRowStarAsAuto)
                {
                    x.IsRowsAuto |= x.IsRowsStar;
                    x.IsRowsStar = false;
                }
            });

            return res;
        }

        private void InitCellsCache(ref SizeF finalSize)
        {
            ////using (new PerformanceChecker(this, new object[] { finalSize }))
            ////{

            if (finalSize.IsEmpty)
            {
                return;
            }

            #region Prepare

            SizeF avaliableSize = new SizeF(finalSize.Width, finalSize.Height);

            this.CellsStructureDirty = false;

            bool treatColumnStarAsAuto = this.HorizontalAlignment != HorizontalAlignment.Stretch || nfloat.IsInfinity(finalSize.Width);
            bool treatRowStarAsAuto = this.VerticalAlignment != VerticalAlignment.Stretch || nfloat.IsInfinity(finalSize.Height);

            this.cells = this.PrepareCells(avaliableSize, treatColumnStarAsAuto, treatRowStarAsAuto);

            #endregion // Prepare

            #region Calculating draft sizes and offfsets for columns and rows

            int columnIndex = 0;
            double currentX = 0;
            foreach (var column in this.ColumnDefinitionsInternal)
            {
                column.FinalOffset = currentX; // This is draft value

                if (column.Width.IsAbsolute)
                {
                    column.ActualWidth = column.Width.Value;
                }
                else
                {
                    column.ActualWidth = column.MinWidth;
                }
                currentX += column.ActualWidth;
                ++columnIndex;
            }

            int rowIndex = 0;
            double currentY = 0;
            foreach (var row in this.RowDefinitionsInternal)
            {
                row.FinalOffset = currentY; // This is draft value

                if (row.Height.IsAbsolute)
                {
                    row.ActualHeight = row.Height.Value;
                }
                else
                {
                    row.ActualHeight = row.MinHeight;
                }
                currentY += row.ActualHeight;
                ++rowIndex;
            }

            #endregion //Calculating draft sizes and offfsets for columns and rows

            #region Pre-measure

            foreach (var cell in this.cells)
            {
                if (cell.IsColumnsStar && cell.IsRowsStar)
                {
                    continue;
                }

                double availableWidth;
                double availableHeight;

                if (cell.IsColumnsAbsolute)
                {
                    availableWidth = this.ColumnDefinitionsInternal.EnumerableRange(cell.ColumnStart, cell.ColumnSpan).Sum(x => x.ActualWidth);
                }
                else
                {
                    availableWidth = avaliableSize.Width - this.ColumnDefinitionsInternal[cell.ColumnStart].Offset;
                }

                if (cell.IsRowsAbsolute)
                {
                    availableHeight = this.RowDefinitionsInternal.EnumerableRange(cell.RowStart, cell.RowSpan).Sum(x => x.ActualHeight);
                }
                else
                {
                    availableHeight = avaliableSize.Height - this.RowDefinitionsInternal[cell.RowStart].Offset;
                }

                var t = cell.Measure(new SizeF((nfloat)availableWidth, (nfloat)availableHeight));
            }

            #endregion // Pre-measure

            #region Add size for 'auto' columns and rows if content doesn't fit in them

            foreach (var cellCache in this.cells.OrderBy(x => x.ColumnSpan).ThenBy(x => x.RowSpan))
            {
                if (cellCache.IsColumnsAuto && !cellCache.IsColumnsStar)
                {
                    var columns = this.ColumnDefinitionsInternal.EnumerableRange(cellCache.ColumnStart, cellCache.ColumnSpan, true);
                    var widthToAdd = cellCache.MeasuredSize.Width - columns.Sum(x => x.ActualWidth);

                    if (widthToAdd > 0)
                    {
                        foreach (var column in columns.Where(x => x.Width.IsAuto || (x.Width.IsStar && treatColumnStarAsAuto)))
                        {
                            var maxWidthToAdd = column.MaxWidth - column.ActualWidth;
                            if (widthToAdd > maxWidthToAdd)
                            {
                                column.ActualWidth = column.MaxWidth;
                                widthToAdd -= maxWidthToAdd;
                            }
                            else
                            {
                                column.ActualWidth += widthToAdd;
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var cellCache in this.cells.OrderBy(x => x.RowSpan))
            {
                if (cellCache.IsRowsAuto && !cellCache.IsRowsStar)
                {
                    var rows = this.RowDefinitionsInternal.EnumerableRange(cellCache.RowStart, cellCache.RowSpan, true);
                    var heighToAdd = cellCache.MeasuredSize.Height - rows.Sum(x => x.ActualHeight);

                    if (heighToAdd > 0)
                    {
                        foreach (var row in rows.Where(x => x.Height.IsAuto || (x.Height.IsStar && treatRowStarAsAuto)))
                        {
                            var maxHeightToAdd = row.MaxHeight - row.ActualHeight;                            
                            if (heighToAdd > maxHeightToAdd)
                            {
                                row.ActualHeight = row.MaxHeight;
                                heighToAdd -= maxHeightToAdd;
                            }
                            else
                            {
                                row.ActualHeight += heighToAdd;
                                break;
                            }
                        }
                    }
                }
            }

            #endregion //Add size for 'auto' columns and rows if content doesn't fit in them

            #region Devide rest size between 'star' columns and rows

            if (!treatColumnStarAsAuto)
            {
                double fullWidth = this.ColumnDefinitionsInternal.Sum(x => x.ActualWidth);

                var widthToDevide = finalSize.Width - fullWidth;
                if (widthToDevide > 0)
                {
                    var starWidthWeightSum = this.ColumnDefinitionsInternal.Where(x => x.Width.IsStar).Sum(x => x.Width.Value);
                    var widthForWeightOne = widthToDevide / starWidthWeightSum;

                    var possibleColumnsToRecalculateWidth = new List<ColumnDefinition>();

                    foreach (var columnDefinition in this.ColumnDefinitionsInternal.Where(x => x.Width.IsStar))
                    {
                        columnDefinition.ActualWidth = columnDefinition.Width.Value * widthForWeightOne;

                        if (columnDefinition.ActualWidth < columnDefinition.MinWidth)
                        {
                            columnDefinition.ActualWidth = columnDefinition.MinWidth;
                            starWidthWeightSum -= columnDefinition.Width.Value;
                        }
                        else if (columnDefinition.ActualWidth > columnDefinition.MaxWidth)
                        {
                            columnDefinition.ActualWidth = columnDefinition.MaxWidth;
                            starWidthWeightSum -= columnDefinition.Width.Value;
                        }
                        else
                        {
                            possibleColumnsToRecalculateWidth.Add(columnDefinition);
                        }

                        widthToDevide -= columnDefinition.ActualWidth;
                    }

                    if (possibleColumnsToRecalculateWidth.Count != 0 && widthToDevide > 0 && starWidthWeightSum > 0)
                    {
                        widthForWeightOne = widthToDevide / starWidthWeightSum;
                        foreach (var columnDefinition in possibleColumnsToRecalculateWidth)
                        {
                            columnDefinition.ActualWidth += columnDefinition.Width.Value * widthForWeightOne;
                        }
                    }
                }
            }

            if (!treatRowStarAsAuto)
            {
                double fullHeight = this.RowDefinitionsInternal.Sum(x => x.ActualHeight);

                var heightToDevide = finalSize.Height - fullHeight;
                if (heightToDevide > 0)
                {
                    var starHeightWeightSum = this.RowDefinitionsInternal.Where(x => x.Height.IsStar).Sum(x => x.Height.Value);
                    var heightForWeightOne = heightToDevide / starHeightWeightSum;

                    var possibleRowsToRecalculateHeight = new List<RowDefinition>();

                    foreach (var rowDefinition in this.RowDefinitionsInternal.Where(x => x.Height.IsStar))
                    {
                        rowDefinition.ActualHeight = rowDefinition.Height.Value * heightForWeightOne;

                        if (rowDefinition.ActualHeight < rowDefinition.MinHeight)
                        {
                            rowDefinition.ActualHeight = rowDefinition.MinHeight;
                            starHeightWeightSum -= rowDefinition.Height.Value;
                        }
                        else if (rowDefinition.ActualHeight > rowDefinition.MaxHeight)
                        {
                            rowDefinition.ActualHeight = rowDefinition.MaxHeight;
                            starHeightWeightSum -= rowDefinition.Height.Value;
                        }
                        else
                        {
                            possibleRowsToRecalculateHeight.Add(rowDefinition);
                        }

                        heightToDevide -= rowDefinition.ActualHeight;
                    }

                    if (possibleRowsToRecalculateHeight.Count != 0 && heightToDevide > 0 && starHeightWeightSum > 0)
                    {
                        heightForWeightOne = heightToDevide / starHeightWeightSum;
                        foreach (var rowDefinition in possibleRowsToRecalculateHeight)
                        {
                            rowDefinition.ActualHeight += rowDefinition.Height.Value * heightForWeightOne;
                        }
                    }
                }
            }

            #endregion //Devide rest size between 'star' columns and rows

            #region Final remeasure for cells with 'star' size

            foreach (var cell in this.cells)
            {
                if (!cell.IsColumnsStar && !cell.IsRowsStar)
                {
                    continue;
                }

                double width = 0.0;
                int startColumn = cell.ColumnStart;
                int endColumn = cell.ColumnStart + cell.ColumnSpan;
                for (int i = startColumn; i < endColumn; i++)
                {
                    width += this.ColumnDefinitionsInternal[i].ActualWidth;
                }

                double height = 0.0;
                int startRow = cell.RowStart;
                int endRow = cell.RowStart + cell.RowSpan;
                for (int i = startRow; i < endRow; i++)
                {
                    height += this.RowDefinitionsInternal[i].ActualHeight;
                }

                var t = cell.Measure(new SizeF((nfloat)width, (nfloat)height));
            }

            #endregion // Final remeasure for cells with 'star' size

            #region Offsets calculation

            currentX = 0;
            foreach (var columnDefinition in this.ColumnDefinitionsInternal)
            {
                columnDefinition.FinalOffset = currentX;
                currentX += columnDefinition.ActualWidth;
            }

            currentY = 0;
            foreach (var rowDefinition in this.RowDefinitionsInternal)
            {
                rowDefinition.FinalOffset = currentY;
                currentY += rowDefinition.ActualHeight;
            }

            #endregion //Offsets calculation

            finalSize = new SizeF(
                ////(nfloat)(treatColumnStarAsAuto ? currentX : finalSize.Width),
                ////(nfloat)(treatRowStarAsAuto ? currentY : finalSize.Height)
                (nfloat)currentX, (nfloat)currentY
            );

            ////if (EnsureGridLinesRenderer() != null)
            ////{
            ////    _GridLinesRenderer.InitLines(finalSize);
            ////}

            ////}
        }

        #endregion // Private methods

        #region Internal types

        /// <summary>
        /// Represents an additional cell information cache object
        /// </summary>
        internal class CellCache
        {
            #region Constructors

            /// <summary>
            /// Initializes the cell cache object
            /// </summary>
            public CellCache()
            {
                this.MeasuredSize = SizeF.Empty; //new SizeF(nfloat.PositiveInfinity, nfloat.PositiveInfinity);
            }

            #endregion //Constructors

            #region Properties

            /// <summary>
            /// Gets or sets starting row index
            /// </summary>
            public int RowStart { get; set; }

            /// <summary>
            /// Gets or sets row count
            /// </summary>
            public int RowSpan { get; set; }

            /// <summary>
            /// Gets or sets starting column index
            /// </summary>
            public int ColumnStart { get; set; }

            /// <summary>
            /// Gets or sets column count
            /// </summary>
            public int ColumnSpan { get; set; }

            /// <summary>
            /// Gets or sets related control
            /// </summary>
            public UIElement Element { get; set; }

            /// <summary>
            /// True if at least one related column has "star" width
            /// </summary>
            public bool IsColumnsStar { get; set; }

            /// <summary>
            /// True if at least one related column has "auto" width
            /// </summary>
            public bool IsColumnsAuto { get; set; }

            public bool IsColumnsAbsolute
            {
                get { return !this.IsColumnsAuto && !this.IsColumnsStar; }
            }

            /// <summary>
            /// True if at least one related row has "star" height
            /// </summary>
            public bool IsRowsStar { get; set; }

            /// <summary>
            /// True if at least one related row has "auto" height
            /// </summary>
            public bool IsRowsAuto { get; set; }

            public bool IsRowsAbsolute
            {
                get { return !this.IsRowsAuto && !this.IsRowsStar; }
            }

            /// <summary>
            /// Gets or sets size of container that MeasuredSize calculated for
            /// </summary>
            public SizeF? MeasuredFor { get; private set; }

            /// <summary>
            /// Gets or sets measured size
            /// </summary>
            public SizeF MeasuredSize { get; private set; }

            public int ColumnEnd
            {
                get { return this.ColumnStart + this.ColumnSpan - 1; }
            }

            public int RowEnd
            {
                get { return this.RowStart + this.RowSpan - 1; }
            }

            #endregion //Properties

            #region Public methods

            public bool IsInColumn(int columnIndex)
            {
                return
                    columnIndex >= this.ColumnStart
                    && columnIndex <= this.ColumnEnd;
            }

            public bool IsInRow(int rowIndex)
            {
                return
                    rowIndex >= this.RowStart
                    && rowIndex <= this.RowEnd;
            }

            public SizeF Measure(SizeF availableSize)
            {
                ////using (new PerformanceChecker(this.Element, new object[] { availableSize }))
                ////{
                ////var empty = SizeF.Empty;

                bool needReMeasure =
                    this.MeasuredFor == null
                    || (availableSize.Height < this.MeasuredFor.Value.Height
                        && this.MeasuredSize.Height > availableSize.Height)
                    || (availableSize.Width < this.MeasuredFor.Value.Width
                        && this.MeasuredSize.Width > availableSize.Width);

                if (needReMeasure)
                {
                    var newMeasuredSize = this.Element.MeasureOverride(availableSize);

                    this.MeasuredFor = availableSize;
                    this.MeasuredSize = newMeasuredSize;
                }

                return this.MeasuredSize;
                ////}
            }

            #endregion // Public methods
        }

        #endregion // Internal types
    }
}