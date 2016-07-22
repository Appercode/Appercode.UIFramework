using Android.App;
using Android.OS;
using Android.Views;
using Appercode.Samples.Pages;
using Appercode.UI.Controls.Navigation;

namespace Appercode.Samples
{
    [Activity(Label = "Appercode Samples", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : StackNavigationFrame
    {
        protected override void OnCreate(Bundle bundle)
        {
            Window.SetSoftInputMode(SoftInput.StateHidden);
            this.StartPage = typeof(TestPage);
            base.OnCreate(bundle);
        }
    }
}

