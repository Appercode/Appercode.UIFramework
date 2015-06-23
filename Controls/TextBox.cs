using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Appercode.UI.Controls.Input;

namespace Appercode.UI.Controls
{
    public partial class TextBox : Control
    {
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextBox), new PropertyMetadata(TextAlignment.Left, (d, e) => (d as TextBox).OnTextAlignmentChanged()));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(Control),
                                        new PropertyMetadata(TextWrapping.NoWrap, (d, e) =>
                                        {
                                            ((TextBox)d).OnTextWrappingChanged();
                                        }));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBox),
                                        new PropertyMetadata(string.Empty, (d, e) =>
                                        {
                                            ((TextBox)d).OnTextChanged();
                                        }));

        /// <summary>
        /// The max length dependency property. If value 0, then length is unlimited. DefaultValue = 0
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBox), new PropertyMetadata(0, (d, e) =>
            {
                ((TextBox)d).NativeMaxLength = (int)e.NewValue;
            }));

        public static readonly DependencyProperty AcceptsReturnProperty =
            DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(TextBox), new PropertyMetadata(false, (d, e) =>
                {
                    ((TextBox)d).NativeAcceptsReturn = (bool)e.NewValue;
                }));

        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register("SelectionStart", typeof(int), typeof(TextBox), new PropertyMetadata(0));

        public static readonly DependencyProperty SelectionLengthProperty =
            DependencyProperty.Register("SelectionLength", typeof(int), typeof(TextBox), new PropertyMetadata(0));
        public static readonly DependencyProperty InputScopeProperty =
            DependencyProperty.Register("InputScope", typeof(InputScope), typeof(TextBox), new PropertyMetadata(InputScope.Default, (d, e) =>
            {
                ((TextBox)d).NativeInputScope = (InputScope)e.NewValue;
            }));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextBox), new PropertyMetadata(false, (d, e) =>
            {
                ((TextBox)d).NativeIsReadOnly = (bool)e.NewValue;
            }));

        public TextBox()
        {
        }

        public event RoutedEventHandler SelectionChanged;
        public event TextChangedEventHandler TextChanged;

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)this.GetValue(TextWrappingProperty); }
            set { this.SetValue(TextWrappingProperty, value); }
        }

        public int MaxLength
        {
            get { return (int)this.GetValue(MaxLengthProperty); }
            set { this.SetValue(MaxLengthProperty, value); }
        }

        public bool AcceptsReturn
        {
            get { return (bool)this.GetValue(AcceptsReturnProperty); }
            set { this.SetValue(AcceptsReturnProperty, value); }
        }

        public int SelectionStart
        {
            get { return (int)this.GetValue(SelectionStartProperty); }
            set { this.SetValue(SelectionStartProperty, value); }
        }

        public int SelectionLength
        {
            get { return (int)this.GetValue(SelectionLengthProperty); }
            set { this.SetValue(SelectionLengthProperty, value); }
        }

        public InputScope InputScope
        {
            get { return (InputScope)this.GetValue(InputScopeProperty); }
            set { this.SetValue(InputScopeProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }

        public void Select(int start, int length)
        {
            if (start <= 0)
            {
                start = 0;
            }
            else if (start > this.Text.Length)
            {
                start = this.Text.Length;
            }
            if (length <= 0)
            {
                length = 0;
            }
            else if (start + length > this.Text.Length)
            {
                length = this.Text.Length - start;
            }
            this.NativeSelect(start, length);
        }

        public void SelectAll()
        {
            this.NativeSelect(0, this.Text.Length);
        }

        protected virtual void OnTextWrappingChanged()
        {
            this.NativeTextWrapping = this.TextWrapping;
        }

        protected virtual void OnTextAlignmentChanged()
        {
            this.NativeTextAlignment = this.TextAlignment;
        }

        protected virtual void OnTextChanged()
        {
            this.NativeText = this.Text;
            if (this.TextChanged != null)
            {
                this.TextChanged(this, new TextChangedEventArgs());
            }
            this.OnLayoutUpdated();
        }

        protected virtual void OnSelectionChanged()
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, new RoutedEventArgs());
            }
        }
    }
}