using Appercode.UI.Controls.Primitives;
using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Provides control with popup to select item from list
    /// </summary>
    public partial class ListPicker : PickerBase
    {
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(ListPicker), new PropertyMetadata(null, (d, e) =>
            {
                ((ListPicker)d).ApplyNativeSelectedValue(e.NewValue);
            }));

        /// <summary>
        /// Initializes instans of ListPicker class
        /// </summary>
        public ListPicker()
        {
        }

        public object SelectedValue
        {
            get { return (object)this.GetValue(SelectedValueProperty); }
            set { this.SetValue(SelectedValueProperty, value); }
        }

        protected override void OnItemsSourceChanged(object oldValue, object newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            this.RefreshItems();
        }

        partial void RefreshItems();
    }
}