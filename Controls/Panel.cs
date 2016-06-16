using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Provides a base class for all Panel elements. Use Panel elements to position and arrange child objects in Appercode applications. 
    /// </summary>
    public abstract partial class Panel : UIElement
    {
        /// <summary>
        /// Identifies the <seealso cref="Children"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(UIElementCollection), typeof(Panel), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Background"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(Panel), new PropertyMetadata(null, OnBackgroundChanged));

        /// <summary>
        /// Panel constructor
        /// </summary>
        public Panel()
        {
            this.Children = new UIElementCollection();
            this.Children.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (UIElement child in e.NewItems)
                    {
                        this.AddLogicalChild(child);
                        this.AddNativeChildView(child);
                    }
                }

                var oldItems = e.OldItems;
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    oldItems = (UIElementCollection)sender;
                }

                if (oldItems != null)
                {
                    foreach (UIElement child in oldItems)
                    {
                        this.RemoveLogicalChild(child);
                        this.RemoveNativeChildView(child);
                    }
                }
            };
        }

        /// <summary>
        /// Gets the collection of child elements of the panel. 
        /// </summary>
        public UIElementCollection Children
        {
            get { return (UIElementCollection)this.GetValue(ChildrenProperty); }
            private set { this.SetValue(ChildrenProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <seealso cref="Brush"/> that fills the background of the Panel.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)this.GetValue(BackgroundProperty); }
            set { this.SetValue(BackgroundProperty, value); }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                return this.Children.GetEnumerator();
            }
        }

        protected virtual void OnBackgroundChanged(Brush oldValue, Brush newValue)
        {
            this.ApplyNativeBackground(newValue);
        }

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Panel)d).OnBackgroundChanged(e.OldValue as Brush, e.NewValue as Brush);
        }

        partial void ApplyNativeBackground(Brush newValue);
    }
}