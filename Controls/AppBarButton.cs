using Appercode.UI.Controls.Primitives;
using System;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class AppBarButton : Button, ICommandBarElement
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(IconElement), typeof(AppBarButton), new PropertyMetadata(IconChanged));

        public static readonly DependencyProperty IsCompactProperty =
            DependencyProperty.Register("IsCompact", typeof(bool), typeof(AppBarButton));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(AppBarButton));

        static AppBarButton()
        {
            Button.VisibilityProperty.OverrideMetadata(typeof(AppBarButton), new PropertyMetadata(OnVisibilityChanged));
        }

        public event EventHandler VisibilityChanged;

        public IconElement Icon
        {
            get { return (IconElement)GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        public bool IsCompact
        {
            get { return (bool)GetValue(IsCompactProperty); }
            set { this.SetValue(IsCompactProperty, value); }
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { this.SetValue(LabelProperty, value); }
        }

        private static void IconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as AppBarButton;
            if (button != null)
            {
                button.IconChanged(e.NewValue as IconElement);
            }
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as AppBarButton;
            if (button != null)
            {
                if (button.VisibilityChanged != null)
                {
                    button.VisibilityChanged(button, EventArgs.Empty);
                }
            }
        }

        partial void IconChanged(IconElement newValue);
    }
}