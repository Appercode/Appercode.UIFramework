using Appercode.UI.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class ObjectTargetConverter : DefaultValueConverter, IValueConverter
    {
        public ObjectTargetConverter(Type sourceType)
            : base(null, sourceType, typeof(object), true, false)
        {
        }

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return o;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            if (o == null && DefaultValueConverter.AcceptsNull(this.sourceType))
            {
                return o;
            }
            if (o != null && this.sourceType.IsAssignableFrom(o.GetType()))
            {
                return o;
            }
            if (this.sourceType != typeof(string))
            {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                string str = "ValueConverter can't convert Type {0} to Type {1}";
                object[] objArray = new object[] { o.GetType().ToString(), this.sourceType.ToString() };
                throw new InvalidOperationException(string.Format(invariantCulture, str, objArray));
            }
            return o.ToString();
        }
    }
}
