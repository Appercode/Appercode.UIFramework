using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace Appercode.UI.Controls.Primitives
{
    public partial class ToggleButton : ButtonBase
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleButton), new PropertyMetadata(false, (d, e) =>
            {
                ((ToggleButton)d).NativeIsChecked = (bool?)e.NewValue;

                if (((bool?)e.NewValue).HasValue)
                {
                    if (((bool?)e.NewValue).Value)
                    {
                        if (((ToggleButton)d).Checked != null)
                        {
                            ((ToggleButton)d).Checked(d, null);
                        }
                    }
                    else
                    {
                        if (((ToggleButton)d).Unchecked != null)
                        {
                            ((ToggleButton)d).Unchecked(d, null);
                        }
                    }
                }
                else
                {
                    if (((ToggleButton)d).Indeterminate != null)
                    {
                        ((ToggleButton)d).Indeterminate(d, null);
                    }
                }
            }));

        public static readonly DependencyProperty IsThreeStateProperty = DependencyProperty.Register("IsThreeState", typeof(bool), typeof(ToggleButton), new PropertyMetadata(false, (d, e) =>
            {
            }));

        public static readonly RoutedEvent CheckedEvent = new RoutedEvent("Checked", typeof(RoutedEventHandler));
        public static readonly RoutedEvent UncheckedEvent = new RoutedEvent("Unchecked", typeof(RoutedEventHandler));
        public static readonly RoutedEvent IndeterminateEvent = new RoutedEvent("Indeterminate", typeof(RoutedEventHandler));

        public ToggleButton()
        {
        }

        public event RoutedEventHandler Checked;
        public event RoutedEventHandler Indeterminate;
        public event RoutedEventHandler Unchecked;

        public bool IsThreeState
        {
            get
            {
                return (bool)this.GetValue(IsThreeStateProperty);
            }
            set
            {
                this.SetValue(IsThreeStateProperty, value);
            }
        }

        [TypeConverterAttribute(typeof(BoolNullableConverter))]
        public bool? IsChecked
        {
            get
            {
                return (bool?)this.GetValue(IsCheckedProperty);
            }
            set
            {
                this.SetValue(IsCheckedProperty, value);
            }
        }

        protected internal virtual void OnToggle()
        {
            if (this.IsThreeState)
            {
                if (this.IsChecked.HasValue)
                {
                    if (this.IsChecked.Value)
                    {
                        this.IsChecked = null;
                    }
                    else
                    {
                        this.IsChecked = true;
                    }
                }
                else
                {
                    this.IsChecked = false;
                }
            }
            else
            {
                if (this.IsChecked.HasValue)
                {
                    this.IsChecked = !this.IsChecked;
                }
                else
                {
                    this.IsChecked = false;
                }
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (this.IsEnabled)
            {
                this.OnToggle();
            }
        }
    }

    public class BoolNullableConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }
    }
}