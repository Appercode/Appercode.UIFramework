using Appercode.UI.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class InterfaceConverter : IValueConverter
    {
        private Type sourceType;

        private Type targetType;

        internal InterfaceConverter(Type sourceType, Type targetType)
        {
            this.sourceType = sourceType;
            this.targetType = targetType;
        }

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return this.ConvertTo(o, this.targetType);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return this.ConvertTo(o, this.sourceType);
        }

        private object ConvertTo(object o, Type type)
        {
            if (!type.IsInstanceOfType(o))
            {
                return null;
            }
            return o;
        }
    }
}
