using System.Windows;

#if __IOS__
using System;
#else
using nfloat = System.Single;
#endif

namespace Appercode.UI
{
    internal static class ThicknessExtensions
    {
        public static double HorizontalThickness(this Thickness thickness)
        {
            return thickness.Left + thickness.Right;
        }

        public static double VerticalThickness(this Thickness thickness)
        {
            return thickness.Top + thickness.Bottom;
        }

        public static nfloat HorizontalThicknessF(this Thickness thickness)
        {
            return (nfloat)HorizontalThickness(thickness);
        }

        public static nfloat VerticalThicknessF(this Thickness thickness)
        {
            return (nfloat)VerticalThickness(thickness);
        }

        public static nfloat TopF(this Thickness thickness)
        {
            return (nfloat)thickness.Top;
        }

        public static nfloat BottomF(this Thickness thickness)
        {
            return (nfloat)thickness.Bottom;
        }

        public static nfloat LeftF(this Thickness thickness)
        {
            return (nfloat)thickness.Left;
        }

        public static nfloat RightF(this Thickness thickness)
        {
            return (nfloat)thickness.Right;
        }
    }
}
