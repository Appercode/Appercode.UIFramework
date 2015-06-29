using Appercode.UI.StylesAndResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;

namespace Appercode.UI.Controls.Navigation
{
    /// <summary>
    /// Class for simple lineral navigaion in Appercode
    /// </summary>
    public partial class StackNavigationFrame : IFrame
    {
        private readonly IFrameStyler styler;
        private NavigationService navigationService;
        private AppercodePage currentPage;
        private AppercodeVisualRoot visualRoot;
        private Stack<AppercodePage> backStack;
        private bool modalIsDisplayed;

        /// <summary>
        /// Creates an instance of StackNavigationFrame class.
        /// </summary>
        public StackNavigationFrame(IFrameStyler styler = null)
        {
            this.styler = styler;
            this.Initialize();
        }

#if WINDOWS_PHONE
        
        public new event NavigatedEventHandler Navigated;

        public new event NavigatingCancelEventHandler Navigating;

        public new event NavigationFailedEventHandler NavigationFailed;

        public new event NavigationStoppedEventHandler NavigationStopped;
#else
        public event NavigatedEventHandler Navigated;

        public event NavigatingCancelEventHandler Navigating;

        public event NavigationFailedEventHandler NavigationFailed;

        public event NavigationStoppedEventHandler NavigationStopped;
#endif

#pragma warning disable 108
        /// <summary>
        /// not used now
        /// </summary>
        public int CacheSize
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// true if thare is screens in the current BackStack and you can <see cref="GoBack"/>
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return this.BackStack.Count() > 0;
            }
        }

        /// <summary>
        /// true if you can <see cref="GoForward"/>
        /// </summary>
        /// <remarks>
        /// not used now
        /// </remarks>
        public bool CanGoForward
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dispatcher Dispatcher
        {
            get { throw new NotImplementedException(); }
        }
#pragma warning restore 108

        /// <summary>
        /// Is navigation process is in progress
        /// </summary>
        public bool IsNavigationInProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// <see cref="AppercodePage"/> that currently is showing
        /// </summary>
        public AppercodePage CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            private set
            {
                this.currentPage = value;
            }
        }

        protected NavigationService NavigationService
        {
            get
            {
                return this.navigationService;
            }
            set
            {
                this.navigationService = value;
            }
        }

#pragma warning disable 108
        private IEnumerable<AppercodePage> BackStack
        {
            get
            {
                return this.backStack;
            }
        }
#pragma warning restore 108

        /// <summary>
        /// Navigate to page
        /// </summary>
        /// <param name="sourcePageType">Type of <see cref="AppercodePage"/> to Navigate</param>
        /// <returns>was <see cref="AppercodePage"/> navigated to or it was canceled</returns>
        public bool Navigate(Type sourcePageType, NavigationType navigationType)
        {
            return this.Navigate(sourcePageType, null, navigationType);
        }

        /// <summary>
        /// Navigate to page
        /// </summary>
        /// <param name="sourcePageType">Type of <see cref="AppercodePage"/> to Navigate</param>
        /// <param name="parameter">Parameter to put on <see cref="AppercodePage"/> when navigated to</param>
        /// <returns>was <see cref="AppercodePage"/> navigated to or it was canceled</returns>
        public bool Navigate(Type sourcePageType, object parameter, NavigationType navigationType)
        {
            if (sourcePageType == null)
            {
                throw new ArgumentNullException("sourcePageType", "sourcePageType must not be null");
            }
            if (!sourcePageType.IsSubclassOf(typeof(AppercodePage)) && sourcePageType != typeof(AppercodePage))
            {
                throw new ArgumentException("sourcePageType must be an AppercodePage", "sourcePageType");
            }

            if (this.IsNavigationInProgress)
            {
                return false;
            }
            this.IsNavigationInProgress = true;

            // navigating from
            NavigatingCancelEventArgs navigatingCancelEventArgs = new NavigatingCancelEventArgs(sourcePageType, NavigationMode.New);

            if (this.CurrentPage != null)
            {
                this.CurrentPage.InternalOnNavigatingFrom(navigatingCancelEventArgs);
                if (navigatingCancelEventArgs.Cancel == true)
                {
                    this.IsNavigationInProgress = false;
                    return false;
                }
            }

            // navigeted from
            if (this.CurrentPage != null)
            {
                this.CurrentPage.InternalOnNavigatedFrom(new NavigationEventArgs(sourcePageType, parameter));
                this.backStack.Push(this.CurrentPage);
            }

            // Create page
            var pageConstructorInfo = sourcePageType.GetConstructor(new Type[] { });
            AppercodePage pageInstance = null;
            try
            {
                pageInstance = (AppercodePage)pageConstructorInfo.Invoke(new object[] { });
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                this.IsNavigationInProgress = false;
                throw e.InnerException;
            }

            pageInstance.NavigationService = this.NavigationService;
            this.visualRoot.Child = pageInstance;
            this.NativeShowPage(pageInstance, NavigationMode.New, navigationType);
            this.modalIsDisplayed |= navigationType == NavigationType.Modal;

            // navigated to
            pageInstance.InternalOnNavigatedTo(new NavigationEventArgs(sourcePageType, parameter));

            this.CurrentPage = pageInstance;

            this.IsNavigationInProgress = false;
            return true;
        }

        /// <summary>
        /// not used
        /// </summary>
        public void Load()
        {
            throw new NotImplementedException();
        }

