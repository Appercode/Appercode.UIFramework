using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Class for pick range value from one <see cref="ItemsSource"/>
    /// </summary>
    public partial class RangePicker : ListPicker
    {
        public static readonly DependencyProperty LeftValueProperty =
            DependencyProperty.Register("LeftValue", typeof(object), typeof(RangePicker), new PropertyMetadata(null, (d, e)=>
            {
                ((RangePicker)d).ApplyNativeLeftValue(e.NewValue);
            }));

        public static readonly DependencyProperty RightValueProperty =
            DependencyProperty.Register("RightValue", typeof(object), typeof(RangePicker), new PropertyMetadata(null, (d, e)=>
            {
                ((RangePicker)d).ApplyNativeRightValue(e.NewValue);
            }));

        /// <summary>
        /// Picker left value
        /// </summary>
        public object LeftValue
        {
            get { return (object)this.GetValue(LeftValueProperty); }
            set { this.SetValue(LeftValueProperty, value); }
        }

        /// <summary>
        /// Picker right value
        /// </summary>
        public object RightValue
        {
            get { return (object)this.GetValue(RightValueProperty); }
            set { this.SetValue(RightValueProperty, value); }
        }
    }
}