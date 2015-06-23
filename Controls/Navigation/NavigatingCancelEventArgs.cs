using System;
using System.ComponentModel;

namespace Appercode.UI.Controls.Navigation
{
    public sealed class NavigatingCancelEventArgs : CancelEventArgs
    {
        public NavigatingCancelEventArgs(Type pageType, NavigationMode mode)
        {
            this.SourcePageType = pageType;
            this.NavigationMode = mode;
            this.IsCancelable = true;
            this.IsNavigationInitiator = true;
        }

        public NavigatingCancelEventArgs(Type pageType, NavigationMode mode, bool isCancelable, bool isNavigationInitiator)
        {
            this.SourcePageType = pageType;
            this.NavigationMode = mode;
            this.IsCancelable = isCancelable;
            this.IsNavigationInitiator = isNavigationInitiator;
        }

        public bool IsCancelable
        {
            get;
            private set;
        }

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

        public Type SourcePageType
        {
            get;
            private set;
        }
    }
}
