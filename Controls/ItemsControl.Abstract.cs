using System.Windows.Markup;

namespace Appercode.UI.Controls
{
    // ReSharper disable CSharpWarnings::CS1591
    /// <summary>
    /// Represents a control that can be used to present a collection of items.
    /// </summary>
    [ContentProperty("Items")]
    public partial class ItemsControl 
    {

        /// <summary>
        /// Adds panel to native visual tree
        /// </summary>
        protected virtual void AddPanelToNativeContainer()
        {
        }

        /// <summary>
        /// Removes panel from native visual tree
        /// </summary>
        protected virtual void RemovePanelFromNativeContainer()
        {
        }
    }
}