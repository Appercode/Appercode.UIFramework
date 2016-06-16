using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.Navigation.Primitives;
using Appercode.UI.Device;
using Java.Lang;
using Java.Lang.Reflect;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using static Android.Resource;

namespace Appercode.UI.Controls.Navigation
{
    [Activity(Label = "Appercode", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public partial class TabsNavigationFrame : Activity, ViewTreeObserver.IOnGlobalLayoutListener
    {
        protected int fragmentPageFrameLayoutResourceId = 1;
        private RootLayout rootLayout;
        private ViewPagerPage pagerPage;
        private FrameLayout frameLayout;

        public bool CanCloseApp
        {
            get
            {
                return true;
            }
        }

        public int PageWidth
        {
            get;
            set;
        }

        public int PageHeight
        {
            get;
            set;
        }

        public override void OnBackPressed()
        {
            this.OnBackKeyPress();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (this.pagerPage == null)
            {
                this.pagerPage = new ViewPagerPage() { FragmentManager = this.FragmentManager };
                this.visualRoot.Child = pagerPage;
            }

            // this will start Navigation to first tab
            this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            var styledAttributes = this.Theme.ObtainStyledAttributes(new int[] { Attribute.ActionBarSize, Attribute.ActionBarStyle });
            var mActionBarSize = (int)styledAttributes.GetDimension(0, 0);
            var actionbarStyleId = styledAttributes.GetResourceId(1, 0);
            styledAttributes.Recycle();

            styledAttributes = this.Theme.ObtainStyledAttributes(actionbarStyleId, new int[] { Attribute.BackgroundStacked });
            var tabsBackground = styledAttributes.GetDrawable(0);
            styledAttributes.Recycle();

            this.rootLayout = new RootLayout(this)
            {
                Background = tabsBackground
            };
            this.rootLayout.SetPadding(0, mActionBarSize, 0, 0);
            this.Window.DecorView.RootView.ViewTreeObserver.AddOnGlobalLayoutListener(this);

            frameLayout = new FrameLayout(this);
            frameLayout.Id = this.fragmentPageFrameLayoutResourceId;
            frameLayout.Visibility = ViewStates.Gone;
            this.rootLayout.SizeChanged += (s, e) => UpdateRootLayouts();
            this.pagerPage.ViewPager.LayoutChange += ViewPagerLayotChange;
            this.pagerPage.ViewPager.PageSelected += Pager_PageSelected;

            this.rootLayout.AddView(this.pagerPage.ViewPager, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            this.rootLayout.AddView(frameLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

            this.SetContentView(this.rootLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        private void ViewPagerLayotChange(object sender, View.LayoutChangeEventArgs e)
        {
            var styledAttributes = this.Theme.ObtainStyledAttributes(new int[] { Attribute.ActionBarSize, Attribute.ActionBarStyle });
            var mActionBarSize = (int)styledAttributes.GetDimension(0, 0);
            styledAttributes.Recycle();

            rootLayout.SetPadding(0, mActionBarSize, 0, 0);
            SetViewPagerMargin();
        }

        private void SetViewPagerMargin()
        {
            var lp = (ViewGroup.MarginLayoutParams)this.pagerPage.ViewPager.LayoutParameters;
            if (!this.GetIsTabsEmbedded())
            {
                lp.SetMargins(0, this.GetTabsView().Height, 0, 0);
            }
            else
            {
                lp.SetMargins(0, 0, 0, 0);
            }
            this.pagerPage.ViewPager.LayoutParameters = lp;
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }

        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var tab = e.NewItems[0] as TabBarTab;
                    var androidTab = ActionBar.NewTab();

                    if (!string.IsNullOrEmpty(tab.Title))
                    {
                        androidTab.SetText("  " + tab.Title);
                    }
                    if (tab.Icon != null)
                    {

                        if (tab.Icon.ImageLoadStatus == Media.ImageStatus.Loaded)
                        {
                            androidTab.SetIcon(new BitmapDrawable(tab.Icon.GetBitmap()));
                        }
                        else
                        {
                            tab.Icon.ImageOpened += (s, ea) => androidTab.SetIcon(new BitmapDrawable(tab.Icon.GetBitmap()));
                        }
                    }

                    androidTab.TabSelected += TabSelected;
                    this.ActionBar.AddTab(androidTab, e.NewStartingIndex);
                    if (this.pagerPage == null)
                    {
                        this.pagerPage = new ViewPagerPage { FragmentManager = this.FragmentManager };
                        this.visualRoot.Child = pagerPage;
                    }

                    var page = PageFactory.InstantiatePage(tab.PageType, ref this.isNavigationInProgress);
                    page.NavigationService = this.navigationService;
                    this.pagerPage.AddPage(page, tab.NavigationParameter);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void Pager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            this.ActionBar.SetSelectedNavigationItem(e.Position);
        }

        private void TabSelected(object sender, ActionBar.TabEventArgs e)
        {
            if (this.currentTabIndex != this.ActionBar.SelectedTab.Position)
                this.SelectTabAt(this.ActionBar.SelectedTab.Position);
        }

        private void NativeSelectTabAt(int index)
        {
            this.pagerPage.ViewPager.SetCurrentItem(index, true);
            this.CurrentPage = this.pagerPage.CurrentPage;
            //this.Navigate(this.Tabs[index].PageType, this.Tabs[index].NavigationParameter, true); // , adapter.Pages[index]);
        }

        private void NativeShowPage(AppercodePage page, NavigationMode mode, bool isTabSwitching)
        {
            this.Title = page.Title;

            this.visualRoot.Child = page;

            frameLayout.Visibility = ViewStates.Visible;
            var transaction = this.FragmentManager.BeginTransaction();
            transaction.SetCustomAnimations(Resource.Animation.fadein, Resource.Animation.fadeout, Resource.Animation.fadein, Resource.Animation.fadeout);
            transaction.Replace(this.fragmentPageFrameLayoutResourceId, page.NativeFragment);

            page.NativeFragment.Create += (s, e) =>
            {
                if (mode == NavigationMode.New && !isTabSwitching)
                {
                    ChangeActionBarNavigationMode(ActionBarNavigationMode.Standard);
                }
                else if (mode == NavigationMode.Back && this.backStack.Count == 0)
                {
                    ChangeActionBarNavigationMode(ActionBarNavigationMode.Tabs);
                }
                UpdateRootLayouts();
            };

            transaction.Commit();
        }

        private void NativeBackToTabs()
        {
            this.backStack.Pop();
            this.frameLayout.Visibility = ViewStates.Gone;

            this.ChangeActionBarNavigationMode(ActionBarNavigationMode.Tabs);

            var transaction = this.FragmentManager.BeginTransaction();
            transaction.SetCustomAnimations(Resource.Animation.fadein, Resource.Animation.fadeout, Resource.Animation.fadein, Resource.Animation.fadeout);
            transaction.Remove(this.CurrentPage.NativeFragment);

            transaction.Commit();

            ViewPagerLayotChange(this, new View.LayoutChangeEventArgs(0, 0, 0, 0, 0, 0, 0, 0));
            UpdateRootLayouts();
        }

        private bool GetIsTabsEmbedded()
        {
            Class actionBarClass = this.ActionBar.Class;
            Field mHasEmbeddedTabs = actionBarClass.GetDeclaredField("mHasEmbeddedTabs");
            mHasEmbeddedTabs.Accessible = true;
            return mHasEmbeddedTabs.GetBoolean(this.ActionBar);
        }

        private HorizontalScrollView GetTabsView()
        {
            var tabsContainer = this.ActionBar.Class.GetDeclaredField("mTabScrollView");
            tabsContainer.Accessible = true;
            var scroll = (HorizontalScrollView)tabsContainer.Get(this.ActionBar);
            return scroll;
        }

        private void ChangeActionBarNavigationMode(ActionBarNavigationMode navigationMode)
        {
            if (this.ActionBar.NavigationMode != navigationMode)
            {
                this.ActionBar.NavigationMode = navigationMode;
            }
        }

        private void UpdateRootLayouts()
        {
            GetTabsView().Post(RearrangeVisualRoot);
        }

        private void RearrangeVisualRoot()
        {
            var marginTop = !GetIsTabsEmbedded() 
                && this.ActionBar.NavigationMode == ActionBarNavigationMode.Tabs? GetTabsView().Height : 0;
            this.PageHeight = rootLayout.Height - rootLayout.PaddingTop - marginTop;
            this.PageWidth = rootLayout.Width;

            var dpiPageSize = new RectangleF(
                0.0f, 0.0f, ScreenProperties.ConvertPixelsToDPI(this.PageWidth), ScreenProperties.ConvertPixelsToDPI(this.PageHeight));
            this.pagerPage.Arrange(dpiPageSize);
            AppercodeVisualRoot.Instance.Arrange(dpiPageSize);
        }

        private void NativeSetBage(int index, string value)
        {
        }

        private void NativeCloseApplication()
        {
            this.Finish();
        }

        private void NativeTabsNavigationFrame()
        {
            UIElement.StaticContext = this;
        }


        private bool? keyboardShown = false;
        private int prevoiusDiff = 0;
        public void OnGlobalLayout()
        {
            var rootView = this.Window.DecorView.RootView.RootView;
            var rect = new Rect();
            var diff = rootView.Height - rootLayout.Height;
            if (diff > (keyboardShown == true ? 0 : ScreenProperties.ConvertDPIToPixels(100)) && diff != prevoiusDiff)
            {
                keyboardShown = keyboardShown == true ? !keyboardShown : true;
                prevoiusDiff = diff;
                UpdateRootLayouts();
            }
        }
    }
}