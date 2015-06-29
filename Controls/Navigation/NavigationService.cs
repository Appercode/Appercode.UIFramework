using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls.Navigation
{
    public sealed class NavigationService
    {
        private IFrame navigationFrame;

        internal NavigationService(IFrame navigationFrame)
        {
            this.navigationFrame = navigationFrame;
        }

        public event NavigatedEventHandler Navigated;

        public void GoBack()
        {
            var page = this.navigationFrame.CurrentPage.GetType();
            this.navigationFrame.GoBack();
            this.OnNavigated(new NavigationEventArgs(page, null));
        }

        public void GoForward()
        {
            var page = this.navigationFrame.CurrentPage.GetType();
            this.navigationFrame.GoForward();
            this.OnNavigated(new NavigationEventArgs(page, null));
        }

        public void RemoveBackEntry()
        {
            this.navigationFrame.RemoveBackEntry();
        }

        public bool Navigate(Type pageType, object parameter, NavigationType navigationType = NavigationType.Default)
        {
            var navigated = this.navigationFrame.Navigate(pageType, parameter, navigationType);
            if (navigated)
            {
                this.OnNavigated(new NavigationEventArgs(pageType, parameter));
            }
            return navigated;
        }

        public bool Navigate(Type pageType, NavigationType navigationType = NavigationType.Default)
        {
            var navigated = this.navigationFrame.Navigate(pageType, navigationType);
            if (navigated)
            {
                this.OnNavigated(new NavigationEventArgs(pageType, null));
            }
            return navigated;
        }

        internal void OnNavigated(NavigationEventArgs e)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, e);
            }
        }
    }
}