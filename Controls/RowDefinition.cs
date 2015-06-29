using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Defines row-specific properties that apply to Grid
    /// </summary>
    public class RowDefinition : DefinitionBase
    {
        #region Dependency property definitions

        /// <summary>
        /// Identifies the RowDefinition.Height property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                "Height",
                typeof(GridLength),
                typeof(RowDefinition),
                new PropertyMetadata(
                    new GridLength(1.0, GridUnitType.Star),
                    DefinitionBase.OnUserSizePropertyChanged) /*,
                    DefinitionBase.IsUserSizePropertyValueValid*/);

        /// <summary>
        /// Identifies the RowDefinition.MaxHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                "MaxHeight",
                typeof(double),
                typeof(RowDefinition),
                new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Identifies the RowDefinition.MixHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                "MinWidth",
                typeof(double),
                typeof(RowDefinition),
                new PropertyMetadata(0d));

        #endregion //Dependency property definitions

        #region Properties

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

        #endregion //Properties
    }
}
