using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    public class WrapedProgressBar : Android.Widget.ProgressBar, ITapableView, IJavaFinalizable, View.IOnClickListener
    {
        public WrapedProgressBar(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            this.SetOnClickListener(this);
        }

        public WrapedProgressBar(Context context, Android.Util.IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            this.SetOnClickListener(this);
        }

        public event EventHandler NativeTap;
        public event EventHandler JavaFinalized;

        public void WrapedNativeRaiseTap()
        {
            if (this.NativeTap != null)
            {
                this.NativeTap(this, null);
            }
        }

        protected override void JavaFinalize()
        {
            if (this.JavaFinalized != null)
            {
                this.JavaFinalized(null, null);
            }
            base.JavaFinalize();
        }

        public void OnClick(View v)
        {
            WrapedNativeRaiseTap();
        }
    }
}