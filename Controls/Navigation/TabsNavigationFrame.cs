using Appercode.UI.Controls.Navigation.Primitives;
using Appercode.UI.StylesAndResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Appercode.UI.Controls.Navigation
{
    /// <summary>
    /// Class providing tab navigation in Appercode
    /// </summary>
    public partial class TabsNavigationFrame : IFrame
    {
        private readonly IFrameStyler styler;
        private AppercodeVisualRoot visualRoot;
        private AppercodePage currentPage;
        private int currentTabIndex = -1;
        private bool isNavigationInProgress = false;

#if __ANDROID__ || WINDOWS_PHONE
        private NavigationService navigationService;
        private Stack<AppercodePage> backStack;
#else
        private List<StackNavigationFrame> navigationStacks;
#endif

        /// <summary>
        /// Creates an instance of <see cref="TabsNavigationFrame" /> class.
        /// </summary>

        public TabsNavigationFrame()
            : this(null) { }

        /// <summary>
        /// Creates an instance of <see cref="TabsNavigationFrame" /> class using an implementation of <see cref="IFrameStyler" />.
        /// </summary>
        public TabsNavigationFrame(IFrameStyler styler)
        {
            this.styler = styler;
#if __ANDROID__ || WINDOWS_PHONE
            this.navigationService = new NavigationService(this);
            this.backStack = new Stack<AppercodePage>();
#else
            this.navigationStacks = new List<StackNavigationFrame>();
#endif
            this.visualRoot = AppercodeVisualRoot.Instance;
            this.NativeTabsNavigationFrame();

            this.Tabs = new TabBarTabsCollection();
            this.Tabs.CollectionChanged += Tabs_CollectionChanged;

            this.LoadApplicationResourcesFromAssembly(Assembly.GetCallingAssembly());
            if (this.styler != null)
            {
                this.styler.StyleTabBar(this);
            }
        }

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
        /// Returns an IEnumerable that can be used to enumerate the entries in back navigation history for a Frame.
        /// </summary>
        public IEnumerable<AppercodePage> BackStack
        {
            get
            {
#if __ANDROID__ || WINDOWS_PHONE
                // return a read-only copy of the backStack to avoid cast and modification by external code
                return this.backStack.AsReadonly();
#else
                return Enumerable.Empty<AppercodePage>();
#endif
            }
        }

        /// <summary>
        /// True if there are any pages in the current BackStack and you can <see cref="GoBack" />
        /// </summary>
        public bool CanGoBack
        {
            get
            {
#if __ANDROID__ || WINDOWS_PHONE
                return this.backStack.Count > 0;
#else
                return false;
#endif
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
            get { throw new NotImplementedException(); }
        }

        public System.Windows.Threading.Dispatcher Dispatcher
        {
            get { throw new NotImplementedException(); }
        }

        public PresentationFrameworkCollection<TabBarTab> Tabs
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

        /// <summary>
        /// Gets if navigation process is in progress.
        /// </summary>
        public bool IsNavigationInProgress => this.isNavigationInProgress;

        /// <summary>
        /// Navigate to page in cuttent stack
        /// </summary>
        /// <param name="sourcePageType">Type of <see cref="AppercodePage"/> to Navigate</param>
        /// <returns>was <see cref="AppercodePage"/> navigated to or it was canceled</returns>
        public bool Navigate(Type sourcePageType, NavigationType navigationType)
        {
            return this.Navigate(sourcePageType, null, navigationType);
        }

        /// <summary>
        /// Navigate to page in cuttent stack
        /// </summary>
        /// <param name="sourcePageType">Type of <see cref="AppercodePage"/> to Navigate</param>
        /// <param name="parameter">Parameter to put on <see cref="AppercodePage"/> when navigated to</param>
        /// <returns>was <see cref="AppercodePage"/> navigated to or it was canceled</returns>
        public bool Navigate(Type sourcePageType, object parameter, NavigationType navigationType)
        {
            return this.Navigate(sourcePageType, parameter, false, navigationType);
        }

        public void SelectTabAt(int index)
        {
            this.currentTabIndex = index;
            this.NativeSelectTabAt(index);
        }

        /// <summary>
        /// Sets the bage. Removes it if <paramref name="value"/> is null or empty
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="value">Value.</param>
        public void SetBage(int index, string value)
        {
            this.NativeSetBage(index, value);
        }

        private bool Navigate(Type sourcePageType, object parameter, bool isTabSwitching, NavigationType navigationType, AppercodePage tabPage = null)
        {
#if __ANDROID__ || WINDOWS_PHONE
            if (sourcePageType == null)
            {
                throw new ArgumentNullException("sourcePageType", "sourcePageType must not be null");
            }
            if (!sourcePageType.IsSubclassOf(typeof(AppercodePage)) && sourcePageType != typeof(AppercodePage))
            {
                throw new ArgumentException("sourcePageType must be an AppercodePage", "sourcePageType");
            }

            if (this.isNavigationInProgress)
            {
                return false;
            }

            this.isNavigationInProgress = true;

            // navigating from with check
            NavigatingCancelEventArgs navigatingCancelEventArgs = new NavigatingCancelEventArgs(sourcePageType, NavigationMode.New);

            if (this.CurrentPage != null)
            {
                this.CurrentPage.InternalOnNavigatingFrom(navigatingCancelEventArgs);
                if (navigatingCancelEventArgs.Cancel == true)
                {
                    this.isNavigationInProgress = false;
                    return false;
                }
            }
            AppercodePage pageInstance;

            // navigated from
            if (this.CurrentPage != null)
            {
                this.CurrentPage.InternalOnNavigatedFrom(new NavigationEventArgs(sourcePageType, parameter));
                this.backStack.Push(this.CurrentPage);
            }

            // Create page
            if (isTabSwitching && backStack.Any())
            {
                this.backStack.Clear();
            }

            if (isTabSwitching && tabPage != null)
            {
                pageInstance = tabPage;
            }
            else
            {
                pageInstance = PageFactory.InstantiatePage(sourcePageType, ref this.isNavigationInProgress);
            }

            pageInstance.NavigationService = this.navigationService;
            //this.visualRoot.Child = pageInstance;
            this.NativeShowPage(pageInstance, NavigationMode.New, isTabSwitching);

            // navigated to
            pageInstance.InternalOnNavigatedTo(new NavigationEventArgs(sourcePageType, parameter));

            this.CurrentPage = pageInstance;

            this.isNavigationInProgress = false;
            return true;
#else
            if (this.currentTabIndex < 0 || this.currentTabIndex >= this.navigationStacks.Count)
            {
                return false; 
            }
            return this.navigationStacks[this.currentTabIndex].Navigate(sourcePageType, parameter, navigationType);
#endif
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void OnBackKeyPress()
        {            
            this.CurrentPage.InternalOnBackKeyPress(new CancelEventArgs(false));
        }

        public void GoBack()
        { 
#if __ANDROID__ || WINDOWS_PHONE
         
            if (this.isNavigationInProgress)
            {
                return;
            }

            this.isNavigationInProgress = true;
            if (this.backStack.Count == 0)
            {
                this.CloseApplication();
                this.isNavigationInProgress = false;
                return;
            }

            var previousPage = this.backStack.Peek();
            var previousPageType = previousPage.GetType();

            // navigating from
            var navigatingCancelEventArgs = new NavigatingCancelEventArgs(previousPageType, NavigationMode.Back);
            this.CurrentPage.InternalOnNavigatingFrom(navigatingCancelEventArgs);
            if (navigatingCancelEventArgs.Cancel == true)
            {
                this.isNavigationInProgress = false;
                return;
            }

            if (backStack.Count == 1)
            {
                this.NativeBackToTabs();

                this.CurrentPage.InternalOnNavigatedFrom(new NavigationEventArgs(previousPageType, null, NavigationMode.Back, true));
                this.CurrentPage = previousPage;
                // navigated to
                previousPage.InternalOnNavigatedTo(new NavigationEventArgs(previousPageType, null, NavigationMode.Back, true));
                this.isNavigationInProgress = false;
                return;
            }

            // navigated from
            this.backStack.Pop();
            this.CurrentPage.InternalOnNavigatedFrom(new NavigationEventArgs(previousPageType, null, NavigationMode.Back, true));

            // navigation
            //this.visualRoot.Child = previosPage;
            this.NativeShowPage(previousPage, NavigationMode.Back, false);

            this.CurrentPage = previousPage;
            // navigated to
            previousPage.InternalOnNavigatedTo(new NavigationEventArgs(previousPageType, null, NavigationMode.Back, true));
            this.isNavigationInProgress = false;
#else
            if (this.currentTabIndex < 0 || this.currentTabIndex >= this.navigationStacks.Count)
            {
                return;
            }
            this.navigationStacks[this.currentTabIndex].GoBack();
#endif
        }
            

        public void RemoveBackEntry()
        {
            #if __ANDROID__ || WINDOWS_PHONE
            if (this.backStack.Count > 0)
            {
                this.backStack.Pop();
            }
            #else
            this.navigationStacks[this.currentTabIndex].RemoveBackEntry();
            #endif
        }

        /// <summary>
        /// Go forward to <see cref="AppercodePage"/> previously went back from
        /// </summary>
        /// <remarks>not used</remarks>
        public void GoForward()
        {
            throw new NotImplementedException();
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