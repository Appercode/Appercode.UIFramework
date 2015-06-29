using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    [TypeConverterAttribute(typeof(FontFamilyConverter))]
    public class FontFamily
    {
        private string familyName;

        public FontFamily(string familyName)
        {
            this.familyName = familyName;
        }

        public string Source
        {
            get
            {
                return this.familyName;
            }
        }

        public override bool Equals(object o)
        {
            FontFamily fontFamily = o as FontFamily;
            if (fontFamily == null)
            {
                return false;
            }
            if (this.familyName == null)
            {
                return this.Equals(o);
            }
            string lower = this.familyName.ToLower(CultureInfo.CurrentCulture);
            string source = fontFamily.Source;
            if (fontFamily.Source != null)
            {
                source = fontFamily.Source.ToLower(CultureInfo.CurrentCulture);
            }
            return lower.Equals(source);
        }

        public override int GetHashCode()
        {
            if (this.familyName == null)
            {
                return this.GetHashCode();
            }
            return this.familyName.ToLower(CultureInfo.CurrentCulture).GetHashCode();
        }

        public override string ToString()
        {
            if (this.familyName == null)
            {
                return string.Empty;
            }
            return this.familyName;
        }
    }

    public class FontFamilyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                return new FontFamily(s);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}