#pragma warning disable 108

        public void RemoveBackEntry()
        {
            if (this.backStack.Count > 0)
            {
                this.backStack.Pop();
            }
        }

        /// <summary>
        /// Go to previous <see cref="AppercodePage"/>
        /// </summary>
        public void GoBack()
        {
            AppercodePage previosPage = this.BackStack.FirstOrDefault();
            if (previosPage == null)
            {
                this.CloseApplication();
                return;
            }
            Type previosPageType = previosPage.GetType();

            // navigating from
            NavigatingCancelEventArgs navigatingCancelEventArgs = new NavigatingCancelEventArgs(previosPageType, NavigationMode.Back);
            this.CurrentPage.InternalOnNavigatingFrom(navigatingCancelEventArgs);
            if (navigatingCancelEventArgs.Cancel == true)
            {
                return;
            }

            // navigated from
            this.backStack.Pop();
            this.visualRoot.Child = null;
            this.CurrentPage.InternalOnNavigatedFrom(new NavigationEventArgs(previosPageType, null, NavigationMode.Back, true));

            // navigation
            this.visualRoot.Child = previosPage;
            this.NativeShowPage(previosPage, NavigationMode.Back, this.modalIsDisplayed ? NavigationType.Modal : NavigationType.Default);
            this.modalIsDisplayed = false;

            this.CurrentPage = previosPage;

            // navigated to
            previosPage.InternalOnNavigatedTo(new NavigationEventArgs(previosPageType, null, NavigationMode.Back, true));
        }

        /// <summary>
        /// Go forward to <see cref="AppercodePage"/> previously went back from
        /// </summary>
        /// <remarks>not used</remarks>
        public void GoForward()
        {
            throw new NotImplementedException();
        }
#pragma warning restore 108

        private void Initialize()
        {
            this.NativeStackNavigationFrame();
            this.NavigationService = new NavigationService(this);
            this.visualRoot = AppercodeVisualRoot.Instance;
            this.backStack = new Stack<AppercodePage>();

            this.LoadApplicationResourcesFromAssembly(Assembly.GetCallingAssembly());
            if (this.styler != null)
            {
                this.styler.StyleNavBar(this);
            }
        }

        private void LoadApplicationResourcesFromAssembly(Assembly assembly)
        {
            var allTypes = assembly.GetExportedTypes();
            var applicationResourcesSources = allTypes.Where(t => t.GetCustomAttribute(typeof(Appercode.UI.StylesAndResources.ApplicationResourcesAttribute)) != null);

            foreach (var source in applicationResourcesSources)
            {
                var generatorMethod = source.GetMethod("GetResourceDictionary", BindingFlags.Static | BindingFlags.Public);
                if (generatorMethod == null || generatorMethod.ReturnType != typeof(ResourceDictionary))
                {
                    throw new InvalidOperationException(string.Format("Can't find GetResourceDictionary method returning ResourceDictionary for ApplicationResource {0}", source.FullName));
                }
                var rd = (ResourceDictionary)generatorMethod.Invoke(null, new object[] { });
                this.visualRoot.Resources.MergedDictionaries.Add(rd);
            }
        }

        private void OnNavigated(NavigationEventArgs e)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, e);
            }
        }

        private void OnNavigating(NavigatingCancelEventArgs e)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, e);
            }
        }

        private void OnNavigationFailed(NavigationFailedEventArgs e)
        {
            if (this.NavigationFailed != null)
            {
                this.NavigationFailed(this, e);
            }
        }

        private void OnNavigationStoped(NavigationEventArgs e)
        {
            if (this.NavigationStopped != null)
            {
                this.NavigationStopped(this, e);
            }
        }

        private void CloseApplication()
        {
            if (this.CanCloseApp)
            {
                this.CurrentPage.InternalOnNavigatedFrom(new NavigationEventArgs(default(Type), null));
                this.NativeCloseApplication();
            }
        }
    }
}