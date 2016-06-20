namespace System.Windows.Media
{
    public sealed partial class SolidColorBrush : Brush
    {
        public static readonly DependencyProperty ColorProperty =
                DependencyProperty.Register(nameof(Color), typeof(Color), typeof(SolidColorBrush));

        public SolidColorBrush(Color color)
        {
            this.Color = color;
        }

        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }
    }
}