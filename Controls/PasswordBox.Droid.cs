using Android.Text;
using Appercode.UI.Controls.NativeControl;
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
            ((NativeEditText)this.NativeUIElement).SetSelection(start, start + length);
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeView = new NativeEditText(this.Context)
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
            if (((NativeEditText)this.NativeUIElement).Text != password)
            {
                ((NativeEditText)this.NativeUIElement).Text = password != null ? (string)password : string.Empty;
            }

            ((NativeEditText)this.NativeUIElement).SetSelection(this.Password.Length);
        }

        private void ApplyNativeMaxLength(int maxLength)
        {
            this.ApplyInputFilters(maxLength);
        }

        private void ApplyInputFilters(int maxLength)
        {
            IInputFilter[] filterArray;

            if (maxLength > 0)
            {

                filterArray = new IInputFilter[1];

                filterArray[0] = new InputFilterLengthFilter(maxLength);
            }
            else
            {
                filterArray = new IInputFilter[0];
            }

            ((NativeEditText)this.NativeUIElement).SetFilters(filterArray);
        }
    }
}