#region Using directives
using System.Windows;
#endregion //Using directives

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Defines a base class for grid columns/rows objects
    /// </summary>
    public abstract class DefinitionBase : DependencyObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets final offset
        /// </summary>
        internal double FinalOffset { get; set; }

        /// <summary>
        /// Gets or sets parent grid
        /// </summary>
        internal Grid ParentGrid { get; set; }

        #endregion // Properties

        #region Static methods

        /// <summary>
        /// Called when user size property (width for columns and height for rows) changed
        /// Invalidates parent grid
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        internal static void OnUserSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DefinitionBase db = (DefinitionBase)d;
            if (db.ParentGrid == null)
            {
                return;
            }
            db.ParentGrid.Invalidate();
        }

        /// <summary>
        /// Validates that specified grid lengrh value is greater than or equals to zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsUserSizePropertyValueValid(object value)
        {
            var length = (GridLength)value;
            return length.Value >= 0.0;
        }

        #endregion // Static methods
    }
}
