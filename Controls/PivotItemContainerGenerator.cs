using Appercode.UI.Controls.Primitives;
using System.Windows;

namespace Appercode.UI.Controls
{
    public class PivotItemContainerGenerator : ItemContainerGenerator
    {
        internal PivotItemContainerGenerator(FrameworkElementFactory containerFactory, ItemCollection collection)
            : base(containerFactory, collection) { }

        internal override DependencyObject LinkContainerToItem(int index, DependencyObject container)
        {
            var itemData = this.collection[index] as PivotItem;
            return itemData ?? base.LinkContainerToItem(index, container);
        }
    }
}