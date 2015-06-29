using Appercode.UI.Controls;

namespace System.Windows.Input
{
    public static partial class FocusManager
    {
        private static bool ChechFocus(UIElement element)
        {
            return element.NativeUIElement.IsFirstResponder;
        }
    }
}
