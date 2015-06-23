using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appercode.UI.Controls
{
    public partial class PasswordBox
    {
        public string NativePassword { get; set; }

        public int NativeMaxLength { get; set; }

        private void NativeSelect(int start, int length)
        {
            PasswordChanged(this, new RoutedEventArgs());
        }
    }
}
