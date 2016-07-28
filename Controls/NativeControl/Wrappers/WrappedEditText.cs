using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Appercode.UI.Controls.NativeControl.Wrappers
{
    internal class WrappedEditText : EditText, View.IOnClickListener
    {
        private readonly UIElement owner;

        public WrappedEditText(UIElement owner)
            : base(owner.Context)
        {
            this.owner = owner;
            this.SetOnClickListener(this);
        }

        public event RoutedEventHandler NativeSelectionChanged;

        public void OnClick(View v)
        {
            this.owner.OnTap();
        }

        protected override void OnSelectionChanged(int selStart, int selEnd)
        {
            if (this.Text.Length <= selStart)
            {
                base.OnSelectionChanged(selStart, selEnd);
                this.NativeSelectionChanged?.Invoke(this, new RoutedEventArgs());
            }
        }

        protected override void OnTextChanged(ICharSequence text, int start, int before, int after)
        {
            if (this.Text == text.ToString())
            {
                base.OnTextChanged(text, start, before, after);
            }
        }

        protected override void JavaFinalize()
        {
            this.owner.FreeNativeView(this);
            base.JavaFinalize();
        }
    }
}