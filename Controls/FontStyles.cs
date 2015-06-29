using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    public static class FontStyles
    {
        public static FontStyle Italic
        {
            get
            {
                return FontStyles.EnsureFontStyle(FontStyleType.Italic);
            }
        }

        public static FontStyle Normal
        {
            get
            {
                return FontStyles.EnsureFontStyle(FontStyleType.Normal);
            }
        }

        internal static FontStyle EnsureFontStyle(FontStyleType style)
        {
            switch (style)
            {
                case FontStyleType.Normal:
                case FontStyleType.Italic:
                    {
                        return new FontStyle(style);
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("style");
                    }
            }
        }
    }
}