using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Defines column-specific properties that apply to Grid elements.
    /// </summary>
    public class ColumnDefinition : DefinitionBase
    {
        #region Dependency property definitions

        /// <summary>
        /// Identifies the ColumnDefinition.Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                "Width",
                typeof(GridLength),
                typeof(ColumnDefinition),
                new PropertyMetadata(
                    new GridLength(1.0, GridUnitType.Star),
                    DefinitionBase.OnUserSizePropertyChanged) /*,
                    DefinitionBase.IsUserSizePropertyValueValid*/);

        /// <summary>
        /// Identifies the ColumnDefinition.MaxWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                "MaxWidth",
                typeof(double),
                typeof(ColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Identifies the ColumnDefinition.MixWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                "MinWidth",
                typeof(double),
                typeof(ColumnDefinition),
                new PropertyMetadata(0d));

        #endregion //Dependency property definitions

        #region Properties

        /// <summary>
        /// Gets a value that represents the offset value of this ColumnDefinition
        /// </summary>
        public double Offset
        {
            get { return this.FinalOffset; }
        }

        /// <summary>
        /// Gets a value that represents the actual calculated width of the ColumnDefinition
        /// </summary>
        public double ActualWidth { get; internal set; }

        /// <summary>
        /// Gets or sets column width
        /// </summary>
        public GridLength Width
        {
            get
            {
                return (GridLength)this.GetValue(WidthProperty);
            }
            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets column maximum width
        /// </summary>
        public double MaxWidth
        {
            get
            {
                return (double)this.GetValue(MaxWidthProperty);
            }
            set
            {
                this.SetValue(MaxWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets column minimum width
        /// </summary>
        public double MinWidth
        {
            get
            {
                return (double)this.GetValue(MinWidthProperty);
            }
            set
            {
                this.SetValue(MinWidthProperty, value);
            }
        }

        #endregion // Properties
    }
}
