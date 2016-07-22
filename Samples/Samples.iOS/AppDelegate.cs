using Appercode.Samples.Pages;
using Appercode.UI.Controls.Navigation;
using Foundation;
using UIKit;

namespace Appercode.Samples
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        private UIWindow window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            var frame = new StackNavigationFrame();
            window.RootViewController = frame;
            frame.Navigate(typeof(TestPage), NavigationType.Default);

            window.MakeKeyAndVisible();
            return true;
        }
    }
}