using Android.Content;

namespace Appercode.UI.Controls.NativeControl
{
    internal class NativeToggleButton : Wrapers.WrapedViewGroup
    {
        private static readonly int[] CheckedStateSet =
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
                MergeDrawableStates(drawableState, CheckedStateSet);
            }

            return drawableState;
        }
    }
}