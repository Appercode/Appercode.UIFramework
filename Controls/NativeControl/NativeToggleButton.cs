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

namespace Appercode.UI.Controls.NativeControl
{
    public class NativeToggleButton : NativeContentControl
    {
        private static int[] checkedStateSet =
        {
            Android.Resource.Attribute.StateChecked
        };

        public NativeToggleButton(Context context)
            : base(context)
        {
        }

        public bool IsChecked
        {
            get;
            set;
        }

        protected override int[] OnCreateDrawableState(int extraSpace)
        {
            var drawableState = base.OnCreateDrawableState(extraSpace + 1);
            if (this.IsChecked)
            {
                NativeToggleButton.MergeDrawableStates(drawableState, checkedStateSet);
            }
            return drawableState;
        }
    }
}