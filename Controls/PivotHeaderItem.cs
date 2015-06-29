using System.Windows;

namespace Appercode.UI.Controls
{
    public class PivotHeaderItem : ListBoxItem
    {
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ListBoxItem.IsSelectedProperty)
            {
                int maxIteration = 4;
                for (var ancestor = this.Parent; maxIteration >= 0 && ancestor != null; --maxIteration, ancestor = ancestor.Parent)
                {
                    if (ancestor is PivotHeadersControl)
                    {
                        ((PivotHeadersControl)ancestor).NotifyItemSelectionChanged(this, (bool)e.OldValue, (bool)e.NewValue);
                    }
                }
            }
        }
    }
}