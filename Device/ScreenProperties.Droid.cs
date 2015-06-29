using Android.App;
using Android.Views;
using Appercode.UI.Controls;
using System.Drawing;

namespace Appercode.UI.Device
{
    public static partial class ScreenProperties
    {
        private static float GetDensity()
        {
            return Application.Context.Resources.DisplayMetrics.Density; // StackNavigationFrame.Instance.Resources.DisplayMetrics.Density;
        }

        private static SizeF GetDisplaySize()
        {
            Display display = UIElement.StaticContext.WindowManager.DefaultDisplay; // StackNavigationFrame.Instance.WindowManager.DefaultDisplay;
            return new SizeF(display.Width, display.Height);
        }

        private static DisplayType GetDisplayType()
        {
            switch (Application.Context.Resources.Configuration.ScreenLayout & Android.Content.Res.ScreenLayout.SizeMask)
            {
                case Android.Content.Res.ScreenLayout.SizeLarge:
                    {
                        return DisplayType.Phablet;
                    }
                case Android.Content.Res.ScreenLayout.SizeNormal:
                    {
                        return DisplayType.Phone;
                    }
                case Android.Content.Res.ScreenLayout.SizeSmall:
                    {
                        return DisplayType.Phone;
                    }
                case Android.Content.Res.ScreenLayout.SizeXlarge:
                    {
                        return DisplayType.Tablet;
                    }
                default:
                    {
                        return DisplayType.Phone;
                    }
            }
        }

        private static InterfaceOrientation GetInterfaceOrientation()
        {
            var rotation = UIElement.StaticContext.WindowManager.DefaultDisplay.Rotation;

            switch (rotation)
            {
                case SurfaceOrientation.Rotation0:
                    {
                        return Device.InterfaceOrientation.Portrait;
                    }
                case SurfaceOrientation.Rotation90:
                    {
                        return Device.InterfaceOrientation.LandscapeLeft;
                    }
                case SurfaceOrientation.Rotation180:
                    {
                        return Device.InterfaceOrientation.PortraitUpsideDown;
                    }
                case SurfaceOrientation.Rotation270:
                    {
                        return Device.InterfaceOrientation.LandScapeRight;
                    }
                default:
                    {
                        return Device.InterfaceOrientation.Undefined;
                    }
            }
        }
    }
}