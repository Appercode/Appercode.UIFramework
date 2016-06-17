#if __IOS__
using SizeF = CoreGraphics.CGSize;
#else
using System.Drawing;
#endif

namespace Appercode.UI.Device
{
    public enum DisplayType
    {
        Undefined,
        Phablet,
        Phone,
        Tablet
    }

    public enum InterfaceOrientation
    {
        Undefined,
        Portrait,
        PortraitUpsideDown,
        LandscapeLeft,
        LandScapeRight
    }

    public static partial class ScreenProperties
    {
        private static float density;
        private static DisplayType displayType;

        public static float Density
        {
            get
            {
                if (density == 0f)
                {
                    density = GetDensity();
                }

                return density;
            }
        }

        public static SizeF DisplaySize
        {
            get { return GetDisplaySize(); }
        }

        public static DisplayType DisplayType
        {
            get
            {
                if (displayType == DisplayType.Undefined)
                {
                    displayType = GetDisplayType();
                }

                return displayType;
            }
        }

        public static InterfaceOrientation InterfaceOrientation
        {
            get { return GetInterfaceOrientation(); }
        }

        public static float ConvertPixelsToDPI(float pixels)
        {
            return pixels / Density;
        }

        public static float ConvertDPIToPixels(float dpi)
        {
            return dpi * Density;
        }
    }
}
