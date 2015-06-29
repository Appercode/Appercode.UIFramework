using Appercode.UI.Controls.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Appercode.UI.Controls
{
    public class Page : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Page), new PropertyMetadata(null, (d, e) =>
            {
                ((Page)d).OnTitleChanged((string)e.OldValue, (string)e.NewValue);
            }));

        public static readonly DependencyProperty TopAppBarProperty =
            DependencyProperty.Register("TopAppBar", typeof(CommandBar), typeof(Page), new PropertyMetadata(OnTopAppBarChanged));

        internal Page()
        {
        }

        public NavigationService NavigationService
        {
            get;
            internal set;
        }

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public CommandBar TopAppBar
        {
            get { return (CommandBar)GetValue(TopAppBarProperty); }
            set { this.SetValue(TopAppBarProperty, value); }
        }

        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this.TopAppBar != null)
                {
                    yield return this.TopAppBar;
                }

                var baseIterator = base.LogicalChildren;
                while (baseIterator.MoveNext())
                {
                    yield return baseIterator.Current;
                }
            }
        }

        internal void InternalOnNavigatedTo(NavigationEventArgs e)
        {
            this.OnNavigatedTo(e);
        }

        internal void InternalOnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.OnNavigatingFrom(e);
        }

        internal void InternalOnNavigatedFrom(NavigationEventArgs e)
        {
            this.OnNavigatedFrom(e);
        }

        internal void InternalOnBackKeyPress(CancelEventArgs e)
        {
            this.OnBackKeyPress(e);
        }

        protected virtual void OnBackKeyPress(CancelEventArgs e)
        {
            if (e.Cancel == false)
            {
                this.NavigationService.GoBack();
            }
        }

        protected virtual void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        protected virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

        protected virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        protected virtual void OnTitleChanged(string oldValue, string newValue)
        {
        }

        protected virtual void SetTopItems(IEnumerable<ICommandBarElement> items)
        {
        }

        protected void UpdatePageTopBarCommands(CommandBar commandBar)
        {
            if (commandBar != null)
            {
                this.SetTopItems(commandBar.PrimaryCommands);
            }
        }

        private static void OnTopAppBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as Page;
            if (page != null)
            {
                var newValue = e.NewValue as CommandBar;
                var oldValue = e.OldValue as CommandBar;
                if (oldValue != null)
                {
                    oldValue.BarUpdated -= page.TopBarUpdated;
                    page.RemoveLogicalChild(oldValue);
                    if (newValue == null)
                    {
                        page.SetTopItems(Enumerable.Empty<ICommandBarElement>());
                    }
                }

                if (newValue != null)
                {
                    newValue.BarUpdated += page.TopBarUpdated;
                    page.AddLogicalChild(newValue);
                    page.UpdatePageTopBarCommands(newValue);
                }
            }
        }

        private void TopBarUpdated(object sender, EventArgs e)
        {
            this.UpdatePageTopBarCommands(this.TopAppBar);
        }
    }
}