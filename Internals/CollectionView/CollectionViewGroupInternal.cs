using Appercode.UI.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Appercode.UI.Internals.CollectionView
{
    internal class CollectionViewGroupInternal : CollectionViewGroup
    {
        private int fullCount = 1;
        private GroupDescription groupBy;
        private CollectionViewGroupInternal parentGroup;
        private int lastIndex;
        private int version;

        internal CollectionViewGroupInternal(object name, CollectionViewGroupInternal parent)
            : base(name)
        {
            this.parentGroup = parent;
        }

        public override bool IsBottomLevel
        {
            get
            {
                return this.groupBy == null;
            }
        }

        internal GroupDescription GroupBy
        {
            get
            {
                return this.groupBy;
            }
            set
            {
                bool isBottomLevel = this.IsBottomLevel;
                if (this.groupBy != null)
                {
                    this.groupBy.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(this.OnGroupByChanged);
                }
                this.groupBy = value;
                if (this.groupBy != null)
                {
                    this.groupBy.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.OnGroupByChanged);
                }
                if (isBottomLevel == this.IsBottomLevel)
                {
                    return;
                }
                this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsBottomLevel"));
            }
        }

        internal int FullCount
        {
            get
            {
                return this.fullCount;
            }
            set
            {
                this.fullCount = value;
            }
        }

        internal int LastIndex
        {
            get
            {
                return this.lastIndex;
            }
            set
            {
                this.lastIndex = value;
            }
        }

        internal object SeedItem
        {
            get
            {
                if (this.ItemCount <= 0 || (this.GroupBy != null && this.GroupBy.GroupNames.Count != 0))
                {
                    return DependencyProperty.UnsetValue;
                }
                int index = 0;
                for (int count = this.Items.Count; index < count; ++index)
                {
                    CollectionViewGroupInternal viewGroupInternal = this.Items[index] as CollectionViewGroupInternal;
                    if (viewGroupInternal == null)
                    {
                        return this.Items[index];
                    }
                    if (viewGroupInternal.ItemCount > 0)
                    {
                        return viewGroupInternal.SeedItem;
                    }
                }
                return DependencyProperty.UnsetValue;
            }
        }

        private CollectionViewGroupInternal Parent
        {
            get
            {
                return this.parentGroup;
            }
        }

        internal void Add(object item)
        {
            this.ChangeCounts(item, 1);
            this.ProtectedItems.Add(item);
        }

        internal int Remove(object item, bool returnLeafIndex)
        {
            int num = -1;
            int index = this.ProtectedItems.IndexOf(item);
            if (index >= 0)
            {
                if (returnLeafIndex)
                {
                    num = this.LeafIndexFromItem((object)null, index);
                }
                this.ChangeCounts(item, -1);
                this.ProtectedItems.RemoveAt(index);
            }
            return num;
        }

        internal void Clear()
        {
            this.ProtectedItems.Clear();
            this.FullCount = 1;
            this.ProtectedItemCount = 0;
        }

        internal int LeafIndexOf(object item)
        {
            int num1 = 0;
            int index = 0;
            for (int count = this.Items.Count; index < count; ++index)
            {
                CollectionViewGroupInternal viewGroupInternal = this.Items[index] as CollectionViewGroupInternal;
                if (viewGroupInternal != null)
                {
                    int num2 = viewGroupInternal.LeafIndexOf(item);
                    if (num2 >= 0)
                    {
                        return num1 + num2;
                    }
                    num1 += viewGroupInternal.ItemCount;
                }
                else
                {
                    if (object.Equals(item, this.Items[index]))
                    {
                        return num1;
                    }
                    ++num1;
                }
            }
            return -1;
        }

        internal int LeafIndexFromItem(object item, int index)
        {
            int num = 0;
            CollectionViewGroupInternal viewGroupInternal1 = this;
            while (viewGroupInternal1 != null)
            {
                int index1 = 0;
                for (int count = viewGroupInternal1.Items.Count; index1 < count && (index >= 0 || !object.Equals(item, viewGroupInternal1.Items[index1])) && index != index1; ++index1)
                {
                    CollectionViewGroupInternal viewGroupInternal2 = viewGroupInternal1.Items[index1] as CollectionViewGroupInternal;
                    num += viewGroupInternal2 == null ? 1 : viewGroupInternal2.ItemCount;
                }
                item = (object)viewGroupInternal1;
                viewGroupInternal1 = viewGroupInternal1.Parent;
                index = -1;
            }
            return num;
        }

        internal object LeafAt(int index)
        {
            int index1 = 0;
            for (int count = this.Items.Count; index1 < count; ++index1)
            {
                CollectionViewGroupInternal viewGroupInternal = this.Items[index1] as CollectionViewGroupInternal;
                if (viewGroupInternal != null)
                {
                    if (index < viewGroupInternal.ItemCount)
                    {
                        return viewGroupInternal.LeafAt(index);
                    }
                    index -= viewGroupInternal.ItemCount;
                }
                else
                {
                    if (index == 0)
                    {
                        return this.Items[index1];
                    }
                    --index;
                }
            }
            throw new ArgumentOutOfRangeException("index");
        }

        internal IEnumerator GetLeafEnumerator()
        {
            return (IEnumerator)new CollectionViewGroupInternal.LeafEnumerator(this);
        }

        internal int Insert(object item, object seed, IComparer<object> comparer)
        {
            int low = this.GroupBy == null ? 0 : this.GroupBy.GroupNames.Count;
            int index = this.FindIndex(item, seed, comparer, low, this.ProtectedItems.Count);
            this.ChangeCounts(item, 1);
            this.ProtectedItems.Insert(index, item);
            return index;
        }

        protected virtual int FindIndex(object item, object seed, IComparer<object> comparer, int low, int high)
        {
            int index;
            if (comparer != null)
            {
                CollectionViewGroupInternal.IListComparer ilistComparer = comparer as CollectionViewGroupInternal.IListComparer;
                if (ilistComparer != null)
                {
                    ilistComparer.Reset();
                }
                for (index = low; index < high; ++index)
                {
                    CollectionViewGroupInternal viewGroupInternal = this.ProtectedItems[index] as CollectionViewGroupInternal;
                    object y = viewGroupInternal != null ? viewGroupInternal.SeedItem : this.ProtectedItems[index];
                    if (y != DependencyProperty.UnsetValue && comparer.Compare(seed, y) < 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                index = high;
            }
            return index;
        }

        protected virtual void OnGroupByChanged()
        {
            if (this.Parent == null)
            {
                return;
            }
            this.Parent.OnGroupByChanged();
        }

        protected void ChangeCounts(object item, int delta)
        {
            bool flag = !(item is CollectionViewGroup);
            for (CollectionViewGroupInternal group = this; group != null; group = group.parentGroup)
            {
                group.FullCount += delta;
                if (flag)
                {
                    CollectionViewGroupInternal viewGroupInternal = group;
                    int num = viewGroupInternal.ProtectedItemCount + delta;
                    viewGroupInternal.ProtectedItemCount = num;
                    if (group.ProtectedItemCount == 0)
                    {
                        this.RemoveEmptyGroup(group);
                    }
                }
            }
            ++this.version;
        }

        private void RemoveEmptyGroup(CollectionViewGroupInternal group)
        {
            CollectionViewGroupInternal parent = group.Parent;
            if (parent == null)
            {
                return;
            }
            GroupDescription groupBy = parent.GroupBy;
            if (parent.ProtectedItems.IndexOf((object)group) < groupBy.GroupNames.Count)
            {
                return;
            }
            parent.Remove((object)group, false);
        }

        private void OnGroupByChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnGroupByChanged();
        }

        internal class IListComparer : IComparer<object>
        {
            private int index;
            private IList list;

            internal IListComparer(IList list)
            {
                this.ResetList(list);
            }

            public int Compare(object x, object y)
            {
                if (object.Equals(x, y))
                {
                    return 0;
                }
                for (int index = this.list != null ? this.list.Count : 0; this.index < index; ++this.index)
                {
                    object objB = this.list[this.index];
                    if (object.Equals(x, objB))
                    {
                        return -1;
                    }
                    if (object.Equals(y, objB))
                    {
                        return 1;
                    }
                }
                return 1;
            }        

            internal void Reset()
            {
                this.index = 0;
            }

            internal void ResetList(IList list)
            {
                this.list = list;
                this.index = 0;
            }
        }

        private class LeafEnumerator : IEnumerator
        {
            private CollectionViewGroupInternal group;
            private int version;
            private int index;
            private IEnumerator subEnum;
            private object current;

            public LeafEnumerator(CollectionViewGroupInternal group)
            {
                this.group = group;
                this.DoReset();
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this.index < 0 || this.index >= this.group.Items.Count)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        return this.current;
                    }
                }
            }

            void IEnumerator.Reset()
            {
                this.DoReset();
            }

            bool IEnumerator.MoveNext()
            {
                if (this.group.version != this.version)
                {
                    throw new InvalidOperationException();
                }
                while (this.subEnum == null || !this.subEnum.MoveNext())
                {
                    CollectionViewGroupInternal.LeafEnumerator leafEnumerator = this;
                    leafEnumerator.index = leafEnumerator.index + 1;
                    if (this.index >= this.group.Items.Count)
                    {
                        return false;
                    }
                    CollectionViewGroupInternal item = this.group.Items[this.index] as CollectionViewGroupInternal;
                    if (item == null)
                    {
                        this.current = this.group.Items[this.index];
                        this.subEnum = null;
                        return true;
                    }
                    this.subEnum = item.GetLeafEnumerator();
                }
                this.current = this.subEnum.Current;
                return true;
            }

            private void DoReset()
            {
                this.version = this.group.version;
                this.index = -1;
                this.subEnum = (IEnumerator)null;
            }
        }
    }
}
