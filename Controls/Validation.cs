using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Appercode.UI.Controls
{
 public static class Validation
    {
        public static readonly DependencyProperty ErrorsProperty;

        public static readonly DependencyProperty HasErrorProperty;

        internal static readonly DependencyProperty InternalErrorsProperty;

        static Validation()
        {
            Validation.ErrorsProperty = DependencyProperty.RegisterAttached("Errors", typeof(ReadOnlyObservableCollection<ValidationError>), typeof(Validation), new PropertyMetadata(new ReadOnlyObservableCollection<ValidationError>(new ObservableCollection<ValidationError>())));
            Validation.HasErrorProperty = DependencyProperty.RegisterAttached("HasErrors", typeof(bool), typeof(Validation), new PropertyMetadata(false));
            Validation.InternalErrorsProperty = DependencyProperty.RegisterAttached("InternalErrors", typeof(ObservableCollection<ValidationError>), typeof(Validation), null);
        }

        public static ReadOnlyObservableCollection<ValidationError> GetErrors(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ReadOnlyObservableCollection<ValidationError>)element.GetValue(Validation.ErrorsProperty);
        }

        public static bool GetHasError(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(Validation.HasErrorProperty);
        }

        internal static void AddValidationError(UIElement fe, ValidationError error)
        {
            ObservableCollection<ValidationError> internalErrors = Validation.GetInternalErrors(fe);
            ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(fe);
            bool hasError = Validation.GetHasError(fe);
            if (internalErrors == null)
            {
                internalErrors = new ObservableCollection<ValidationError>();
                errors = new ReadOnlyObservableCollection<ValidationError>(internalErrors);
                fe.SetValue(Validation.InternalErrorsProperty, internalErrors);
                fe.SetValue(Validation.ErrorsProperty, errors);
            }
            internalErrors.Add(error);
            fe.SetValue(Validation.HasErrorProperty, true);
            if (!hasError && fe is Control)
            {
                ((Control)fe).ShowValidationError();
            }
        }

        internal static ObservableCollection<ValidationError> GetInternalErrors(DependencyObject d)
        {
            return (ObservableCollection<ValidationError>)d.GetValue(Validation.InternalErrorsProperty);
        }

        internal static void RemoveValidationError(UIElement fe, ValidationError error)
        {
            ObservableCollection<ValidationError> internalErrors = Validation.GetInternalErrors(fe);
            if (internalErrors == null || internalErrors.Count == 0)
            {
                return;
            }
            internalErrors.Remove(error);
            if (internalErrors.Count == 0)
            {
                fe.SetValue(Validation.HasErrorProperty, false);
                if (fe is Control)
                {
                    ((Control)fe).HideValidationError();
                }
            }
        }
    }
}