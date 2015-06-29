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
    public class WrapedImageView : ImageView, ITapableView, IJavaFinalizable, View.IOnClickListener
    {
        public WrapedImageView(Context context)
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