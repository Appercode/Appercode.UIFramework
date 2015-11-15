using Appercode.UI.Controls;
using Appercode.UI.Internals;
using Appercode.UI.Internals.PathParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Appercode.UI.Data
{
    public sealed class BindingExpression : BindingExpressionBase, IErrorsChangedListener, IDataContextChangedListener
    {
        private static Type stringType;

        private static PropertyPath viewPropertyPath;

        private Binding binding;

        private DependencyObject targetDO;

        private DependencyProperty targetProperty;

        private object source;

        private UIElement mentor;

        private WeakDataContextChangedListener dataContextChangedListener;

        private WeakErrorsChangedListener errorsChangedListener;

        private WeakErrorsChangedListener childErrorsChangedListener;

        private PropertyPathListener listener;

        private PropertyPathListener cvsViewListener;

        private List<ValidationErrorEventArgs> errorNotifications;

        private bool listeningToLostFocus;

        private bool lastSourceUpdateThrewException;

        private BindingExpression.TargetPropertyState targetPropertyState;

        private BindingExpression.UpdateState updateState;

        private ValidationError currentValidationError;

        private Collection<ValidationError> currentNotifyValidationErrors;

        private Collection<ValidationError> currentNotifyChildValidationErrors;

        private INotifyDataErrorInfo currentNotifyDataErrorInfo;

        private INotifyDataErrorInfo currentNotifyChildDataErrorInfo;

        private DynamicValueConverter dynamicConverter;

        static BindingExpression()
        {
            BindingExpression.stringType = typeof(string);
        }

        internal BindingExpression(Binding binding)
        {
            this.binding = binding;
        }

        private enum TargetPropertyState
        {
            Clean,
            Dirty
        }

        private enum UpdateState
        {
            None,
            UpdatingSource,
            UpdatingTarget,
            SettingUpBinding
        }

        public object DataItem
        {
            get
            {
                return this.EffectiveSource;
            }
        }

        public Binding ParentBinding
        {
            get
            {
                return this.binding;
            }
        }

        internal bool CanSetValue
        {
            get
            {
                if (this.binding.Mode != BindingMode.TwoWay)
                {
                    return false;
                }
                return !this.binding.Path.IsPathToSource;
            }
        }

        internal object ConvertedFallbackValue
        {
            get
            {
                Type type = this.binding.FallbackValue.GetType();
                if (type == this.targetProperty.PropertyType || this.targetProperty.PropertyType.IsAssignableFrom(type))
                {
                    return this.binding.FallbackValue;
                }
                return this.ConvertValue(this.binding.FallbackValue, this.targetProperty, "BindingExpression_FailedConvertFallbackValue");
            }
        }

        internal bool IsAssociated
        {
            get
            {
                return this.targetDO != null;
            }
        }

        internal object LeafItem
        {
            get
            {
                if (this.listener == null)
                {
                    return DependencyProperty.UnsetValue;
                }
                return this.listener.LeafItem;
            }
        }

        private static PropertyPath ViewPropertyPath
        {
            get
            {
                if (BindingExpression.viewPropertyPath == null)
                {
                    BindingExpression.viewPropertyPath = new PropertyPath("View", new object[0]);
                    BindingExpression.viewPropertyPath.ParsePathInternal(false);
                }
                return BindingExpression.viewPropertyPath;
            }
        }

        private DynamicValueConverter DynamicConverter
        {
            get
            {
                if (this.dynamicConverter == null)
                {
                    this.dynamicConverter = new DynamicValueConverter(this.binding.Mode == BindingMode.TwoWay);
                }
                return this.dynamicConverter;
            }
        }

        private object EffectiveSource
        {
            get;
            set;
        }

        private bool ListenToChanges
        {
            get
            {
                return this.binding.Mode != BindingMode.OneTime;
            }
        }

        private object SourceForBinding
        {
            get
            {
                return this.source;
            }
            set
            {
                this.EffectiveSource = value;
                if (!this.binding.BindsDirectlyToSource)
                {
                    CollectionViewSource collectionViewSource = this.source as CollectionViewSource;
                    CollectionViewSource collectionViewSource1 = value as CollectionViewSource;
                    if (collectionViewSource1 != null)
                    {
                        this.EffectiveSource = collectionViewSource1.View;
                    }
                    if (collectionViewSource != null)
                    {
                        if (collectionViewSource1 == null)
                        {
                            this.cvsViewListener.Disconnect();
                            this.cvsViewListener.PropertyPathChanged -= new PropertyPathStepChangedHandler(this.SourcePropertyChanged);
                            this.cvsViewListener = null;
                        }
                        else
                        {
                            this.cvsViewListener.ReConnect(collectionViewSource1);
                        }
                    }
                    else if (collectionViewSource1 != null)
                    {
                        this.cvsViewListener = BindingExpression.ViewPropertyPath.GetListener(value, true, this);
                        this.cvsViewListener.PropertyPathChanged += new PropertyPathStepChangedHandler(this.CollectionViewSourceViewChanged);
                    }
                }
                this.source = value;
            }
        }

        private UIElement TargetElement
        {
            get
            {
                return this.TargetFE ?? this.mentor;
            }
        }

        private UIElement TargetFE
        {
            get
            {
                return this.targetDO as UIElement;
            }
        }

        public void UpdateSource()
        {
            if (this.targetDO == null)
            {
                throw new InvalidOperationException();
            }
            if (!this.CanSetValue)
            {
                return;
            }
            this.UpdateValue();
        }

        void IErrorsChangedListener.OnErrorsChanged(object sender, bool notifyChild, DataErrorsChangedEventArgs e)
        {
            if (notifyChild)
            {
                this.NotifyChildDataErrorInfo_ErrorsChanged(sender, e);
                return;
            }
            this.NotifyDataErrorInfo_ErrorsChanged(sender, e);
        }

        void IDataContextChangedListener.OnDataContextChanged(object sender, DataContextChangedEventArgs e)
        {
            if (this.targetDO == null)
            {
                return;
            }

            object dataContext = this.GetDataContext(this.TargetElement);
            if (e.ChangeReason == DataContextChangedReason.EnteringLiveTree
                || dataContext != this.SourceForBinding)
            {
                this.ResetNotifyDataErrorInfo();
                this.lastSourceUpdateThrewException = false;
                this.SourceForBinding = dataContext;
                this.SourceAcquired();
            }
        }

        internal static bool IsNullableType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        internal static object TryParse(object value, Type targetType, CultureInfo formatInfo)
        {
            object unsetValue;
            Type type = targetType;
            Type underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                type = underlyingType;
            }
            try
            {
                Type[] typeArray = new Type[] { BindingExpression.stringType, typeof(NumberStyles), typeof(IFormatProvider) };
                MethodInfo method = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, typeArray, null);
                if (method == null)
                {
                    Type[] typeArray1 = new Type[] { BindingExpression.stringType, typeof(IFormatProvider) };
                    method = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, typeArray1, null);
                    if (method == null)
                    {
                        Type[] typeArray2 = new Type[] { BindingExpression.stringType };
                        method = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, typeArray2, null);
                        if (method == null)
                        {
                            return DependencyProperty.UnsetValue;
                        }
                        else
                        {
                            object[] objArray = new object[] { (string)value };
                            unsetValue = method.Invoke(null, objArray);
                        }
                    }
                    else
                    {
                        object[] objArray1 = new object[] { (string)value, formatInfo };
                        unsetValue = method.Invoke(null, objArray1);
                    }
                }
                else
                {
                    object[] objArray2 = new object[] { (string)value, NumberStyles.Any, formatInfo };
                    unsetValue = method.Invoke(null, objArray2);
                }
            }
            catch
            {
                unsetValue = DependencyProperty.UnsetValue;
            }
            return unsetValue;
        }

        internal bool IsPropertyChangeRelevant(DependencyProperty dp)
        {
            if (this.targetProperty == dp && this.listener != null && this.listener.FullPathExists)
            {
                return true;
            }
            return false;
        }

        internal bool IsTargetFocused()
        {
            var targetUIElement = this.targetDO as UIElement;
            return targetUIElement != null
                ? targetUIElement.IsFocused
                : FocusManager.GetFocusedElement() == this.targetDO;
        }

        internal void AddErrorToTarget(ValidationError error, bool isNotifyDataErrorInfo)
        {
            if (this.TargetElement == null)
            {
                return;
            }
            if (!isNotifyDataErrorInfo)
            {
                this.currentValidationError = error;
            }
            if (error == null)
            {
                return;
            }
            Validation.AddValidationError(this.TargetElement, error);
        }

        internal void BeginSetBinding()
        {
            this.updateState = BindingExpression.UpdateState.SettingUpBinding;
        }

        internal object ConvertValue(object value, DependencyProperty dp, string failureResource)
        {
            CultureInfo cultureInfo = this.CalculateCulture();
            try
            {
                value = this.DynamicConverter.Convert(value, dp.PropertyType, null, cultureInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return value;
        }

        internal void EndSetBinding()
        {
            if (this.binding.ValidatesOnDataErrors)
            {
                string dataErrorInfoMessage = this.GetDataErrorInfoMessage();
                if (!string.IsNullOrEmpty(dataErrorInfoMessage))
                {
                    ValidationError validationError = new ValidationError(dataErrorInfoMessage);
                    this.AddErrorNotificationToQueue(ValidationErrorEventAction.Added, validationError);
                    this.AddErrorToTarget(validationError, false);
                    this.RaiseErrorNotifications();
                }
            }
            if (this.binding.ValidatesOnNotifyDataErrors)
            {
                this.ConsumeNotifyDataErrorInfo();
            }
            this.updateState = BindingExpression.UpdateState.None;
        }

        internal string GetExpressionTraceString()
        {
            string path = this.binding.Path.Path;
            string str = this.SourceForBinding != null ? this.SourceForBinding.ToString() : "null";
            string str1 = this.SourceForBinding != null ? this.SourceForBinding.GetHashCode().ToString() : "0";
            string str2 = this.targetDO.GetType().ToString();
            string str3 = this.TargetFE != null ? this.TargetFE.Name : "null";
            string name = this.targetProperty.Name;
            string str4 = this.targetProperty.PropertyType.ToString();
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string str5 = "BindingExpression TraceString: {0} \n {1} {2} {3} {4} {5} {6}";
            object[] objArray = new object[] { path, str, str1, str2, str3, name, str4 };
            return string.Format(currentCulture, str5, objArray);
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            if (this.listener != null && this.listener.FullPathExists)
            {
                return this.ConvertToTarget(this.listener.LeafValue);
            }
            if (this.listener == null || !this.binding.Path.IsPathToSource)
            {
                return this.GetConvertedFallbackOrDefaultValue(d, dp);
            }
            if (this.binding.TargetNullValue != null)
            {
                return this.ConvertToTarget(null);
            }
            return dp.GetDefaultValue(d);
        }

        internal override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            this.targetDO = d;
            this.targetProperty = dp;
            if (this.TargetFE != null && dp == UIElement.DataContextProperty)
            {
                this.TargetFE.IsDataContextBound = true;
            }
            UIElement targetFE = this.TargetFE;
            if (this.binding.Source != null)
            {
                this.SourceForBinding = this.binding.Source;
            }
            else if (this.binding.RelativeSource == null || this.binding.RelativeSource.Mode != RelativeSourceMode.Self)
            {
                if (targetFE == null)
                {
                    if (this.mentor == null)
                    {
                        this.mentor = this.targetDO.GetMentor();
                    }
                    targetFE = this.mentor;
                    this.targetDO.InheritanceContextChanged += new EventHandler(this.InheritanceContextChanged);
                }
                this.AttachToMentor(targetFE);
            }
            else
            {
                this.SourceForBinding = this.targetDO;
            }
            if (this.CanSetValue && this.binding.UpdateSourceTrigger == UpdateSourceTrigger.Default)
            {
                this.targetDO.DPChanged += this.TargetPropertyChanged;
                if (this.TargetFE != null)
                {
                    if ((this.TargetFE is TextBox && this.targetProperty == TextBox.TextProperty) || (this.TargetFE is PasswordBox && this.targetProperty == PasswordBox.PasswordProperty))
                    {
                        this.listeningToLostFocus = true;
                        this.TargetFE.LostFocus += new RoutedEventHandler(this.TargetLostFocus);
                    }
                }
            }
            this.ConnectToSource();
        }

        internal void OnDetach()
        {
            this.DisconnectFromSource();
            if (this.TargetFE != null && this.targetProperty == UIElement.DataContextProperty)
            {
                this.TargetFE.IsDataContextBound = false;
            }
            this.DetachFromMentor(this.TargetElement);
            if (this.CanSetValue && this.binding.UpdateSourceTrigger == UpdateSourceTrigger.Default)
            {
                this.targetDO.DPChanged -= this.TargetPropertyChanged;
                if (this.TargetFE != null)
                {
                    if (this.listeningToLostFocus)
                    {
                        this.TargetFE.LostFocus -= new RoutedEventHandler(this.TargetLostFocus);
                        this.listeningToLostFocus = false;
                    }
                }
            }
            this.RemoveErrorFromTarget(this.currentValidationError);
            this.ResetNotifyDataErrorInfo();
            this.lastSourceUpdateThrewException = false;
            if (this.targetDO != null)
            {
                this.targetDO.InheritanceContextChanged -= new EventHandler(this.InheritanceContextChanged);
            }
            this.targetDO = null;
            this.targetProperty = null;
        }

        internal bool RefreshExpression()
        {
            return this.targetDO.RefreshExpression(this.targetProperty);
        }

        internal void RemoveErrorFromTarget(ValidationError error)
        {
            if (this.TargetElement == null || error == null)
            {
                return;
            }
            Validation.RemoveValidationError(this.TargetElement, error);
        }

        internal void SendDataToTarget()
        {
            ValidationError validationError = null;
            if (this.updateState == BindingExpression.UpdateState.None && this.binding.ValidatesOnDataErrors)
            {
                string dataErrorInfoMessage = this.GetDataErrorInfoMessage();
                if (!string.IsNullOrEmpty(dataErrorInfoMessage))
                {
                    validationError = new ValidationError(dataErrorInfoMessage);
                }
            }
            if (validationError == null)
            {
                this.RemoveErrorFromTarget(this.currentValidationError);
                this.AddErrorNotificationToQueue(ValidationErrorEventAction.Removed, this.currentValidationError);
                this.RaiseErrorNotifications();
                this.currentValidationError = null;
            }
            BindingExpression.UpdateState updateState = this.updateState;
            this.updateState = BindingExpression.UpdateState.UpdatingTarget;
            try
            {
                this.RefreshExpression();
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Exception occured during binding: {0}\nStackTrace:\n{1}", e.Message, e.StackTrace));
#if DEBUG
                throw;
#endif
            }
            finally
            {
                this.updateState = updateState;
            }
            if (validationError != null)
            {
                this.AddErrorNotificationToQueue(ValidationErrorEventAction.Removed, this.currentValidationError);
                this.AddErrorNotificationToQueue(ValidationErrorEventAction.Added, validationError);
            }
            if (updateState != BindingExpression.UpdateState.UpdatingSource && this.lastSourceUpdateThrewException)
            {
                this.lastSourceUpdateThrewException = false;
                this.AddNotifyDataErrorInfosToQueue(ValidationErrorEventAction.Added);
                this.AddNotifyDataErrorInfosToTarget();
            }
            if (validationError != null)
            {
                ValidationError validationError1 = this.currentValidationError;
                this.AddErrorToTarget(validationError, false);
                this.RemoveErrorFromTarget(validationError1);
            }
            this.RaiseErrorNotifications();
        }

        internal void TargetLostFocus(object sender, RoutedEventArgs e)
        {
            if (this.targetPropertyState == BindingExpression.TargetPropertyState.Dirty)
            {
                this.UpdateValue();
            }
        }

        internal void TargetPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (!this.IsPropertyChangeRelevant(args.Property))
            {
                return;
            }
            if (this.updateState != BindingExpression.UpdateState.None)
            {
                return;
            }
            this.UpdateValueIfNecessary();
        }

        internal void SourcePropertyChanged(PropertyPathListener sender, PropertyPathChangedEventArgs args)
        {
            this.UpdateNotifyDataErrorInfos();
            this.SendDataToTarget();
        }

        private void AddErrorNotificationToQueue(ValidationErrorEventAction action, ValidationError error)
        {
            if (!this.binding.NotifyOnValidationError || error == null || this.TargetElement == null)
            {
                return;
            }
            ValidationErrorEventArgs validationErrorEventArg = new ValidationErrorEventArgs(action, error);
            if (this.errorNotifications == null)
            {
                this.errorNotifications = new List<ValidationErrorEventArgs>();
            }
            this.errorNotifications.Add(validationErrorEventArg);
        }

        private void AddNotifyDataErrorInfosToQueue(ValidationErrorEventAction action)
        {
            this.AddNotifyDataErrorInfosToQueue(action, this.currentNotifyChildValidationErrors);
            this.AddNotifyDataErrorInfosToQueue(action, this.currentNotifyValidationErrors);
        }

        private void AddNotifyDataErrorInfosToQueue(ValidationErrorEventAction action, Collection<ValidationError> validationErrors)
        {
            if (validationErrors == null)
            {
                return;
            }
            foreach (ValidationError validationError in validationErrors)
            {
                this.AddErrorNotificationToQueue(action, validationError);
            }
        }

        private void AddNotifyDataErrorInfosToTarget()
        {
            this.AddNotifyDataErrorInfosToTarget(this.currentNotifyChildValidationErrors);
            this.AddNotifyDataErrorInfosToTarget(this.currentNotifyValidationErrors);
        }

        private void AddNotifyDataErrorInfosToTarget(Collection<ValidationError> validationErrors)
        {
            if (validationErrors == null)
            {
                return;
            }
            foreach (ValidationError validationError in validationErrors)
            {
                this.AddErrorToTarget(validationError, true);
            }
        }

        private object ApplyStringFormat(object value, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(this.binding.StringFormat))
            {
                CultureInfo cultureInfo = culture ?? this.CalculateCulture();
                try
                {
                    if (!this.binding.StringFormat.Contains("{"))
                    {
                        string str = string.Concat("{0:", this.binding.StringFormat, "}");
                        object[] objArray = new object[] { value };
                        value = string.Format(cultureInfo, str, objArray);
                    }
                    else
                    {
                        string stringFormat = this.binding.StringFormat;
                        object[] objArray1 = new object[] { value };
                        value = string.Format(cultureInfo, stringFormat, objArray1);
                    }
                }
                catch (FormatException e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            return value;
        }

        private void AttachToMentor(UIElement mentor)
        {
            if (mentor == null)
            {
                this.SourceForBinding = null;
                return;
            }
            if (this.binding.RelativeSource != null)
            {
                this.SourceForBinding = mentor.TemplatedParent;
                return;
            }
            if (this.binding.ElementName == null)
            {
                if (this.TargetFE == null)
                {
                    this.dataContextChangedListener = new WeakDataContextChangedListener(mentor, this);
                }
                else
                {
                    this.TargetFE.DataContextChanged += this.DataContextChanged;
                }
                this.SourceForBinding = this.GetDataContext(mentor);
            }
            else
            {
                this.SourceForBinding = this.GetSourceElement();
                if (this.SourceForBinding == null && !mentor.IsInLiveTree)
                {
                    mentor.Loaded += new RoutedEventHandler(this.TargetLoaded);
                    this.SourceForBinding = null;
                    return;
                }
            }
        }

        private CultureInfo CalculateCulture()
        {
            return this.binding.ConverterCulture;
        }

        private void CollectionViewSourceViewChanged(PropertyPathListener sender, PropertyPathChangedEventArgs args)
        {
            this.EffectiveSource = (this.SourceForBinding as CollectionViewSource).View;
            this.listener.ReConnect(this.EffectiveSource);
            this.SendDataToTarget();
        }

        private void ConnectToSource()
        {
            this.binding.IsSealed = true;
            this.listener = this.binding.Path.GetListener(this.EffectiveSource, this.ListenToChanges, this);
            if (this.ListenToChanges)
            {
                this.listener.PropertyPathChanged += new PropertyPathStepChangedHandler(this.SourcePropertyChanged);
            }
        }

        private void ConsumeNotifyDataErrorInfo()
        {
            string str = null;
            this.GetNotifyDataErrorInfoInstances(out str, out this.currentNotifyDataErrorInfo, out this.currentNotifyChildDataErrorInfo);
            this.ConsumeNotifyDataErrorInfo(str, this.currentNotifyDataErrorInfo, this.currentNotifyChildDataErrorInfo);
        }

        private void ConsumeNotifyDataErrorInfo(string propertyName, INotifyDataErrorInfo notifyDataErrorInfo, INotifyDataErrorInfo notifyChildDataErrorInfo)
        {
            if (notifyDataErrorInfo != null && this.binding.Mode != BindingMode.OneTime)
            {
                this.errorsChangedListener = new WeakErrorsChangedListener(notifyDataErrorInfo, false, this);
            }
            if (notifyChildDataErrorInfo != null && this.binding.Mode != BindingMode.OneTime)
            {
                this.childErrorsChangedListener = new WeakErrorsChangedListener(notifyChildDataErrorInfo, true, this);
            }
            if (notifyDataErrorInfo != null)
            {
                this.NotifyNewDataErrorInfos(propertyName, notifyDataErrorInfo, false);
            }
            if (notifyChildDataErrorInfo != null)
            {
                this.NotifyNewDataErrorInfos(string.Empty, notifyChildDataErrorInfo, true);
            }
        }

        private object ConvertToTarget(object value)
        {
            CultureInfo cultureInfo = null;
            if (this.binding.Converter != null)
            {
                cultureInfo = this.CalculateCulture();
                value = this.binding.Converter.Convert(value, this.targetProperty.PropertyType, this.binding.ConverterParameter, cultureInfo);
                if (value == DependencyProperty.UnsetValue)
                {
                    if (this.binding.FallbackValue == null)
                    {
                        value = this.targetProperty.GetDefaultValue(this.targetDO);
                    }
                    else
                    {
                        value = this.binding.FallbackValue;
                    }
                }
            }
            if (value != null)
            {
                value = this.ApplyStringFormat(value, cultureInfo);
            }
            else if (this.binding.TargetNullValue != null)
            {
                value = this.binding.TargetNullValue;
            }
            if (value != null)
            {
                Type type = value.GetType();
                if (type == this.targetProperty.PropertyType || this.targetProperty.PropertyType.IsAssignableFrom(type))
                {
                    return value;
                }
            }
            if (cultureInfo == null)
            {
                cultureInfo = this.CalculateCulture();
            }
            try
            {
                value = this.DynamicConverter.Convert(value, this.targetProperty.PropertyType, null, cultureInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                if (e is OutOfMemoryException || e is StackOverflowException || e is AccessViolationException || e is ThreadAbortException)
                {
                    throw;
                }
            }
            return value;
        }

        private void DataContextChanged(object sender, DataContextChangedEventArgs e)
        {
            ((IDataContextChangedListener)this).OnDataContextChanged(sender, e);
        }

        private void DetachFromMentor(UIElement mentor)
        {
            if (mentor != null)
            {
                mentor.Loaded -= new RoutedEventHandler(this.TargetLoaded);
                if (this.dataContextChangedListener != null)
                {
                    this.dataContextChangedListener.Disconnect();
                    this.dataContextChangedListener = null;
                }
                else
                {
                    mentor.DataContextChanged -= this.DataContextChanged;
                }
            }
        }

        private void DisconnectFromSource()
        {
            if (this.listener != null)
            {
                this.listener.Disconnect();
                if (this.ListenToChanges)
                {
                    this.listener.PropertyPathChanged -= new PropertyPathStepChangedHandler(this.SourcePropertyChanged);
                }
                this.listener = null;
            }
            if (this.cvsViewListener != null)
            {
                this.cvsViewListener.Disconnect();
                this.cvsViewListener.PropertyPathChanged -= new PropertyPathStepChangedHandler(this.SourcePropertyChanged);
                this.cvsViewListener = null;
            }
        }

        private object GetConvertedFallbackOrDefaultValue(DependencyObject d, DependencyProperty dp)
        {
            if (this.binding.FallbackValue != null)
            {
                return this.ConvertedFallbackValue;
            }
            return dp.GetDefaultValue(d);
        }

        private object GetDataContext(UIElement targetFE)
        {
            if (targetFE == null)
            {
                return null;
            }
            return targetFE.DataContext;
        }

        private string GetDataErrorInfoMessage()
        {
            if (this.listener != null && this.listener.FullPathExists)
            {
                IDataErrorInfo leafItem = this.listener.LeafItem as IDataErrorInfo;
                if (leafItem != null)
                {
                    string leafPropertyName = this.listener.LeafPropertyName;
                    if (!string.IsNullOrEmpty(leafPropertyName))
                    {
                        return this.GetDataErrorInfoMessage(leafPropertyName, leafItem);
                    }
                }
            }
            return null;
        }

        private string GetDataErrorInfoMessage(string propertyName, IDataErrorInfo dataErrorInfo)
        {
            string item;
            try
            {
                item = dataErrorInfo[propertyName];
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                if (e is OutOfMemoryException || e is StackOverflowException || e is AccessViolationException || e is ThreadAbortException)
                {
                    throw;
                }
                return null;
            }
            return item;
        }

        private void GetNotifyDataErrorInfoInstances(out string propertyName, out INotifyDataErrorInfo notifyDataErrorInfo, out INotifyDataErrorInfo notifyChildDataErrorInfo)
        {
            propertyName = null;
            notifyDataErrorInfo = null;
            notifyChildDataErrorInfo = null;
            if (this.listener != null && this.listener.FullPathExists)
            {
                notifyDataErrorInfo = this.listener.LeafItem as INotifyDataErrorInfo;
                if (notifyDataErrorInfo != null)
                {
                    propertyName = this.listener.LeafPropertyName;
                }
                if (this.listener.LeafProperty != null)
                {
                    notifyChildDataErrorInfo = this.listener.LeafValue as INotifyDataErrorInfo;
                }
            }
        }

        private object GetSourceElement()
        {
            object obj;
            if (this.TargetFE != null)
            {
                return this.TargetFE.FindNameInPage(this.binding.ElementName, false);
            }
            if (this.mentor == null)
            {
                return null;
            }
            UserControl userControl = this.mentor as UserControl;
            if (userControl != null)
            {
                var resources = userControl.InternalResources;
                if (resources != null)
                {
                    IEnumerator enumerator = resources.Values.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current != this.targetDO)
                            {
                                continue;
                            }
                            obj = this.mentor.FindNameInPage(this.binding.ElementName, true);
                            return obj;
                        }
                        return this.mentor.FindNameInPage(this.binding.ElementName, false);
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }
            return this.mentor.FindNameInPage(this.binding.ElementName, false);
        }

        private void InheritanceContextChanged(object sender, EventArgs e)
        {
            if (this.targetDO == null)
            {
                return;
            }
            UIElement mentor = this.targetDO.GetMentor();
            if (this.mentor != mentor && (this.mentor == null || mentor == null))
            {
                this.DetachFromMentor(this.mentor);
                this.mentor = mentor;
                this.AttachToMentor(this.mentor);
                this.SourceAcquired();
            }
            if (this.mentor != null && (this.binding.Source != null || this.binding.ElementName != null ? true : this.binding.RelativeSource != null))
            {
                this.targetDO.InheritanceContextChanged -= new EventHandler(this.InheritanceContextChanged);
            }
        }

        private void NotifyChildDataErrorInfo_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                this.NotifyOldDataErrorInfos(true);
                if (this.listener != null)
                {
                    INotifyDataErrorInfo leafValue = this.listener.LeafValue as INotifyDataErrorInfo;
                    if (leafValue != null)
                    {
                        this.NotifyNewDataErrorInfos(string.Empty, leafValue, true);
                    }
                }
            }
        }

        private void NotifyDataErrorInfo_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (this.listener != null && this.listener.FullPathExists)
            {
                string empty = string.Empty;
                INotifyDataErrorInfo leafItem = this.listener.LeafItem as INotifyDataErrorInfo;
                if (leafItem != null)
                {
                    empty = this.listener.LeafPropertyName;
                }
                if (string.CompareOrdinal(empty, e.PropertyName) == 0 || (e.PropertyName == null && empty == string.Empty))
                {
                    this.NotifyOldDataErrorInfos(false);
                    this.NotifyNewDataErrorInfos(empty, leafItem, false);
                }
            }
        }

        private void NotifyNewDataErrorInfos(string propertyName, INotifyDataErrorInfo notifyDataErrorInfo, bool isNotifyChildDataErrorInfo)
        {
            IEnumerable errors = null;
            try
            {
                errors = notifyDataErrorInfo.GetErrors(propertyName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                if (e is OutOfMemoryException || e is StackOverflowException || e is AccessViolationException || e is ThreadAbortException)
                {
                    throw;
                }
            }
            this.NotifyNewDataErrorInfos(this.RegisterErrorCollection(propertyName, errors, isNotifyChildDataErrorInfo));
        }

        private void NotifyNewDataErrorInfos(Collection<ValidationError> validationErrors)
        {
            if (validationErrors == null || this.lastSourceUpdateThrewException)
            {
                return;
            }
            foreach (ValidationError validationError in validationErrors)
            {
                this.AddErrorNotificationToQueue(ValidationErrorEventAction.Added, validationError);
                this.AddErrorToTarget(validationError, true);
            }
            this.RaiseErrorNotifications();
        }

        private void NotifyOldDataErrorInfos(bool isNotifyChildDataErrorInfo)
        {
            Collection<ValidationError> validationErrors;
            if (!isNotifyChildDataErrorInfo)
            {
                validationErrors = this.currentNotifyValidationErrors;
                this.currentNotifyValidationErrors = null;
            }
            else
            {
                validationErrors = this.currentNotifyChildValidationErrors;
                this.currentNotifyChildValidationErrors = null;
            }
            if (validationErrors != null)
            {
                this.NotifyOldDataErrorInfos(validationErrors);
            }
        }

        private void NotifyOldDataErrorInfos(Collection<ValidationError> validationErrors)
        {
            if (validationErrors == null || this.lastSourceUpdateThrewException)
            {
                return;
            }
            foreach (ValidationError validationError in validationErrors)
            {
                this.AddErrorNotificationToQueue(ValidationErrorEventAction.Removed, validationError);
                this.RemoveErrorFromTarget(validationError);
            }
            this.RaiseErrorNotifications();
        }

        private void RaiseErrorNotifications()
        {
            if (this.errorNotifications == null || this.TargetElement == null)
            {
                return;
            }
            List<ValidationErrorEventArgs> validationErrorEventArgs = new List<ValidationErrorEventArgs>(this.errorNotifications);
            this.errorNotifications = null;
            foreach (ValidationErrorEventArgs validationErrorEventArg in validationErrorEventArgs)
            {
                if (this.TargetElement == null)
                {
                    continue;
                }
                this.TargetElement.RaiseBindingValidationError(validationErrorEventArg);
            }
        }

        private Collection<ValidationError> RegisterErrorCollection(string propertyName, IEnumerable errors, bool isNotifyChildDataErrorInfo)
        {
            if (errors == null)
            {
                return null;
            }
            Collection<ValidationError> validationErrors = new Collection<ValidationError>();
            try
            {
                foreach (object error in errors)
                {
                    if (error == null)
                    {
                        continue;
                    }
                    validationErrors.Add(new ValidationError(error));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                if (e is OutOfMemoryException || e is StackOverflowException || e is AccessViolationException || e is ThreadAbortException)
                {
                    throw;
                }
            }
            if (!isNotifyChildDataErrorInfo)
            {
                this.currentNotifyValidationErrors = validationErrors;
            }
            else
            {
                this.currentNotifyChildValidationErrors = validationErrors;
            }
            return validationErrors;
        }

        private void RemoveNotifyDataErrorInfosFromTarget()
        {
            this.RemoveNotifyDataErrorInfosFromTarget(this.currentNotifyChildValidationErrors);
            this.RemoveNotifyDataErrorInfosFromTarget(this.currentNotifyValidationErrors);
        }

        private void RemoveNotifyDataErrorInfosFromTarget(Collection<ValidationError> validationErrors)
        {
            if (validationErrors == null)
            {
                return;
            }
            foreach (ValidationError validationError in validationErrors)
            {
                this.RemoveErrorFromTarget(validationError);
            }
        }

        private void ResetNotifyDataErrorInfo()
        {
            this.ResetNotifyDataErrorInfo(true, true);
        }

        private void ResetNotifyDataErrorInfo(bool resetCurrentNotifyDataErrorInfo, bool resetCurrentNotifyChildDataErrorInfo)
        {
            if (resetCurrentNotifyDataErrorInfo & this.currentNotifyDataErrorInfo != null)
            {
                if (this.errorsChangedListener != null)
                {
                    this.errorsChangedListener.Disconnect();
                    this.errorsChangedListener = null;
                }
                this.currentNotifyDataErrorInfo = null;
            }
            if (resetCurrentNotifyChildDataErrorInfo && this.currentNotifyChildDataErrorInfo != null)
            {
                if (this.childErrorsChangedListener != null)
                {
                    this.childErrorsChangedListener.Disconnect();
                    this.childErrorsChangedListener = null;
                }
                this.currentNotifyChildDataErrorInfo = null;
            }
            if (resetCurrentNotifyDataErrorInfo)
            {
                this.NotifyOldDataErrorInfos(false);
            }
            if (resetCurrentNotifyChildDataErrorInfo)
            {
                this.NotifyOldDataErrorInfos(true);
            }
        }

        private void SourceAcquired()
        {
            this.listener.ReConnect(this.EffectiveSource);
            if (this.binding.ValidatesOnNotifyDataErrors)
            {
                this.ConsumeNotifyDataErrorInfo();
            }
            this.SendDataToTarget();
        }

        private void TargetLoaded(object o, RoutedEventArgs e)
        {
            this.TargetElement.Loaded -= new RoutedEventHandler(this.TargetLoaded);
            this.targetDO.InheritanceContextChanged -= new EventHandler(this.InheritanceContextChanged);
            this.SourceForBinding = this.GetSourceElement();
            if (this.EffectiveSource != null)
            {
                this.SourceAcquired();
            }
        }

        private void UpdateNotifyDataErrorInfos()
        {
            INotifyDataErrorInfo notifyDataErrorInfo;
            INotifyDataErrorInfo notifyDataErrorInfo1;
            INotifyDataErrorInfo notifyDataErrorInfo2;
            INotifyDataErrorInfo notifyDataErrorInfo3;
            if (this.binding.ValidatesOnNotifyDataErrors)
            {
                string str = null;
                this.GetNotifyDataErrorInfoInstances(out str, out notifyDataErrorInfo, out notifyDataErrorInfo1);
                bool flag = notifyDataErrorInfo != this.currentNotifyDataErrorInfo;
                bool flag1 = notifyDataErrorInfo1 != this.currentNotifyChildDataErrorInfo;
                if (flag || flag1)
                {
                    this.ResetNotifyDataErrorInfo(flag, flag1);
                    this.currentNotifyDataErrorInfo = notifyDataErrorInfo;
                    this.currentNotifyChildDataErrorInfo = notifyDataErrorInfo1;
                    string str1 = str;
                    if (flag)
                    {
                        notifyDataErrorInfo2 = this.currentNotifyDataErrorInfo;
                    }
                    else
                    {
                        notifyDataErrorInfo2 = null;
                    }
                    if (flag1)
                    {
                        notifyDataErrorInfo3 = this.currentNotifyChildDataErrorInfo;
                    }
                    else
                    {
                        notifyDataErrorInfo3 = null;
                    }
                    this.ConsumeNotifyDataErrorInfo(str1, notifyDataErrorInfo2, notifyDataErrorInfo3);
                }
            }
        }

        private void UpdateValue()
        {
            Exception innerException = null;
            string dataErrorInfoMessage = null;
            CultureInfo cultureInfo = null;
            bool flag = false;
            if (this.listener == null || !this.listener.FullPathExists)
            {
                return;
            }
            this.updateState = BindingExpression.UpdateState.UpdatingSource;
            try
            {
                object value = this.targetDO.GetValue(this.targetProperty);
                if (this.binding.TargetNullValue != null)
                {
                    object targetNullValue = null;
                    try
                    {
                        Type type = this.binding.TargetNullValue.GetType();
                        if (type == this.targetProperty.PropertyType || this.targetProperty.PropertyType.IsAssignableFrom(type))
                        {
                            targetNullValue = this.binding.TargetNullValue;
                        }
                        else
                        {
                            if (cultureInfo == null)
                            {
                                cultureInfo = this.CalculateCulture();
                            }
                            targetNullValue = this.DynamicConverter.Convert(this.binding.TargetNullValue, this.targetProperty.PropertyType, null, cultureInfo);
                        }
                        if (object.Equals(value, targetNullValue))
                        {
                            value = null;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
                if (this.binding.Converter != null)
                {
                    if (cultureInfo == null)
                    {
                        cultureInfo = this.CalculateCulture();
                    }
                    value = this.binding.Converter.ConvertBack(value, this.listener.LeafType, this.binding.ConverterParameter, cultureInfo);
                    if (value == DependencyProperty.UnsetValue)
                    {
                        return;
                    }
                }
                if (value != null && value is string)
                {
                    if (cultureInfo == null)
                    {
                        cultureInfo = this.CalculateCulture();
                    }
                    object obj = BindingExpression.TryParse(value, this.listener.LeafType, cultureInfo);
                    if (obj != DependencyProperty.UnsetValue)
                    {
                        value = obj;
                    }
                }
                try
                {
                    if (!this.IsValidValueForUpdate(value, this.listener.LeafType))
                    {
                        if (cultureInfo == null)
                        {
                            cultureInfo = this.CalculateCulture();
                        }
                        flag = true;
                        value = this.DynamicConverter.Convert(value, this.listener.LeafType, null, cultureInfo);
                        flag = false;
                    }
                    this.listener.LeafValue = value;
                }
                catch (TargetInvocationException targetInvocationException)
                {
                    Debug.WriteLine(targetInvocationException.ToString());
                    innerException = targetInvocationException.InnerException;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    if (e is OutOfMemoryException || e is StackOverflowException || e is AccessViolationException || e is ThreadAbortException)
                    {
                        throw;
                    }
                }
            }
            finally
            {
                if (this.binding.ValidatesOnDataErrors && (innerException == null || !this.binding.ValidatesOnExceptions))
                {
                    dataErrorInfoMessage = this.GetDataErrorInfoMessage();
                }
                this.updateState = BindingExpression.UpdateState.None;
                this.targetPropertyState = BindingExpression.TargetPropertyState.Clean;
            }
            ValidationError validationError = null;
            if (innerException != null && this.binding.ValidatesOnExceptions)
            {
                validationError = new ValidationError(innerException, flag);
            }
            else if (!string.IsNullOrEmpty(dataErrorInfoMessage))
            {
                validationError = new ValidationError(dataErrorInfoMessage);
            }
            bool flag1 = innerException != null;
            if (flag1 && !this.lastSourceUpdateThrewException)
            {
                this.AddNotifyDataErrorInfosToQueue(ValidationErrorEventAction.Removed);
            }
            this.AddErrorNotificationToQueue(ValidationErrorEventAction.Removed, this.currentValidationError);
            if (!flag1 && this.lastSourceUpdateThrewException)
            {
                this.AddNotifyDataErrorInfosToQueue(ValidationErrorEventAction.Added);
            }
            this.AddErrorNotificationToQueue(ValidationErrorEventAction.Added, validationError);
            ValidationError validationError1 = this.currentValidationError;
            if (this.lastSourceUpdateThrewException && !flag1)
            {
                this.AddNotifyDataErrorInfosToTarget();
            }
            this.AddErrorToTarget(validationError, false);
            this.RemoveErrorFromTarget(validationError1);
            if (!this.lastSourceUpdateThrewException && flag1)
            {
                this.RemoveNotifyDataErrorInfosFromTarget();
            }
            this.lastSourceUpdateThrewException = flag1;
            this.RaiseErrorNotifications();
        }

        private void UpdateValueIfNecessary()
        {
            if (this.listeningToLostFocus)
            {
                this.targetPropertyState = TargetPropertyState.Dirty;
                if (this.IsTargetFocused())
                {
                    return;
                }
            }

            this.UpdateValue();
        }
    }  
}
