using System;

namespace Appercode.UI.StylesAndResources
{
    /// <summary>
    /// Attribute that determenates resources that availible in all pages in application
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ApplicationResourcesAttribute : Attribute
    {
        /// <summary>
        /// ApplicationResourcesAttribute constructor
        /// </summary>
        public ApplicationResourcesAttribute()
        {
        }
    }
}