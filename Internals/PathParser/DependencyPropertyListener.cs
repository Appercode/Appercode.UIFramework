using System;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal class DependencyPropertyListener : PropertyListener
    {
        private readonly DependencyProperty property;

        private readonly Type sourceType;

        private DependencyObject source;

        private object value = DependencyProperty.UnsetValue;

        private WeakDependencyPropertyChangedListener propertyChangedListener;        

        internal DependencyPropertyListener(IRaisePropertyPathStepChanged pathStep, Type sourceType, DependencyProperty dp, bool listenToChanges)
            : base(pathStep, listenToChanges)
        {
            this.property = dp;
            this.sourceType = sourceType;
        }

        internal override object Property
        {
            get
            {
                return this.property;
            }
        }

        internal override Type PropertyType
        {
            get
            {
                return this.property.PropertyType;
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
                this.source = (DependencyObject)value;
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
                    this.value = this.source.GetValue(this.property);
                }
                return this.value;
            }
            set
            {
                this.value = value;
                this.source.SetValue(this.property, this.value);
            }
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

        internal void SourcePropertyChanged(object sender, DependencyProperty dp)
        {
            if (this.property == dp)
            {
                this.value = DependencyProperty.UnsetValue;
                this.PathStep.RaisePropertyPathStepChanged(this);
            }
        }

        private void ConnectToProperty()
        {
            if (this.listenToChanges && this.source != null)
            {
                this.propertyChangedListener = WeakDependencyPropertyChangedListener.CreateIfNecessary(this.source, false, this);
            }
        }
    }
}
