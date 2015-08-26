using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class AppercodePage
    {
        public event EventHandler Appeared = delegate { };

        protected internal UIScrollView ScrollView
        {
            get
            {
                if (this.ViewController != null)
                {
                    return this.ViewController.Scroll;
                }
                return null;
            }
        }

        protected internal AppercodePageViewController ViewController { get; set; }

        protected internal override void NativeInit()
        {
            if (this.ViewController == null)
            {
                this.ViewController = new AppercodePageViewController();
                WeakReference wr = new WeakReference(this);
                this.ViewController.Appeared += (sender, e) =>
                {
                    if (wr.IsAlive)
                    {
                        ((AppercodePage)wr.Target).Appeared(this, EventArgs.Empty);
                    }
                };

                ((AppercodeUIScrollView)this.ScrollView).ContentInsetsChanged += delegate(object sender, EventArgs e)
                {
                    if (wr.IsAlive && AppercodeVisualRoot.Instance.Child == wr.Target)
                    {
                        AppercodeVisualRoot.Instance.Arrange(AppercodeVisualRoot.Instance.CurrentRect);
                    }
                };

                this.ViewController.View = this.ScrollView;
                this.ViewController.View.BackgroundColor = UIColor.White;
                this.NativeUIElement = this.ViewController.View;
                this.UpdatePageTopBarCommands(this.TopAppBar);
            }
            if (this.Title != null)
            {
                this.ViewController.Title = this.Title;
            }
            base.NativeInit();
        }

        protected override void OnTitleChanged(string oldValue, string newValue)
        {
            if (this.ViewController != null)
            {
                this.ViewController.Title = newValue ?? "";
            }
        }

        protected override void NativeArrange(CGRect finalRect)
        {
            this.ScrollView.ContentSize = finalRect.Size;

            // That's right. We don't need to rearrange page;
            ////base.NativeArrange(finalRect);
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null)
            {
                if (this.Background != null)
                {
                    this.NativeUIElement.BackgroundColor = this.Background.ToUIColor(this.NativeUIElement.Frame.Size);
                }
                else
                {
                    this.NativeUIElement.BackgroundColor = UIColor.White;
                }
            }
        }

        protected override void SetTopItems(IEnumerable<ICommandBarElement> items)
        {
            base.SetTopItems(items);
            if (this.ViewController != null)
            {
                this.ViewController.NavigationItem.RightBarButtonItems =
                    (from item in items.OfType<AppBarButton>()
                     where item.Visibility == Visibility.Visible
                     select item.GetNativeItem()).ToArray();
            }
        }

        internal class AppercodeUIScrollView : UIScrollView
        {
            public event EventHandler ContentInsetsChanged = delegate { };

            public override UIEdgeInsets ContentInset
            {
                get
                {
                    return base.ContentInset;
                }
                set
                {
                    if (!base.ContentInset.Equals(value))
                    {
                        base.ContentInset = value;
                        this.ContentInsetsChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        protected internal class AppercodePageViewController : UIViewController
        {
            internal readonly AppercodeUIScrollView Scroll;
            private bool defaultInsetsAreSet;
            private UIEdgeInsets defaultContentInsets;
            private UIEdgeInsets defaultIndicatorsInsets;

            private NSObject keyboardObserverWillShow;
            private NSObject keyboardObserverWillHide;

            bool ignoreScroling = false;

            public AppercodePageViewController()
            {
                this.Scroll = new AppercodeUIScrollView();
                this.Scroll.ScrollEnabled = false;
                this.Scroll.Scrolled += HandleScrolled;
            }

            void HandleScrolled (object sender, EventArgs e)
            {
                if(!ignoreScroling)
                {
                    UIView activeView = this.KeyboardGetActiveView();
                    if (activeView != null)
                    {
                        activeView.ResignFirstResponder();
                    }
                }
            }

            public event EventHandler Appeared = delegate { };

            public override void ViewDidLoad()
            {
                base.ViewDidLoad();
            }

            public override void ViewWillAppear(bool animated)
            {
                base.ViewWillAppear(animated);
                this.RegisterForKeyboardNotifications();
            }

            public override void ViewWillDisappear(bool animated)
            {
                var firstResponder = FindFirstResponder(this.View);
                if (firstResponder != null)
                {
                    firstResponder.ResignFirstResponder();
                }

                this.UnregisterKeyboardNotifications();
                base.ViewWillDisappear(animated);
            }

            public override void ViewDidAppear(bool animated)
            {
                base.ViewDidAppear(animated);
                var inset = ((UIScrollView)this.View).ContentInset;
                this.Appeared(this, EventArgs.Empty);
            }

            #region KeybordMagic

            protected virtual void RegisterForKeyboardNotifications()
            {
                this.keyboardObserverWillShow = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, this.KeyboardWillShowNotification);
                this.keyboardObserverWillHide = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, this.KeyboardWillHideNotification);
            }

            protected virtual void UnregisterKeyboardNotifications()
            {
                if (this.keyboardObserverWillShow != null)
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(this.keyboardObserverWillShow);
                }
                if (this.keyboardObserverWillHide != null)
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(this.keyboardObserverWillHide);
                }
            }

            protected virtual UIView KeyboardGetActiveView()
            {
                return FindFirstResponder(this.View);
            }

            protected virtual void KeyboardWillShowNotification(NSNotification notification)
            {
                UIView activeView = this.KeyboardGetActiveView();

                if (this.Scroll == null)
                {
                    return;
                }

                this.Scroll.ScrollEnabled = true;

                var keyboardBounds = UIKeyboard.FrameEndFromNotification(notification);
                if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft | UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
                {
                    keyboardBounds = new CGRect(keyboardBounds.Y, keyboardBounds.X, keyboardBounds.Height, keyboardBounds.Width);
                }

                var scroll = this.GetScrollView();
                if (defaultInsetsAreSet == false)
                {
                    defaultInsetsAreSet = true;
                    this.defaultContentInsets = scroll.ContentInset;
                    this.defaultIndicatorsInsets = scroll.ScrollIndicatorInsets;
                }

                UIEdgeInsets contentInsets = new UIEdgeInsets(scroll.ContentInset.Top, 0.0f, keyboardBounds.Size.Height, 0.0f);
                scroll.ScrollIndicatorInsets = contentInsets;
                scroll.ContentInset = contentInsets;
                if (activeView != null)
                {
                    this.ignoreScroling = true;
                    UIView.Animate(0.3, () =>
                        {
                            scroll.ScrollRectToVisible(scroll.Subviews[0].ConvertRectFromView(activeView.Frame, activeView.Superview), false);
                        },
                        () => this.ignoreScroling = false);
                }
            }

            protected virtual void KeyboardWillHideNotification(NSNotification notification)
            {
                UIView activeView = this.KeyboardGetActiveView();
                if (this.Scroll != null)
                {
                    this.Scroll.ScrollEnabled = false;

                    // Reset the content inset of the ScrollView and animate using the current keyboard animation duration
                    var animationDuration = activeView == null ? 0 : UIKeyboard.AnimationDurationFromNotification(notification);
                    UIView.Animate(animationDuration, this.ResetScrollView);
                }
            }

            private static UIView FindFirstResponder(UIView view)
            {
                if (view.IsFirstResponder)
                {
                    return view;
                }
                foreach (UIView subView in view.Subviews)
                {
                    var firstResponder = FindFirstResponder(subView);
                    if (firstResponder != null)
                    {
                        return firstResponder;
                    }
                }
                return null;
            }

            private void ResetScrollView()
            {
                var scroll = this.GetScrollView();
                scroll.ScrollIndicatorInsets = this.defaultContentInsets;
                scroll.ContentInset = this.defaultIndicatorsInsets;
                defaultInsetsAreSet = false;
            }

            private UIScrollView GetScrollView()
            {
                return this.Scroll.Subviews.FirstOrDefault() as UIScrollView ?? this.Scroll;
            }

            #endregion
        }
    }
}