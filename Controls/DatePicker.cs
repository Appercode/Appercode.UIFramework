using System;
using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class DatePicker : Control
    {
        private const string ElementButton = "PART_Button";
        private const string ElementPresenter = "PART_Presenter";

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            nameof(Date), typeof(DateTimeOffset), typeof(DatePicker), new PropertyMetadata(default(DateTimeOffset), OnDatePropertyChanged));

        public static readonly DependencyProperty MaxYearProperty = DependencyProperty.Register(
            nameof(MaxYear), typeof(DateTimeOffset), typeof(DatePicker), new PropertyMetadata(DateTimeOffset.MaxValue, OnDatePropertyChanged));

        public static readonly DependencyProperty MinYearProperty = DependencyProperty.Register(
            nameof(MinYear), typeof(DateTimeOffset), typeof(DatePicker), new PropertyMetadata(DateTimeOffset.MinValue, OnDatePropertyChanged));

        private static readonly ControlTemplate DefaultTemplate;

        private ContentPresenter pickerPresenter;
        private Button mainButton;
        private PickerView pickerView;

        static DatePicker()
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var row0Factory = new FrameworkElementFactory(typeof(RowDefinition));
            row0Factory.SetValue(RowDefinition.HeightProperty, GridLength.Auto);
            gridFactory.AppendChild(row0Factory);

            var row1Factory = new FrameworkElementFactory(typeof(RowDefinition));
            row1Factory.SetValue(RowDefinition.HeightProperty, GridLength.Auto);
            gridFactory.AppendChild(row1Factory);

            var buttonFactory = new FrameworkElementFactory(typeof(Button)) { Name = ElementButton };
            gridFactory.AppendChild(buttonFactory);

            var presenterFactory = new FrameworkElementFactory(typeof(ContentPresenter)) { Name = ElementPresenter };
            presenterFactory.SetValue(Grid.RowProperty, 1);
            gridFactory.AppendChild(presenterFactory);

            DefaultTemplate = new ControlTemplate { VisualTree = gridFactory };
            DefaultTemplate.SetValue(ControlTemplate.TargetTypeProperty, typeof(DatePicker));
        }

        public DatePicker()
        {
            this.SetValue(TemplateProperty, DefaultTemplate);
        }

        public DateTimeOffset Date
        {
            get { return (DateTimeOffset)this.GetValue(DateProperty); }
            set { this.SetValue(DateProperty, value); }
        }

        public DateTimeOffset MaxYear
        {
            get { return (DateTimeOffset)this.GetValue(MaxYearProperty); }
            set { this.SetValue(MaxYearProperty, value); }
        }

        public DateTimeOffset MinYear
        {
            get { return (DateTimeOffset)this.GetValue(MinYearProperty); }
            set { this.SetValue(MinYearProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (pickerPresenter == null)
            {
                pickerPresenter = this.GetTemplateChild(ElementPresenter) as ContentPresenter;
                if (pickerPresenter != null)
                {
                    this.HidePicker();
                }
            }

            if (this.mainButton == null)
            {
                this.mainButton = this.GetTemplateChild(ElementButton) as Button;
                if (mainButton != null)
                {
                    this.mainButton.Click += this.OnMainButtonClicked;
                    this.OnDateChanged(this.Date);
                }
            }
        }

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);
            if (this.pickerPresenter != null)
            {
                this.pickerPresenter = null;
            }

            if (this.mainButton != null)
            {
                this.mainButton.Click -= this.OnMainButtonClicked;
                this.mainButton = null;
            }
        }

        private static void OnDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = (DatePicker)d;
            if (e.NewValue is DateTimeOffset)
            {
                var newValue = (DateTimeOffset)e.NewValue;
                if (e.Property == DateProperty)
                {
                    datePicker.OnDateChanged(newValue);
                }
                else if (e.Property == MaxYearProperty)
                {
                    datePicker.OnMaxYearChanged(newValue);
                }
                else if (e.Property == MinYearProperty)
                {
                    datePicker.OnMinYearChanged(newValue);
                }
            }
        }

        private void OnDateChanged(DateTimeOffset newValue)
        {
            this.ApplyDate(newValue);
            if (this.mainButton != null)
            {
                this.mainButton.Content = newValue.ToString("d");
            }
        }

        private void OnMaxYearChanged(DateTimeOffset newValue)
        {
            if (newValue < this.MinYear)
            {
                this.MaxYear = this.MinYear;
            }
            else
            {
                this.ApplyMaxYear(newValue);
                if (this.Date > newValue)
                {
                    this.Date = newValue;
                }
            }
        }

        private void OnMinYearChanged(DateTimeOffset newValue)
        {
            if (newValue > this.MaxYear)
            {
                this.MinYear = this.MaxYear;
            }
            else
            {
                this.ApplyMinYear(newValue);
                if (this.Date < newValue)
                {
                    this.Date = newValue;
                }
            }
        }

        private void OnMainButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.pickerPresenter != null)
            {
                if (this.pickerView == null)
                {
                    this.pickerView = new PickerView(this);
                    this.pickerPresenter.Content = pickerView;
                }

                if (this.pickerPresenter.Visibility == Visibility.Collapsed)
                {
                    this.ShowPicker();
                }
                else
                {
                    this.HidePicker();
                }
            }
        }

        private void ShowPicker()
        {
            this.pickerPresenter.Visibility = Visibility.Visible;
        }

        private void HidePicker()
        {
            this.pickerPresenter.Visibility = Visibility.Collapsed;
        }

        partial void ApplyDate(DateTimeOffset value);

        partial void ApplyMaxYear(DateTimeOffset value);

        partial void ApplyMinYear(DateTimeOffset value);

        private partial class PickerView : UIElement
        {
            private readonly DatePicker parent;

            internal PickerView(DatePicker parent)
            {
                this.parent = parent;
            }
        }
    }
}
