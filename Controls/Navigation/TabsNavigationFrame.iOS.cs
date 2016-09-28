using Appercode.UI.Controls.Navigation.Primitives;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;

namespace Appercode.UI.Controls.Navigation
{
    public partial class TabsNavigationFrame : UITabBarController
    {
        public bool CanCloseApp
        {
            get
            {
                return false;
            }
        }

        private void NativeTabsNavigationFrame()
        {
            this.ViewControllerSelected += this.TabSelected;
        }

        private void TabSelected(object sender, UITabBarSelectionEventArgs e)
        {
            this.SelectTabAt((int)this.SelectedIndex);
        }

        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var tab = e.NewItems[0] as TabBarTab;
                    tab.BadgeChanged += this.OnTabBadgeChanged;           // TODO: unsubscribe from the event for removed tabs
                    var stack = new StackNavigationFrame(this.styler);
                    this.navigationStacks.Add(stack);
                    if (tab.Icon != null)
                    {
                        stack.TabBarItem.Image = tab.Icon.GetUIImage();
                    }
                    stack.TabBarItem.Title = tab.Title;

                    this.AddChildViewController(stack);
                    stack.Navigate(tab.PageType, tab.NavigationParameter, NavigationType.Default);
                    if (this.currentTabIndex == -1)
                    {
                        this.currentTabIndex = 0;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            this.CurrentPage = this.navigationStacks[this.currentTabIndex].CurrentPage;
            this.visualRoot.Child = this.CurrentPage;
            this.visualRoot.Arrange();
        }

        private void OnTabBadgeChanged(object sender, EventArgs e)
        {
            var tab = sender as TabBarTab;
            if (tab != null)
            {
                var index = this.Tabs.IndexOf(tab);
                if (index >= 0)
                {
                    this.TabBar.Items[index].BadgeValue = tab.Badge;
                }
            }
        }

        private void NativeSelectTabAt(int index)
        {
            nint nIndex = index;
            if (this.SelectedIndex != nIndex)
            {
                this.SelectedIndex = nIndex;
            }

            this.CurrentPage = this.navigationStacks[this.currentTabIndex].CurrentPage;
            this.visualRoot.Child = this.CurrentPage;
            this.visualRoot.Arrange();
        }

        private void NativeSetBage(int index, string value)
        {
            this.TabBar.Items[index].BadgeValue = value;
        }

        private void NativeCloseApplication()
        {
        }
    }
}