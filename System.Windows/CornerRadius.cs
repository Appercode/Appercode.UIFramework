using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace System.Windows
{
    [TypeConverter(typeof(CornerRadiusConverter))]
    public struct CornerRadius
    {
        public CornerRadius(double uniformRadius)
            : this()
        {
            this.TopLeft = this.BottomLeft = this.TopRight = this.BottomRight = uniformRadius;
        }

        public CornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft)
            : this()
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomRight = bottomRight;
            this.BottomLeft = bottomLeft;
        }
        public double TopLeft
        {
            get;
            set;
        }

        public double TopRight
        {
            get;
            set;
        }

        public double BottomRight
        {
            get;
            set;
        }

        public double BottomLeft
        {
            get;
            set;
        }

        public static bool operator ==(CornerRadius a, CornerRadius b)
        {
            return a.BottomLeft == b.BottomLeft && a.TopRight == b.TopRight && a.BottomRight == b.BottomRight && a.TopLeft == b.TopLeft;
        }

        public static bool operator !=(CornerRadius a, CornerRadius b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return (obj is CornerRadius) && (this == (CornerRadius)obj);
        }

        public override int GetHashCode()
        {
            return this.BottomLeft.GetHashCode() ^ this.TopLeft.GetHashCode() ^ this.TopRight.GetHashCode() ^ this.BottomRight.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[CornerRadius: TopLeft={0}, TopRight={1}, BottomRight={2}, BottomLeft={3}]", this.TopLeft, this.TopRight, this.BottomRight, this.BottomLeft);
        }
    }

    public class CornerRadiusConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, global::System.Globalization.CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                var margins = s.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (margins.Length == 1)
                {
                    double uniformLength;
                    if (double.TryParse(margins[0], out uniformLength))
                    {
                        return new CornerRadius(uniformLength);
                    }
                }
                if (margins.Length == 4)
                {
                    double topLeft, topRight, bottomRight, bottomLeft;
                    if (double.TryParse(margins[0], out topLeft) && double.TryParse(margins[1], out topRight)
                        && double.TryParse(margins[2], out bottomRight) && double.TryParse(margins[3], out bottomLeft))
                    {
                        return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
