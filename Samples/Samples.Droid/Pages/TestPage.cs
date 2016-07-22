using Appercode.UI.Controls.Navigation;
using Appercode.Samples.Model;

namespace Appercode.Samples.Pages
{
    partial class TestPage
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                DataContext = new TestModel();
            }
        }
    }
}
