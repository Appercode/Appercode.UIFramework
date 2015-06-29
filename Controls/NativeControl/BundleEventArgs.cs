using Android.OS;
using System;

namespace Appercode.UI.Controls.NativeControl
{
    public class BundleEventArgs : EventArgs
    {
        public BundleEventArgs(Bundle bundle)
        {
            this.Bundle = bundle;
        }

        public Bundle Bundle { get; set; }
    }
}
