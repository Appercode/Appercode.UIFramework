using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Appercode.UI.Controls.Input;

namespace Appercode.UI.Controls
{
    public partial class TextBox
    {
        private string nativeText = null;

        public int NativeSelectionStart { get; set; }

        public int NativeSelectionLength { get; set; }

        public bool NativeIsReadOnly { get; set; }

        protected string NativeText
        {
            get
            {
                return nativeText;
            }
            set
            {
                this.nativeText = value;
            }
        }

        protected int NativeMaxLength { get; set; }

        protected bool NativeAcceptsReturn { get; set; }

        protected TextAlignment NativeTextAlignment { get; set; }

        protected InputScope NativeInputScope { get; set; }

        public void NativeSelect(int start, int length)
        {
        }
    }
}