using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class PasswordBox : Control
    {
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(PasswordBox), new PropertyMetadata(int.MaxValue, (d, e) =>
                {
                    ((PasswordBox)d).NativeMaxLength = (int)e.NewValue;
                }));

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBox), new PropertyMetadata(string.Empty, (d, e) =>
                {
                    ((PasswordBox)d).NativePassword = (string)e.NewValue;
                    ((PasswordBox)d).OnLayoutUpdated();
                }));

        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register("PasswordChar", typeof(char), typeof(PasswordBox), new PropertyMetadata(null));

        public PasswordBox()
        {
        }

        public event RoutedEventHandler PasswordChanged;

        /// <summary>
        /// Gets or sets Maximum password Length
        /// </summary>
        public int MaxLength
        {
            get { return (int)this.GetValue(MaxLengthProperty); }
            set { this.SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets Password string
        /// </summary>
        public string Password
        {
            get { return (string)this.GetValue(PasswordProperty); }
            set { this.SetValue(PasswordProperty, value); }
        }

        public char PasswordChar
        {
            get { return (char)this.GetValue(PasswordCharProperty); }
            set { this.SetValue(PasswordCharProperty, value); }
        }

        public void SelectAll()
        {
            this.NativeSelect(0, this.Password.Length);
        }
    }
}
