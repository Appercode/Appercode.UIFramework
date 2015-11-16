using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
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
            this.ConnectToPropertyInSource(false);
        }

        internal PropertyAccessPathStep(PropertyPathListener listener, object source, DependencyProperty property, bool listenToChanges)
            : base(listener)
        {
            this.source = source as DependencyObject;
            this.property = property;
            this.propertyName = property.Name;
            this.listenToChanges = listenToChanges;
            this.ConnectToPropertyInSource(false);
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
            this.ConnectToPropertyInSource(false);
        }

        internal override void Disconnect()
        {
            this.DisconnectListener();
            if (this.collectionView != null)
            {
                this.collectionView.CurrentChanged -= this.CurrentItemChanged;
            }

            this.collectionView = null;
            this.source = null;
        }

        private bool ConnectToPropertyInSource(bool isSourceCollectionViewCurrentItem)
        {
            if (this.source == null || this.source == DependencyProperty.UnsetValue)
            {
                return false;
            }

            var sourceType = this.source.GetType();
            if (this.propertyListener == null || this.propertyListener.SourceType != sourceType)
            {
                this.propertyListener = null;
                var dependencyObject = this.source as DependencyObject;
                if (dependencyObject != null)
                {
                    var dependencyProperty = this.property ?? DependencyProperty.GetRegisteredDependencyProperty(dependencyObject, this.propertyName);
                    if (dependencyProperty != null)
                    {
                        this.propertyListener = new DependencyPropertyListener(this, sourceType, dependencyProperty, this.listenToChanges);
                    }
                }

                if (this.propertyListener == null && this.propertyName != null)
                {
                    var property = sourceType.GetProperty(this.propertyName);
                    if (property != null)
                    {
                        this.propertyListener = new CLRPropertyListener(this, sourceType, property, this.listenToChanges);
                    }
                }
            }

            if (this.propertyListener == null)
            {
                if (!isSourceCollectionViewCurrentItem)
                {
                    var collectionViews = this.source as ICollectionView;
                    if (collectionViews != null)
                    {
                        this.source = collectionViews.CurrentItem;
                        this.collectionView = collectionViews;
                        this.collectionView.CurrentChanged += this.CurrentItemChanged;
                        return this.ConnectToPropertyInSource(true);
                    }
                }
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
                    Debug.WriteLine(e);
                    if (e is OutOfMemoryException || e is StackOverflowException || e is AccessViolationException || e is ThreadAbortException)
                    {
                        throw;
                    }

                    this.Disconnect();
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