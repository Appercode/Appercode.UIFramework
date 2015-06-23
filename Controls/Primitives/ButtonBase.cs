using System;
using System.Windows;
using System.Windows.Input;

namespace Appercode.UI.Controls.Primitives
{
    public abstract partial class ButtonBase : ContentControl
    {
        public static readonly DependencyProperty ClickModeProperty =
                    DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(ButtonBase),
                                                new PropertyMetadata(ClickMode.Release, (d, e) => { ((ButtonBase)d).NativeClickMode = (ClickMode)e.NewValue; }));

        public static readonly DependencyProperty CommandProperty =
                    DependencyProperty.Register("Command", typeof(ICommand), typeof(ButtonBase),
                                                new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
                    DependencyProperty.Register("CommandParameter", typeof(object), typeof(ButtonBase),
                                                new PropertyMetadata(null));

        public static readonly DependencyProperty IsPressedProperty =
                    DependencyProperty.Register("IsPressed", typeof(bool), typeof(ButtonBase),
                                                new PropertyMetadata(false, (d, e) =>
                                                {
                                                    throw new InvalidOperationException("You can't assign IsPressed property.");
                                                }));

        public static readonly RoutedEvent ClickEvent = new RoutedEvent("Click", typeof(RoutedEventHandler));

        public virtual event RoutedEventHandler Click = delegate { };

        public bool IsPressed
        {
            get
            {
                return (bool)this.GetValue(ButtonBase.IsPressedProperty);
            }
            protected internal set
            {
                this.SetValue(ButtonBase.IsPressedProperty, value);

                this.OnIsPressedChanged(new DependencyPropertyChangedEventArgs());
            }
        }

        public ClickMode ClickMode
        {
            get
            {
                return (ClickMode)this.GetValue(ButtonBase.ClickModeProperty);
            }
            set
            {
                this.SetValue(ButtonBase.ClickModeProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(ButtonBase.CommandProperty);
            }
            set
            {
                this.SetValue(ButtonBase.CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return (object)this.GetValue(ButtonBase.CommandParameterProperty);
            }
            set
            {
                this.SetValue(ButtonBase.CommandParameterProperty, value);
            }
        }

        protected virtual void OnClick()
        {
            if(!this.IsEnabled)
            {
                return;
            }
            if(this.Command != null && this.Command.CanExecute(this.CommandParameter))
            {
                this.Command.Execute(this.CommandParameter);
            }
            this.Click(this, new RoutedEventArgs() { OriginalSource = this });
        }

        protected virtual void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
        }
    }
}