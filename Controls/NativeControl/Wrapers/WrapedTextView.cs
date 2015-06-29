using System.Diagnostics;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Windows.Input;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    public class WrapedTextView : TextView, ITapableView, IJavaFinalizable, View.IOnClickListener
    {
        public WrapedTextView(Context context)
            : base(context)
        {
            this.SetIncludeFontPadding(false);
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
        public override void Invalidate()
        {
            var sw = new Stopwatch();
            sw.Start();
            base.Invalidate();
            sw.Stop();
            TimeToInvalidate += sw.ElapsedMilliseconds;
            TimesInvalidate++;
        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            var sw = new Stopwatch();
            sw.Start();
            base.OnDraw(canvas);
            sw.Stop();
            TimeToLayoutRequests += sw.ElapsedMilliseconds;
            LayoutRequested++;
        }

        public static long TimeToInvalidate = 0;
        public static long TimeToLayoutRequests = 0;
        public static long TimesInvalidate = 0;
        public static long LayoutRequested = 0;
    }
}