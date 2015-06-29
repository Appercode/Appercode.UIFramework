using Appercode.UI.Data;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Appercode.UI.Internals
{
    internal class DefaultValueConverter
    {
        protected Type sourceType;

        protected Type targetType;

        private TypeConverter typeConverter;

        private bool shouldConvertFrom;

        private bool shouldConvertTo;

        protected DefaultValueConverter(TypeConverter typeConverter, Type sourceType, Type targetType, bool shouldConvertFrom, bool shouldConvertTo)
        {
            this.typeConverter = typeConverter;
            this.sourceType = sourceType;
            this.targetType = targetType;
            this.shouldConvertFrom = shouldConvertFrom;
            this.shouldConvertTo = shouldConvertTo;
        }

        internal static bool AcceptsNull(Type type)
        {
            if (!type.IsValueType)
            {
                return true;
            }
            return Nullable.GetUnderlyingType(type) != null;
        }

        internal static IValueConverter Create(Type sourceType, Type targetType, bool targetToSource)
        {
            if (targetType == typeof(object))
            {
                return new ObjectTargetConverter(sourceType);
            }
            if (sourceType == typeof(object))
            {
                return new ObjectSourceConverter(targetType);
            }
            if (SystemConvertConverter.CanConvert(sourceType, targetType))
            {
                return new SystemConvertConverter(sourceType, targetType);
            }
            Type underlyingType = Nullable.GetUnderlyingType(sourceType);
            if (underlyingType != null)
            {
                sourceType = underlyingType;
                return DefaultValueConverter.Create(sourceType, targetType, targetToSource);
            }
            underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
                return DefaultValueConverter.Create(sourceType, targetType, targetToSource);
            }
            if (sourceType.IsInterface || targetType.IsInterface)
            {
                return new InterfaceConverter(sourceType, targetType);
            }
            TypeConverter converter = DefaultValueConverter.GetConverter(sourceType);
            bool canConvertTo = converter != null ? converter.CanConvertTo(targetType) : false;
            bool canConvertFrom = converter != null ? converter.CanConvertFrom(targetType) : false;
            if ((canConvertTo || targetType.IsAssignableFrom(sourceType)) && (!targetToSource || canConvertFrom || sourceType.IsAssignableFrom(targetType)))
            {
                return new SourceDefaultValueConverter(converter, sourceType, targetType, !targetToSource ? false : canConvertFrom, canConvertTo);
            }
            converter = DefaultValueConverter.GetConverter(targetType);
            canConvertTo = converter != null ? converter.CanConvertTo(sourceType) : false;
            canConvertFrom = converter != null ? converter.CanConvertFrom(sourceType) : false;
            if ((!canConvertFrom && !targetType.IsAssignableFrom(sourceType)) || (targetToSource && !canConvertTo && !sourceType.IsAssignableFrom(targetType)))
            {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                string str = "ValueConverter can't convert Type {0} to Type {1}";
                object[] objArray = new object[] { sourceType.ToString(), targetType.ToString() };
                throw new InvalidOperationException(string.Format(invariantCulture, str, objArray));
            }
            return new TargetDefaultValueConverter(converter, sourceType, targetType, canConvertFrom, !targetToSource ? false : canConvertTo);
        }

        internal static TypeConverter GetConverter(Type type)
        {
            try
            {
                return TypeDescriptor.GetConverter(type);
            }
            catch
            {
                return null;
            }
        }

        protected object ConvertFrom(object o, Type destinationType, CultureInfo culture)
        {
            return this.ConvertHelper(o, destinationType, culture, false);
        }

        protected object ConvertTo(object o, Type destinationType, CultureInfo culture)
        {
            return this.ConvertHelper(o, destinationType, culture, true);
        }

        private object ConvertHelper(object o, Type destinationType, CultureInfo culture, bool isForward)
        {
            object obj;
            bool flag = isForward ? !this.shouldConvertTo : !this.shouldConvertFrom;
            if (flag)
            {
                if (!flag || ((o == null || !destinationType.IsAssignableFrom(o.GetType())) && (o != null || !DefaultValueConverter.AcceptsNull(destinationType))))
                {
                    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                    string str = "ValueConverter can't convert Type {0} to Type {1}";
                    object[] objArray = new object[] { (o == null ? "'null'" : o.GetType().ToString()), destinationType.ToString() };
                    throw new InvalidOperationException(string.Format(invariantCulture, str, objArray));
                }
                obj = o;
                flag = false;
            }
            else
            {
                obj = !isForward ? this.typeConverter.ConvertFrom(o) : this.typeConverter.ConvertTo(o, destinationType);
            }
            return obj;
        }
    }
}