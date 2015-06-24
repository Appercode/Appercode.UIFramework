using UIKit;

namespace System.Windows.Media
{
    public partial struct Color
    {
        private const float Divider = (float)byte.MaxValue;

        public static implicit operator UIColor(Color color)
        {
            return color.ToUIColor();
        }

        public UIColor ToUIColor()
        {
            return new UIColor(
                this.innerRgbColor.R / Divider,
                this.innerRgbColor.G / Divider,
                this.innerRgbColor.B / Divider,
                this.innerRgbColor.A / Divider);
        }
    }
}