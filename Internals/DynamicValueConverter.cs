using Appercode.UI.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class DynamicValueConverter : IValueConverter
    {
        private Type sourceType;

        private Type targetType;

        private IValueConverter converter;

        private bool targetToSourceNeeded;

        internal DynamicValueConverter(bool targetToSourceNeeded)
        {
            this.targetToSourceNeeded = targetToSourceNeeded;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                if (!DefaultValueConverter.AcceptsNull(targetType))
                {
                    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                    string str = "ValueConverter can't convert Type {0}  to Type {1}";
                    object[] objArray = new object[] { "null", targetType.ToString() };
                    throw new InvalidOperationException(string.Format(invariantCulture, str, objArray));
                }
                value = null;
            }
            else
            {
                Type type = value.GetType();
                this.EnsureConverter(type, targetType);
                if (this.converter == null)
                {
                    CultureInfo cultureInfo = CultureInfo.InvariantCulture;
                    string str1 = "ValueConverter can't convert Type {0}  to Type {1}";
                    object[] objArray1 = new object[] { type.ToString(), targetType.ToString() };
                    throw new InvalidOperationException(string.Format(cultureInfo, str1, objArray1));
                }
                value = this.converter.Convert(value, targetType, parameter, culture);
            }
            return value;
        }

        public object ConvertBack(object value, Type sourceType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                if (!DefaultValueConverter.AcceptsNull(sourceType))
                {
                    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                    string str = "ValueConverter can't convert Type {0}  to Type {1}";
                    object[] objArray = new object[] { "'null'", sourceType.ToString() };
                    throw new InvalidOperationException(string.Format(invariantCulture, str, objArray));
                }
                value = null;
            }
            else
            {
                Type type = value.GetType();
                this.EnsureConverter(sourceType, type);
                if (this.converter == null)
                {
                    CultureInfo cultureInfo = CultureInfo.InvariantCulture;
                    string str1 = "ValueConverter can't convert Type {0}  to Type {1}";
                    object[] objArray1 = new object[] { type.ToString(), sourceType.ToString() };
                    throw new InvalidOperationException(string.Format(cultureInfo, str1, objArray1));
                }
                value = this.converter.ConvertBack(value, sourceType, parameter, culture);
            }
            return value;
        }

        private void EnsureConverter(Type sourceType, Type targetType)
        {
            if (this.sourceType != sourceType || this.targetType != targetType)
            {
                this.converter = DefaultValueConverter.Create(sourceType, targetType, this.targetToSourceNeeded);
                this.sourceType = sourceType;
                this.targetType = targetType;
            }
        }
    }
}
