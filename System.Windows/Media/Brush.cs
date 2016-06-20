namespace System.Windows.Media
{
    public abstract partial class Brush : DependencyObject
    {
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(Brush), new PropertyMetadata(1d));

        //// TODO TransformProperty, RelativeTransformProperty

        public double Opacity
        {
            get { return (double)this.GetValue(OpacityProperty); }
            set { this.SetValue(OpacityProperty, value); }
        }
    }
}