using Appercode.UI.Controls.Media;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class ProgressRing : Control
    {
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), new PropertyMetadata(false, OnIsActiveChanged));

        static ProgressRing()
        {
            ProgressRing.ForegroundProperty.AddOwner(typeof(ProgressRing), new PropertyMetadata(AppercodeColors.White));
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { this.SetValue(IsActiveProperty, value); }
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressRing;
            if (control != null && e.NewValue is bool)
            {
                control.IsActiveChanged((bool)e.NewValue);
            }
        }

        partial void IsActiveChanged(bool newValue);
    }
}