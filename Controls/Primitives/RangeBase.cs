using System;
using System.Windows;

namespace Appercode.UI.Controls.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    public delegate void RoutedPropertyChangedEventHandler<T>(object sender, RoutedPropertyChangedEventArgs<T> args);

    /// <summary>
    /// Represents an element that has a value within a specific range, such as the <see cref="ProgressBar"/>, <see cref="Slider"/> controls.
    /// </summary>
    public abstract partial class RangeBase : Control
    {
        /// <summary>
        /// Identifies the <see cref="LargeChange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register("LargeChange", typeof(double), typeof(RangeBase), new PropertyMetadata(0.0, (d, e) =>
                {
                    ((RangeBase)d).NativeLargeChange = (double)e.NewValue;
                }));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeBase), new PropertyMetadata(100.0, (d, e) =>
            {
                ((RangeBase)d).NativeMaximum = (double)e.NewValue;
            }));

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeBase), new PropertyMetadata(0.0, (d, e) =>
            {
                ((RangeBase)d).NativeMinimum = (double)e.NewValue;
            }));

        /// <summary>
        /// Identifies the <see cref="SmallChange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(RangeBase), new PropertyMetadata(0.0, (d, e) =>
            {
                ((RangeBase)d).NativeSmallChange = (double)e.NewValue;
            }));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(RangeBase), new PropertyMetadata(0.0, (d, e) =>
            {
                ((RangeBase)d).NativeValue = (double)e.NewValue;
            }));

        /// <summary>
        /// Initializes a new instance of the RangeBase class.
        /// </summary>
        public RangeBase()
        {
        }

        /// <summary>
        /// Occurs when the range value changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="Value"/> of a RangeBase control.
        /// </summary>
        public double LargeChange
        {
            get { return (double)this.GetValue(LargeChangeProperty); }
            set { this.SetValue(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the highest possible <see cref="Value"/> of the range element.
        /// </summary>
        public double Maximum
        {
            get 
            { 
                return (double)this.GetValue(MaximumProperty); 
            }
            set
            {
                var oldMaximum = (double)this.GetValue(MaximumProperty);
                this.SetValue(MaximumProperty, value);
                this.OnMaximumChanged(oldMaximum, value);
            }
        }

        /// <summary>
        /// Gets or sets the Minimum possible <see cref="Value"/> of the range element.
        /// </summary>
        public double Minimum
        {
            get 
            { 
                return (double)this.GetValue(MinimumProperty); 
            }
            set
            {
                var oldMinimum = (double)this.GetValue(MinimumProperty);
                this.SetValue(MinimumProperty, value);
                this.OnMinimumChanged(oldMinimum, value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Value"/> to be added to or subtracted from the <see cref="Value"/> of a RangeBase control.
        /// </summary>
        public double SmallChange
        {
            get { return (double)this.GetValue(SmallChangeProperty); }
            set { this.SetValue(SmallChangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current setting of the range control, which may be coerced.
        /// </summary>
        public double Value
        {
            get 
            { 
                return (double)this.GetValue(ValueProperty); 
            }
            set
            {
                var oldValue = (double)this.GetValue(ValueProperty);
                this.SetValue(ValueProperty, value);
                this.OnValueChanged(oldValue, value);
            }
        }

        internal virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            if (newMaximum.Equals(oldMaximum))
            {
                return;
            }
            if (newMaximum < this.Minimum)
            {
                this.Minimum = newMaximum;
            }

            if (this.Value > newMaximum)
            {
                this.Value = newMaximum;
            }
        }

        internal virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            if (newMinimum.Equals(oldMinimum))
            {
                return;
            }
            if (newMinimum > this.Maximum)
            {
                this.Maximum = newMinimum;
            }

            if (this.Value < newMinimum)
            {
                this.Value = newMinimum;
            }
        }

        internal virtual void OnValueChanged(double oldValue, double newValue)
        {
            if (newValue.Equals(oldValue))
            {
                return;
            }
            if (newValue > this.Maximum)
            {
                this.Value = this.Maximum;
            }

            if (newValue < this.Minimum)
            {
                this.Value = this.Minimum;
            }

            RoutedPropertyChangedEventHandler<double> routedPropertyChangedEventHandler = this.ValueChanged;
            if (routedPropertyChangedEventHandler != null)
            {
                routedPropertyChangedEventHandler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
            }
        }
    }

    /// <summary>
    /// Provides data about a change in value to a dependency property as reported by particular routed events, including the previous and current value of the property that changed.
    /// </summary>
    /// <typeparam name="TVal">The type of the dependency property that has changed.</typeparam>
    public class RoutedPropertyChangedEventArgs<TVal>
    {
        /// <summary>
        /// Initializes a new instance of the RoutedPropertyChangedEventArgs class, with provided old and new values.
        /// </summary>
        /// <param name="oldValue">Old vlaue</param>
        /// <param name="newValue">New value</param>
        public RoutedPropertyChangedEventArgs(TVal oldValue, TVal newValue)
        {
        }
    }
}