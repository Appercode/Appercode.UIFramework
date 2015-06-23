using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Appercode.UI.Internals.CollectionView
{
    internal class ListCollectionView : Appercode.UI.Data.CollectionView, IComparer<object>, IEditableCollectionView
    {
        internal const string CanAddNewPropertyName = "CanAddNew";

        internal const string CanCancelEditPropertyName = "CanCancelEdit";

        internal const string CanRemovePropertyName = "CanRemove";

        internal const string CurrentAddItemPropertyName = "CurrentAddItem";

        internal const string CurrentEditItemPropertyName = "CurrentEditItem";

        internal const string IsAddingNewPropertyName = "IsAddingNew";

        internal const string IsEditingItemPropertyName = "IsEditingItem";

        private bool canAddNew;

        private bool canRemove;

        private bool canCancelEdit;

        private IList internalList;

        private CollectionViewGroupRoot group;

        private bool isGrouping;

        private IComparer<object> activeComparer;

        private Predicate<object> activeFilter;

        private SortDescriptionCollection sort;

        private bool currentElementWasRemoved;

        private object newItem = Appercode.UI.Data.CollectionView.NoNewItem;

        private object newGroupedItem;

        private object editItem;

        private int newItemIndex;

        private bool isItemConstructorValid;

        private ConstructorInfo itemConstructor;

        public ListCollectionView(IList list)
            : base(list)
        {
            this.internalList = list;
            this.RefreshCanAddNew();
            this.RefreshCanRemove();
            this.RefreshCanCancelEdit();
            if (this.InternalList.Count != 0)
            {
                this.SetCurrent(this.InternalList[0], 0, 1);
            }
            else
            {
                this.SetCurrent(null, -1, 0);
            }
            this.group = new CollectionViewGroupRoot(this);
            this.group.GroupDescriptionChanged += new EventHandler(this.OnGroupDescriptionChanged);
            this.group.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupChanged);
            this.group.GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupByChanged);
        }

        private enum EffectiveNotifyCollectionChangedAction
        {
            Add,
            Remove,
            Replace,
            Move,
            Reset
        }

        public bool CanAddNew
        {
            get
            {
                return this.canAddNew;
            }
            set
            {
                if (this.canAddNew != value)
                {
                    this.canAddNew = value;
                    this.OnPropertyChanged("CanAddNew");
                }
            }
        }

        public bool CanCancelEdit
        {
            get
            {
                return this.canCancelEdit;
            }
            set
            {
                if (this.canCancelEdit != value)
                {
                    this.canCancelEdit = value;
                    this.OnPropertyChanged("CanCancelEdit");
                }
            }
        }

        public override bool CanFilter
        {
            get
            {
                return true;
            }
        }

        public override bool CanGroup
        {
            get
            {
                return true;
            }
        }

        public bool CanRemove
        {
            get
            {
                return this.canRemove;
            }
            set
            {
                if (this.canRemove != value)
                {
                    this.canRemove = value;
                    this.OnPropertyChanged("CanRemove");
                }
            }
        }

        public override bool CanSort
        {
            get
            {
                return true;
            }
        }

        public object CurrentAddItem
        {
            get
            {
                if (!this.IsAddingNew)
                {
                    return null;
                }
                return this.newItem;
            }
        }

        public object CurrentEditItem
        {
            get
            {
                return this.editItem;
            }
        }

        public override Predicate<object> Filter
        {
            get
            {
                return base.Filter;
            }
            set
            {
                if (this.IsAddingNew || this.IsEditingItem)
                {
                    throw new InvalidOperationException("Filter is not allowed during Add or Edit");
                }
                base.Filter = value;
            }
        }

        public override ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this.group.GroupDescriptions;
            }
        }

        public override ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                if (!this.IsGrouping)
                {
                    return null;
                }
                return this.group.Items;
            }
        }

        public override int Count
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.InternalCount;
            }
        }

        public bool IsAddingNew
        {
            get
            {
                return this.newItem != Appercode.UI.Data.CollectionView.NoNewItem;
            }
        }

        public bool IsEditingItem
        {
            get
            {
                return this.editItem != null;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return this.InternalCount == 0;
            }
        }

        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get
            {
                return NewItemPlaceholderPosition.None;
            }
            set
            {
                if (value != NewItemPlaceholderPosition.None)
                {
                    throw new ArgumentException("value");
                }
            }
        }

        public override SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (this.sort == null)
                {
                    this.SetSortDescriptions(new SortDescriptionCollection());
                }
                return this.sort;
            }
        }

        internal bool HasSortDescriptions
        {
            get
            {
                if (this.sort == null)
                {
                    return false;
                }
                return this.sort.Count > 0;
            }
        }

        protected int InternalCount
        {
            get
            {
                if (this.IsGrouping)
                {
                    return this.group.ItemCount;
                }
                return (!this.UsesLocalArray || !this.IsAddingNew ? 0 : 1) + this.InternalList.Count;
            }
        }

        protected IList InternalList
        {
            get
            {
                return this.internalList;
            }
        }

        protected IComparer<object> ActiveComparer
        {
            get
            {
                return this.activeComparer;
            }
            set
            {
                this.activeComparer = value;
            }
        }

        protected Predicate<object> ActiveFilter
        {
            get
            {
                return this.activeFilter;
            }
            set
            {
                this.activeFilter = value;
            }
        }

        protected bool IsGrouping
        {
            get
            {
                return this.isGrouping;
            }
        }

        protected bool UsesLocalArray
        {
            get
            {
                if (this.ActiveComparer != null)
                {
                    return true;
                }
                return this.ActiveFilter != null;
            }
        }

        private bool CanGroupNamesChange
        {
            get
            {
                return true;
            }
        }

        private bool CanConstructItem
        {
            get
            {
                if (!this.isItemConstructorValid)
                {
                    this.EnsureItemConstructor();
                }
                return this.itemConstructor != null;
            }
        }

        private bool IsCurrentInView
        {
            get
            {
                if (0 > this.CurrentPosition)
                {
                    return false;
                }
                return this.CurrentPosition < this.InternalCount;
            }
        }

        private IList SourceList
        {
            get
            {
                return this.SourceCollection as IList;
            }
        }

        public object AddNew()
        {
            this.VerifyRefreshNotDeferred();
            if (this.IsEditingItem)
            {
                this.CommitEdit();
            }
            this.CommitNew();
            if (!this.CanAddNew)
            {
                throw new InvalidOperationException("AddNew is not allowed during Add or Edit");
            }
            object obj = this.itemConstructor.Invoke(null);
            this.newItemIndex = -2;
            int num = this.SourceList.Add(obj);
            if (!(this.SourceList is INotifyCollectionChanged))
            {
                if (!object.Equals(obj, this.SourceList[num]))
                {
                    num = this.SourceList.IndexOf(obj);
                }
                this.BeginAddNew(obj, num);
            }
            this.MoveCurrentTo(obj);
            ISupportInitialize supportInitialize = obj as ISupportInitialize;
            if (supportInitialize != null)
            {
                supportInitialize.BeginInit();
            }
            IEditableObject editableObject = obj as IEditableObject;
            if (editableObject != null)
            {
                editableObject.BeginEdit();
            }
            return obj;
        }

        public void CancelEdit()
        {
            if (this.IsAddingNew)
            {
                throw new InvalidOperationException("CancelEdit not allowed during AddNew transaction");
            }
            this.VerifyRefreshNotDeferred();
            if (this.editItem == null)
            {
                return;
            }
            object obj = this.editItem;
            this.SetEditItem(null);
            IEditableObject editableObject = obj as IEditableObject;
            if (editableObject == null)
            {
                throw new InvalidOperationException("CancelEditNotSupported");
            }
            editableObject.CancelEdit();
        }

        public void CancelNew()
        {
            if (this.IsEditingItem)
            {
                throw new InvalidOperationException("CancelNew not allowed during EditItem transaction");
            }
            this.VerifyRefreshNotDeferred();
            if (this.newItem == Appercode.UI.Data.CollectionView.NoNewItem)
            {
                return;
            }
            this.SourceList.RemoveAt(this.newItemIndex);
            if (this.newItem != Appercode.UI.Data.CollectionView.NoNewItem)
            {
                int num = this.AdjustBefore(NotifyCollectionChangedAction.Remove, this.newItem, this.newItemIndex);
                object obj = this.EndAddNew(true);
                this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, num), num, -1);
            }
        }

        public void CommitEdit()
        {
            bool flag;
            if (this.IsAddingNew)
            {
                throw new InvalidOperationException("CommitEdit not allowed during AddNew transaction");
            }
            this.VerifyRefreshNotDeferred();
            if (this.editItem == null)
            {
                return;
            }
            object obj = this.editItem;
            this.SetEditItem(null);
            IEditableObject editableObject = obj as IEditableObject;
            if (editableObject != null)
            {
                editableObject.EndEdit();
            }
            int num = -1;
            bool flag1 = false;
            bool flag2 = false;
            if (this.IsGrouping || this.UsesLocalArray)
            {
                num = this.InternalIndexOf(obj);
                flag1 = num >= 0;
                if (flag1)
                {
                    flag = this.PassesFilter(obj);
                }
                else
                {
                    flag = !this.SourceList.Contains(obj) ? false : this.PassesFilter(obj);
                }
                flag2 = flag;
            }
            if (this.IsGrouping)
            {
                object obj1 = obj == this.CurrentItem ? obj : null;
                if (flag1)
                {
                    this.RemoveItemFromGroups(obj);
                }
                object currentItem = this.CurrentItem;
                int currentPosition = this.CurrentPosition;
                bool isCurrentAfterLast = this.IsCurrentAfterLast;
                bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
                if (flag2)
                {
                    this.AddItemToGroups(obj);
                }
                if (this.CurrentPosition == -1 && obj1 != null)
                {
                    int num1 = this.InternalIndexOf(obj1);
                    if (num1 >= 0 && this.PassesFilter(obj1) && this.OKToChangeCurrent())
                    {
                        this.SetCurrent(obj1, num1);
                    }
                }
                this.RaiseCurrencyChanges(currentItem != this.CurrentItem, this.CurrentItem != currentItem, this.CurrentPosition != currentPosition, this.IsCurrentBeforeFirst != isCurrentBeforeFirst, this.IsCurrentAfterLast != isCurrentAfterLast);
                return;
            }
            if (this.UsesLocalArray)
            {
                List<object> internalList = this.InternalList as List<object>;
                int num2 = -1;
                if (flag1)
                {
                    if (!flag2)
                    {
                        this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, num), num, -1);
                        return;
                    }
                    if (this.ActiveComparer != null)
                    {
                        int num3 = num;
                        if (num3 > 0 && this.ActiveComparer.Compare(internalList[num3 - 1], obj) > 0)
                        {
                            num2 = internalList.BinarySearch(0, num3, obj, this.ActiveComparer);
                            if (num2 < 0)
                            {
                                num2 = ~num2;
                            }
                        }
                        else if (num3 < internalList.Count - 1 && this.ActiveComparer.Compare(obj, internalList[num3 + 1]) > 0)
                        {
                            num2 = internalList.BinarySearch(num3 + 1, internalList.Count - num3 - 1, obj, this.ActiveComparer);
                            if (num2 < 0)
                            {
                                num2 = ~num2;
                            }
                            num2--;
                        }
                        if (num2 >= 0)
                        {
                            this.ProcessCollectionChangedWithAdjustedIndex(obj, num, num2);
                            return;
                        }
                    }
                }
                else if (flag2)
                {
                    num2 = this.AdjustBefore(NotifyCollectionChangedAction.Add, obj, this.SourceList.IndexOf(obj));
                    this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, num2), -1, num2);
                }
            }
        }

        public void CommitNew()
        {
            if (this.IsEditingItem)
            {
                throw new InvalidOperationException("CommitNew not allowed during EditItem transaction");
            }
            this.VerifyRefreshNotDeferred();
            if (this.newItem == Appercode.UI.Data.CollectionView.NoNewItem)
            {
                return;
            }
            if (this.IsGrouping)
            {
                this.CommitNewForGrouping();
                return;
            }
            int num = this.UsesLocalArray ? this.InternalCount - 1 : this.newItemIndex;
            object obj = this.EndAddNew(false);
            int num1 = this.AdjustBefore(NotifyCollectionChangedAction.Add, obj, this.newItemIndex);
            if (num1 < 0)
            {
                this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, num), num, -1);
                return;
            }
            if (num != num1)
            {
                this.ProcessCollectionChangedWithAdjustedIndex(obj, num, num1);
            }
            else if (this.UsesLocalArray)
            {
                this.InternalList.Insert(num1, obj);
                return;
            }
        }

        public override bool Contains(object item)
        {
            this.VerifyRefreshNotDeferred();
            return this.InternalContains(item);
        }

        public void EditItem(object item)
        {
            this.VerifyRefreshNotDeferred();
            if (this.IsAddingNew)
            {
                if (object.Equals(item, this.newItem))
                {
                    return;
                }
                this.CommitNew();
            }
            this.CommitEdit();
            this.SetEditItem(item);
            IEditableObject editableObject = item as IEditableObject;
            if (editableObject != null)
            {
                editableObject.BeginEdit();
            }
        }

        public override int IndexOf(object item)
        {
            this.VerifyRefreshNotDeferred();
            return this.InternalIndexOf(item);
        }

        public override object GetItemAt(int index)
        {
            this.VerifyRefreshNotDeferred();
            return this.InternalItemAt(index);
        }

        public override bool MoveCurrentToPosition(int position)
        {
            this.VerifyRefreshNotDeferred();
            if (position < -1 || position > this.InternalCount)
            {
                throw new ArgumentOutOfRangeException("position");
            }
            if ((position != this.CurrentPosition || !this.IsCurrentInSync) && this.OKToChangeCurrent())
            {
                object obj = 0 > position || position >= this.InternalCount ? null : this.InternalItemAt(position);
                bool isCurrentAfterLast = this.IsCurrentAfterLast;
                bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
                this.SetCurrent(obj, position);
                this.RaiseCurrencyChanges(true, true, true, this.IsCurrentBeforeFirst != isCurrentBeforeFirst, this.IsCurrentAfterLast != isCurrentAfterLast);
            }
            return this.IsCurrentInView;
        }

        public override bool PassesFilter(object item)
        {
            if (this.ActiveFilter == null)
            {
                return true;
            }
            return this.ActiveFilter(item);
        }

        public void Remove(object item)
        {
            int num = this.InternalIndexOf(item);
            if (num >= 0)
            {
                this.RemoveAt(num);
            }
        }

        public void RemoveAt(int index)
        {
            if (this.IsEditingItem || this.IsAddingNew)
            {
                throw new InvalidOperationException("RemoveAt is not allowed during Add or Edit");
            }
            if (!this.CanRemove)
            {
                throw new InvalidOperationException("RemoveAt is not allowed during Add or Edit");
            }
            this.VerifyRefreshNotDeferred();
            object itemAt = this.GetItemAt(index);
            int num = index;
            bool sourceList = !(this.SourceList is INotifyCollectionChanged);
            if (!this.UsesLocalArray && !this.IsGrouping)
            {
                this.SourceList.RemoveAt(num);
            }
            else if (!sourceList)
            {
                this.SourceList.Remove(itemAt);
            }
            else
            {
                num = this.SourceList.IndexOf(itemAt);
                this.SourceList.RemoveAt(num);
            }
            if (sourceList)
            {
                this.ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemAt, num));
            }
        }

        int System.Collections.Generic.IComparer<object>.Compare(object o1, object o2)
        {
            return this.Compare(o1, o2);
        }

        protected override void RefreshOverride()
        {
            object currentItem = this.CurrentItem;
            int num = this.IsEmpty ? -1 : this.CurrentPosition;
            bool isCurrentAfterLast = this.IsCurrentAfterLast;
            bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
            this.OnCurrentChanging();
            IList sourceCollection = this.SourceCollection as IList;
            this.PrepareSortAndFilter(sourceCollection);
            if (this.UsesLocalArray)
            {
                this.internalList = this.PrepareLocalArray(sourceCollection);
            }
            else
            {
                this.internalList = sourceCollection;
            }
            this.PrepareGroups();
            if (isCurrentBeforeFirst || this.IsEmpty)
            {
                this.SetCurrent(null, -1);
            }
            else if (!isCurrentAfterLast)
            {
                int num1 = this.InternalIndexOf(currentItem);
                if (num1 >= 0)
                {
                    this.SetCurrent(currentItem, num1);
                }
                else
                {
                    this.SetCurrent(this.InternalItemAt(0), 0);
                }
            }
            else
            {
                this.SetCurrent(null, this.InternalCount);
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.RaiseCurrencyChanges(true, this.CurrentItem != currentItem, this.CurrentPosition != num, this.IsCurrentBeforeFirst != isCurrentBeforeFirst, this.IsCurrentAfterLast != isCurrentAfterLast);
        }

        protected virtual int Compare(object o1, object o2)
        {
            if (this.IsGrouping)
            {
                return this.InternalIndexOf(o1) - this.InternalIndexOf(o2);
            }
            if (this.ActiveComparer != null)
            {
                return this.ActiveComparer.Compare(o1, o2);
            }
            int num = this.InternalList.IndexOf(o1);
            return num - this.InternalList.IndexOf(o2);
        }

        protected override IEnumerator GetEnumerator()
        {
            this.VerifyRefreshNotDeferred();
            return this.InternalGetEnumerator();
        }

        protected bool InternalContains(object item)
        {
            if (!this.IsGrouping)
            {
                return this.InternalList.Contains(item);
            }
            return this.group.LeafIndexOf(item) >= 0;
        }

        protected IEnumerator InternalGetEnumerator()
        {
            if (this.IsGrouping)
            {
                return this.group.GetLeafEnumerator();
            }
            return new Appercode.UI.Data.CollectionView.PlaceholderAwareEnumerator(this, this.InternalList.GetEnumerator(), this.newItem);
        }

        protected int InternalIndexOf(object item)
        {
            if (this.IsGrouping)
            {
                return this.group.LeafIndexOf(item);
            }
            if (this.IsAddingNew && object.Equals(item, this.newItem) && this.UsesLocalArray)
            {
                return this.InternalCount - 1;
            }
            return this.InternalList.IndexOf(item);
        }

        protected object InternalItemAt(int index)
        {
            if (this.IsGrouping)
            {
                return this.group.LeafAt(index);
            }
            if (this.IsAddingNew && this.UsesLocalArray && index == this.InternalCount - 1)
            {
                return this.newItem;
            }
            return this.InternalList[index];
        }

        protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (!this.isItemConstructorValid)
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                        {
                            this.RefreshCanAddNew();
                            break;
                        }
                }
            }
            int num = -1;
            int num1 = -1;
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                if (this.IsEditingItem)
                {
                    this.ImplicitlyCancelEdit();
                }
                if (this.IsAddingNew)
                {
                    this.newItemIndex = this.SourceList.IndexOf(this.newItem);
                    if (this.newItemIndex < 0)
                    {
                        this.EndAddNew(true);
                    }
                }
                this.RefreshOrDefer();
                return;
            }
            if (args.Action == NotifyCollectionChangedAction.Add && this.newItemIndex == -2)
            {
                this.BeginAddNew(args.NewItems[0], args.NewStartingIndex);
                return;
            }
            if (args.Action != NotifyCollectionChangedAction.Remove)
            {
                num1 = this.AdjustBefore(NotifyCollectionChangedAction.Add, args.NewItems[0], args.NewStartingIndex);
            }
            if (args.Action != NotifyCollectionChangedAction.Add)
            {
                num = this.AdjustBefore(NotifyCollectionChangedAction.Remove, args.OldItems[0], args.OldStartingIndex);
                if (this.UsesLocalArray && num >= 0 && num < num1)
                {
                    num1--;
                }
            }
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (args.NewStartingIndex > this.newItemIndex)
                        {
                            break;
                        }
                        ListCollectionView listCollectionView = this;
                        listCollectionView.newItemIndex = listCollectionView.newItemIndex + 1;
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (args.OldStartingIndex < this.newItemIndex)
                        {
                            ListCollectionView listCollectionView1 = this;
                            listCollectionView1.newItemIndex = listCollectionView1.newItemIndex - 1;
                        }
                        object item = args.OldItems[0];
                        if (item != this.CurrentEditItem)
                        {
                            if (item != this.CurrentAddItem)
                            {
                                break;
                            }
                            this.EndAddNew(true);
                            break;
                        }
                        else
                        {
                            this.ImplicitlyCancelEdit();
                            break;
                        }
                    }
            }
            this.ProcessCollectionChangedWithAdjustedIndex(args, num, num1);
        }

        private void AddItemToGroups(object item)
        {
            if (!this.IsAddingNew || item != this.newItem)
            {
                this.group.AddToSubgroups(item, false);
                return;
            }
            this.group.InsertSpecialItem(this.group.Items.Count, item, false);
        }

        private int AdjustBefore(NotifyCollectionChangedAction action, object item, int index)
        {
            if (action == NotifyCollectionChangedAction.Reset)
            {
                return -1;
            }
            IList sourceCollection = this.SourceCollection as IList;
            if (index < -1 || index > sourceCollection.Count)
            {
                throw new InvalidOperationException("CollectionChangedOutOfRange");
            }
            if (action == NotifyCollectionChangedAction.Add)
            {
                if (index < 0)
                {
                    index = sourceCollection.IndexOf(item);
                    if (index < 0)
                    {
                        throw new InvalidOperationException("AddedItemNotInCollection");
                    }
                }
                else if (!object.Equals(item, sourceCollection[index]))
                {
                    throw new InvalidOperationException("AddedItemNotAtIndex");
                }
            }
            if (!this.UsesLocalArray)
            {
                if (this.IsAddingNew && index > this.newItemIndex)
                {
                    index--;
                }
                return index;
            }
            if (action == NotifyCollectionChangedAction.Add)
            {
                if (!this.PassesFilter(item))
                {
                    return -2;
                }
                List<object> internalList = this.InternalList as List<object>;
                if (internalList == null)
                {
                    index = -1;
                }
                else if (this.ActiveComparer == null)
                {
                    int num = 0;
                    int num1 = 0;
                    while (num < index && num1 < internalList.Count)
                    {
                        if (object.Equals(sourceCollection[num], internalList[num1]))
                        {
                            num++;
                            num1++;
                        }
                        else if (!object.Equals(item, internalList[num1]))
                        {
                            num++;
                        }
                        else
                        {
                            num1++;
                        }
                    }
                    index = num1;
                }
                else
                {
                    index = internalList.BinarySearch(item, this.ActiveComparer);
                    if (index < 0)
                    {
                        index = ~index;
                    }
                }
            }
            else if (action != NotifyCollectionChangedAction.Remove)
            {
                index = -1;
            }
            else
            {
                if (this.IsAddingNew && item == this.newItem)
                {
                    return this.InternalCount - 1;
                }
                index = this.InternalList.IndexOf(item);
                if (index < 0)
                {
                    return -2;
                }
            }
            if (index >= 0)
            {
                return index;
            }
            return index;
        }

        private void AdjustCurrencyForAdd(int index)
        {
            if (this.InternalCount == 1)
            {
                if (this.CurrentItem != null || this.CurrentPosition != -1)
                {
                    this.OnCurrentChanging();
                }
                this.SetCurrent(null, -1);
                return;
            }
            if (index > this.CurrentPosition)
            {
                if (!this.IsCurrentInSync)
                {
                    this.SetCurrent(this.CurrentItem, this.InternalIndexOf(this.CurrentItem));
                }
                return;
            }
            int currentPosition = this.CurrentPosition + 1;
            if (currentPosition >= this.InternalCount)
            {
                this.SetCurrent(null, this.InternalCount);
                return;
            }
            this.SetCurrent(this.GetItemAt(currentPosition), currentPosition);
        }

        private void AdjustCurrencyForMove(int oldIndex, int newIndex)
        {
            if (oldIndex == this.CurrentPosition)
            {
                this.SetCurrent(this.GetItemAt(newIndex), newIndex);
                return;
            }
            if (oldIndex < this.CurrentPosition && this.CurrentPosition <= newIndex)
            {
                this.SetCurrent(this.CurrentItem, this.CurrentPosition - 1);
                return;
            }
            if (newIndex <= this.CurrentPosition && this.CurrentPosition < oldIndex)
            {
                this.SetCurrent(this.CurrentItem, this.CurrentPosition + 1);
            }
        }

        private void AdjustCurrencyForRemove(int index)
        {
            if (index >= this.CurrentPosition)
            {
                if (index == this.CurrentPosition)
                {
                    this.currentElementWasRemoved = true;
                }
                return;
            }
            this.SetCurrent(this.CurrentItem, this.CurrentPosition - 1);
        }

        private void AdjustCurrencyForReplace(int index)
        {
            if (index == this.CurrentPosition)
            {
                this.currentElementWasRemoved = true;
            }
        }

        private void BeginAddNew(object newItem, int index)
        {
            this.SetNewItem(newItem);
            this.newItemIndex = index;
            int num = this.UsesLocalArray ? this.InternalCount - 1 : this.newItemIndex;
            this.ProcessCollectionChangedWithAdjustedIndex(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, num), -1, num);
        }

        private void CommitNewForGrouping()
        {
            int count = this.group.Items.Count - 1;
            int num = this.newItemIndex;
            object obj = this.EndAddNew(false);
            try
            {
                this.newGroupedItem = obj;
                this.group.RemoveSpecialItem(count, obj, false);
                this.ProcessCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, num));
            }
            finally
            {
                this.newGroupedItem = null;
            }
            if (this.CurrentPosition == -1)
            {
                this.MoveCurrentTo(obj);
            }
        }

        private object EndAddNew(bool cancel)
        {
            object obj = this.newItem;
            this.SetNewItem(Appercode.UI.Data.CollectionView.NoNewItem);
            IEditableObject editableObject = obj as IEditableObject;
            if (editableObject != null)
            {
                if (!cancel)
                {
                    editableObject.EndEdit();
                }
                else
                {
                    editableObject.CancelEdit();
                }
            }
            ISupportInitialize supportInitialize = obj as ISupportInitialize;
            if (supportInitialize != null)
            {
                supportInitialize.EndInit();
            }
            return obj;
        }

        private void EnsureItemConstructor()
        {
            if (!this.isItemConstructorValid)
            {
                Type itemType = this.GetItemType(true);
                if (itemType != null)
                {
                    this.itemConstructor = itemType.GetConstructor(Type.EmptyTypes);
                    this.isItemConstructorValid = true;
                }
            }
        }

        private void ImplicitlyCancelEdit()
        {
            IEditableObject editableObject = this.editItem as IEditableObject;
            this.SetEditItem(null);
            if (editableObject != null)
            {
                editableObject.CancelEdit();
            }
        }

        private void MoveCurrencyOffDeletedElement(int oldCurrentPosition)
        {
            int internalCount = this.InternalCount - 1;
            int num = oldCurrentPosition < internalCount ? oldCurrentPosition : internalCount;
            this.currentElementWasRemoved = false;
            this.OnCurrentChanging();
            if (num >= 0)
            {
                this.SetCurrent(this.InternalItemAt(num), num);
            }
            else
            {
                this.SetCurrent(null, num);
            }
            this.OnCurrentChanged();
        }

        private void OnGroupByChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsAddingNew || this.IsEditingItem)
            {
                throw new InvalidOperationException("Grouping is not allowed during Add or Edit");
            }
            this.RefreshOrDefer();
        }

        private void OnGroupChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.AdjustCurrencyForAdd(e.NewStartingIndex);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.AdjustCurrencyForRemove(e.OldStartingIndex);
            }
            this.OnCollectionChanged(e);
        }

        private void OnGroupDescriptionChanged(object sender, EventArgs e)
        {
            if (this.IsAddingNew || this.IsEditingItem)
            {
                throw new InvalidOperationException("Grouping is not allowed during Add or Edit");
            }
            this.RefreshOrDefer();
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void PrepareGroups()
        {
            this.group.Clear();
            this.group.Initialize();
            this.isGrouping = this.group.GroupBy != null;
            if (!this.isGrouping)
            {
                return;
            }
            IComparer<object> activeComparer = this.ActiveComparer;
            if (activeComparer == null)
            {
                CollectionViewGroupInternal.IListComparer listComparer = this.group.ActiveComparer as CollectionViewGroupInternal.IListComparer;
                if (listComparer == null)
                {
                    this.group.ActiveComparer = new CollectionViewGroupInternal.IListComparer(this.InternalList);
                }
                else
                {
                    listComparer.ResetList(this.InternalList);
                }
            }
            else
            {
                this.group.ActiveComparer = activeComparer;
            }
            int num = 0;
            int count = this.InternalList.Count;
            while (num < count)
            {
                object item = this.InternalList[num];
                if (!this.IsAddingNew || !object.Equals(this.newItem, item))
                {
                    this.group.AddToSubgroups(item, true);
                }
                num++;
            }
            if (this.IsAddingNew)
            {
                this.group.InsertSpecialItem(this.group.Items.Count, this.newItem, true);
            }
        }

        private IList PrepareLocalArray(IList list)
        {
            List<object> objs;
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (this.ActiveFilter != null)
            {
                objs = new List<object>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    if (this.ActiveFilter(list[i]))
                    {
                        objs.Add(list[i]);
                    }
                }
            }
            else
            {
                objs = new List<object>();
                foreach (object obj in list)
                {
                    objs.Add(obj);
                }
            }
            if (this.ActiveComparer != null)
            {
                objs.Sort(this.ActiveComparer);
            }
            return objs;
        }

        private void PrepareSortAndFilter(IList list)
        {
            if (this.sort == null || this.sort.Count <= 0)
            {
                this.ActiveComparer = null;
            }
            else
            {
                this.ActiveComparer = new SortFieldComparer(this.sort, this.Culture);
            }
            this.ActiveFilter = this.Filter;
        }

        private void ProcessCollectionChangedWithAdjustedIndex(NotifyCollectionChangedEventArgs args, int adjustedOldIndex, int adjustedNewIndex)
        {
            object obj;
            object obj1;
            NotifyCollectionChangedAction action = args.Action;
            obj = args.OldItems == null || args.OldItems.Count == 0 ? null : args.OldItems[0];
            obj1 = args.NewItems == null || args.NewItems.Count == 0 ? null : args.NewItems[0];
            this.ProcessCollectionChangedWithAdjustedIndex((ListCollectionView.EffectiveNotifyCollectionChangedAction)action, obj, obj1, adjustedOldIndex, adjustedNewIndex);
        }

        private void ProcessCollectionChangedWithAdjustedIndex(object movedItem, int adjustedOldIndex, int adjustedNewIndex)
        {
            this.ProcessCollectionChangedWithAdjustedIndex(ListCollectionView.EffectiveNotifyCollectionChangedAction.Move, movedItem, movedItem, adjustedOldIndex, adjustedNewIndex);
        }

        private void ProcessCollectionChangedWithAdjustedIndex(ListCollectionView.EffectiveNotifyCollectionChangedAction action, object oldItem, object newItem, int adjustedOldIndex, int adjustedNewIndex)
        {
            ListCollectionView.EffectiveNotifyCollectionChangedAction effectiveNotifyCollectionChangedAction = action;
            if (adjustedOldIndex == adjustedNewIndex && adjustedOldIndex >= 0)
            {
                effectiveNotifyCollectionChangedAction = ListCollectionView.EffectiveNotifyCollectionChangedAction.Replace;
            }
            else if (adjustedOldIndex == -1)
            {
                if (adjustedNewIndex < 0)
                {
                    if (action == ListCollectionView.EffectiveNotifyCollectionChangedAction.Add)
                    {
                        return;
                    }
                    effectiveNotifyCollectionChangedAction = ListCollectionView.EffectiveNotifyCollectionChangedAction.Remove;
                }
            }
            else if (adjustedOldIndex >= -1)
            {
                effectiveNotifyCollectionChangedAction = adjustedNewIndex >= 0 ? ListCollectionView.EffectiveNotifyCollectionChangedAction.Move : ListCollectionView.EffectiveNotifyCollectionChangedAction.Remove;
            }
            else
            {
                if (adjustedNewIndex < 0)
                {
                    return;
                }
                effectiveNotifyCollectionChangedAction = ListCollectionView.EffectiveNotifyCollectionChangedAction.Add;
            }
            int currentPosition = this.CurrentPosition;
            int num = this.CurrentPosition;
            object currentItem = this.CurrentItem;
            bool isCurrentAfterLast = this.IsCurrentAfterLast;
            bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArg = null;
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArg1 = null;
            switch (effectiveNotifyCollectionChangedAction)
            {
                case ListCollectionView.EffectiveNotifyCollectionChangedAction.Add:
                    {
                        if (this.UsesLocalArray && (!this.IsAddingNew || !object.Equals(this.newItem, newItem)))
                        {
                            this.InternalList.Insert(adjustedNewIndex, newItem);
                        }
                        if (this.IsGrouping)
                        {
                            this.AddItemToGroups(newItem);
                            break;
                        }
                        else
                        {
                            this.AdjustCurrencyForAdd(adjustedNewIndex);
                            notifyCollectionChangedEventArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, adjustedNewIndex);
                            break;
                        }
                    }
                case ListCollectionView.EffectiveNotifyCollectionChangedAction.Remove:
                    {
                        if (this.UsesLocalArray)
                        {
                            int num1 = adjustedOldIndex;
                            if (num1 < this.InternalList.Count && object.Equals(this.InternalList[num1], oldItem))
                            {
                                this.InternalList.RemoveAt(num1);
                            }
                        }
                        if (this.IsGrouping)
                        {
                            this.RemoveItemFromGroups(oldItem);
                            break;
                        }
                        else
                        {
                            this.AdjustCurrencyForRemove(adjustedOldIndex);
                            notifyCollectionChangedEventArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, adjustedOldIndex);
                            break;
                        }
                    }
                case ListCollectionView.EffectiveNotifyCollectionChangedAction.Replace:
                    {
                        if (this.UsesLocalArray)
                        {
                            this.InternalList[adjustedOldIndex] = newItem;
                        }
                        if (this.IsGrouping)
                        {
                            this.RemoveItemFromGroups(oldItem);
                            this.AddItemToGroups(newItem);
                            break;
                        }
                        else
                        {
                            this.AdjustCurrencyForReplace(adjustedOldIndex);
                            notifyCollectionChangedEventArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, adjustedOldIndex);
                            break;
                        }
                    }
                case ListCollectionView.EffectiveNotifyCollectionChangedAction.Move:
                    {
                        bool flag = oldItem == newItem;
                        if (this.UsesLocalArray)
                        {
                            int num2 = adjustedOldIndex;
                            int num3 = adjustedNewIndex;
                            if (num2 < this.InternalList.Count && object.Equals(this.InternalList[num2], oldItem))
                            {
                                this.InternalList.RemoveAt(num2);
                            }
                            this.InternalList.Insert(num3, newItem);
                        }
                        if (this.IsGrouping)
                        {
                            if (flag)
                            {
                                break;
                            }
                            this.RemoveItemFromGroups(oldItem);
                            this.AddItemToGroups(newItem);
                            break;
                        }
                        else
                        {
                            this.AdjustCurrencyForMove(adjustedOldIndex, adjustedNewIndex);
                            notifyCollectionChangedEventArg1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, adjustedNewIndex);
                            notifyCollectionChangedEventArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, adjustedOldIndex);
                            break;
                        }
                    }
            }
            bool isCurrentAfterLast1 = this.IsCurrentAfterLast != isCurrentAfterLast;
            bool isCurrentBeforeFirst1 = this.IsCurrentBeforeFirst != isCurrentBeforeFirst;
            bool currentPosition1 = this.CurrentPosition != num;
            bool currentItem1 = this.CurrentItem != currentItem;
            isCurrentAfterLast = this.IsCurrentAfterLast;
            isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
            num = this.CurrentPosition;
            currentItem = this.CurrentItem;
            if (!this.IsGrouping)
            {
                this.CurrentChangedMonitor.Enter();
                using (this.CurrentChangedMonitor)
                {
                    this.OnCollectionChanged(notifyCollectionChangedEventArg);
                    if (notifyCollectionChangedEventArg1 != null)
                    {
                        this.OnCollectionChanged(notifyCollectionChangedEventArg1);
                    }
                }
                if (this.IsCurrentAfterLast != isCurrentAfterLast)
                {
                    isCurrentAfterLast1 = false;
                    isCurrentAfterLast = this.IsCurrentAfterLast;
                }
                if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
                {
                    isCurrentBeforeFirst1 = false;
                    isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
                }
                if (this.CurrentPosition != num)
                {
                    currentPosition1 = false;
                    num = this.CurrentPosition;
                }
                if (this.CurrentItem != currentItem)
                {
                    currentItem1 = false;
                    currentItem = this.CurrentItem;
                }
            }
            if (this.currentElementWasRemoved)
            {
                this.MoveCurrencyOffDeletedElement(this.newGroupedItem == null ? currentPosition : this.IndexOf(this.newGroupedItem));
                isCurrentAfterLast1 = isCurrentAfterLast1 ? true : this.IsCurrentAfterLast != isCurrentAfterLast;
                isCurrentBeforeFirst1 = isCurrentBeforeFirst1 ? true : this.IsCurrentBeforeFirst != isCurrentBeforeFirst;
                currentPosition1 = currentPosition1 ? true : this.CurrentPosition != num;
                currentItem1 = currentItem1 ? true : this.CurrentItem != currentItem;
            }
            this.RaiseCurrencyChanges(false, currentItem1, currentPosition1, isCurrentBeforeFirst1, isCurrentAfterLast1);
        }

        private void RaiseCurrencyChanges(bool raiseCurrentChanged, bool raiseCurrentItem, bool raiseCurrentPosition, bool raiseIsCurrentBeforeFirst, bool raiseIsCurrentAfterLast)
        {
            if (raiseCurrentChanged)
            {
                this.OnCurrentChanged();
            }
            if (raiseIsCurrentAfterLast)
            {
                this.OnPropertyChanged("IsCurrentAfterLast");
            }
            if (raiseIsCurrentBeforeFirst)
            {
                this.OnPropertyChanged("IsCurrentBeforeFirst");
            }
            if (raiseCurrentPosition)
            {
                this.OnPropertyChanged("CurrentPosition");
            }
            if (raiseCurrentItem)
            {
                this.OnPropertyChanged("CurrentItem");
            }
        }

        private void RefreshCanAddNew()
        {
            this.CanAddNew = this.IsEditingItem || this.SourceList == null || this.SourceList.IsFixedSize ? false : this.CanConstructItem;
        }

        private void RefreshCanCancelEdit()
        {
            this.CanCancelEdit = this.editItem is IEditableObject;
        }

        private void RefreshCanRemove()
        {
            this.CanRemove = this.IsEditingItem || this.IsAddingNew ? false : !this.SourceList.IsFixedSize;
        }

        private void RemoveItemFromGroups(object item)
        {
            if (this.CanGroupNamesChange || this.group.RemoveFromSubgroups(item))
            {
                this.group.RemoveItemFromSubgroupsByExhaustiveSearch(item);
            }
        }

        private void SetEditItem(object item)
        {
            if (!object.Equals(item, this.editItem))
            {
                this.editItem = item;
                this.OnPropertyChanged("CurrentEditItem");
                this.OnPropertyChanged("IsEditingItem");
                this.RefreshCanCancelEdit();
                this.RefreshCanAddNew();
                this.RefreshCanRemove();
            }
        }

        private void SetNewItem(object item)
        {
            if (!object.Equals(item, this.newItem))
            {
                this.newItem = item;
                this.OnPropertyChanged("CurrentAddItem");
                this.OnPropertyChanged("IsAddingNew");
                this.RefreshCanRemove();
            }
        }

        private void SetSortDescriptions(SortDescriptionCollection descriptions)
        {
            if (this.sort != null)
            {
                this.sort.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.SortDescriptionsChanged);
            }
            this.sort = descriptions;
            if (this.sort != null)
            {
                this.sort.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SortDescriptionsChanged);
            }
        }

        private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsAddingNew || this.IsEditingItem)
            {
                throw new InvalidOperationException("Sorting is not allowed during Add or Edit");
            }
            this.RefreshOrDefer();
        }
    }
}