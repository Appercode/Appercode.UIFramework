using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    [TypeConverterAttribute(typeof(FontWeightsConverter))]
    public static class FontWeights
    {
        public static FontWeight Black
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.Black);
            }
        }

        public static FontWeight Bold
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.Bold);
            }
        }

        public static FontWeight ExtraBlack
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.ExtraBlack);
            }
        }

        public static FontWeight ExtraBold
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.ExtraBold);
            }
        }

        public static FontWeight ExtraLight
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.ExtraLight);
            }
        }

        public static FontWeight Light
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.Light);
            }
        }

        public static FontWeight Medium
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.Medium);
            }
        }

        public static FontWeight Normal
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.Normal);
            }
        }

        public static FontWeight SemiBold
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.SemiBold);
            }
        }

        public static FontWeight Thin
        {
            get
            {
                return FontWeights.EnsureFontWeight(FontWeightType.Thin);
            }
        }

        internal static FontWeight EnsureFontWeight(FontWeightType weight)
        {
            if ((int)weight == 0)
            {
                weight = FontWeightType.Normal;
            }
            FontWeightType fontWeightType = weight;
            if (fontWeightType <= FontWeightType.Medium)
            {
                if (fontWeightType > FontWeightType.ExtraLight)
                {
                    if (fontWeightType == FontWeightType.Light || fontWeightType == FontWeightType.Normal || fontWeightType == FontWeightType.Medium)
                    {
                        return new FontWeight(weight);
                    }
                    throw new ArgumentOutOfRangeException("weight");
                }
                else
                {
                    if (fontWeightType == FontWeightType.Thin || fontWeightType == FontWeightType.ExtraLight)
                    {
                        return new FontWeight(weight);
                    }
                    throw new ArgumentOutOfRangeException("weight");
                }
            }
            else if (fontWeightType <= FontWeightType.Bold)
            {
                if (fontWeightType == FontWeightType.SemiBold || fontWeightType == FontWeightType.Bold)
                {
                    return new FontWeight(weight);
                }
                throw new ArgumentOutOfRangeException("weight");
            }
            else if (fontWeightType != FontWeightType.ExtraBold && fontWeightType != FontWeightType.Black && fontWeightType != FontWeightType.ExtraBlack)
            {
                throw new ArgumentOutOfRangeException("weight");
            }
            return new FontWeight(weight);
        }   
    }

    public class FontWeightsConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                if (Enum.IsDefined(typeof(FontWeightType), sourceType))
                {
                    return true;
                }
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                return new FontWeight((FontWeightType)Enum.Parse(typeof(FontWeightType), s));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}