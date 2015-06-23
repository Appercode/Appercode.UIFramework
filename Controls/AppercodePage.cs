using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Class for pages in Appercode
    /// </summary>
    public partial class AppercodePage : Page
    {
        /// <summary>
        /// Identifies the <seealso cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(PageOrientation), typeof(Page), new PropertyMetadata(default(PageOrientation)));

        /// <summary>
        /// Identifies the <seealso cref="SupportedOrientations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SupportedOrientationsProperty =
            DependencyProperty.Register("SupportedOrientations", typeof(SupportedPageOrientation), typeof(Page), new PropertyMetadata(default(SupportedPageOrientation)));

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
        /// <value><seealso cref="SupportedPageOrientation"/></value>
        public SupportedPageOrientation SupportedOrientations
        {
            get { return (SupportedPageOrientation)this.GetValue(SupportedOrientationsProperty); }
            set { this.SetValue(SupportedOrientationsProperty, value); }
        }
    }
}