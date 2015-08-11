using Appercode.UI.Controls.Input;
using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class TextBox
    {
        private static double defaultHeight = 30;
        private static nfloat textViewTop = 4;

        private string nativeText = string.Empty;
        private int nativeMaxLength;
        private TextWrapping nativeTextWrapping;
        private bool nativeAcceptsReturn = false;
        private TextAlignment nativeTextAlignment;
        private nint nativeSelectionLength;
        private bool nativeIsReadOnly;
        private nint nativeSelectionStart;
        private UITextView textView;
        private UITextField textField;

        private string NativeText
        {
            get
            {
                return this.nativeText;
            }
            set
            {
                this.nativeText = value??string.Empty;
                if (this.NativeUIElement != null)
                {
                    if (this.TextWrapping == Controls.TextWrapping.Wrap || this.AcceptsReturn)
                    {
                        this.textView.Text = this.nativeText;
                        this.ChangeSizeToContent();
                    }
                    else
                    {
                        this.textField.Text = this.nativeText;
                        this.ChangeSizeToContent();
                    }
                }
            }
        }

        private int NativeMaxLength
        {
            get { return this.nativeMaxLength; }
            set { this.nativeMaxLength = value; }
        }

        private TextWrapping NativeTextWrapping
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
                    this.ChangeVisualControl();
                }
            }
        }

        private bool NativeAcceptsReturn
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
                    this.ChangeVisualControl();
                }
            }
        }

        private TextAlignment NativeTextAlignment
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
                    switch (value)
                    {
                        case TextAlignment.Center:
                            this.textField.TextAlignment = UITextAlignment.Center;
                            this.textView.TextAlignment = UITextAlignment.Center;
                            break;
                        case TextAlignment.Right:
                            this.textField.TextAlignment = UITextAlignment.Right;
                            this.textView.TextAlignment = UITextAlignment.Right;
                            break;
                        case TextAlignment.Left:
                        default:
                            this.textField.TextAlignment = UITextAlignment.Left;
                            this.textView.TextAlignment = UITextAlignment.Left;
                            break;
                    }
                }
            }
        }

        private nint NativeSelectionLength
        {
            get
            {
                return this.nativeSelectionLength;
            }
            set
            {
                this.nativeSelectionLength = value;
                this.Select(this.SelectionStart, (int)value);
            }
        }

        private nint NativeSelectionStart
        {
            get
            {
                return this.nativeSelectionStart;
            }
            set
            {
                this.nativeSelectionStart = value;
                this.Select((int)value, this.SelectionLength);
            }
        }

        private InputScope NativeInputScope
        {
            get
            {
                return this.InputScope;
            }
            set
            {
                if (this.NativeUIElement != null)
                {
                    var keyBoardType = new UIKeyboardType();
                    switch (value)
                    {
                        case InputScope.Numbers:
                            keyBoardType = UIKeyboardType.NumberPad;
                            break;
                        case InputScope.Search:
                            keyBoardType = UIKeyboardType.Default;
                            this.textView.ReturnKeyType = UIReturnKeyType.Search;
                            this.textField.ReturnKeyType = UIReturnKeyType.Search;
                            break;
                        case Input.InputScope.EmailAddress:
                            keyBoardType = UIKeyboardType.EmailAddress;
                            break;
                        default:
                            keyBoardType = UIKeyboardType.Default;
                            break;
                    }

                    this.textField.KeyboardType = keyBoardType;
                    this.textView.KeyboardType = keyBoardType;
                }
            }
        }

        private bool NativeIsReadOnly
        {
            get
            {
                return this.nativeIsReadOnly;
            }
            set
            {
                this.nativeIsReadOnly = value;
                if (value)
                {
                    this.textField.UserInteractionEnabled = false;
                    this.textView.UserInteractionEnabled = false;
                }
                else if (this.nativeAcceptsReturn || this.TextWrapping == TextWrapping.Wrap)
                {
                    this.textView.UserInteractionEnabled = true;
                }
                else
                {
                    this.textField.UserInteractionEnabled = true;
                }
            }
        }

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                UIView v = new UIView(new CGRect(10, 10, 300, 60));
                this.textField = new MyTextField();
                this.textField.Frame = new CGRect(0, 0, 300, 60);
                this.textField.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

                this.textView = new UITextView();
                this.textView.BackgroundColor = UIColor.Clear; // FromRGBA(255, 0, 0, 50);
                this.textView.Font = this.textField.Font;
                this.textView.Frame = new CGRect(0, 3, 300, 60);
                this.textView.ContentInset = new UIEdgeInsets(-8, 0, -8, 0);
                this.textView.ScrollIndicatorInsets = new UIEdgeInsets(8, 0, 8, 0);
                this.textView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
                this.textView.Changed += (s, e) =>
                {
                    this.ChangeSizeToContent();
                    this.ChangeText();
                };

                NSNotificationCenter.DefaultCenter.AddObserver(UITextField.TextFieldTextDidChangeNotification, (notification) =>
                {
                    if (notification.Object == this.textField)
                    {
                        this.ChangeText();
                    }
                });

                this.textView.SelectionChanged += (s, e) =>
                {
                    var range = this.textView.SelectedRange;
                    this.nativeSelectionStart = range.Location;
                    this.nativeSelectionLength = range.Length;
                };

                this.textView.ShouldChangeText = (tv, range, str) => this.ChangeTextBaseOnLimit(ref range, str);
                this.textField.ShouldChangeCharacters = (tf, range, str) => this.ChangeTextBaseOnLimit(ref range, str);

                v.AddSubview(this.textField);
                v.AddSubview(this.textView);
                this.NativeUIElement = v;
            }

            base.NativeInit();

            this.AcceptsReturn = this.nativeAcceptsReturn;

            this.NativeText = this.nativeText;

            this.NativeInputScope = this.InputScope;

            this.ChangeVisualControl();

            this.OnTextAlignmentChanged();
            if (this.ReadLocalValue(TextBox.SelectionStartProperty) == DependencyProperty.UnsetValue && this.ReadLocalValue(TextBox.SelectionLengthProperty) == DependencyProperty.UnsetValue)
            {
                this.Select(this.NativeText == null ? 0 : this.NativeText.Length, 0);
            }
            else
            {
                this.Select(this.SelectionStart, this.SelectionLength);
            }
        }

        protected override void UpdateFontWeightAndStyle()
        {
        }

        protected override CGSize NativeMeasureOverride(CGSize availableSize)
        {
            var size = new CGSize(availableSize.Width - this.Margin.HorizontalThicknessF(), availableSize.Height - this.Margin.VerticalThicknessF());
            var height = defaultHeight;
            var width = 0.0;
            if (this.AcceptsReturn)
            {
                var textToFit = string.IsNullOrEmpty(this.Text) || this.Text.Last() == '\n' ? this.Text + " " : this.Text;

                var needSize = new NSString(this.textView.Text).GetBoundingRect(
                    new CGSize(size.Width - 18, size.Height - 18 - textViewTop),
                    NSStringDrawingOptions.UsesLineFragmentOrigin,
                    new UIStringAttributes { Font = this.textView.Font },
                    null);
                height = Math.Max(needSize.Height + textViewTop + 18, defaultHeight);
                width = needSize.Width + 18;
            }
            else
            {
                var needSize = this.textField.SizeThatFits(size); // StringSize(textToFit, this.textField.Font, size);
                height = Math.Max(needSize.Height, defaultHeight);
                width = needSize.Width;
            }

            height = this.ReadLocalValue(UIElement.HeightProperty) == DependencyProperty.UnsetValue ? height + this.Margin.VerticalThickness() : this.Height;
            width = this.ReadLocalValue(UIElement.WidthProperty) == DependencyProperty.UnsetValue ? width + this.Margin.HorizontalThicknessF() : this.Width;

            return new CGSize(width, height);
        }

        protected override bool InternalFocus()
        {
            if (this.NativeUIElement != null)
            {
                return (this.TextWrapping == Controls.TextWrapping.Wrap || this.AcceptsReturn) ? this.textView.BecomeFirstResponder() : this.textField.BecomeFirstResponder();
            }
            return false;
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null)
            {
                var backgroundIsSet = this.Background != null;
                this.textField.BorderStyle = backgroundIsSet ? UITextBorderStyle.None : UITextBorderStyle.RoundedRect;
                this.textField.Frame = backgroundIsSet ? new CGRect(3, 3, this.NativeUIElement.Frame.Width - 3, this.NativeUIElement.Frame.Height - 6) : new CGRect(CGPoint.Empty, this.NativeUIElement.Frame.Size);
                this.NativeUIElement.BackgroundColor = backgroundIsSet ? this.Background.ToUIColor(this.NativeUIElement.Frame.Size) : UIColor.Clear;
            }
        }

        private void NativeSelect(int start, int length)
        {
            this.nativeSelectionStart = start;
            this.nativeSelectionLength = length;
            if (NativeUIElement == null)
            {
                return;
            }

            if (this.TextWrapping == Controls.TextWrapping.Wrap || this.AcceptsReturn)
            {
                this.textView.SelectedRange = new NSRange(start, length);
            }
            else
            {
                // First of all TextBox must come in edeting mode
                if (this.textField.IsEditing)
                {
                    this.SelectOnEditing(this.textField, EventArgs.Empty);
                }
                else
                {
                    // there will be crash if there is two same handlers added
                    this.textField.EditingDidBegin -= this.SelectOnEditing;
                    this.textField.EditingDidBegin += this.SelectOnEditing;
                }
            }
        }

        private bool ChangeTextBaseOnLimit(ref NSRange range, string str)
        {
            var location = (int)range.Location;
            var length = (int)range.Length;
            var maxLength = this.MaxLength;
            var text = this.Text;
            if (maxLength == 0 || str == string.Empty)
            {
                var newText = str;
                if (text != null)
                {
                    newText = text.Substring(0, location) + str + text.Substring(location + length);
                }

                this.Text = newText;
            }
            else if (text == null)
            {
                this.Text = str.Substring(0, Math.Min(str.Length, maxLength));
            }
            else
            {
                var count = maxLength - (text.Length - length);
                if (count > 0)
                {
                    if (count < str.Length)
                    {
                        str = str.Remove(count);
                    }

                    this.Text = text.Substring(0, location) + str + text.Substring(location + length);
                }
                else
                {
                    str = string.Empty;
                }
            }

            this.Select(location + str.Length, 0);
            return false;
        }

        private void SelectOnEditing(object sender, EventArgs e)
        {
            this.textField.EditingDidBegin -= this.SelectOnEditing;
            var startPosition = this.textField.GetPosition(this.textField.BeginningOfDocument, this.nativeSelectionStart);
            var endPosition = this.textField.GetPosition(this.textField.BeginningOfDocument, this.nativeSelectionStart + this.nativeSelectionLength);
            if (startPosition == null || endPosition == null)
            {
                this.nativeSelectionStart = 0;
                this.nativeSelectionLength = 0;
                return;
            }

            // if TextBox just come to editing mode, it selects last position after calling this event
            // So we need to select text after it
            var t = new Task(() =>
            {
                Thread.Sleep(1);
                this.textField.InvokeOnMainThread(() =>
                {
                    var tr = this.textField.GetTextRange(startPosition, endPosition);
                    if(tr != null)
                    {
                        this.textField.SelectedTextRange = tr;
                    }
                });
            });
            t.Start();
        }

        private void ChangeText()
        {
            if (this.nativeAcceptsReturn || this.TextWrapping == Controls.TextWrapping.Wrap)
            {
                this.nativeText = this.textView.Text;
            }
            else
            {
                this.nativeText = this.textField.Text;
            }
        }

        private void ChangeVisualControl()
        {
            if (this.nativeAcceptsReturn || this.TextWrapping == Controls.TextWrapping.Wrap)
            {
                this.textField.Text = null;
                this.textView.Text = this.Text;
                if (this.textField.IsFirstResponder)
                {
                    this.textView.BecomeFirstResponder();
                }
                this.textView.Hidden = false;
                this.textField.UserInteractionEnabled = false;
            }
            else
            {
                this.textView.Text = null;
                this.textField.Text = this.Text;
                if (this.textView.IsFirstResponder)
                {
                    this.textField.BecomeFirstResponder();
                }
                this.textView.Hidden = true;
                this.textField.UserInteractionEnabled = true;
            }
        }

        private void ChangeSizeToContent()
        {
        }

        protected class MyTextField : UITextField
        {
            public MyTextField()
            {
                this.BorderStyle = UITextBorderStyle.RoundedRect;
            }

            public delegate void SelectionChanged(int start, int length);

            public SelectionChanged SlectionChangedDelegate { get; set; }
        }
    }
}