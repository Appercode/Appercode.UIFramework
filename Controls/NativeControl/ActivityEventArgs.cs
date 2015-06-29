using Android.App;
using System;

namespace Appercode.UI.Controls.NativeControl
{
    public class ActivityEventArgs : EventArgs
    {
        public ActivityEventArgs(Activity activity)
        {
            this.Activity = activity;
        }

        public Activity Activity { get; set; }
    }
}
