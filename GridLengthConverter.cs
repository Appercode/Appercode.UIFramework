using System;
using System.ComponentModel;
using System.Globalization;

namespace Appercode.UI
{
    /// <summary>
    /// Converts instances of other types to and from GridLength instances.
    /// </summary>
    public class GridLengthConverter : TypeConverter
    {
        #region TypeConverter methods implementations

        /// <summary>
        /// Determines whether a class can be converted from a given type to an instance of GridLength structure
        /// </summary>
        /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
        /// <param name="sourceType">The type of the source that is being evaluated for conversion.</param>
        /// <returns>true if the converter can convert from the specified type to an instance of GridLength; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            switch (Type.GetTypeCode(sourceType))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
            }
            return false;
        }

        /// <summary>
        ///  Determines whether an instance of GridLength can be converted to a different type.
        /// </summary>
        /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
        /// <param name="destinationType">The desired type that this instance of GridLength is being evaluated for conversion.</param>
        /// <returns>true if the converter can convert this instance of GridLength to the specified type; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Attempts to convert a specified object to an instance of GridLength.
        /// </summary>
        /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Cultural specific information that should be respected during conversion.</param>
        /// <param name="source">The object being converted.</param>
        /// <returns>The instance of GridLength that is created from the converted source.</returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            GridUnitType auto;
            if (source == null)
            {
                // throw base.GetConvertFromException(source);
                throw new Exception(""); // TODO: Message
            }
            if (source is string)
            {
                return FromString((string)source, cultureInfo);
            }
            double num = Convert.ToDouble(source, cultureInfo);
            if (double.IsNaN(num))
            {
                num = 1.0;
                auto = GridUnitType.Auto;
            }
            else
            {
                auto = GridUnitType.Pixel;
            }
            return new GridLength(num, auto);
        }

        /// <summary>
        /// Attempts to convert an instance of GridLength to a specified type.
        /// </summary>
        /// <param name="typeDescriptorContext">Describes the context information of a type.</param>
        /// <param name="cultureInfo">Cultural specific information that should be respected during conversion.</param>
        /// <param name="value">The instance of GridLength to convert.</param>
        /// <param name="destinationType">The type that this instance of GridLength is converted to.</param>
        /// <returns>he object that is created from the converted instance of GridLength.</returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((value != null) && (value is GridLength))
            {
                GridLength gl = (GridLength)value;
                if (destinationType == typeof(string))
                {
                    return ToString(gl, cultureInfo);
                }
                /*
                //if (destinationType == typeof(InstanceDescriptor))
                //{
                //    return new InstanceDescriptor(typeof(GridLength).GetConstructor(new Type[] { typeof(double), typeof(GridUnitType) }), new object[] { gl.Value, gl.GridUnitType });
                //}
                 * */
            }

            // throw base.GetConvertToException(value, destinationType);
            throw new Exception(""); // TODO: Message
        }

        #endregion // TypeConverter methods implementations

        #region Static helper methods

        /// <summary>
        /// Parces a string to GridLength structure
        /// </summary>
        /// <param name="s"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        internal static GridLength FromString(string s, CultureInfo cultureInfo)
        {
            double num;
            GridUnitType type;

            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException("s");
            }

            if (s == "auto")
            {
                num = 1;
                type = GridUnitType.Auto;
            }
            else if (s[s.Length - 1] == '*')
            {
                s = s.Remove(s.Length - 1);
                if (string.IsNullOrEmpty(s))
                {
                    num = 1;
                }
                else
                {
                    if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
                    {
                        throw new ArgumentException("s");
                    }
                }
                type = GridUnitType.Star;
            }
            else
            {
                if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
                {
                    throw new ArgumentException("s");
                }
                type = GridUnitType.Pixel;
            }

            return new GridLength(num, type);
        }

        /// <summary>
        /// Converts specified grid length structure to string
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        internal static string ToString(GridLength gl, CultureInfo cultureInfo)
        {
            switch (gl.GridUnitType)
            {
                case GridUnitType.Auto:
                    return "Auto";

                case GridUnitType.Star:
                    if (Math.Abs(gl.Value - 1.0) < double.Epsilon)
                    {
                        return "*";
                    }
                    return Convert.ToString(gl.Value, cultureInfo) + "*";
            }
            return Convert.ToString(gl.Value, cultureInfo);
        }

        #endregion //Static helper methods
    }
}
