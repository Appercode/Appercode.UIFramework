using Appercode.UI.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class SystemConvertConverter : IValueConverter
    {
        private static readonly Type[] SupportedTypes;

        private static readonly Type[] CharSupportedTypes;

        private Type sourceType;

        private Type targetType;

        static SystemConvertConverter()
        {
            Type[] typeArray = new Type[] { typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(bool), typeof(byte), typeof(short), typeof(uint), typeof(ulong), typeof(ushort), typeof(sbyte) };
            SystemConvertConverter.SupportedTypes = typeArray;
            Type[] typeArray1 = new Type[] { typeof(string), typeof(int), typeof(long), typeof(byte), typeof(short), typeof(uint), typeof(ulong), typeof(ushort), typeof(sbyte) };
            SystemConvertConverter.CharSupportedTypes = typeArray1;
        }

        public SystemConvertConverter(Type sourceType, Type targetType)
        {
            this.sourceType = sourceType;
            this.targetType = targetType;
        }

        public static bool CanConvert(Type sourceType, Type targetType)
        {
            if (sourceType == typeof(DateTime))
            {
                return targetType == typeof(string);
            }
            if (targetType == typeof(DateTime))
            {
                return sourceType == typeof(string);
            }
            if (sourceType == typeof(char))
            {
                return SystemConvertConverter.CanConvertChar(targetType);
            }
            if (targetType == typeof(char))
            {
                return SystemConvertConverter.CanConvertChar(sourceType);
            }
            for (int i = 0; i < (int)SystemConvertConverter.SupportedTypes.Length; i++)
            {
                if (sourceType == SystemConvertConverter.SupportedTypes[i])
                {
                    for (i++; i < (int)SystemConvertConverter.SupportedTypes.Length; i++)
                    {
                        if (targetType == SystemConvertConverter.SupportedTypes[i])
                        {
                            return true;
                        }
                    }
                }
                else if (targetType == SystemConvertConverter.SupportedTypes[i])
                {
                    for (i++; i < (int)SystemConvertConverter.SupportedTypes.Length; i++)
                    {
                        if (sourceType == SystemConvertConverter.SupportedTypes[i])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(o, this.targetType, culture);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(o, this.sourceType, culture);
        }

        private static bool CanConvertChar(Type type)
        {
            for (int i = 0; i < (int)SystemConvertConverter.CharSupportedTypes.Length; i++)
            {
                if (type == SystemConvertConverter.CharSupportedTypes[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
