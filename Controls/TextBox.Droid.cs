using Android.Content;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Views.InputMethods;
using Appercode.UI.Controls.Input;
using Appercode.UI.Controls.NativeControl;
using Appercode.UI.Controls.NativeControl.Wrapers;
using System;
using System.Linq;
using System.Windows.Media;

namespace Appercode.UI.Controls
{
    public class EditTextReadOnlyInputFilter : Java.Lang.Object, IInputFilter
    {
        public string LockedText { get; set; }

        public Java.Lang.ICharSequence FilterFormatted(Java.Lang.ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            return source.SubSequenceFormatted(0, 0);
        }
    }

    public class NativeEditText : WrapedEditText
    {
        public NativeEditText(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        public NativeEditText(Context context)
            : base(context)
        {
        }

        public event RoutedEventHandler NativeSelectionChanged;

        public void PublicSetSelection(int start, int stop)
        {
            base.SetSelection(start, stop);
        }

        public override void SetSelection(int start, int stop)
        {
            base.SetSelection(start, stop);
        }

        protected override void OnSelectionChanged(int selStart, int selEnd)
        {
            if (this.Text.Length <= selStart)
            {
                base.OnSelectionChanged(selStart, selEnd);
                if (this.NativeSelectionChanged != null)
                {
                    this.NativeSelectionChanged(this, new RoutedEventArgs());
                }
            }
        }

        protected override void OnTextChanged(Java.Lang.ICharSequence text, int start, int before, int after)
        {
            if (this.Text != text.ToString())
            {
            }
            else
            {
                base.OnTextChanged(text, start, before, after);
            }
        }
    }

    public partial class TextBox
    {
        private string nativeText;
        private int maxLength = 0;
        private TextWrapping nativeTextWrapping;
        private bool nativeAcceptsReturn = true;
        private TextAlignment nativeTextAlignment;
        private int nativeSelectionStart = 0;
        private int nativeSelectionLength = 0;
        private bool nativeIsReadOnly = false;
        private EditTextReadOnlyInputFilter readOnlyFilter = new EditTextReadOnlyInputFilter();

        public int NativeSelectionStart
        {
            get
            {
                return this.nativeSelectionStart;
            }
            set
            {
                this.nativeSelectionStart = value;
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeSelectionStart(value);
                }
            }
        }
        public int NativeSelectionLength
        {
            get
            {
                return this.nativeSelectionLength;
            }
            set
            {
                this.nativeSelectionLength = value;
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeSelectionLength(value);
                }
            }
        }

