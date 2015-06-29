using Android.Content;
using Android.Views;
using Appercode.UI.Controls.NativeControl.Wrapers;

namespace Appercode.UI.Controls.NativeControl
{
    public class NativeContentControl : WrapedViewGroup
    {
        public NativeContentControl(Context context)
            : base(context)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
        }
    }
}