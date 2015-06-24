namespace Appercode.UI.Controls
{
    public partial class Pivot
    {
        protected override void AddPanelToNativeContainer()
        {
            base.AddPanelToNativeContainer();

            if (this.NativeUIElement != null)
            {
                this.NativeUIElement.AddSubview(((UIElement)this.PivotHeader).NativeUIElement);
            }
        }

        private void SetNativeViewPagerToHeaderControl()
        {
            if (this.PivotHeader != null && this.panel != null)
            {
                this.PivotHeader.SetVirtualizingPanel((PivotVirtualizingPanel)this.panel);
            }
        }

        private void RemoveNativeViewPagerFromHeaderControl()
        {
            if (this.PivotHeader != null && this.panel != null)
            {
                this.PivotHeader.FreeVirtualizingPanel();
            }
        }
    }
}