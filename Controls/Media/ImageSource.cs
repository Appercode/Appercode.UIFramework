using Appercode.UI.Controls.Media.Imaging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Appercode.UI.Controls.Media
{
    internal enum ImageStatus
    {
        None,
        Loading,
        Loaded,
        Failed
    }

    [TypeConverterAttribute(typeof(ImageSourceConverter))]
    public abstract partial class ImageSource : DependencyObject
    {
        internal ImageSource()
        {
        }

        internal ImageStatus ImageLoadStatus
        {
            get;
            set;
        }
    }

    public class ImageSourceConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                if (s.Contains(':'))
                {
                    return new BitmapImage(new Uri(s, UriKind.Absolute));
                }
                else
                {
                    return new BitmapImage(new Uri(s, UriKind.Relative));
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}