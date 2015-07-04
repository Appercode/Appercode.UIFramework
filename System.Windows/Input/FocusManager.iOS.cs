using Appercode.UI.Controls;

namespace System.Windows.Input
{
    public static partial class FocusManager
    {
        private static bool ChechFocus(UIElement element)
        {
            // An element might have NativeUIElement set to null,
            // because a control could not be presented by a descendant of UIView.
            var nativeElement = element.NativeUIElement;
            return nativeElement != null && nativeElement.IsFirstResponder;
        }
    }
}
