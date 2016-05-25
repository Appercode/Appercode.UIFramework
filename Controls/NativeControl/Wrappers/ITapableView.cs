using System;
using System.Windows.Input;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    public interface ITapableView
    {
        event EventHandler NativeTap;
        void WrapedNativeRaiseTap();
    }
}