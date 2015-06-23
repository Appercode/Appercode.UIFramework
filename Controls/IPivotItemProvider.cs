using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appercode.UI.Controls;

namespace Appercode.UI.Controls
{
    public interface IPivotItemProvider
    {
        int Count { get; }
        UIElement CreateItemElement(int position);
        object GetHeader(int position);
    }
}