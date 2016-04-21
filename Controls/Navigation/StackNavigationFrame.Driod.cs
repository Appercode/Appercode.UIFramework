using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Device;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Appercode.UI.Controls.Navigation
{
    public class RootLayout : RelativeLayout
    {
        public RootLayout(Context context)
            : base(context)
        {
        }

        public event EventHandler SizeChanged;

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            if (h != 0 && w != 0 && oldh != h && oldw != w)
            {
                this.SizeChanged?.Invoke(this, null);
            }
        }
    }

    [Activity(Label = "Appercode", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public partial class StackNavigationFrame : Activity
    {
        protected int fragmentPageFrameLayoutResourceId = 1;

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
            this.CurrentPage.InternalOnBackKeyPress(new CancelEventArgs());
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RootLayout rootLayout = new RootLayout(this);

            FrameLayout frgmCont = new FrameLayout(this);
            frgmCont.Id = this.fragmentPageFrameLayoutResourceId;
            rootLayout.AddView(frgmCont, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            rootLayout.SizeChanged += (s, e) =>
            {
                this.PageHeight = rootLayout.Height;
                this.PageWidth = rootLayout.Width;

                var dpiPageSize = new RectangleF(
                    0.0f, 0.0f, ScreenProperties.ConvertPixelsToDPI(this.PageWidth), ScreenProperties.ConvertPixelsToDPI(this.PageHeight));
                AppercodeVisualRoot.Instance.Arrange(dpiPageSize);
            };

            this.SetContentView(rootLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            this.Navigate(this.StartPage, null, NavigationType.Default);
        }

        private void NativeShowPage(AppercodePage page, NavigationMode mode, NavigationType navigationType)
        {
            this.Title = page.Title;
            var transaction = this.FragmentManager.BeginTransaction();
            transaction.Replace(this.fragmentPageFrameLayoutResourceId, page.NativeFragment);
            transaction.Commit();
        }

        private void NativeCloseApplication()
        {
            this.Finish();
        }

        private void NativeStackNavigationFrame()
        {
            UIElement.StaticContext = this;
        }
    }
}