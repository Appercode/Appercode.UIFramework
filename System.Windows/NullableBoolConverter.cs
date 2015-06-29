using System.ComponentModel;
using System.Globalization;
using Appercode.UI.Controls;

namespace System.Windows
{
    public sealed class NullableBoolConverter : TypeConverter
    {
        public NullableBoolConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (TypeConverters.CanConvertFrom<bool?>(sourceType))
            {
                return true;
            }
            return sourceType == typeof(bool);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            bool? nullable;
            if (value is bool)
            {
                return (bool?)value;
            }
            string str = value as string;
            if (str == null && value != null)
            {
                return TypeConverters.ConvertFrom<bool?>(this, value, null);
            }
            if (!string.IsNullOrEmpty(str))
            {
                nullable = new bool?(bool.Parse(str));
            }
            else
            {
                bool? nullable1 = null;
                nullable = nullable1;
            }
            return nullable;
        }
    }
}