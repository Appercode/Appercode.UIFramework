using Appercode.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace System.Windows
{
    public delegate void RoutedPropertyChangedEventHandler<T>(object sender, RoutedPropertyChangedEventArgs<T> e);

    public class RoutedPropertyChangedEventArgs<T> : RoutedEventArgs
    {
        public RoutedPropertyChangedEventArgs(T oldValue, T newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
        public T NewValue
        {
            get;
            private set;
        }

        public T OldValue
        {
            get;
            private set;
        }
    }
}