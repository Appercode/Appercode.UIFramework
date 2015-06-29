using Appercode.UI.Controls.Base;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Provides access to an ordered, strongly typed collection of ColumnDefinition objects.
    /// </summary>
    public class ColumnDefinitionCollection : GridSizeCollectionBase<ColumnDefinition>
    {
        /// <summary>
        /// Initializes the column definition collection for specified grid
        /// </summary>
        internal ColumnDefinitionCollection(Grid owner) : base(owner)
        {
        }
    }
}
