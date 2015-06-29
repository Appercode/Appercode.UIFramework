using Appercode.UI.Controls.NativeControl.Wrapers;

namespace Appercode.UI.Controls
{
    public partial class Pivot
    {
        protected override void AddPanelToNativeContainer()
        {
            base.AddPanelToNativeContainer();

            if (this.PivotHeader != null)
            {
                ((WrapedViewGroup)this.NativeUIElement).AddView(((UIElement)this.PivotHeader).NativeUIElement);
            }
        }

        private void SetNativeViewPagerToHeaderControl()
        {
            if (PivotHeader != null && this.panel != null && this.panel.NativeUIElement != null)
            {
                PivotHeader.SetNativeViewPager((WrappedViewPager)this.panel.NativeUIElement);
            }
        }

        private void RemoveNativeViewPagerFromHeaderControl()
        {
            if (PivotHeader != null)
            {
                PivotHeader.FreeNativeViewPager();
            }
        }
    }
}
