using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Appercode.UI.Controls.Navigation
{
    [Activity(Label = "Appercode", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public partial class StackNavigationFrame : Activity
    {
        private const string DialogFragmentTag = "DialogFragment";
        private const int FragmentPageFrameLayoutResourceId = 1;

        private RectangleF nonModalPageRect;

        public StackNavigationFrame(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public Type StartPage { get; set; }

        public bool CanCloseApp
        {
            get
            {
                return true;
            }
        }

        public override void OnBackPressed()
        {
            this.CurrentPage.InternalOnBackKeyPress(new CancelEventArgs());
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var rootLayout = new RootLayout(this);
            var frgmCont = new FrameLayout(this) { Id = FragmentPageFrameLayoutResourceId };
            rootLayout.AddView(frgmCont, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            this.SetContentView(rootLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            if (this.StartPage != null)
            {
                this.Navigate(this.StartPage, null, NavigationType.Default);
            }
        }

        private void NativeShowPage(AppercodePage page, NavigationMode mode, NavigationType navigationType)
        {
            if (modalIsDisplayed && mode != NavigationMode.Back)
            {
                throw new NotSupportedException("Modal page should be dismissed before any other navigation.");
            }

            if (navigationType == NavigationType.Modal)
            {
                if (mode == NavigationMode.Back)
                {
                    var fragment = this.FragmentManager.FindFragmentByTag<DialogFragment>(DialogFragmentTag);
                    fragment.Dismiss();
                    AppercodeVisualRoot.Instance.Arrange(nonModalPageRect);
                }
                else
                {
                    this.nonModalPageRect = AppercodeVisualRoot.Instance.CurrentRect;
                    var fragment = (DialogFragment)page.NativeFragment;
                    fragment.Show(this.FragmentManager, DialogFragmentTag);
                }
            }
            else
            {
                this.Title = page.Title;
                var transaction = this.FragmentManager.BeginTransaction();
                transaction.Replace(FragmentPageFrameLayoutResourceId, page.NativeFragment);
                transaction.SetTransition(
                    mode == NavigationMode.Back ? FragmentTransit.FragmentClose : FragmentTransit.FragmentOpen);
                transaction.Commit();
            }
        }

        private void NativeCloseApplication()
        {
            this.Finish();
        }

        private void NativeStackNavigationFrame()
        {
            UIElement.StaticContext = this;
        }

        private class RootLayout : RelativeLayout
        {
            private readonly StackNavigationFrame frame;

            public RootLayout(StackNavigationFrame frame)
                : base(frame)
            {
                this.frame = frame;
            }

            protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
            {
                base.OnSizeChanged(w, h, oldw, oldh);
                if (h != 0 && w != 0 && (oldh != h || oldw != w))
                {
                    this.frame.CurrentPage.ApplyPageSize(w, h);
                }
            }
        }
    }
}