using Android.App;
using Android.Content.Res;
using Android.Views;
using Appercode.UI.Controls;
using System.Drawing;

namespace Appercode.UI.Device
{
    public static partial class ScreenProperties
    {
        private static float GetDensity()
        {
            return Application.Context.Resources.DisplayMetrics.Density;
        }

        private static SizeF GetDisplaySize()
        {
            var display = UIElement.StaticContext.WindowManager.DefaultDisplay;
            var size = new Android.Graphics.Point();
            display.GetSize(size);
            return new SizeF(size.X, size.Y);
        }

        private static DisplayType GetDisplayType()
        {
            switch (Application.Context.Resources.Configuration.ScreenLayout & ScreenLayout.SizeMask)
            {
                case ScreenLayout.SizeLarge:
                    return DisplayType.Phablet;
                case ScreenLayout.SizeNormal:
                    return DisplayType.Phone;
                case ScreenLayout.SizeSmall:
                    return DisplayType.Phone;
                case ScreenLayout.SizeXlarge:
                    return DisplayType.Tablet;
                default:
                    return DisplayType.Phone;
            }
        }

        private static InterfaceOrientation GetInterfaceOrientation()
        {
            switch (UIElement.StaticContext.WindowManager.DefaultDisplay.Rotation)
            {
                case SurfaceOrientation.Rotation0:
                    return InterfaceOrientation.Portrait;
                case SurfaceOrientation.Rotation90:
                    return InterfaceOrientation.LandscapeLeft;
                case SurfaceOrientation.Rotation180:
                    return InterfaceOrientation.PortraitUpsideDown;
                case SurfaceOrientation.Rotation270:
                    return InterfaceOrientation.LandScapeRight;
                default:
                    return InterfaceOrientation.Undefined;
            }
        }
    }
}