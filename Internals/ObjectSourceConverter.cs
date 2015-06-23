using Appercode.UI.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class ObjectSourceConverter : DefaultValueConverter, IValueConverter
    {
        public ObjectSourceConverter(Type targetType)
            : base(null, typeof(object), targetType, true, false)
        {
        }

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            if ((o != null && this.targetType.IsAssignableFrom(o.GetType())) || (o == null && DefaultValueConverter.AcceptsNull(this.targetType)))
            {
                return o;
            }
            if (this.targetType != typeof(string))
            {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                string str = "ValueConverter can't convert Type {0} to Type {1}";
                object[] objArray = new object[] { o.GetType().ToString(), this.targetType.ToString() };
                throw new InvalidOperationException(string.Format(invariantCulture, str, objArray));
            }
            return o.ToString();
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return o;
        }
    }
}
