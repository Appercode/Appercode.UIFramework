using Appercode.UI.Controls.Primitives;
using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Represents a control that indicates the progress of an operation.
    /// </summary>
    public partial class ProgressBar : RangeBase
    {
        #region Declarations
        /// <summary>
        /// Identifies the <see cref="IsIndeterminate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProgressBar),
                new PropertyMetadata(false, (d, e) => { ((ProgressBar)d).NativeIsIndeterminate = (bool)e.NewValue; }));
        #endregion

        #region Constructors
        static ProgressBar()
        {
            MinimumProperty.AddOwner(typeof(ProgressBar), new PropertyMetadata(0.0));
            MaximumProperty.AddOwner(typeof(ProgressBar), new PropertyMetadata(100.0));
        }

        /// <summary>
        /// Initializes a new instance of the ProgressBar class.
        /// </summary>
        public ProgressBar()
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value that indicates whether the progress bar reports generic progress with a repeating pattern or reports progress based on the <see cref="RangeBase.Value"/> property.
        /// </summary>
        public bool IsIndeterminate
        {
            get { return (bool)this.GetValue(IsIndeterminateProperty); }
            set { this.SetValue(IsIndeterminateProperty, value); }
        }
        #endregion

        #region Implementation
        internal override void OnValueChanged(double oldValue, double newValue)
        {
        }
        #endregion
    }
}