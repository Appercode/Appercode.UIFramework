using UIKit;

namespace Appercode.UI.Controls.Primitives
{
    public abstract partial class IconElement : UIElement
    {
        protected internal abstract UIBarButtonItem GetNativeItem();
    }
}