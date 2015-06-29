using CoreGraphics;
using System;
using UIKit;

namespace Appercode.UI.Helpers
{
    public static class UIImageExtension
    {
        public static UIImage CropByX(this UIImage image, nfloat x)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(image.Size.Width - x, image.Size.Height), false, 0);

            UIImage result = null;

            using (CGContext context = UIGraphics.GetCurrentContext())
            {
                context.TranslateCTM(0, image.Size.Height);
                context.ScaleCTM(1, -1);

                context.DrawImage(new CGRect(CGPoint.Empty, image.Size), image.CGImage);

                using (CGImage img = context.AsBitmapContext().ToImage())
                {
                    result = new UIImage(img, image.CurrentScale, UIImageOrientation.Up);
                    img.Dispose();
                }

                context.Dispose();
                UIGraphics.EndImageContext();
            }

            return result;
        }

        public static UIImage AttachImageRight(this UIImage image, UIImage image2)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(image.Size.Width + image2.Size.Width, image.Size.Height), false, 0);
            CGContext context = UIGraphics.GetCurrentContext();

            context.TranslateCTM(0, image.Size.Height);
            context.ScaleCTM(1, -1);

            context.DrawImage(new CGRect(CGPoint.Empty, image.Size), image.CGImage);
            context.DrawImage(new CGRect(image.Size.Width, 0, image2.Size.Width, image.Size.Height), image2.CGImage);

            UIImage result = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return result;
        }
    }
}