using System;

namespace Appercode.UI.Controls.Navigation
{
    public sealed class NavigationEventArgs : EventArgs
    {
        public NavigationEventArgs(Type sourcePageType, object parameter, NavigationMode navigationMode, bool isNavigationInitiator)
        {
            this.SourcePageType = sourcePageType;
            this.NavigationMode = navigationMode;
            this.IsNavigationInitiator = isNavigationInitiator;
        }

        internal NavigationEventArgs(Uri uri, object parameter)
        {
            this.Uri = uri;
            this.Parameter = parameter;
            this.NavigationMode = NavigationMode.New;
            this.IsNavigationInitiator = true;
        }

        internal NavigationEventArgs(Type sourcePageType, object parameter)
        {
            this.SourcePageType = sourcePageType;
            this.Parameter = parameter;
            this.NavigationMode = NavigationMode.New;
            this.IsNavigationInitiator = true;
        }

        /// <summary>
        /// Gets or sets related URI
        /// </summary>
        public Uri Uri { get; internal set; }

        public bool IsNavigationInitiator
        {
            get;
            private set;
        }

        public NavigationMode NavigationMode
        {
            get;
            private set;
        }

        public object Parameter 
        { 
            get;
            private set; 
        }

        public Type SourcePageType
        {
            get;
            private set;
        }
    }
}
