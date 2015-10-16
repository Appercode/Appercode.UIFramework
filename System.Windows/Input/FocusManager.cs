using Appercode.UI;
using Appercode.UI.Controls;

namespace System.Windows.Input
{
    /// <summary>
    /// A helper class that allows getting the UI element that has focus.
    /// </summary>
    public static partial class FocusManager
    {
        /// <summary>
        /// Gets the element in the UI that has focus.
        /// </summary>
        /// <returns>The object that has focus or null if there is no focused element.</returns>
        public static object GetFocusedElement()
        {
            return GetFocusedElement(AppercodeVisualRoot.Instance.Child);
        }

        internal static object GetFocusedElement(UIElement element)
        {
            if (element.IsFocused)
            {
                return element;
            }

            var children = LogicalTreeHelper.GetChildren(element);
            foreach (var child in children)
            {
                var childUIElement = child as UIElement;
                if (childUIElement != null)
                {
                    var focused = GetFocusedElement(childUIElement);
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
