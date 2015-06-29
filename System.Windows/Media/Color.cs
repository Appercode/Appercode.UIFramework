using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Appercode.UI.Controls;
using Appercode.UI.Controls.Media;

namespace System.Windows.Media
{
    public partial struct Color : IFormattable
    {
        private Color.MILColor innerRgbColor;

        public byte A
        {
            get
            {
                return this.innerRgbColor.A;
            }
            set
            {
                this.innerRgbColor.A = value;
            }
        }

        public byte B
        {
            get
            {
                return this.innerRgbColor.B;
            }
            set
            {
                this.innerRgbColor.B = value;
            }
        }

        public byte G
        {
            get
            {
                return this.innerRgbColor.G;
            }
            set
            {
                this.innerRgbColor.G = value;
            }
        }

        public byte R
        {
            get
            {
                return this.innerRgbColor.R;
            }
            set
            {
                this.innerRgbColor.R = value;
            }
        }

        /*internal static Color Create(object o)
        {
            if (o != null)
            {
                return Color.FromUInt32((uint)o);
            }
            Color color = new Color();
            return color;
        }*/

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            Color color = new Color();
            color.innerRgbColor.A = a;
            color.innerRgbColor.R = r;
            color.innerRgbColor.G = g;
            color.innerRgbColor.B = b;
            return color;
        }

        public static Color FromWhiteAlpha(byte a, byte white)
        {
            return Color.FromArgb(a, white, white, white);
        }

        public static bool operator ==(Color color1, Color color2)
        {
            if (color1.R != color2.R)
            {
                return false;
            }
            if (color1.G != color2.G)
            {
                return false;
            }
            if (color1.B != color2.B)
            {
                return false;
            }
            if (color1.A != color2.A)
            {
                return false;
            }
            return true;
        }

        public static bool operator !=(Color color1, Color color2)
        {
            return !(color1 == color2);
        }

        public override int GetHashCode()
        {
            return this.innerRgbColor.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (!(o is Color))
            {
                return false;
            }
            return this == (Color)o;
        }

        public bool Equals(Color color)
        {
            return this == color;
        }

        string System.IFormattable.ToString(string format, IFormatProvider provider)
        {
            return this.ConvertToString(format, provider);
        }

        public override string ToString()
        {
            return this.ConvertToString(null, null);
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ConvertToString(null, provider);
        }

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (format != null)
            {
                char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
                string[] strArrays = new string[] { "sc#{1:", format, "}{0} {2:", format, "}{0} {3:", format, "}{0} {4:", format, "}" };
                string str = string.Concat(strArrays);
                object[] objArray = new object[] { numericListSeparator, this.innerRgbColor.A, this.innerRgbColor.R, this.innerRgbColor.G, this.innerRgbColor.B };
                stringBuilder.AppendFormat(provider, str, objArray);
            }
            else
            {
                object[] objArray1 = new object[] { this.innerRgbColor.A };
                stringBuilder.AppendFormat(provider, "#{0:X2}", objArray1);
                object[] objArray2 = new object[] { this.innerRgbColor.R };
                stringBuilder.AppendFormat(provider, "{0:X2}", objArray2);
                object[] objArray3 = new object[] { this.innerRgbColor.G };
                stringBuilder.AppendFormat(provider, "{0:X2}", objArray3);
                object[] objArray4 = new object[] { this.innerRgbColor.B };
                stringBuilder.AppendFormat(provider, "{0:X2}", objArray4);
            }
            return stringBuilder.ToString();
        }

        internal uint ToUInt32()
        {
            uint num = (uint)(this.innerRgbColor.A << 24 | this.innerRgbColor.R << 16 | this.innerRgbColor.G << 8 | this.innerRgbColor.B);
            return num;
        }

        private struct MILColor
        {
            public byte A;

            public byte R;

            public byte G;

            public byte B;
        }
    }
}