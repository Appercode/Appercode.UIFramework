using Appercode.UI.Controls;
using System;
using System.Windows;

namespace Appercode.UI.Input
{
    public sealed class GestureEventArgs : RoutedEventArgs
    {
        public bool Handled { get; set; }

        public Point GetPosition(UIElement relativeTo)
        {
            throw new NotImplementedException();
        }
    }
}