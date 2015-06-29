using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Appercode.UI.Controls.Navigation
{
    /// <summary>
    /// interface for navigaion frames in Appercode
    /// </summary>
    public interface IFrame
    {
        event NavigatedEventHandler Navigated;

        event NavigatingCancelEventHandler Navigating;

        event NavigationFailedEventHandler NavigationFailed;

        event NavigationStoppedEventHandler NavigationStopped;

        int CacheSize
        {
            get;
            set;
        }

        bool CanGoBack
        {
            get;
        }

        bool CanGoForward
        {
            get;
        }

        AppercodePage CurrentPage
        {
            get;
        }

        Dispatcher Dispatcher
        {
            get;
        }

        bool IsNavigationInProgress
        {
            get;
        }

        bool Navigate(Type sourcePageType, NavigationType navigationType);

        bool Navigate(Type sourcePageType, object parameter, NavigationType navigationType);

        ////bool IsInDesignModeX();

        void Load();

        void GoBack();
        void GoForward();

        void RemoveBackEntry();
    }
}
