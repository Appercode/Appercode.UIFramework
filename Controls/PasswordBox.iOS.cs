using CoreGraphics;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UIKit;

namespace Appercode.UI.Controls
{
    public partial class PasswordBox
    {
        // TODO: check on iOS 7
        private static nfloat defaultHeight = 30;
        private int nativeSelectionStart;
        private int nativeSelectionLength;
        private UIView background;

        static PasswordBox()
        {
            PasswordBox.MinWidthProperty.AddOwner(typeof(PasswordBox), new PropertyMetadata(10.0));
        }

        public string NativePassword { get; set; }

        public int NativeMaxLength { get; set; }

        public override CGSize MeasureOverride(CGSize availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            size.Height = MathF.Max(size.Height, defaultHeight + this.Margin.TopF() + this.Margin.LeftF());
            var needSize = ((UITextField)this.NativeUIElement).SizeThatFits(size); // this.Password, ((UITextField)this.NativeUIElement).Font, size);
            size.Width += needSize.Width;
            return size;
        }

        protected internal override void NativeInit()
        {
            if (this.NativeUIElement == null)
            {
                var textField = new UITextField { SecureTextEntry = true };
                textField.BorderStyle = UITextBorderStyle.RoundedRect;
                foreach (UIView view in textField.Subviews[0].Subviews)
                {
                    view.Alpha = 0.6f;
                }
                this.background = new UIView { BackgroundColor = UIColor.White, AutoresizingMask = UIViewAutoresizing.FlexibleDimensions };
                this.background.Layer.CornerRadius = int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]) < 7 ? 10f : 6f;
                this.background.Layer.BorderWidth = 0.5f;
                this.background.Layer.BorderColor = UIColor.LightGray.CGColor;
                textField.Subviews[0].InsertSubview(this.background, 0);
                textField.ShouldChangeCharacters += (sender, e, txt) =>
                {
                    return this.NativeMaxLength == 0 || textField.Text.Length + txt.Length <= this.NativeMaxLength;
                };
                textField.EditingChanged += this.OnEditingChanged;
                textField.EditingDidEnd += (sender, e) => this.OnLostFocus(new RoutedEventArgs());
                this.NativeUIElement = textField;
            }
            base.NativeInit();
        }

        protected override void NativeOnbackgroundChange()
        {
            if (this.NativeUIElement != null && this.Background != null)
            {
                this.background.BackgroundColor = this.Background.ToUIColor(this.RenderSize);
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

            var textField = (UITextField)this.NativeUIElement;

            // First of all TextBox must come in edeting mode
            if (textField.IsEditing)
            {
                this.SelectOnEditing(textField, EventArgs.Empty);
            }
            else
            {
                // there will be crash if there is two same handlers added
                textField.EditingDidBegin -= this.SelectOnEditing;
                textField.EditingDidBegin += this.SelectOnEditing;
            }
        }

        private void OnEditingChanged(object sender, EventArgs e)
        {
            this.Password = ((UITextField)sender).Text;
            if (this.PasswordChanged != null)
            {
                this.PasswordChanged(this, new RoutedEventArgs());
            }

            this.OnLayoutUpdated();
        }

        private void SelectOnEditing(object sender, EventArgs e)
        {
            var textField = (UITextField)this.NativeUIElement;
            textField.EditingDidBegin -= this.SelectOnEditing;
            var startPosition = textField.GetPosition(textField.BeginningOfDocument, this.nativeSelectionStart);
            var endPosition = textField.GetPosition(textField.BeginningOfDocument, this.nativeSelectionStart + this.nativeSelectionLength);
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
                textField.InvokeOnMainThread(() =>
                {
                    textField.SelectedTextRange = textField.GetTextRange(startPosition, endPosition);
                });
            });
            t.Start();
        }
    }
}