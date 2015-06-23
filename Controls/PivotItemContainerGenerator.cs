using Appercode.UI.Controls.Primitives;
using System.Collections;
using System.Windows;

namespace Appercode.UI.Controls
{
    public class PivotItemContainerGenerator : ItemContainerGenerator
    {
        public PivotItemContainerGenerator(FrameworkElementFactory containerFactory, IList collection = null)
            : base(containerFactory, collection)
        {
        }

        protected override DependencyObject FillElementWithData(int index, UIElement element)
        {
            var itemData = this.Collection[index];
            if (itemData is PivotItem)
            {
                return (PivotItem) itemData;
            }

            return base.FillElementWithData(index, element);
        }
    }
}