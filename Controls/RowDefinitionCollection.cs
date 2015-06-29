using Appercode.UI.Controls.Base;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Provides access to an ordered, strongly typed collection of RowDefinition objects.
    /// </summary>
    public class RowDefinitionCollection : GridSizeCollectionBase<RowDefinition>
    {
        /// <summary>
        /// Initializes the row definition collection for specified grid
        /// </summary>
        /// <param name="owner"></param>
        internal RowDefinitionCollection(Grid owner) : base(owner)
        {
        }
    }
}
