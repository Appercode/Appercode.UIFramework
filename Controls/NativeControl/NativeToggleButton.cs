using Appercode.UI.Controls.NativeControl.Wrappers;

namespace Appercode.UI.Controls.NativeControl
{
    internal class NativeToggleButton : WrappedViewGroup
    {
        private static readonly int[] CheckedStateSet =
        {
            Android.Resource.Attribute.StateChecked
        };

        public NativeToggleButton(UIElement owner)
            : base(owner)
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