using UIKit;
using CoreGraphics;

namespace Appercode.UI.Device
{
    public static partial class ScreenProperties
    {
        private static float GetDensity()
        {
            return (float)UIScreen.MainScreen.Scale;
        }

        private static DisplayType GetDisplayType()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                return Device.DisplayType.Tablet;
            }
            return Device.DisplayType.Phone;
        }

        private static CGSize GetDisplaySize()
        {
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown)
            {
                return UIScreen.MainScreen.Bounds.Size;
            }
            return new CGSize(UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width);
        }

        private static InterfaceOrientation GetInterfaceOrientation()
        {
            var o = UIApplication.SharedApplication.StatusBarOrientation;
            switch (o)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    return Device.InterfaceOrientation.LandscapeLeft;
                case UIInterfaceOrientation.LandscapeRight:
                    return Device.InterfaceOrientation.LandScapeRight;
                case UIInterfaceOrientation.Portrait:
                    return Device.InterfaceOrientation.Portrait;
                case UIInterfaceOrientation.PortraitUpsideDown:
                    return Device.InterfaceOrientation.PortraitUpsideDown;
            }
            return Device.InterfaceOrientation.Undefined;
        }
    }
}