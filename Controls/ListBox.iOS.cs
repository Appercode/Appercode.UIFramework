using UIKit;

namespace Appercode.UI.Controls
{
    public partial class ListBox
    {
        public override UIView NativeUIElement
        {
            get
            {
                return this.scrollViewer == null ? base.NativeUIElement : this.scrollViewer.NativeUIElement;
            }
            protected internal set
            {
                if(this.scrollViewer == null)
                {
                    base.NativeUIElement = value;
                    return;
                }
                this.scrollViewer.NativeUIElement = value;
            }
        }
    }
}