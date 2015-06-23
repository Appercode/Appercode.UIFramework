using Appercode.UI.Controls.Primitives;
using System;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class BitmapIcon : IconElement
    {
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(BitmapIcon), new PropertyMetadata(UriSourceChanged));

        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { this.SetValue(UriSourceProperty, value); }
        }

        private static void UriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var icon = d as BitmapIcon;
            if (icon != null)
            {
                icon.UriSourceChanged(e.NewValue as Uri);
            }
        }

        partial void UriSourceChanged(Uri newValue);
    }
}