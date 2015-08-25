using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Appercode.UI.Controls.Navigation
{
    public partial class StackNavigationFrame : UINavigationController
    {
        private bool isGoingBack = false;

        private StackNavigationFrame(StackNavigationFrame parent, UIViewController rootViewController)
            : base(rootViewController)
        {
            this.styler = parent.styler;
            this.Initialize();
        }

        public bool CanCloseApp
        {
            get
            {
                return true;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (this.RespondsToSelector(new Selector("interactivePopGestureRecognizer")))
            {
                this.InteractivePopGestureRecognizer.ShouldBegin = (gr) =>
                {
                    if (this.CanGoBack)
                    {
                        this.GoBack();
                    }
                    return false;
                };
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.visualRoot.Arrange(this.ViewFrame());
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            this.visualRoot.Arrange(this.ViewFrame());
        }

        [Export("navigationBar:shouldPopItem:")]
        public bool ShouldPopItem(UINavigationBar navigationBar, UINavigationItem item)
        {
            if (this.isGoingBack && this.ViewControllers.Length < navigationBar.Items.Length)
            {
                this.isGoingBack = false;
                return true;
            }

            this.GoBack();
            return false;
        }

        private void NativeStackNavigationFrame()
        {
        }

        private void NativeShowPage(AppercodePage pageInstance, NavigationMode mode, NavigationType navigationType)
        {
            if (mode == NavigationMode.New || mode == NavigationMode.Forward)
            {
                if (navigationType == NavigationType.Modal)
                {
                    // the idea taken at https://github.com/HugeLawn-MiracleApps/Xamarin-Forms-Labs/commit/87a8d5a5c1b9628de222d1d509e46865d255db7b
                    var topViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                    while (topViewController.PresentedViewController != null)
                    {
                        topViewController = topViewController.PresentedViewController;
                    }

                    var modalNavigationFrame = new StackNavigationFrame(this, pageInstance.ViewController);
                    topViewController.PresentViewController(modalNavigationFrame, true, null);
                }
                else
                {
                    this.PushViewController(pageInstance.ViewController, true);
                }
            }
            else if (mode == NavigationMode.Back)
            {
                if (navigationType == NavigationType.Modal)
                {
                    this.DismissViewController(true, null);
                }
                else
                {
                    this.isGoingBack = true;
                    this.PopToViewController(pageInstance.ViewController, true);
                }
            }
        }

        private CGRect ViewFrame()
        {
            var navbarHeight = this.NavigationBarHidden ? 0f : this.NavigationBar.Frame.Height;
            var tabbarHeight = int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7 || this.TabBarController == null || this.TabBarController.TabBar.Hidden ? 0f : this.TabBarController.TabBar.Frame.Height;

            var statusBarHeight = default(nfloat);
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft | UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Width;
                return new CGRect(0, 0, this.View.Frame.Height, this.View.Frame.Width - navbarHeight - statusBarHeight - tabbarHeight);
            }
            statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;
            return new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height - navbarHeight - statusBarHeight - tabbarHeight);
        }

        public override UIViewController[] PopToRootViewController(bool animated)
        {
            List<UIViewController> vcs = new List<UIViewController>();
            for (int i = 1; i < this.ViewControllers.Length; i++)
            {
                vcs.Add(this.ViewControllers[i]);
            }
            if (vcs.Any())
            {
                while (this.backStack.Count > 1)
                {
                    this.backStack.Pop();
                }
                this.GoBack();
            }
            return vcs.ToArray();
        }

        private void NativeCloseApplication()
        {
            // not need in iOS
        }
    }
}