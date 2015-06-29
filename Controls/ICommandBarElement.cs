using System;

namespace Appercode.UI.Controls
{
    public interface ICommandBarElement
    {
        event EventHandler VisibilityChanged;

        bool IsCompact { get; set; }
    }
}