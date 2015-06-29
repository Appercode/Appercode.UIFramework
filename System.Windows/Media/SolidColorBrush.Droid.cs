using Android.Graphics.Drawables;

namespace System.Windows.Media
{
    public partial class SolidColorBrush
    {
        public override Drawable ToDrawable()
        {
            return new ColorDrawable(Color.ToNativeColor());
        }
    }
}