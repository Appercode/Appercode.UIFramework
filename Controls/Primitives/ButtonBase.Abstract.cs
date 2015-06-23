using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ButtonBase
    {

        public ICommand NativeCommand { get; set; }

        public Controls.ClickMode NativeClickMode { get; set; }

        public object NativeCommandParameter { get; set; }

        public bool NativeIsPressed { get; set; }
    }
}
