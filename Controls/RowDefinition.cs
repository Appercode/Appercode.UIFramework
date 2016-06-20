using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Defines row-specific properties that apply to Grid
    /// </summary>
    public class RowDefinition : DefinitionBase
    {
        /// <summary>
        /// Identifies the <see cref="Height" /> property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
                nameof(Height),
                typeof(GridLength),
                typeof(RowDefinition),
                new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), OnUserSizePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MaxHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(nameof(MaxHeight), typeof(double), typeof(RowDefinition), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Identifies the <see cref="MinHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(nameof(MinHeight), typeof(double), typeof(RowDefinition), new PropertyMetadata(0d));

        /// <summary>
        /// Gets a value that represents the offset value of this RowDefinition
        /// </summary>
        public double Offset
        {
            get
            {
                return this.FinalOffset;
            }
        }

        /// <summary>
        /// Gets a value that represents the actual calculated height of the RowDefinition
        /// </summary>
        public double ActualHeight { get; internal set; }

        /// <summary>
        /// Gets or sets row height
        /// </summary>
        public GridLength Height
        {
            get
            {
                return (GridLength)this.GetValue(HeightProperty);
            }
            set
            {
                this.SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets row maximum height
        /// </summary>
        public double MaxHeight
        {
            get
            {
                return (double)this.GetValue(MaxHeightProperty);
            }
            set
            {
                this.SetValue(MaxHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets row minimum height
        /// </summary>
        public double MinHeight
        {
            get
            {
                return (double)this.GetValue(MinHeightProperty);
            }
            set
            {
                this.SetValue(MinHeightProperty, value);
            }
        }
    }
}
