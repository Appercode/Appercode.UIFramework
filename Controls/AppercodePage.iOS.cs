using Appercode.UI.Input;
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

        internal override void OnTap(GestureEventArgs args)
        {
            base.OnTap(args);
            this.ViewController.HideKeyboard();
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

            public event EventHandler Appeared = delegate { };

            public override void ViewWillAppear(bool animated)
            {
                base.ViewWillAppear(animated);
                this.RegisterForKeyboardNotifications();
            }

            public override void ViewWillDisappear(bool animated)
            {
                this.HideKeyboard();
                this.UnregisterKeyboardNotifications();
                base.ViewWillDisappear(animated);
            }

            public override void ViewDidAppear(bool animated)
            {
                base.ViewDidAppear(animated);
                var inset = ((UIScrollView)this.View).ContentInset;
                this.Appeared(this, EventArgs.Empty);
            }

            internal void HideKeyboard()
            {
                var firstResponder = FindFirstResponder(this.View);
                if (firstResponder != null)
                {
                    firstResponder.ResignFirstResponder();
                }
            }

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

            protected virtual void KeyboardWillShowNotification(NSNotification notification)
            {
                this.Scroll.ScrollEnabled = true;
                var scroll = this.GetScrollView();
                if (this.defaultInsetsAreSet == false)
                {
                    this.defaultInsetsAreSet = true;
                    this.defaultContentInsets = scroll.ContentInset;
                    this.defaultIndicatorsInsets = scroll.ScrollIndicatorInsets;
                }

                var keyboardBounds = UIKeyboard.FrameEndFromNotification(notification);
                var contentBottomInset =
                    UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? keyboardBounds.Width : keyboardBounds.Height;
                var contentInsets = new UIEdgeInsets(scroll.ContentInset.Top, default(nfloat), contentBottomInset, default(nfloat));
                scroll.ScrollIndicatorInsets = contentInsets;
                scroll.ContentInset = contentInsets;
                var firstResponder = FindFirstResponder(this.View);
                if (firstResponder != null)
                {
                    this.ignoreScroling = true;
                    UIView.Animate(
                        UIKeyboard.AnimationDurationFromNotification(notification),
                        () => scroll.ScrollRectToVisible(scroll.ConvertRectFromView(firstResponder.Frame, firstResponder.Superview), false),
                        () => this.ignoreScroling = false);
                }
            }

            protected virtual void KeyboardWillHideNotification(NSNotification notification)
            {
                this.Scroll.ScrollEnabled = false;
                var firstResponder = FindFirstResponder(this.View);

                // Reset the content inset of the ScrollView and animate using the current keyboard animation duration
                var animationDuration = firstResponder == null ? 0 : UIKeyboard.AnimationDurationFromNotification(notification);
                UIView.Animate(animationDuration, this.ResetScrollView);
            }

            private static UIView FindFirstResponder(UIView view)
            {
                if (view.IsFirstResponder)
                {
                    return view;
                }

                foreach (var subView in view.Subviews)
                {
                    var firstResponder = FindFirstResponder(subView);
                    if (firstResponder != null)
                    {
                        return firstResponder;
                    }
                }

                return null;
            }

            private void HandleScrolled(object sender, EventArgs e)
            {
                if (this.ignoreScroling == false)
                {
                    this.HideKeyboard();
                }
            }

            private void ResetScrollView()
            {
                var scroll = this.GetScrollView();
                scroll.ScrollIndicatorInsets = this.defaultContentInsets;
                scroll.ContentInset = this.defaultIndicatorsInsets;
                this.defaultInsetsAreSet = false;
            }

            private UIScrollView GetScrollView()
            {
                // UIScrollView contains two UIImageView subviews for scroll indicators: http://stackoverflow.com/a/5664311
                // They might be found earlier than the page subview and affect the returned result
                return this.Scroll.Subviews.OfType<UIScrollView>().FirstOrDefault() ?? this.Scroll;
            }
        }
    }
}