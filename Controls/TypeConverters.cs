using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    internal static class TypeConverters
    {
        internal static bool CanConvertFrom<T>(Type sourceType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException("sourceType");
            }
            if (sourceType == typeof(string))
            {
                return true;
            }
            return typeof(T).IsAssignableFrom(sourceType);
        }

        internal static object ConvertFrom<T>(TypeConverter converter, object value, IDictionary<string, T> knownValues)
        {
            T t;
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            string str = value as string;
            if (str != null && knownValues != null)
            {
                if (!knownValues.TryGetValue(str, out t))
                {
                    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                    throw new FormatException(string.Format(invariantCulture, "Cannot Convert from string {0}", str));
                }
                return t;
            }
            if (!(value is T))
            {
                if (value != null)
                {
                    CultureInfo currentCulture = CultureInfo.CurrentCulture;
                    object[] objArray = new object[] { converter.GetType().Name, null };
                    throw new NotSupportedException(string.Format(currentCulture, "Cannot Convert from {0}", objArray));
                }
                throw new ArgumentNullException("value");
            }
            return value;
        }
    }
}