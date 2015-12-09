using Appercode.UI.Helpers;
using Appercode.UI.Input;
using Appercode.UI.Internals;
using Appercode.UI.StylesAndResources;
using System;
using System.Collections;
using System.Windows;
using SW = System.Windows;

#if __IOS__
using PointF = CoreGraphics.CGPoint;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
using nfloat = System.Single;
#endif

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Base class for all Appercode Controls
    /// </summary>
    public abstract partial class UIElement : DependencyObject
    {
        #region DependencyProperties

        /// <summary>
        /// Identifies the <seealso cref="ActualHeight"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ActualHeightProperty;

        /// <summary>
        /// Identifies the <seealso cref="Tag"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register("Tag", typeof(object), typeof(UIElement),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="ActualWidth"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ActualWidthProperty;

        /// <summary>
        /// Identifies the <seealso cref="Height"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(UIElement), new PropertyMetadata(double.NaN, (d, e) =>
            {
                var uiEl = (UIElement)d;
                uiEl.NativeHeight = (double)e.NewValue;
                uiEl.InvalidateMeasure();
            }));

        /// <summary>
        /// Identifies the <seealso cref="Width"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(UIElement), new PropertyMetadata(double.NaN, (d, e) =>
            {
                var uiEl = (UIElement)d;
                uiEl.NativeWidth = (double)e.NewValue;
                uiEl.InvalidateMeasure();
            }));

        /// <summary>
        /// Identifies the <seealso cref="Margin"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(SW.Thickness), typeof(UIElement), new PropertyMetadata(new SW.Thickness(0, 0, 0, 0), (d, e) =>
            {
                var uiEl = (UIElement)d;
                uiEl.InvalidateMeasure();
            }));

        /// <summary>
        /// Identifies the <seealso cref="Name"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(UIElement), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <seealso cref="Visibility"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(UIElement), new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

        /// <summary>
        /// Identifies the <seealso cref="HorizontalAlignment"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register("HorizontalAlignment", typeof(SW.HorizontalAlignment), typeof(UIElement), new PropertyMetadata(SW.HorizontalAlignment.Stretch, (d, e) =>
                {
                    var uiEl = (UIElement)d;
                    if (uiEl.NativeUIElement != null)
                    {
                        uiEl.OnLayoutUpdated();
                    }
                }));

        /// <summary>
        /// Identifies the <seealso cref="VerticalAlignment"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register("VerticalAlignment", typeof(SW.VerticalAlignment), typeof(UIElement), new PropertyMetadata(SW.VerticalAlignment.Stretch, (d, e) =>
            {
                var uiEl = (UIElement)d;
                if (uiEl.NativeUIElement != null)
                {
                    uiEl.OnLayoutUpdated();
                }
            }));

        /// <summary>
        /// Identifies the <seealso cref="MaxWidth"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register("MaxWidth", typeof(double), typeof(UIElement), new PropertyMetadata(double.PositiveInfinity, (d, e) =>
                {
                    var uiEl = (UIElement)d;
                    uiEl.InvalidateMeasure();
                }));

        /// <summary>
        /// Identifies the <seealso cref="MaxHeight"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register("MaxHeight", typeof(double), typeof(UIElement), new PropertyMetadata(double.PositiveInfinity, (d, e) =>
            {
                var uiEl = (UIElement)d;
                uiEl.InvalidateMeasure();
            }));

        /// <summary>
        /// Identifies the <seealso cref="MinWidth"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(double), typeof(UIElement), new PropertyMetadata(0.0, (d, e) =>
            {
                var uiEl = (UIElement)d;
                uiEl.InvalidateMeasure();
            }));

        /// <summary>
        /// Identifies the <seealso cref="MinHeight"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register("MinHeight", typeof(double), typeof(UIElement), new PropertyMetadata(0.0, (d, e) =>
            {
                var uiEl = (UIElement)d;
                uiEl.InvalidateMeasure();
            }));

        /// <summary>
        /// Identifies the <seealso cref="Style"/> dependency property. 
        /// </summary>\
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register("Style", typeof(Style), typeof(UIElement), new PropertyMetadata(OnStyleChanged));

        /// <summary>
        /// Identifies the <seealso cref="DataContext"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object), typeof(UIElement), new PropertyMetadata(OnDataContextChanged));

        /// <summary>
        /// Identifies the <seealso cref="Opacity"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(UIElement), new PropertyMetadata(1.0d));

        public static readonly RoutedEvent LoadedEvent = new RoutedEvent("Loaded", typeof(RoutedEventHandler));
        public static readonly RoutedEvent TapEvent = new RoutedEvent("Tap", typeof(EventHandler<GestureEventArgs>));

        #endregion DependencyProperties

        /// <summary>
        /// availible size that was used during last calculating of size
        /// </summary>
        internal SizeF? measuredFor;

        /// <summary>
        /// size last time arranged to
        /// </summary>
        internal SizeF arrangedSize;

        /// <summary>
        /// size that was calculated during lasst meassure
        /// </summary>
        protected internal SizeF measuredSize;

        private static readonly DependencyPropertyKey ActualHeightPropertyKey;
        private static readonly DependencyPropertyKey ActualWidthPropertyKey;

        private bool isLayoutUpdatedScheduled = false;
        private UIElement parent;
        private Style styleCache;
        private Visibility visibilityCache;
        private ResourceDictionary resources;
        private SizeF renderSize;
        private bool hasHeightEverChanged;
        private bool hasWidthEverChanged;

        static UIElement()
        {
            ActualHeightPropertyKey = DependencyProperty.RegisterReadOnly(
                "ActualHeight", typeof(double), typeof(UIElement), new ReadOnlyFrameworkPropertyMetadata(default(double), GetActualHeight));
            ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;
            ActualWidthPropertyKey = DependencyProperty.RegisterReadOnly(
                "ActualWidth", typeof(double), typeof(UIElement), new ReadOnlyFrameworkPropertyMetadata(default(double), GetActualWidth));
            ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;
        }

        public UIElement()
        {
            var thisType = this.GetType();

            var styleMetadata = StyleProperty.GetMetadata(thisType);
            if (styleMetadata != null)
            {
                this.styleCache = styleMetadata.DefaultValue as Style;
            }

            var visibilityMetadata = VisibilityProperty.GetMetadata(thisType);
            if (visibilityMetadata != null)
            {
                this.visibilityCache = (Visibility)visibilityMetadata.DefaultValue;
            }
        }

        #region Events

        public event EventHandler<GestureEventArgs> Tap;
        public event RoutedEventHandler Loaded;
        public event EventHandler LayoutUpdated = delegate { };
        public event EventHandler ResourcesChanged = delegate { };
        public event RoutedEventHandler GotFocus;
        public event RoutedEventHandler LostFocus;
        internal event DataContextChangedEventHandler DataContextChanged;
        internal event EventHandler ParentChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the rendered height of a UIElement.
        /// </summary>
        public double ActualHeight
        {
            get { return this.RenderSize.Height; }
        }

        /// <summary>
        /// Gets or sets an arbitrary object value that can be used to store custom information about this element.
        /// </summary>
        public object Tag 
        {
            get { return this.GetValue(UIElement.TagProperty); }
            set { this.SetValue(UIElement.TagProperty, value); }
        }

        /// <summary>
        /// Gets the rendered width of a UIElement.
        /// </summary>
        public double ActualWidth
        {
            get { return this.RenderSize.Width; }
        }

        /// <summary>
        /// Gets or sets the degree of the object's opacity.
        /// </summary>
        public double Opacity
        {
            get { return (double)this.GetValue(OpacityProperty); }
            set { this.SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the suggested height of a UIElement.
        /// </summary>
        public virtual double Height
        {
            get
            {
                return (double)this.GetValue(HeightProperty);
            }
            set
            {
                this.SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the suggested width of a UIElement.
        /// </summary>
        public virtual double Width
        {
            get
            {
                return (double)this.GetValue(WidthProperty);
            }
            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets the locally defined resource dictionary.
        /// </summary>
        public ResourceDictionary Resources
        {
            get
            {
                if (this.resources == null)
                {
                    this.resources = new ResourceDictionary();
                }

                return this.resources;
            }
        }

        /// <summary>
        /// Gets or sets the outer margin of a FrameworkElement.
        /// </summary>
        public SW.Thickness Margin
        {
            get
            {
                return (SW.Thickness)this.GetValue(MarginProperty);
            }
            set
            {
                this.SetValue(MarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the identifying name of the object. 
        /// </summary>
        public string Name
        {
            get
            {
                return (string)this.GetValue(NameProperty);
            }
            set
            {
                this.SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance <seealso cref="Style"/> that is applied for this object during rendering.
        /// </summary>
        public Style Style
        {
            get { return this.styleCache; }
            set { this.SetValue(StyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of a UIElement. A UIElement that is not visible does not render and does not communicate its desired size to layout.
        /// </summary>
        public Visibility Visibility
        {
            get { return this.visibilityCache; }
            set { this.SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment characteristics that are applied to a FrameworkElement when it is composed in a layout parent, such as a panel or items control.
        /// </summary>
        public SW.HorizontalAlignment HorizontalAlignment
        {
            get { return (SW.HorizontalAlignment)this.GetValue(HorizontalAlignmentProperty); }
            set { this.SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment characteristics that are applied to a FrameworkElement when it is composed in a layout parent, such as a panel or items control.
        /// </summary>
        public SW.VerticalAlignment VerticalAlignment
        {
            get { return (SW.VerticalAlignment)this.GetValue(VerticalAlignmentProperty); }
            set { this.SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum height constraint of a UIElement. 
        /// </summary>
        public double MaxHeight
        {
            get { return (double)this.GetValue(MaxHeightProperty); }
            set { this.SetValue(MaxHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum width constraint of a UIElement. 
        /// </summary>
        public double MaxWidth
        {
            get { return (double)this.GetValue(MaxWidthProperty); }
            set { this.SetValue(MaxWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum height constraint of a UIElement. 
        /// </summary>
        public double MinHeight
        {
            get { return (double)this.GetValue(MinHeightProperty); }
            set { this.SetValue(MinHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the minimum width constraint of a UIElement. 
        /// </summary>
        public double MinWidth
        {
            get { return (double)this.GetValue(MinWidthProperty); }
            set { this.SetValue(MinWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data context for a UIElement when it participates in data binding.
        /// </summary>
        public object DataContext
        {
            get
            {
                var localValue = this.GetValue(DataContextProperty);
                if (this.ReadLocalValue(DataContextProperty) == DependencyProperty.UnsetValue)
                {
                    var parent = (UIElement)LogicalTreeHelper.GetParent(this);
                    if (parent != null)
                    {
                        return parent.DataContext;
                    }
                    else
                    {
                        return localValue;
                    }
                }
                else
                {
                    return localValue;
                }
            }
            set
            {
                this.SetValue(DataContextProperty, value);
            }
        }

        /// <summary>
        /// Gets the parent object of this UIElement in the logical tree. 
        /// </summary>
        public UIElement Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public UIElement TemplateChild { get; set; }

        public bool ShouldLookupImplicitStyles { get; set; }

        internal bool IsDataContextBound { get; set; }

        internal DependencyObject TemplatedParent
        {
            get { throw new NotImplementedException(); }
        }

        internal bool IsMeasureValid { get; set; }

        internal virtual bool IsArrangeValid { get; set; }

        internal bool IsInLiveTree
        {
            get
            {
                var parent = this.Parent;
                while (parent != null)
                {
                    if (parent is AppercodeVisualRoot)
                    {
                        return true;
                    }
                    parent = parent.Parent;
                }
                return false;
            }
        }

        /// <summary>
        /// Location of UIElement in the parent view.
        /// </summary>
        internal PointF TranslatePoint { get; set; }

        /// <summary>
        /// Gets the locally defined resource dictionary. May be not initialized.
        /// </summary>
        internal ResourceDictionary InternalResources
        {
            get { return this.resources; }
        }

        internal bool IsTemplateRoot { get; set; }

        internal SizeF RenderSize
        {
            get
            {
                return this.renderSize;
            }
            set
            {
                var oldValue = this.renderSize;
                this.renderSize = value;
                if (MathF.AreNotClose(oldValue.Width, value.Width))
                {
                    this.hasWidthEverChanged = true;
                    this.NotifyPropertyChange(ActualWidthProperty, value.Width, oldValue.Width);
                }

                if (MathF.AreNotClose(oldValue.Height, value.Height))
                {
                    this.hasHeightEverChanged = true;
                    this.NotifyPropertyChange(ActualHeightProperty, value.Height, oldValue.Height);
                }
            }
        }

        protected internal virtual IEnumerator LogicalChildren
        {
            get { return null; }
        }

        #endregion Properties

        /// <summary>
        /// Invoked whenever <see cref="ApplyTemplate"/> is called. In current implementation it is called on measuring.
        /// </summary>
        public virtual void OnApplyTemplate() { }

        public override object GetValue(DependencyProperty dp)
        {
            if (dp.ReadOnly)
            {
                return base.GetValue(dp);
            }

            var localValue = ReadLocalValue(dp);
            if (localValue != DependencyProperty.UnsetValue)
            {
                return localValue;
            }

            if (dp == StyleProperty)
            {
                return DependencyProperty.UnsetValue;
            }

            var fromStyle = this.ReadValueFromStyle(dp);
            return fromStyle == DependencyProperty.UnsetValue ? base.GetValue(dp) : fromStyle;
        }

        public virtual void UpdateLayout()
        {
        }

        /// <summary>
        /// Calculates nessasary size of element
        /// </summary>
        /// <returns>The size required for the element</returns>
        /// <param name="availableSize">Available size.</param>
        public virtual SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                this.measuredFor = availableSize;
                this.measuredSize = SizeF.Empty;
                return SizeF.Empty;
            }

            var isMeasureNotActual =
                !this.IsMeasureValid || this.measuredFor == null || this.measuredFor.Value != availableSize;
            if (isMeasureNotActual && availableSize.Width != 0 && availableSize.Height != 0)
            {
                this.ApplyTemplate();
                this.measuredFor = availableSize;
                this.measuredSize = this.SizeThatFitsMaxAndMin(this.NativeMeasureOverride(this.SizeThatFitsMaxAndMin(availableSize)));
                this.IsMeasureValid = true;
            }

            this.measuredFor = availableSize;
            return this.measuredSize;
        }

        /// <summary>
        /// Arrange element in the specified finalRect.
        /// </summary>
        /// <param name="finalRect">Final rect</param>
        public virtual void Arrange(RectangleF finalRect)
        {
            if (double.IsPositiveInfinity(finalRect.Width) || double.IsPositiveInfinity(finalRect.Height) || double.IsNaN(finalRect.Width) || double.IsNaN(finalRect.Height) || double.IsInfinity(finalRect.X) || double.IsInfinity(finalRect.Y) || double.IsNaN(finalRect.X) || double.IsNaN(finalRect.Y))
            {
                throw new InvalidOperationException("UIElement can't arrange in infinity or NaN rects");
            }

            var margin = this.Margin;
            var height = finalRect.Height - margin.VerticalThicknessF();
            var width = finalRect.Width - margin.HorizontalThicknessF();
            this.RenderSize = new SizeF(width < 0 ? finalRect.Width : width, height < 0 ? finalRect.Height : height);
            this.TranslatePoint = finalRect.Location;
            this.arrangedSize = finalRect.Size;
            this.NativeArrange(finalRect);
            this.IsArrangeValid = true;
        }

#warning was internal, conflicted with maps
        public void ChangeLogicalParent(UIElement newParent)
        {
            if (this.parent != null && newParent != null && this.parent != newParent)
            {
                throw new InvalidOperationException("Specified element is already the logical child of another element. Disconnect it first.");
            }

            if (newParent == this)
            {
                throw new InvalidOperationException("Element can not be selfparent.");
            }

            if (this.parent != newParent)
            {
                this.parent = newParent;
                if (this.parent != null)
                {
                    this.NativeInit();
                }

                var parentChanged = this.ParentChanged;
                if (parentChanged != null)
                {
                    parentChanged(this, EventArgs.Empty);
                }
            }
        }

        internal virtual void ApplyTemplate()
        {
            this.OnApplyTemplate();
        }

        internal virtual SizeF MeasureContentViewPort(SizeF availableSize)
        {
            SizeF availableSizeToContent = availableSize;

            availableSizeToContent.Width -= this.Margin.HorizontalThicknessF();
            availableSizeToContent.Height -= this.Margin.VerticalThicknessF();

            return availableSizeToContent;
        }

        internal virtual object ReadValueFromStyle(DependencyProperty dp)
        {
            var style = this.styleCache ?? ResourceDictionaryManager.GetResourceFromLogicalTree(this, this.GetType()) as Style;
            return style == null ? DependencyProperty.UnsetValue : style.FindValue(dp);
        }

        protected internal void InvalidateMeasure()
        {
            this.IsMeasureValid = false;
            this.InvalidateArrange();
        }

        private void InvalidateArrange()
        {
            if (this.IsArrangeValid)
            {
                this.IsArrangeValid = false;
                if (this.NativeUIElement != null)
                {
                    this.ScheduleLayoutUpdatedIfNeeded();
                }
            }
        }

        private void ScheduleLayoutUpdatedIfNeeded()
        {
            if (this.isLayoutUpdatedScheduled)
            {
                return;
            }

            this.isLayoutUpdatedScheduled = true;

            this.Dispatcher.BeginInvoke(
                delegate
            {
                try
                {
                    this.OnLayoutUpdated();
                }
                finally
                {
                    this.isLayoutUpdatedScheduled = false;
                }
            });
        }

        internal bool HasNonDefaultValue(DependencyProperty dependencyProperty)
        {
            return this.ReadLocalValue(dependencyProperty) != DependencyProperty.UnsetValue;
        }

        internal void RaiseBindingValidationError(ValidationErrorEventArgs validationErrorEventArg)
        {
            throw new NotImplementedException();
        }

        internal virtual object FindNameInPage(string name, bool calledFromUserControl)
        {
            return AppercodeVisualRoot.Instance.Child.FindName(name);
        }

        internal virtual void OnAncestorDataContextChanged(DataContextChangedEventArgs e)
        {
            if (!this.IsDataContextChangeRelevant(e))
            {
                return;
            }
            this.OnDataContextChanged(e);
            this.NotifyDataContextChanged(e);
        }

        internal virtual bool IsDataContextChangeRelevant(DataContextChangedEventArgs e)
        {
            if (this.IsDataContextBound || e.ChangeReason == DataContextChangedReason.EnteringLiveTree)
            {
                return true;
            }
            if (e.ChangeReason != DataContextChangedReason.NewDataContext)
            {
                return false;
            }
            return true;
        }

        internal void OnLoaded()
        {
            if (this.Loaded != null)
            {
                this.Loaded(this, new RoutedEventArgs() { OriginalSource = this });
            }
        }

        /// <summary>
        /// If this element is in a descendant of the ancestor
        /// </summary>
        internal bool InDescendantsOf(UIElement ancestor)
        {
            var parent = this.Parent;
            while (parent != null)
            {
                if (parent == ancestor)
                {
                    return true;
                }
                parent = parent.Parent;
            }
            return false;
        }

        protected internal void AddLogicalChild(object child)
        {
            if (child != null)
            {
                UIElement childUIElement = child as UIElement;
                if (childUIElement != null)
                {
                    childUIElement.ChangeLogicalParent(this);
                    childUIElement.OnAncestorDataContextChanged(new DataContextChangedEventArgs(DataContextChangedReason.EnteringLiveTree));
                }
            }
        }

        protected internal void RemoveLogicalChild(object child)
        {
            if (child != null)
            {
                UIElement childUIElement = child as UIElement;
                if (childUIElement != null && childUIElement.Parent == this)
                {
                    childUIElement.ChangeLogicalParent(null);
                }
            }
        }

        protected internal SizeF SizeThatFitsMaxAndMin(SizeF size)
        {
            var margin = this.Margin;
            var horizontalThickness = margin.HorizontalThicknessF();
            var verticalThickness = margin.VerticalThicknessF();

            if (size.Width > this.MaxWidth + horizontalThickness)
            {
                size.Width = (nfloat)this.MaxWidth + horizontalThickness;
            }

            if (size.Width < this.MinWidth + horizontalThickness)
            {
                size.Width = (nfloat)this.MinWidth + horizontalThickness;
            }

            if (size.Height > this.MaxHeight + verticalThickness)
            {
                size.Height = (nfloat)this.MaxHeight + verticalThickness;
            }

            if (size.Height < this.MinHeight + verticalThickness)
            {
                size.Height = (nfloat)this.MinHeight + verticalThickness;
            }

            return size;
        }

        protected virtual void OnLayoutUpdated()
        {
            this.LayoutUpdated(this, EventArgs.Empty);
        }

        protected virtual void OnGotFocus(RoutedEventArgs e)
        {
            var gotFocus = this.GotFocus;
            if (gotFocus != null)
            {
                gotFocus(this, e);
            }
        }

        protected virtual void OnLostFocus(RoutedEventArgs e)
        {
            var lostFocus = this.LostFocus;
            if (lostFocus != null)
            {
                lostFocus(this, e);
            }
        }

        protected virtual void OnVisibilityChanged(Visibility newValue)
        {
        }

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var args = new DataContextChangedEventArgs(DataContextChangedReason.NewDataContext);
            var uiElement = (UIElement)d;
            uiElement.OnDataContextChanged(args);
            uiElement.NotifyDataContextChanged(args);
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;
            if (uiElement != null && e.NewValue is Visibility)
            {
                var newValue = (Visibility)e.NewValue;
                uiElement.visibilityCache = newValue;
                uiElement.NativeVisibility = newValue;
                uiElement.OnVisibilityChanged(newValue);
                uiElement.InvalidateMeasure();
            }
        }

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;
            if (uiElement != null)
        {
                uiElement.styleCache = e.NewValue as Style;
                if (uiElement.NativeUIElement != null)
            {
                uiElement.NativeInit();
            }
        }
        }

        private static object GetActualHeight(DependencyObject d, out BaseValueSourceInternal source)
        {
            var uiElement = d as UIElement;
            if (uiElement != null && uiElement.hasHeightEverChanged)
            {
                source = BaseValueSourceInternal.Local;
                return uiElement.RenderSize.Height;
            }
            else
            {
                source = BaseValueSourceInternal.Default;
                return default(double);
            }
        }

        private static object GetActualWidth(DependencyObject d, out BaseValueSourceInternal source)
        {
            var uiElement = d as UIElement;
            if (uiElement != null && uiElement.hasWidthEverChanged)
            {
                source = BaseValueSourceInternal.Local;
                return uiElement.RenderSize.Width;
            }
            else
            {
                source = BaseValueSourceInternal.Default;
                return default(double);
            }
        }

        private object FindName(string name)
        {
            if (this.Name == name)
            {
                return this;
            }
            var children = LogicalTreeHelper.GetChildren(this);
            foreach (var child in children)
            {
                if (child is UIElement)
                {
                    var obj = ((UIElement)child).FindName(name);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        private void NotifyDataContextChanged(DataContextChangedEventArgs ea)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(this))
            {
                var childView = child as UIElement;
                if (childView != null &&
                    (ea.ChangeReason == DataContextChangedReason.EnteringLiveTree
                    || childView.ReadLocalValue(DataContextProperty) == DependencyProperty.UnsetValue))
                {
                    childView.OnAncestorDataContextChanged(ea);
                }
            }
        }

        private void OnDataContextChanged(DataContextChangedEventArgs ea)
        {
            if (this.IsInLiveTree)
            {
                this.DataContextChanged?.Invoke(this, ea);
            }
        }

        private void RaiseTap()
        {
            GestureEventArgs args = new GestureEventArgs();
            args.Handled = false;
            args.OriginalSource = this;

            UIElement currentElement = this;

            while (currentElement != null && args.Handled == false)
            {
                if (currentElement.Tap != null)
                {
                    currentElement.Tap(currentElement, args);
                }
                currentElement = (UIElement)LogicalTreeHelper.GetParent(currentElement);
            }
        }
    }
}