        internal InputScope NativeInputScope
        {
            get
            {
                return this.InputScope;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeInputScope(value);
                }
            }
        }

        public bool NativeIsReadOnly
        {
            get
            {
                return this.nativeIsReadOnly;
            }
            set
            {
                this.nativeIsReadOnly = value;
                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeIsReadOnly(value);
                }
            }
        }

        protected string NativeText
        {
            get
            {
                return this.nativeText;
            }
            set
            {
                this.nativeText = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeText(value);
                }
            }
        }

        protected int NativeMaxLength
        {
            get
            {
                return this.maxLength;
            }
            set
            {
                this.maxLength = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeMaxLength(value);
                }
            }
        }

        protected TextWrapping NativeTextWrapping
        {
            get
            {
                return this.nativeTextWrapping;
            }
            set
            {
                this.nativeTextWrapping = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeTextWrapping(value);
                }
            }
        }

        protected bool NativeAcceptsReturn
        {
            get
            {
                return this.nativeAcceptsReturn;
            }
            set
            {
                this.nativeAcceptsReturn = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeAcceptsReturn(value);
                }
            }
        }

        protected TextAlignment NativeTextAlignment
        {
            get
            {
                return this.nativeTextAlignment;
            }
            set
            {
                this.nativeTextAlignment = value;

                if (this.NativeUIElement != null)
                {
                    this.ApplyNativeTextAlignment(value);
                }
            }
        }



        public void NativeSelect(int start, int length)
        {
            ((NativeEditText)this.NativeUIElement).RequestFocus();
            ((NativeEditText)this.NativeUIElement).SetSelection(start, start + length);
        }

        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var nativeView = new NativeEditText(this.Context);
                    this.NativeUIElement = nativeView;
                    nativeView.NativeSelectionChanged += this.TextBoxNativeSelectionChanged;
                    nativeView.TextChanged += this.TextBoxTextChanged;
                    this.SetNativeBackground(this.Background);
                }

                this.ApplyNativeText(this.Text);
                this.ApplyNativeMaxLength(this.MaxLength);
                this.ApplyNativeSelectionLength(this.SelectionLength);
                this.ApplyNativeSelectionStart(this.SelectionStart);
                this.ApplyNativeTextAlignment(this.TextAlignment);
                this.ApplyNativeTextWrapping(this.TextWrapping);
                this.ApplyNativeAcceptsReturn(this.AcceptsReturn);
                this.ApplyNativeIsReadOnly(this.IsReadOnly);
                this.ApplyNativeInputScope(this.InputScope);
            }

            base.NativeInit();
        }

        private void TextBoxTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            this.Text = new string(e.Text.ToArray());
        }

        protected override void OnBackgroundChanged(Brush oldValue, Brush newValue)
        {
            this.SetNativeBackground(newValue);
        }

        private void ApplyNativeInputScope(InputScope inputScope)
        {
            // TODO: check the input type correctly switches, for example, from Text to Number and then back to Text
            var editText = (NativeEditText)this.NativeUIElement;
            var inputTypes = editText.InputType;
            switch (inputScope)
            {
                case InputScope.Numbers:
                    editText.InputType = inputTypes | InputTypes.ClassNumber | InputTypes.NumberFlagDecimal;
                    break;
                case InputScope.Search:
                    editText.InputType = inputTypes | InputTypes.ClassText;
                    editText.ImeOptions = ImeAction.Search;
                    break;
                case Input.InputScope.EmailAddress:
                    editText.InputType = inputTypes | InputTypes.ClassText | InputTypes.TextVariationEmailAddress;
                    break;
                default:
                    editText.InputType = inputTypes | InputTypes.ClassText;
                    break;
            }
        }

        private void TextBoxNativeSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!this.nativeAcceptsReturn)
            {
                this.Text.Replace("\n", " ");
            }

            this.nativeSelectionStart = ((NativeEditText)this.NativeUIElement).SelectionStart;
            this.NativeSelectionLength = ((NativeEditText)this.NativeUIElement).SelectionEnd - ((NativeEditText)this.NativeUIElement).SelectionStart;
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, new RoutedEventArgs());
            }
        }

        private void ApplyNativeText(string value)
        {
            if (((NativeEditText)this.NativeUIElement).Text != value)
            {
                ((NativeEditText)this.NativeUIElement).Text = value != null ? (string)value : string.Empty;
            }
        }

        private void ApplyNativeMaxLength(int value)
        {
            this.ApplyInputFilters(this.IsReadOnly, value);
        }

        private void ApplyNativeTextWrapping(TextWrapping value)
        {
            if (value == TextWrapping.Wrap)
            {
                ((NativeEditText)this.NativeUIElement).TransformationMethod = null;
                ((NativeEditText)this.NativeUIElement).SetHorizontallyScrolling(false);
            }
            else
            {
                ((NativeEditText)this.NativeUIElement).TransformationMethod = SingleLineTransformationMethod.Instance;
                ((NativeEditText)this.NativeUIElement).SetHorizontallyScrolling(true);
            }
        }

        private void ApplyNativeTextAlignment(TextAlignment value)
        {
            switch (value)
            {
                case Controls.TextAlignment.Center:
                    {
                        ((NativeEditText)this.NativeUIElement).Gravity = GravityFlags.Center;
                        break;
                    }
                case Controls.TextAlignment.Left:
                    {
                        ((NativeEditText)this.NativeUIElement).Gravity = GravityFlags.Left;
                        break;
                    }
                case Controls.TextAlignment.Right:
                    {
                        ((NativeEditText)this.NativeUIElement).Gravity = GravityFlags.Right;
                        break;
                    }
                default:
                    {
                        ((NativeEditText)this.NativeUIElement).Gravity = GravityFlags.Left;
                        break;
                    }
            }
        }

        private void ApplyNativeAcceptsReturn(bool value)
        {
            if (!value)
            {
                ((NativeEditText)this.NativeUIElement).SetSingleLine(true);
            }
            else
            {
                ((NativeEditText)this.NativeUIElement).SetSingleLine(false);
            }
        }

        private void ApplyNativeSelectionStart(int value)
        {
            this.Select(value, this.nativeSelectionLength);
        }

        private void ApplyNativeSelectionLength(int value)
        {
            this.Select(this.nativeSelectionStart, value);
        }

        private void ApplyNativeIsReadOnly(bool value)
        {
            this.ApplyInputFilters(value, this.MaxLength);
        }

        private void ApplyInputFilters(bool isReadOnly, int maxLength)
        {
            IInputFilter[] filterArray;
            if (maxLength > 0)
            {
                if (isReadOnly)
                {
                    filterArray = new IInputFilter[2];
                    filterArray[1] = this.readOnlyFilter;
                }
                else
                {
                    filterArray = new IInputFilter[1];
                }
                filterArray[0] = new InputFilterLengthFilter(maxLength);
            }
            else
            {
                if (isReadOnly)
                {
                    filterArray = new IInputFilter[1];
                    filterArray[0] = this.readOnlyFilter;

                    // It is unclear, what to with the background in readonly mode
                    // Android.Graphics.Drawables.StateListDrawable draw = (Android.Graphics.Drawables.StateListDrawable)Resources.System.GetDrawable(Android.Resource.Drawable.EditBoxBackground);
                    // draw.SetState(new int[1] { Android.Resource.Attribute.DisabledAlpha });
                    // Android.Graphics.Drawables.Drawable draw2 = draw.Current;
                    // ((NativeEditText)this.NativeUIElement).SetBackgroundDrawable(draw2);
                }
                else
                {
                    filterArray = new IInputFilter[0];

                    // It is unclear, what to with the background in readonly mode
                    // ((NativeEditText)this.NativeUIElement).SetBackgroundResource(Android.Resource.Drawable.EditBoxBackground);
                }
            }
            ((NativeEditText)this.NativeUIElement).SetFilters(filterArray);
        }
    }
}