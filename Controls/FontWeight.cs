using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    public struct FontWeight
    {
        private FontWeightType weight;

        public FontWeight(FontWeightType weight)
        {
            this.weight = weight;
        }
        
        internal FontWeightType Weight
        {
            get
            {
                if ((int)this.weight == 0)
                {
                    this.weight = FontWeightType.Normal;
                }
                return this.weight;
                }
            }

        public static bool operator ==(FontWeight left, FontWeight right)
        {
            return left.Weight == right.Weight;
        }

        public static bool operator !=(FontWeight left, FontWeight right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FontWeight))
            {
                return false;
            }
            return this == (FontWeight)obj;
        }

        public override int GetHashCode()
        {
            return (int)this.Weight;
        }

        public override string ToString()
        {
            string str = "Normal";
            FontWeightType fontWeightType = this.weight;
            if (fontWeightType <= FontWeightType.Medium)
            {
                if (fontWeightType <= FontWeightType.ExtraLight)
                {
                    if (fontWeightType == FontWeightType.Thin)
                    {
                        str = "Thin";
                    }
                    else if (fontWeightType == FontWeightType.ExtraLight)
                    {
                        str = "ExtraLight";
                    }
                }
                else if (fontWeightType == FontWeightType.Light)
                {
                    str = "Light";
                }
                else if (fontWeightType == FontWeightType.Normal)
                {
                    str = "Normal";
                }
                else if (fontWeightType == FontWeightType.Medium)
                {
                    str = "Medium";
                }
            }
            else if (fontWeightType <= FontWeightType.Bold)
            {
                if (fontWeightType == FontWeightType.SemiBold)
                {
                    str = "SemiBold";
                }
                else if (fontWeightType == FontWeightType.Bold)
                {
                    str = "Bold";
                }
            }
            else if (fontWeightType == FontWeightType.ExtraBold)
            {
                str = "ExtraBold";
            }
            else if (fontWeightType == FontWeightType.Black)
            {
                str = "Black";
            }
            else if (fontWeightType == FontWeightType.ExtraBlack)
            {
                str = "ExtraBlack";
            }
            return str;
        }
    }
}