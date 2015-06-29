using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal class IndexerPathStep : PropertyPathStep, ICollectionChangedListener, IRaisePropertyPathStepChanged
    {
        private object source;

        private ICollectionView collectionView;

        private string index;

        private bool intIndexer;

        private bool listenToChanges;

        private PropertyInfo indexer;

        private WeakCollectionChangedListener collectionChangedListener;

        private PropertyInfo ilistIndexer;

        private PropertyListener propertyListener;

        internal IndexerPathStep(PropertyPathListener listener, object source, string index, bool listenToChanges) : base(listener)
        {
            this.source = source;
            this.index = index;
            this.listenToChanges = listenToChanges;
            this.ConnectToIndexer();
        }

        internal override bool IsConnected
        {
            get
            {
                if (this.indexer == null)
                {
                    return false;
                }
                return this.ValidIndex(this.index);
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
                PropertyInfo property = (PropertyInfo)this.propertyListener.Property;
                return string.Concat(property.Name, "[", this.index, "]");
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
                if (this.propertyListener == null)
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

        void ICollectionChangedListener.OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool flag = false;
            int num = 0;
            if (!int.TryParse(this.index.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out num))
            {
                flag = true;
            }
            else
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewStartingIndex > num)
                        {
                            break;
                        }
                        flag = true;
                        break;
                    }
                    case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldStartingIndex > num)
                        {
                            break;
                        }
                        flag = true;
                        break;
                    }
                    case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.NewStartingIndex != num)
                        {
                            break;
                        }
                        flag = true;
                        break;
                    }
                    case NotifyCollectionChangedAction.Reset:
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                this.Listener.RaisePropertyPathStepChanged(this);
            }
        }

        internal override void ReConnect(object newSource)
        {
            this.Disconnect();
            this.source = newSource;
            this.ConnectToIndexer();
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

        private void ConnectToIndexer()
        {
            if (this.source == null || this.source == DependencyProperty.UnsetValue)
            {
                return;
            }
            if (this.ConnectToIndexerInSource())
            {
                return;
            }
            ICollectionView collectionViews = this.source as ICollectionView;
            if (collectionViews != null)
            {
                this.collectionView = collectionViews;
                this.collectionView.CurrentChanged += new EventHandler(this.CurrentItemChanged);
                this.source = this.collectionView.CurrentItem;
                this.ConnectToIndexerInSource();
            }
        }

        private bool ConnectToIndexerInSource()
        {
            if (this.source == null || this.source == DependencyProperty.UnsetValue)
            {
                return false;
            }
            this.indexer = this.FindIndexerProperty(this.source);
            if (this.indexer == null)
            {
                return false;
            }
            if (this.propertyListener == null)
            {
                this.propertyListener = IndexerListener.CreateListener(this, this.source, this.index, this.intIndexer, this.indexer, this.listenToChanges);
            }
            if (this.listenToChanges)
            {
                this.collectionChangedListener = WeakCollectionChangedListener.CreateIfNecessary(this.source, this);
            }
            return true;
        }

        private void CurrentItemChanged(object o, EventArgs e)
        {
            this.DisconnectListener();
            this.source = this.collectionView.CurrentItem;
            this.ConnectToIndexerInSource();
            this.Listener.RaisePropertyPathStepChanged(this);
        }

        private void DisconnectListener()
        {
            this.indexer = null;
            if (this.collectionChangedListener != null)
            {
                this.collectionChangedListener.Disconnect();
                this.collectionChangedListener = null;
            }
            if (this.propertyListener != null)
            {
                this.propertyListener.Disconnect();
                this.propertyListener = null;
            }
        }

        private PropertyInfo FindIndexerInMembers(params MemberInfo[] members)
        {
            PropertyInfo propertyInfo = null;
            MemberInfo[] memberInfoArray = members;
            for (int i = 0; i < (int)memberInfoArray.Length; i++)
            {
                PropertyInfo propertyInfo1 = (PropertyInfo)memberInfoArray[i];
                if (propertyInfo1 != null)
                {
                    ParameterInfo[] indexParameters = propertyInfo1.GetIndexParameters();
                    if ((int)indexParameters.Length <= 1)
                    {
                        if (indexParameters[0].ParameterType == typeof(int))
                        {
                            int num = -1;
                            if (int.TryParse(this.index.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out num))
                            {
                                this.intIndexer = true;
                                return propertyInfo1;
                            }
                        }
                        if (indexParameters[0].ParameterType == typeof(string))
                        {
                            propertyInfo = propertyInfo1;
                        }
                    }
                }
            }
            return propertyInfo;
        }

        private PropertyInfo FindIndexerProperty(object source)
        {
            var sourceType = source.GetType();
            var listIndexer = this.FindIndexerInMembers(sourceType.GetDefaultMembers());
            if (listIndexer == null)
            {
                // trying to get the property explicitly, useful with Xamarin linker behaviour
                listIndexer = this.FindIndexerInMembers(sourceType.GetProperty("Item"));
                if (listIndexer == null && source is IList)
                {
                    // TODO: get IList's indexer property explicitly, if you know its name
                    listIndexer = this.GetIListIndexer();
                }
            }

            return listIndexer;
        }

        private PropertyInfo GetIListIndexer()
        {
            if (this.ilistIndexer == null)
            {
                this.ilistIndexer = this.FindIndexerInMembers(typeof(IList).GetDefaultMembers());
                this.intIndexer = true;
            }
            return this.ilistIndexer;
        }

        private bool ValidIndex(string index)
        {
            bool flag;
            int num = 0;
            if (this.indexer != null)
            {
                IList lists = this.source as IList;
                if (lists != null && int.TryParse(index.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out num))
                {
                    return lists.Count > num;
                }
                if (index != null)
                {
                    try
                    {
                        if (this.Value != DependencyProperty.UnsetValue)
                        {
                            return true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    catch
                    {
                        flag = false;
                    }
                    return flag;
                }
                string str = this.source as string;
                if (str != null)
                {
                    return str.Length > int.Parse(index.ToString().Trim(), CultureInfo.InvariantCulture);
                }
            }
            return false;
        }
    }
}
