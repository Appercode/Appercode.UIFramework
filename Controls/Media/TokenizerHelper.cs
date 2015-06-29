using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls.Media
{
    internal static class TokenizerHelper
    {
        internal static char GetNumericListSeparator(IFormatProvider provider)
        {
            char chr = ',';
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
            if (instance.NumberDecimalSeparator.Length > 0 && chr == instance.NumberDecimalSeparator[0])
            {
                chr = ';';
            }
            return chr;
        }
    }
}