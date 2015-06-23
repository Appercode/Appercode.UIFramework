using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Appercode.UI.Data;

namespace Appercode.UI
{
    public static class ValueConverterFactory
    {
        private class SimpleValueConverter<TFrom, TTo> : IValueConverter
        {
            public Func<TFrom, TTo> ConvertFunc { get; set; }
            public Func<TTo, TFrom> ConvertBackFunc { get; set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (this.ConvertFunc == null) return null;
                return this.ConvertFunc((TFrom)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (this.ConvertBackFunc == null) return null;
                return this.ConvertBackFunc((TTo)value);
            }
        }

        public static IValueConverter Make<TFrom, TTo>(Func<TFrom, TTo> convertFunc, Func<TTo, TFrom> convertBackFunc = null)
        {
            return new SimpleValueConverter<TFrom, TTo>()
            {
                ConvertFunc = convertFunc,
                ConvertBackFunc = convertBackFunc
            };
        }

        public static IValueConverter Make<TFrom>(Func<TFrom, object> convertFunc, Func<object, TFrom> convertBackFunc = null)
        {
            return new SimpleValueConverter<TFrom, object>()
            {
                ConvertFunc = convertFunc,
                ConvertBackFunc = convertBackFunc
            };
        }
    }
}