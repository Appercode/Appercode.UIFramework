using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Appercode.UI.Controls.Media.Imaging;

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
                    this.NativeUIElement = new NativeEditText(this.Context);
                }

                ((NativeEditText)this.NativeUIElement).InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;

                this.ApplyNativePassword(this.NativePassword);
                this.ApplyNativeMaxLength(this.NativeMaxLength);

                ((NativeEditText)this.NativeUIElement).TextChanged -= this.NativePasswordChange;
                ((NativeEditText)this.NativeUIElement).TextChanged += this.NativePasswordChange;
                SetBackground();
                //((NativeEditText)this.NativeUIElement).NativeSelectionChanged += this.NativePasswordSelectionChanged;

                base.NativeInit();
            }
        }

        protected override void OnBackgroundChanged()
        {
            SetBackground();
        }

        private void SetBackground()
        {
            if (this.Background != null && this.NativeUIElement != null)
            {
                if (IsBackgroundValidImageBrush())
                {
                    ((BitmapImage)(((ImageBrush)this.Background).ImageSource)).ImageOpened += (s, e) =>
                    {
                        if (IsBackgroundValidImageBrush())
                        {
                            this.NativeUIElement.Post(() =>
                            {
                                this.NativeUIElement.SetBackgroundDrawable(this.Background.ToDrawable());
                                this.OnLayoutUpdated();
                            });
                        }
                    };
                }
                else
                    this.NativeUIElement.SetBackgroundDrawable(this.Background.ToDrawable());
            }
        }

        private bool IsBackgroundValidImageBrush()
        {
            return this.Background is ImageBrush
                   && ((ImageBrush)this.Background).ImageSource is BitmapImage
                   && ((BitmapImage)(((ImageBrush)this.Background).ImageSource)).UriSource.IsAbsoluteUri;
        }

        private void NativePasswordChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            this.Password = new string(e.Text.ToArray());
            if (this.PasswordChanged != null)
            {
                this.PasswordChanged(this, new RoutedEventArgs());
            }
        }

        private void NativePasswordSelectionChanged(object sender, RoutedEventArgs e)
        {
            // working with selected part of the text
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