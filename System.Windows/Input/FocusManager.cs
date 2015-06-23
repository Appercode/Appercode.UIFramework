using Appercode.UI;
using Appercode.UI.Controls;

namespace System.Windows.Input
{
    public static partial class FocusManager
    {
        public static object GetFocusedElement()
        {
            return GetFocusedChild(AppercodeVisualRoot.Instance.Child);
        }

        private static object GetFocusedChild(UIElement element)
        {
            if (ChechFocus(element))
            {
                return element;
            }
            var children = LogicalTreeHelper.GetChildren(element);
            foreach (var child in children)
            {
                if (child is UIElement)
                {
                    var focused = GetFocusedChild((UIElement)child);
                    if (focused != null)
                    {
                        return focused;
                    }
                }
            }
            return null;
        }
    }
}
