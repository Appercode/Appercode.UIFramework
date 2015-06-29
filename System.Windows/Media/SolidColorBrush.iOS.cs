using CoreGraphics;
using UIKit;

namespace System.Windows.Media
{
    public partial class SolidColorBrush
    {
        public override UIColor ToUIColor(CGSize size)
        {
            /*byte[] data = new byte[(int)(rect.Width * 4 * rect.Height)];
            CGBitmapContext context = new CGBitmapContext(data, (int)rect.Width, (int)rect.Height, 8, (int)rect.Width * 4, CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.NoneSkipFirst);
            context.ScaleCTM(1f, -1f);
            context.TranslateCTM(0, -rect.Height);
            context.SetFillColor(1.0f, 0.0f, 0.5f, 1.0f);
            context.FillRect(rect);
            
            var cgImage = context.ToImage();
            var image = new UIImage(cgImage);
            
            var color = UIColor.FromPatternImage(image);

            context.Dispose();
            cgImage.Dispose();
             * 
             */

            UIColor color = new UIColor(this.Color.R / 255f, this.Color.G / 255f, this.Color.B / 255f, this.Color.A / 255f);

            return color;
        }
    }
}
