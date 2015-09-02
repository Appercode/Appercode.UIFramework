using Appercode.UI.Internals.Boxes;
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
        private readonly Dispatcher dispatcher;
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
            this.dispatcher = Dispatcher.CurrentDispatcher;
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
        /// True if there are any pages in the current BackStack and you can <see cref="GoBack" />
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return this.backStack.Count > 0;
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
            get { return this.dispatcher; }
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

        /// <summary>
        /// Returns an IEnumerable that can be used to enumerate the entries in back navigation history for a Frame.
        /// </summary>
        public IEnumerable<AppercodePage> BackStack
        {
            get
            {
                // return a read-only copy of the backStack to avoid cast and modification by external code
                return this.backStack.AsReadonly();
            }
        }

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

            return BooleanBoxes.TrueBox == this.Dispatcher.Invoke(
                (Func<Type, object, NavigationType, object>)this.NavigateInternal, sourcePageType, parameter, navigationType);
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
            if (this.backStack.Count == 0)
            {
                this.CloseApplication();
            }
            else
            {
                this.Dispatcher.Invoke((Action)this.GoBackInternal);
            }
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

        private object NavigateInternal(Type sourcePageType, object parameter, NavigationType navigationType)
        {
            this.IsNavigationInProgress = true;

            // navigating from
            if (this.CurrentPage != null)
            {
                var navigatingCancelEventArgs = new NavigatingCancelEventArgs(sourcePageType, NavigationMode.New);
                this.CurrentPage.InternalOnNavigatingFrom(navigatingCancelEventArgs);
                if (navigatingCancelEventArgs.Cancel)
                {
                    this.IsNavigationInProgress = false;
                    return BooleanBoxes.FalseBox;
                }
            }

            // navigated from
            var navigationEventArgs = new NavigationEventArgs(sourcePageType, parameter);
            if (this.CurrentPage != null)
            {
                this.CurrentPage.InternalOnNavigatedFrom(navigationEventArgs);
                this.backStack.Push(this.CurrentPage);
            }

            // Create page
            var pageConstructorInfo = sourcePageType.GetConstructor(new Type[] { });
            AppercodePage pageInstance;
            try
            {
                pageInstance = (AppercodePage)pageConstructorInfo.Invoke(new object[] { });
            }
            catch (TargetInvocationException e)
            {
                this.IsNavigationInProgress = false;
                throw e.InnerException;
            }

            pageInstance.NavigationService = this.NavigationService;
            this.visualRoot.Child = pageInstance;
            this.NativeShowPage(pageInstance, NavigationMode.New, navigationType);
            this.modalIsDisplayed |= navigationType == NavigationType.Modal;

            // navigated to
            pageInstance.InternalOnNavigatedTo(navigationEventArgs);

            this.CurrentPage = pageInstance;
            this.IsNavigationInProgress = false;
            return BooleanBoxes.TrueBox;
        }

        private void GoBackInternal()
        {
            var previousPage = this.backStack.Peek();
            var previousPageType = previousPage.GetType();

            // navigating from
            var navigatingCancelEventArgs = new NavigatingCancelEventArgs(previousPageType, NavigationMode.Back);
            this.CurrentPage.InternalOnNavigatingFrom(navigatingCancelEventArgs);
            if (navigatingCancelEventArgs.Cancel)
            {
                return;
            }

            // navigated from
            this.backStack.Pop();
            this.visualRoot.Child = null;
            this.CurrentPage.InternalOnNavigatedFrom(new NavigationEventArgs(previousPageType, null, NavigationMode.Back, true));

            // navigation
            this.visualRoot.Child = previousPage;
            this.NativeShowPage(previousPage, NavigationMode.Back, this.modalIsDisplayed ? NavigationType.Modal : NavigationType.Default);
            this.modalIsDisplayed = false;
            this.CurrentPage = previousPage;

            // navigated to
            previousPage.InternalOnNavigatedTo(new NavigationEventArgs(previousPageType, null, NavigationMode.Back, true));
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