using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace Appercode.UI.Internals.CollectionView
{
    internal class CollectionViewGroupRoot : CollectionViewGroupInternal, INotifyCollectionChanged
    {
        private static readonly object UseAsItemDirectly = new object();
        private ObservableCollection<GroupDescription> groupBy = new ObservableCollection<GroupDescription>();
        private Appercode.UI.Data.CollectionView view;
        private IComparer<object> comparer;

        internal CollectionViewGroupRoot(Appercode.UI.Data.CollectionView view)
            : base((object)"Root", (CollectionViewGroupInternal)null)
        {
            this.view = view;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        internal event EventHandler GroupDescriptionChanged;

        public virtual ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this.groupBy;
            }
        }

        internal IComparer<object> ActiveComparer
        {
            get
            {
                return this.comparer;
            }
            set
            {
                this.comparer = value;
            }
        }

        internal CultureInfo Culture
        {
            get
            {
                return this.view.Culture;
            }
        }

        public void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (this.CollectionChanged == null)
            {
                return;
            }
            this.CollectionChanged((object)this, args);
        }

        internal void Initialize()
        {
            this.InitializeGroup((CollectionViewGroupInternal)this, 0);
        }

        internal void AddToSubgroups(object item, bool loading)
        {
            this.AddToSubgroups(item, (CollectionViewGroupInternal)this, 0, loading);
        }

        internal bool RemoveFromSubgroups(object item)
        {
            return this.RemoveFromSubgroups(item, (CollectionViewGroupInternal)this, 0);
        }

        internal void RemoveItemFromSubgroupsByExhaustiveSearch(object item)
        {
            this.RemoveItemFromSubgroupsByExhaustiveSearch((CollectionViewGroupInternal)this, item);
        }

        internal void InsertSpecialItem(int index, object item, bool loading)
        {
            this.ChangeCounts(item, 1);
            this.ProtectedItems.Insert(index, item);
            if (loading)
            {
                return;
            }
            int index1 = this.LeafIndexFromItem(item, index);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index1));
        }

        internal void RemoveSpecialItem(int index, object item, bool loading)
        {
            int index1 = -1;
            if (!loading)
            {
                index1 = this.LeafIndexFromItem(item, index);
            }
            this.ChangeCounts(item, -1);
            this.ProtectedItems.RemoveAt(index);
            if (loading)
            {
                return;
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index1));
        }

        protected override void OnGroupByChanged()
        {
            if (this.GroupDescriptionChanged == null)
            {
                return;
            }
            this.GroupDescriptionChanged((object)this, EventArgs.Empty);
        }

        protected override int FindIndex(object item, object seed, IComparer<object> comparer, int low, int high)
        {
            System.ComponentModel.IEditableCollectionView editableCollectionView = this.view as System.ComponentModel.IEditableCollectionView;
            if (editableCollectionView != null && editableCollectionView.IsAddingNew)
            {
                --high;
            }
            return base.FindIndex(item, seed, comparer, low, high);
        }

        private void InitializeGroup(CollectionViewGroupInternal group, int level)
        {
            GroupDescription groupDescription = level < this.GroupDescriptions.Count ? this.GroupDescriptions[level] : (GroupDescription)null;
            group.GroupBy = groupDescription;
            ObservableCollection<object> observableCollection = groupDescription != null ? groupDescription.GroupNames : (ObservableCollection<object>)null;
            if (observableCollection != null)
            {
                int index = 0;
                for (int count = observableCollection.Count; index < count; ++index)
                {
                    CollectionViewGroupInternal group1 = new CollectionViewGroupInternal(observableCollection[index], group);
                    this.InitializeGroup(group1, level + 1);
                    group.Add((object)group1);
                }
            }
            group.LastIndex = 0;
        }

        private void AddToSubgroups(object item, CollectionViewGroupInternal group, int level, bool loading)
        {
            object groupName = this.GetGroupName(item, group.GroupBy, level);
            if (groupName == CollectionViewGroupRoot.UseAsItemDirectly)
            {
                if (loading)
                {
                    group.Add(item);
                }
                else
                {
                    int index1 = group.Insert(item, item, this.ActiveComparer);
                    int index2 = group.LeafIndexFromItem(item, index1);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index2));
                }
            }
            else
            {
                ICollection collection;
                if ((collection = groupName as ICollection) == null)
                {
                    this.AddToSubgroup(item, group, level, groupName, loading);
                }
                else
                {
                    foreach (object name in (IEnumerable)collection)
                    {
                        this.AddToSubgroup(item, group, level, name, loading);
                    }
                }
            }
        }

        private void AddToSubgroup(object item, CollectionViewGroupInternal group, int level, object name, bool loading)
        {
            int index = 0;
            for (int count = group.Items.Count; index < count; ++index)
            {
                CollectionViewGroupInternal group1 = group.Items[index] as CollectionViewGroupInternal;
                if (group1 != null && group.GroupBy.NamesMatch(group1.Name, name))
                {
                    group.LastIndex = index;
                    this.AddToSubgroups(item, group1, level + 1, loading);
                    return;
                }
            }
            CollectionViewGroupInternal group2 = new CollectionViewGroupInternal(name, group);
            this.InitializeGroup(group2, level + 1);
            if (loading)
            {
                group.Add((object)group2);
                group.LastIndex = index;
            }
            else
            {
                group.Insert((object)group2, item, this.ActiveComparer);
            }
            this.AddToSubgroups(item, group2, level + 1, loading);
        }

        private bool RemoveFromSubgroups(object item, CollectionViewGroupInternal group, int level)
        {
            bool flag = false;
            object groupName = this.GetGroupName(item, group.GroupBy, level);
            if (groupName == CollectionViewGroupRoot.UseAsItemDirectly)
            {
                flag = this.RemoveFromGroupDirectly(group, item);
            }
            else
            {
                ICollection collection;
                if ((collection = groupName as ICollection) == null)
                {
                    if (this.RemoveFromSubgroup(item, group, level, groupName))
                    {
                        flag = true;
                    }
                }
                else
                {
                    foreach (object name in (IEnumerable)collection)
                    {
                        if (this.RemoveFromSubgroup(item, group, level, name))
                        {
                            flag = true;
                        }
                    }
                }
            }
            return flag;
        }

        private bool RemoveFromSubgroup(object item, CollectionViewGroupInternal group, int level, object name)
        {
            bool flag = false;
            int index = 0;
            for (int count = group.Items.Count; index < count; ++index)
            {
                CollectionViewGroupInternal group1 = group.Items[index] as CollectionViewGroupInternal;
                if (group1 != null && group.GroupBy.NamesMatch(group1.Name, name))
                {
                    if (this.RemoveFromSubgroups(item, group1, level + 1))
                    {
                        flag = true;
                    }
                    return flag;
                }
            }
            return true;
        }

        private bool RemoveFromGroupDirectly(CollectionViewGroupInternal group, object item)
        {
            int index = group.Remove(item, true);
            if (index < 0)
            {
                return true;
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return false;
        }

        private void RemoveItemFromSubgroupsByExhaustiveSearch(CollectionViewGroupInternal group, object item)
        {
            if (!this.RemoveFromGroupDirectly(group, item))
            {
                return;
            }
            for (int index = group.Items.Count - 1; index >= 0; --index)
            {
                CollectionViewGroupInternal group1 = group.Items[index] as CollectionViewGroupInternal;
                if (group1 != null)
                {
                    this.RemoveItemFromSubgroupsByExhaustiveSearch(group1, item);
                }
            }
        }

        private object GetGroupName(object item, GroupDescription groupDescription, int level)
        {
            if (groupDescription != null)
            {
                return groupDescription.GroupNameFromItem(item, level, this.Culture);
            }
            return CollectionViewGroupRoot.UseAsItemDirectly;
        }
    }
}
