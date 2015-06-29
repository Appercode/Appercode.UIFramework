using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal class IndexerListener : PropertyListener, ISourcePropertyChanged
    {
        private readonly PropertyInfo propertyInfo;

        private readonly Type sourceType;

        private object source;

        private WeakPropertyChangedListener propertyChangedListener;

        private object value = DependencyProperty.UnsetValue;

        private string index;

        private bool intIndexer;

        private IndexerListener(IRaisePropertyPathStepChanged pathStep, object source, string index, bool intIndexer, PropertyInfo propertyInfo, bool listenToChanges)
            : base(pathStep, listenToChanges)
        {
            this.propertyInfo = propertyInfo;
            this.source = source;
            this.index = index;
            this.intIndexer = intIndexer;
            this.ConnectToProperty();
        }

        private IndexerListener(IRaisePropertyPathStepChanged pathStep, object source, string index, bool intIndexer, PropertyInfo propertyInfo, bool listenToChanges, Type sourceType)
            : base(pathStep, listenToChanges)
        {
            this.propertyInfo = propertyInfo;
            this.source = source;
            this.index = index;
            this.intIndexer = intIndexer;
            this.sourceType = sourceType;
            this.ConnectToProperty();
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
                object unsetValue;
                if (this.propertyInfo != null)
                {
                    try
                    {
                        if (this.value == DependencyProperty.UnsetValue)
                        {
                            if (!this.intIndexer)
                            {
                                PropertyInfo propertyInfo = this.propertyInfo;
                                object obj = this.source;
                                object[] objArray = new object[] { this.index };
                                this.value = propertyInfo.GetValue(obj, objArray);
                            }
                            else
                            {
                                PropertyInfo propertyInfo1 = this.propertyInfo;
                                object obj1 = this.source;
                                object[] objArray1 = new object[] { int.Parse(this.index.ToString().Trim(), CultureInfo.InvariantCulture) };
                                this.value = propertyInfo1.GetValue(obj1, objArray1);
                            }
                        }
                        return this.value;
                    }
                    catch
                    {
                        unsetValue = DependencyProperty.UnsetValue;
                    }
                    return unsetValue;
                }
                return this.value;
            }
            set
            {
                if (!this.intIndexer)
                {
                    PropertyInfo propertyInfo = this.propertyInfo;
                    object obj = this.source;
                    object[] objArray = new object[] { this.index };
                    propertyInfo.SetValue(obj, value, objArray);
                }
                else
                {
                    PropertyInfo propertyInfo1 = this.propertyInfo;
                    object obj1 = this.source;
                    object[] objArray1 = new object[] { int.Parse(this.index.ToString().Trim(), CultureInfo.InvariantCulture) };
                    propertyInfo1.SetValue(obj1, value, objArray1);
                }
                this.value = value;
            }
        }

        public void SourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            bool flag = string.IsNullOrEmpty(args.PropertyName) ? true : args.PropertyName == this.propertyInfo.Name;
            if (!flag)
            {
                    flag = args.PropertyName == this.PropertyName ? true : args.PropertyName == string.Concat(this.propertyInfo.Name, "[]");
            }
            if (flag)
            {
                this.value = DependencyProperty.UnsetValue;
                this.PathStep.RaisePropertyPathStepChanged(this);
            }
        }

        internal static IndexerListener CreateListener(IRaisePropertyPathStepChanged pathStep, object source, string index, bool intIndexer, PropertyInfo propertyInfo, bool listenToChanges)
        {
            return new IndexerListener(pathStep, source, index, intIndexer, propertyInfo, listenToChanges);
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

        private void ConnectToProperty()
        {
            if (this.listenToChanges)
            {
                this.propertyChangedListener = WeakPropertyChangedListener.CreateIfNecessary(this.source, this);
            }
        }
    }
}