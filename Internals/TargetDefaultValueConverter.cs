using Appercode.UI.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class TargetDefaultValueConverter : DefaultValueConverter, IValueConverter
    {
        public TargetDefaultValueConverter(TypeConverter typeConverter, Type sourceType, Type targetType, bool shouldConvertFrom, bool shouldConvertTo)
            : base(typeConverter, sourceType, targetType, shouldConvertFrom, shouldConvertTo)
        {
        }

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return this.ConvertFrom(o, this.targetType, culture);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return this.ConvertTo(o, this.sourceType, culture);
        }
    }
}
