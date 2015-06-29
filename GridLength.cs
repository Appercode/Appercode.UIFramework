using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Appercode.UI
{
    /// <summary>
    /// Represents the length of elements that explicitly support "Star" unit types.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(GridLengthConverter))]
    public struct GridLength : IEquatable<GridLength>
    {
        #region Fields

        /// <summary>
        /// Holds unit value
        /// </summary>
        private double unitValue;

        /// <summary>
        /// Holds unit type
        /// </summary>
        private GridUnitType unitType;
        
        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Initializes the GridLength type
        /// </summary>
        static GridLength()
        {
            Auto = new GridLength(1.0, GridUnitType.Auto);
        }

        /// <summary>
        /// Initializes a new instance of the GridLength structure using the specified absolute value in pixels.
        /// </summary>
        /// <param name="pixels"></param>
        public GridLength(double pixels)
            : this(pixels, GridUnitType.Pixel)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the GridLength structure and specifies what kind of value it holds.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public GridLength(double value, GridUnitType type)
        {
            if (double.IsNaN(value))
            {
                throw new ArgumentException("value");
            }
            if (double.IsInfinity(value))
            {
                throw new ArgumentException("value");
            }
            if (((type != GridUnitType.Auto) && (type != GridUnitType.Pixel)) && (type != GridUnitType.Star))
            {
                throw new ArgumentException("type");
            }

            if (type == GridUnitType.Auto)
            {
                this.unitValue = 0.0;
            }
            else
            {
                this.unitValue = (value < 1) ? 1.0 : value;
            }

            this.unitType = type;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets an instance of GridLength that holds a value whose size is determined by the size properties of the content object.
        /// </summary>
        public static GridLength Auto { get; private set; }

        /// <summary>
        /// True if length is absolute
        /// </summary>
        public bool IsAbsolute
        {
            get
            {
                return this.unitType == GridUnitType.Pixel;
            }
        }

        /// <summary>
        /// True if length is auto
        /// </summary>
        public bool IsAuto
        {
            get
            {
                return this.unitType == GridUnitType.Auto;
            }
        }

        /// <summary>
        /// True if length is star
        /// </summary>
        public bool IsStar
        {
            get
            {
                return this.unitType == GridUnitType.Star;
            }
        }

        /// <summary>
        /// Gets a value of the GridLength
        /// </summary>
        public double Value
        {
            get
            {
                if (this.unitType != GridUnitType.Auto)
                {
                    return this.unitValue;
                }
                return 1.0;
            }
        }

        /// <summary>
        /// Gets the associated GridUnitType
        /// </summary>
        public GridUnitType GridUnitType
        {
            get
            {
                return this.unitType;
            }
        }

        #endregion // Properties

        #region Overloaded operators

        /// <summary>
        /// Compares two GridLength structures to determine if they are equal.
        /// </summary>
        /// <param name="gl1"></param>
        /// <param name="gl2"></param>
        /// <returns></returns>
        public static bool operator ==(GridLength gl1, GridLength gl2)
        {
            return (gl1.GridUnitType == gl2.GridUnitType) && (gl1.Value == gl2.Value);
        }

        /// <summary>
        /// Compares two GridLength structures to determine if they are not equal.
        /// </summary>
        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            if (gl1.GridUnitType == gl2.GridUnitType)
            {
                return gl1.Value != gl2.Value;
            }
            return true;
        }

        #endregion // Overloaded operators

        #region Equals and hashcode

        /// <summary>
        /// Determines whether the specified GridLength is equal to the current GridLength
        /// </summary>
        /// <param name="objCompare"></param>
        /// <returns></returns>
        public override bool Equals(object objCompare)
        {
            if (objCompare is GridLength)
            {
                GridLength length = (GridLength)objCompare;
                return this == length;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified GridLength is equal to the current GridLength
        /// </summary>
        /// <param name="gridLength"></param>
        /// <returns></returns>
        public bool Equals(GridLength gridLength)
        {
            return this == gridLength;
        }

        /// <summary>
        /// Gets a hash code for the GridLength.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ((int)this.unitValue) + this.Value.GetHashCode();
        }

        #endregion // Equals and hashcode

        #region Overriden methods

        /// <summary>
        /// Returns a System.String representation of the GridLength
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GridLengthConverter.ToString(this, CultureInfo.InvariantCulture);
        }

        #endregion // Overriden methods
    }
}
