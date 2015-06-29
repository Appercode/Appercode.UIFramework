using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using Android.App;

namespace Appercode.UI.Controls
{
    public class NativeAppercodeFragment : Fragment
    {
        public NativeAppercodeFragment() : base()
        {
        }

        public NativeAppercodeFragment(View nativeViewFromAppercodePage)
        {
            this.NativeViewFromAppercodePage = nativeViewFromAppercodePage;
        }

        public NativeAppercodeFragment(IntPtr javaReference, JniHandleOwnership transfer):base(javaReference, transfer)
        {
        }

        public event EventHandler<BundleEventArgs> CreateView = delegate { };
        public event EventHandler<BundleEventArgs> Create = delegate { };
        public event EventHandler<ActivityEventArgs> ActivityCreated = delegate { };
        public event EventHandler DestroyView = delegate { };
        public event EventHandler Resume = delegate { };
        public event EventHandler Pause = delegate { };
        public event EventHandler LowMemory = delegate { };

        internal View NativeViewFromAppercodePage { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.CreateView(this, new BundleEventArgs(savedInstanceState));
            return this.NativeViewFromAppercodePage;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            this.Create(this, new BundleEventArgs(savedInstanceState));
            base.OnCreate(savedInstanceState);
        }

        public override void OnActivityCreated(Bundle bundle)
        {
            base.OnActivityCreated(bundle);
            this.ActivityCreated(this, new ActivityEventArgs(this.Activity));
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            //((ViewGroup)this.View).RemoveAllViews();
            this.DestroyView(this, EventArgs.Empty);
        }

        public override void OnResume()
        {
            base.OnResume();
            this.Resume(this, EventArgs.Empty);
        }

        public override void OnPause()
        {
            base.OnPause();
            this.Pause(this, EventArgs.Empty);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            this.LowMemory(this, EventArgs.Empty);
        }
    }
}