using System;

#if !__IOS__
using nfloat = System.Single;
#endif

namespace Appercode.UI
{
    internal static class MathF
    {
        public const float E = 2.71828f;
        public const float PI = 3.14159f;
        private const double MinimumDelta = 0.00001;

        public static nfloat Abs(nfloat value)
        {
            return (nfloat)Math.Abs(value);
        }

        public static bool AreNotClose(nfloat value1, nfloat value2)
        {
            if (value1 == value2)
            {
                return false;
            }

            var delta = value1 - value2;
            return delta > MinimumDelta || delta < -MinimumDelta;
        }

        public static nfloat GetNotNaN(this double value)
        {
            return double.IsNaN(value) ? default(nfloat) : (nfloat)value;
        }

        public static nfloat Max(nfloat val1, nfloat val2)
        {
            return val1 > val2 ? val1 : val2;
        }

        public static nfloat Min(nfloat val1, nfloat val2)
        {
            return val1 < val2 ? val1 : val2;
        }
    }
}
