using System;

using Android.Content;
using Android.Views;
using Android.Widget;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    public class WrappedSeekBar : SeekBar, ITapableView, IJavaFinalizable, View.IOnClickListener
    {
        public WrappedSeekBar(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            this.SetOnClickListener(this);
        }

        public WrappedSeekBar(Context context)
            : base(context)
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