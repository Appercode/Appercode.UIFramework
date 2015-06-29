using System;
using System.Globalization;

namespace Appercode.UI.Controls.Primitives
{
    public struct GeneratorPosition
    {
        public GeneratorPosition(int index, int offset) : this()
        {
            this.Index = index;
            this.Offset = offset;
        }

        public int Index { get; set; }

        public int Offset { get; set; }

        public static bool operator ==(GeneratorPosition gp1, GeneratorPosition gp2)
        {
            if (gp1.Index == gp2.Index)
            {
                return gp1.Offset == gp2.Offset;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(GeneratorPosition gp1, GeneratorPosition gp2)
        {
            return !(gp1 == gp2);
        }

        public override int GetHashCode()
        {
            return this.Index.GetHashCode() + this.Offset.GetHashCode();
        }

        public override string ToString()
        {
            return "GeneratorPosition (" + this.Index.ToString((IFormatProvider)CultureInfo.InvariantCulture) + "," + this.Offset.ToString((IFormatProvider)CultureInfo.InvariantCulture) + ")";
        }

        public override bool Equals(object o)
        {
            if (!(o is GeneratorPosition))
            {
                return false;
            }
            GeneratorPosition generatorPosition = (GeneratorPosition)o;
            if (this.Index == generatorPosition.Index)
            {
                return this.Offset == generatorPosition.Offset;
            }
            else
            {
                return false;
            }
        }
    }
}
