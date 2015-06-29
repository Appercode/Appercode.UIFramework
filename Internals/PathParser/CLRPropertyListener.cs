using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal class CLRPropertyListener : PropertyListener, ISourcePropertyChanged
    {
        private readonly Type sourceType;

        private readonly PropertyInfo propertyInfo;

        private object source;

        private object value = DependencyProperty.UnsetValue;

        private WeakPropertyChangedListener propertyChangedListener;

        internal CLRPropertyListener(IRaisePropertyPathStepChanged pathStep, Type sourceType, PropertyInfo property, bool listenToChanges)
            : base(pathStep, listenToChanges)
        {
            this.sourceType = sourceType;
            this.propertyInfo = property;
        }

        internal override object Property
        {
            get
            {
                return this.propertyInfo;
            }
        }

        internal override Type PropertyType
        {
            get
            {
                return this.propertyInfo.PropertyType;
            }
        }

        internal override object Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.Disconnect();
                this.source = value;
                this.ConnectToProperty();
            }
        }

        internal override Type SourceType
        {
            get
            {
                return this.sourceType;
            }
        }

        internal override object Value
        {
            get
            {
                if (this.value == DependencyProperty.UnsetValue)
                {
                    this.value = this.propertyInfo.GetValue(this.source, null);
                }
                return this.value;
            }
            set
            {
                this.value = value;
                this.propertyInfo.SetValue(this.source, this.value, null);
            }
        }

        public void SourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == this.propertyInfo.Name)
            {
                this.value = DependencyProperty.UnsetValue;
                this.PathStep.RaisePropertyPathStepChanged(this);
            }
        }

        internal static CLRPropertyListener CreateListener(IRaisePropertyPathStepChanged pathStep, string name, object source, bool listenToChanges)
        {
            PropertyInfo property = CLRPropertyListener.GetProperty(source.GetType(), name);
            if (property == null)
            {
                return null;
            }
            return new CLRPropertyListener(pathStep, source.GetType(), property, listenToChanges);
        }

        internal override void Disconnect()
        {
            if (this.propertyChangedListener != null)
            {
                this.propertyChangedListener.Disconnect();
            }
            this.value = DependencyProperty.UnsetValue;
            this.source = null;
            this.propertyChangedListener = null;
        }

        private static PropertyInfo GetProperty(Type type, string propertyName)
        {
            PropertyInfo property;
            try
            {
                property = type.GetProperty(propertyName);
            }
            catch (AmbiguousMatchException)
            {
                property = CLRPropertyListener.GetShadowedProperty(type, propertyName);
            }
            return property;
        }

        private static PropertyInfo GetShadowedProperty(Type type, string propertyName)
        {
            for (Type i = type; i != null; i = i.BaseType)
            {
                PropertyInfo property = i.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                if (property != null)
                {
                    return property;
                }
            }
            return null;
        }

        private void ConnectToProperty()
        {
            if (this.listenToChanges)
            {
                this.propertyChangedListener = WeakPropertyChangedListener.CreateIfNecessary(this.source, this);
            }
        }
    }
}
