using Appercode.UI.Controls.Media.Imaging;
using System;
using System.Windows;

namespace Appercode.UI.Controls.Navigation.Primitives
{
    /// <summary>
    /// Represents tab for <see cref="TabNavigationFrame"/>
    /// </summary>
    public class TabBarTab : DependencyObject
    {
        public static readonly DependencyProperty BadgeProperty =
            DependencyProperty.Register("Badge", typeof(string), typeof(TabBarTab), new PropertyMetadata(OnBadgeChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="Appercode.UI.Controls.Navigation.Primitives.TabBarTab"/> class
        /// </summary>
        /// <param name="pageType">Type of <see cref="AppercodePage"/> to navigate to</param>
        /// <param name="title">Tab title</param>
        public TabBarTab(Type pageType, string title)
            : this(pageType, null, title, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Appercode.UI.Controls.Navigation.Primitives.TabBarTab"/> class
        /// </summary>
        /// <param name="pageType">Type of <see cref="AppercodePage"/> to navigate to</param>
        /// <param name="icon">Tab icon</param>
        public TabBarTab(Type pageType, BitmapImage icon)
            : this(pageType, null, null, icon)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Appercode.UI.Controls.Navigation.Primitives.TabBarTab"/> class.
        /// </summary>
        /// <param name="pageType">Type of <see cref="AppercodePage"/> to navigate to</param>
        /// <param name="title">Tab title</param>
        /// <param name="icon">Tab icon</param>
        public TabBarTab(Type pageType, string title, BitmapImage icon)
            : this(pageType, null, title, icon)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Appercode.UI.Controls.Navigation.Primitives.TabBarTab"/> class.
        /// </summary>
        /// <param name="pageType">Type of <see cref="AppercodePage"/> to navigate to</param>
        /// <param name="navigationParameter">Navigation parameter for <paramref name="pageType"/></param>
        /// <param name="title">Tab title</param>
        /// <param name="icon">Tab icon</param>
        public TabBarTab(Type pageType, object navigationParameter, string title, BitmapImage icon)
        {
            this.Title = title;
            this.Icon = icon;
            this.PageType = pageType;
            this.NavigationParameter = navigationParameter;
        }

        public event EventHandler BadgeChanged;

        /// <summary>
        /// Gets or sets tab title
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the tab icon
        /// </summary>
        /// <value>The icon.</value>
        public BitmapImage Icon { get; set; }

        /// <summary>
        /// Gets or sets the type of the <see cref="AppercodePage"/> to navigate to</param>
        /// </summary>
        /// <value>The type of the page.</value>
        public Type PageType { get; set; }

        /// <summary>
        /// Gets or sets the navigation parameter for <see cref="PageType"/>
        /// </summary>
        /// <value>The navigation parameter.</value>
        public object NavigationParameter { get; set; }

        public string Badge
        {
            get { return (string)GetValue(BadgeProperty); }
            set { this.SetValue(BadgeProperty, value); }
        }

        private static void OnBadgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tab = d as TabBarTab;
            if (tab != null)
            {
                if (tab.BadgeChanged != null)
                {
                    tab.BadgeChanged(tab, EventArgs.Empty);
                }
            }
        }
    }
}