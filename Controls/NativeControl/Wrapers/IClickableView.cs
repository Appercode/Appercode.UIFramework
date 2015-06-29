using System;
using System.Windows;
using System.Windows.Input;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    internal interface IClickableView
    {
        event EventHandler NativeClick;
    }
}