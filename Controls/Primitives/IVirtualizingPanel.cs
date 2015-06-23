using System.Collections.Specialized;
using System.Windows;

namespace Appercode.UI.Controls.Primitives
{
    public interface IVirtualizingPanel
    {
        /// <summary>
        /// Generator for containers of items(for ListBoxItem for ex.)
        /// </summary>
        ItemContainerGenerator Generator { get; set; }

        /// <summary>
        /// Gets or set ScrollViewer virtualisation will be based on
        /// </summary>
        ScrollViewer ScrollOwner { get; set; }

        bool HasNativeScroll { get; }

        void ItemsUpdated(NotifyCollectionChangedEventArgs e);

        /// <summary>
        /// Set IsSelected of realized item
        /// </summary>
        /// <param name="index">index of element</param>
        /// <param name="value">value to set</param>
        /// <returns>true if IsSelected was set, otherwise false</returns>
        bool SetIsSelectedOnRealizedItem(int index, bool value);

        void SetPadding(Thickness padding);
    }
}