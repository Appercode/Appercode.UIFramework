using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Appercode.UI.Controls.Primitives
{
    public interface IItemContainerGenerator
    {
        IItemContainerGenerator GetItemContainerGeneratorForPanel(Panel panel);

        IDisposable StartAt(GeneratorPosition position, GeneratorDirection direction, bool allowStartAtRealizedItem);

        DependencyObject GenerateNext(out bool isNewlyRealized);

        void PrepareItemContainer(DependencyObject container);

        void RemoveAll();

        void Remove(GeneratorPosition position, int count);

        GeneratorPosition GeneratorPositionFromIndex(int itemIndex);

        int IndexFromGeneratorPosition(GeneratorPosition position);
    }
}
