namespace Appercode.UI
{
    /// <summary>
    /// Describes the kind of value that a GridLength object is holding.
    /// </summary>
    public enum GridUnitType
    {
        /// <summary>
        /// The size is determined by the size properties of the content object.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// The value is expressed as a pixel.
        /// </summary>
        Pixel = 1,
        
        /// <summary>
        /// The value is expressed as a weighted proportion of available space.
        /// </summary>
        Star = 2,
    }
}
