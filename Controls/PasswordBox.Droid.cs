using Android.Text;
using Android.Widget;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Controls.NativeControl.Wrappers;
using System;
using System.Linq;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public partial class PasswordBox
    {
        protected int NativeMaxLength
        {
            get
            {
                return this.MaxLength;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeMaxLength(value);
                }
            }
        }

        protected string NativePassword
        {
            get
            {
                return this.Password;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativePassword(value);
                }
            }
        }

        public void NativeSelect(int start, int length)
        {
            ((EditText)this.NativeUIElement).SetSelection(start, start + length);
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeView = new WrappedEditText(this)
                    {
                        InputType = InputTypes.TextVariationPassword | InputTypes.ClassText
                    };
                    this.NativeUIElement = nativeView;
                    nativeView.TextChanged += this.NativePasswordChange;
                }

                this.ApplyNativePassword(this.NativePassword);
                this.ApplyNativeMaxLength(this.NativeMaxLength);
                this.SetNativeBackground(this.Background);
                base.NativeInit();
            }
        }

        protected override void OnBackgroundChanged(Brush oldValue, Brush newValue)
        {
            this.SetNativeBackground(newValue);
        }

        private void NativePasswordChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            this.Password = new string(e.Text.ToArray());
            this.PasswordChanged?.Invoke(this, new RoutedEventArgs());
        }

        private void ApplyNativePassword(string password)
        {
            var nativeView = (EditText)this.NativeUIElement;
            if (nativeView.Text != password)
            {
                nativeView.Text = password ?? string.Empty;
            }

            if (string.IsNullOrEmpty(password) == false)
            {
                nativeView.SetSelection(password.Length);
            }
        }

        private void ApplyNativeMaxLength(int maxLength)
        {
            this.ApplyInputFilters(maxLength);
        }

        private void ApplyInputFilters(int maxLength)
        {
            var filterArray =  maxLength > 0
                ? new[] { new InputFilterLengthFilter(maxLength) }
                : Array.Empty<IInputFilter>();
            ((TextView)this.NativeUIElement).SetFilters(filterArray);
        }
    }
}