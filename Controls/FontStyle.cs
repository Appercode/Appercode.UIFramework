using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    [TypeConverterAttribute(typeof(FontStyleConverter))]
    public struct FontStyle
    {
        private FontStyleType style;

        internal FontStyle(FontStyleType style)
        {
            this.style = style;
        }

        internal FontStyleType Style
        {
            get
            {
                return this.style;
            }
        }

        public static bool operator ==(FontStyle left, FontStyle right)
        {
            return left.style == right.style;
        }

        public static bool operator !=(FontStyle left, FontStyle right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FontStyle))
            {
                return false;
            }
            return this == (FontStyle)obj;
        }

        public override int GetHashCode()
        {
            return (int)this.Style;
        }

        public override string ToString()
        {
            string str = "Normal";
            switch (this.style)
            {
                case FontStyleType.Normal:
                    {
                        str = "Normal";
                        return str;
                    }
                case FontStyleType.Italic:
                    {
                        str = "Italic";
                        return str;
                    }
                default:
                    {
                        return str;
                    }
            }
        }
    }

    public class FontStyleConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                if (Enum.IsDefined(typeof(FontStyleType), sourceType))
                {
                    return true;
                }
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                return new FontStyle((FontStyleType)Enum.Parse(typeof(FontStyleType), s));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}