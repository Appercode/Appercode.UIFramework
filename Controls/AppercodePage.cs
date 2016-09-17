using Appercode.UI.Controls.Navigation;
using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Class for pages in Appercode
    /// </summary>
    public partial class AppercodePage : Page
    {
        /// <summary>
        /// Identifies the <seealso cref="Orientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(PageOrientation), typeof(AppercodePage), new PropertyMetadata(default(PageOrientation)));

        /// <summary>
        /// Identifies the <seealso cref="SupportedOrientations" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SupportedOrientationsProperty = DependencyProperty.Register(
            nameof(SupportedOrientations), typeof(SupportedPageOrientation), typeof(AppercodePage), new PropertyMetadata(default(SupportedPageOrientation)));

        /// <summary>
        /// Gets the current orientation.
        /// </summary>
        public PageOrientation Orientation
        {
            get { return (PageOrientation)this.GetValue(OrientationProperty); }
        }

        /// <summary>
        /// Gets or sets the supported device orientations.
        /// </summary>
        public SupportedPageOrientation SupportedOrientations
        {
            get { return (SupportedPageOrientation)this.GetValue(SupportedOrientationsProperty); }
            set { this.SetValue(SupportedOrientationsProperty, value); }
        }

        internal void SetNavigationType(NavigationType navigationType)
        {
            this.ApplyNavigationType(navigationType);
        }

        partial void ApplyNavigationType(NavigationType navigationType);
    }
}