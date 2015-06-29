using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal class PropertyAccessPathStep : PropertyPathStep, IRaisePropertyPathStepChanged
    {
        private readonly string propertyName;

        private readonly DependencyProperty property;

        private readonly bool listenToChanges;

        private object source;

        private ICollectionView collectionView;

        private PropertyListener propertyListener;

        internal PropertyAccessPathStep(PropertyPathListener listener, object source, string propertyName, bool listenToChanges)
            : base(listener)
        {
            this.source = source;
            this.propertyName = propertyName;
            this.listenToChanges = listenToChanges;
            this.ConnectToProperty();
        }

        internal PropertyAccessPathStep(PropertyPathListener listener, object source, DependencyProperty property, bool listenToChanges)
            : base(listener)
        {
            this.source = source as DependencyObject;
            this.property = property;
            this.propertyName = property.Name;
            this.listenToChanges = listenToChanges;
            this.ConnectToProperty();
        }

        internal override bool IsConnected
        {
            get
            {
                if (this.propertyListener == null)
                {
                    return false;
                }
                return this.propertyListener.Source != null;
            }
        }

        internal override object Property
        {
            get
            {
                return this.propertyListener.Property;
            }
        }

        internal override string PropertyName
        {
            get
            {
                PropertyInfo property = this.Property as PropertyInfo;
                if (property != null)
                {
                    return property.Name;
                }
                DependencyProperty dependencyProperty = this.Property as DependencyProperty;
                if (dependencyProperty == null)
                {
                    return string.Empty;
                }
                return dependencyProperty.Name;
            }
        }

        internal override object Source
        {
            get
            {
                return this.source;
            }
        }

        internal override Type Type
        {
            get
            {
                return this.propertyListener.PropertyType;
            }
        }

        internal override object Value
        {
            get
            {
                if (!this.IsConnected)
                {
                    return DependencyProperty.UnsetValue;
                }
                return this.propertyListener.Value;
            }
            set
            {
                this.propertyListener.Value = value;
            }
        }

        public void RaisePropertyPathStepChanged(PropertyListener source)
        {
            this.Listener.RaisePropertyPathStepChanged(this);
        }

        internal override void ReConnect(object newSource)
        {
            this.Disconnect();
            this.source = newSource;
            this.ConnectToProperty();
        }

        internal override void Disconnect()
        {
            this.DisconnectListener();
            if (this.collectionView != null)
            {
                this.collectionView.CurrentChanged -= new EventHandler(this.CurrentItemChanged);
            }
            this.collectionView = null;
            this.source = null;
        }

        [SecuritySafeCritical]
        private static DependencyProperty GetRegisteredDependencyProperty(DependencyObject d, string propertyName)
        {
            return DependencyProperty.GetRegisteredDependencyProperty(d, propertyName, null, null);
        }

        private void ConnectToProperty()
        {
            if (this.source == null || this.source == DependencyProperty.UnsetValue)
            {
                return;
            }
            this.ConnectToPropertyInSource(false);
        }

        private bool ConnectToPropertyInSource(bool isSourceCollectionViewCurrentItem)
        {
            if (this.source == null || this.source == DependencyProperty.UnsetValue)
            {
                return false;
            }
            if (this.propertyListener == null || this.propertyListener.SourceType != this.source.GetType())
            {
                this.propertyListener = null;
                if (this.property != null && this.source is DependencyObject)
                {
                    this.propertyListener = new DependencyPropertyListener(this, this.source.GetType(), this.property, this.listenToChanges);
                }
                else if (this.source is DependencyObject)
                {
                    DependencyObject dependencyObject = (DependencyObject)this.source;
                    DependencyProperty dependencyProperty = PropertyAccessPathStep.GetRegisteredDependencyProperty(dependencyObject, this.propertyName);
                    if (dependencyProperty != null)
                    {
                        this.propertyListener = new DependencyPropertyListener(this, this.source.GetType(), dependencyProperty, this.listenToChanges);
                    }
                }
                if (this.propertyListener == null && this.propertyName != null)
                {
                    PropertyInfo property = this.source.GetType().GetProperty(this.propertyName);
                    if (property != null)
                    {
                        this.propertyListener = new CLRPropertyListener(this, this.source.GetType(), property, this.listenToChanges);
                    }
                }
            }
            if (this.propertyListener == null)
            {
                if (!isSourceCollectionViewCurrentItem)
                {
                    ICollectionView collectionViews = this.source as ICollectionView;
                    if (collectionViews != null)
                    {
                        this.source = collectionViews.CurrentItem;
                        this.collectionView = collectionViews;
                        this.collectionView.CurrentChanged += new EventHandler(this.CurrentItemChanged);
                        return this.ConnectToPropertyInSource(true);
                    }
                }
                string str = this.propertyName;
                string str1 = this.source != null ? this.source.ToString() : "null";
                string str2 = this.source != null ? this.source.GetType().ToString() : "null";
                string str3 = this.source != null ? this.source.GetHashCode().ToString() : "0";
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
            }
            else
            {
                this.propertyListener.Source = this.source;
                try
                {
                    object value = this.propertyListener.Value;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    if (e is OutOfMemoryException || e is StackOverflowException || e is AccessViolationException || e is ThreadAbortException)
                    {
                        throw;
                    }
                    string propertyName = this.propertyListener.PropertyName;
                    string str5 = this.propertyListener.PropertyType.ToString();
                    string str6 = this.source != null ? this.source.ToString() : "null";
                    string str7 = this.source != null ? this.source.GetType().ToString() : "null";
                    this.Disconnect();
                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                }
            }
            return this.propertyListener != null;
        }

        private void CurrentItemChanged(object o, EventArgs e)
        {
            this.DisconnectListener();
            this.source = this.collectionView.CurrentItem;
            this.ConnectToPropertyInSource(true);
            this.RaisePropertyPathStepChanged(null);
        }

        private void DisconnectListener()
        {
            if (this.propertyListener != null)
            {
                this.propertyListener.Disconnect();
            }
        }
    }
}