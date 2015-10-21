using System;
using System.Windows;
using Appercode.UI.Controls.Primitives;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Represents a control that lets the user select from a range of values by moving a Thumb control along a track.
    /// </summary>
    public partial class Slider : RangeBase
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="IsDirectionReversed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDirectionReversedProperty =
            DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(Slider), new PropertyMetadata(false, (d, e) =>
            {
                ((Slider)d).NativeIsDirectionReversed = (bool)e.NewValue;
            }));

        /// <summary>
        /// Identifies the <see cref="IsFocused"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register("IsFocused", typeof(bool), typeof(Slider),
            new PropertyMetadata(false, (d, e) =>
            {
                throw new InvalidOperationException("You can't assign IsFocused property.");
            }));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Slider), new PropertyMetadata(Orientation.Horizontal, (d, e) =>
            {
                ((Slider)d).NativeOrientation = (Orientation)e.NewValue;
            }));

        #endregion

        #region Constructors

        static Slider()
        {
            MinimumProperty.AddOwner(typeof(Slider), new PropertyMetadata(0.0));
            MaximumProperty.AddOwner(typeof(Slider), new PropertyMetadata(100.0));
            ValueProperty.AddOwner(typeof(Slider), new PropertyMetadata(0.0));
        }

        /// <summary>
        /// Initializes a new instance of the Slider class.
        /// </summary>
        public Slider()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value that indicates the direction of increasing value.
        /// </summary>
        public bool IsDirectionReversed
        {
            get { return (bool)this.GetValue(IsDirectionReversedProperty); }
            set { this.SetValue(IsDirectionReversedProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the slider control has focus.
        /// </summary>
        public new bool IsFocused
        {
            get { return (bool)this.GetValue(IsFocusedProperty); }
            internal set { this.SetValue(IsFocusedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation of a Slider.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        #endregion

        #region Methods

        internal override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);

            if (this.NativeMaximum != newMaximum)
            {
                this.NativeMaximum = newMaximum;
            }
        }

        internal override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);

            if (this.NativeMinimum != newMinimum)
            {
                this.NativeMinimum = newMinimum;
            }
        }

        internal override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (this.NativeValue != newValue)
            {
                this.NativeValue = newValue;
            }
        }

        #endregion
    }
}
