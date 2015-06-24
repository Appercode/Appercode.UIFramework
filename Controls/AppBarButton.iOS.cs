using System;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class AppBarButton
    {
        internal UIBarButtonItem GetNativeItem()
        {
            UIBarButtonItem nativeItem;
            var icon = this.Icon;
            if (icon != null)
            {
                nativeItem = icon.GetNativeItem();
            }
            else
            {
                var label = this.Label;
                nativeItem = string.IsNullOrEmpty(label) ?
                    new UIBarButtonItem(UIBarButtonSystemItem.Done)
                    : new UIBarButtonItem { Title = label };
            }

            nativeItem.Clicked += this.NativeItemClicked;
            return nativeItem;
        }

        protected internal override void NativeInit()
        {
            // Do nothing and do not call base method for AppBarButton
        }

        private void NativeItemClicked(object sender, EventArgs e)
        {
            this.OnClick();
        }
    }
}