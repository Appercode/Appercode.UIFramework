using AColor = Android.Graphics.Color;

namespace System.Windows.Media
{
    public partial struct Color
    {
        public AColor ToNativeColor()
        {
            return AColor.Argb(
                innerRgbColor.A,
                innerRgbColor.R,
                innerRgbColor.G,
                innerRgbColor.B);
        }
    }
}