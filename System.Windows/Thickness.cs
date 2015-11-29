using System.ComponentModel;

namespace System.Windows
{
    [TypeConverterAttribute(typeof(ThicknessConverter))]
    public partial struct Thickness
    {
        public Thickness(double uniformLength)
            : this()
        {
            this.Bottom = this.Top = this.Left = this.Right = uniformLength;
        }

        public Thickness(double left, double top, double right, double bottom)
            : this()
        {
            this.Bottom = bottom;
            this.Top = top;
            this.Left = left;
            this.Right = right;
        }

        public double Bottom
        {
            get;
            set;
        }

        public double Left
        {
            get;
            set;
        }

        public double Right
        {
            get;
            set;
        }

        public double Top
        {
            get;
            set;
        }

        public static bool operator ==(Thickness a, Thickness b)
        {
            return a.Top == b.Top && a.Left == b.Left && a.Right == b.Right && a.Bottom == b.Bottom;
        }

        public static bool operator !=(Thickness a, Thickness b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return (obj is Thickness) && (this == (Thickness)obj);
        }

        public override int GetHashCode()
        {
            return this.Top.GetHashCode() ^ this.Bottom.GetHashCode() ^ this.Left.GetHashCode() ^ this.Right.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Thickness: Bottom={0}, Left={1}, Right={2}, Top={3}]", this.Bottom, this.Left, this.Right, this.Top);
        }
    }

    public class ThicknessConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, Globalization.CultureInfo culture, object value)
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
                        return new Thickness(uniformLength);
                    }                    
                }
                if (margins.Length == 2)
                {
                    double leftright, topbottom;
                    if (double.TryParse(margins[0], out leftright) && double.TryParse(margins[1], out topbottom))
                    {
                        return new Thickness(leftright, topbottom, leftright, topbottom);
                    }
                }
                if (margins.Length == 4)
                {
                    double left, right, top, bottom;
                    if (double.TryParse(margins[0], out left) && double.TryParse(margins[1], out top)
                        && double.TryParse(margins[2], out right) && double.TryParse(margins[3], out bottom))
                    {
                        return new Thickness(left, top, right, bottom);
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}