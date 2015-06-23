using Appercode.UI.Internals.CollectionView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using SCM = System.ComponentModel;

namespace Appercode.UI.Data
{
    internal abstract class CollectionView : ICollectionView, IEnumerable, INotifyCollectionChanged, System.ComponentModel.INotifyPropertyChanged, IViewLifetime
    {
        internal const string CountPropertyName = "Count";
        internal const string IsEmptyPropertyName = "IsEmpty";
        internal const string CulturePropertyName = "Culture";
        internal const string CurrentPositionPropertyName = "CurrentPosition";
        internal const string CurrentItemPropertyName = "CurrentItem";
        internal const string IsCurrentBeforeFirstPropertyName = "IsCurrentBeforeFirst";
        internal const string IsCurrentAfterLastPropertyName = "IsCurrentAfterLast";
        internal static readonly object NoNewItem = new object();
        internal CollectionView.SimpleMonitor CurrentChangedMonitor = new CollectionView.SimpleMonitor();
        private static readonly SCM.CurrentChangingEventArgs uncancelableCurrentChangingEventArgs = new SCM.CurrentChangingEventArgs(false);
        private static readonly string IEnumerableT = typeof(IEnumerable<>).Name;
        private CollectionView.CollectionViewFlags flags = CollectionView.CollectionViewFlags.ShouldProcessCollectionChanged | CollectionView.CollectionViewFlags.NeedsRefresh;
        private IEnumerable sourceCollection;
        private int timestamp;
        private object currentItem;
        private Predicate<object> filter;
        private int currentPosition;
        private CultureInfo culture;
        private int deferLevel;

        public CollectionView(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.sourceCollection = collection;
            INotifyCollectionChanged collectionChanged = collection as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
            }
            this.currentItem = (object)null;
            this.currentPosition = -1;
            this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, this.currentPosition < 0);
            this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, this.currentPosition < 0);
            this.SetFlag(CollectionView.CollectionViewFlags.CachedIsEmpty, this.currentPosition < 0);
        }

        public virtual event SCM.CurrentChangingEventHandler CurrentChanging;

        public virtual event EventHandler CurrentChanged;

        protected internal virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected internal virtual event SCM.PropertyChangedEventHandler PropertyChanged;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                this.CollectionChanged += value;
            }
            remove
            {
                this.CollectionChanged -= value;
            }
        }

        event SCM.PropertyChangedEventHandler SCM.INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.PropertyChanged += value;
            }
            remove
            {
                this.PropertyChanged -= value;
            }
        }

        [Flags]
        private enum CollectionViewFlags
        {
            ShouldProcessCollectionChanged = 4,
            IsCurrentBeforeFirst = 8,
            IsCurrentAfterLast = 16,
            NeedsRefresh = 128,
            CachedIsEmpty = 512,
        }

        public object ViewManagerData { get; set; }

        public virtual CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (this.culture == value)
                {
                    return;
                }
                this.culture = value;
                this.OnPropertyChanged("Culture");
            }
        }

        public virtual IEnumerable SourceCollection
        {
            get
            {
                return this.sourceCollection;
            }
        }

        public virtual Predicate<object> Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                if (!this.CanFilter)
                {
                    throw new NotSupportedException();
                }
                this.filter = value;
                this.RefreshOrDefer();
            }
        }

        public abstract bool CanFilter { get; }

        public abstract SortDescriptionCollection SortDescriptions { get; }

        public abstract bool CanSort { get; }

        public abstract bool CanGroup { get; }

        public abstract ObservableCollection<GroupDescription> GroupDescriptions { get; }

        public abstract ReadOnlyObservableCollection<object> Groups { get; }

        public virtual object CurrentItem
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.currentItem;
            }
        }

        public virtual int CurrentPosition
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.currentPosition;
            }
        }

        public virtual bool IsCurrentAfterLast
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.CheckFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast);
            }
        }

        public virtual bool IsCurrentBeforeFirst
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                return this.CheckFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst);
            }
        }

        public abstract int Count { get; }

        public abstract bool IsEmpty { get; }

        public virtual IComparer Comparer
        {
            get
            {
                return this as IComparer;
            }
        }

        public virtual bool NeedsRefresh
        {
            get
            {
                return this.CheckFlag(CollectionView.CollectionViewFlags.NeedsRefresh);
            }
        }

        internal object SyncRoot
        {
            get
            {
                ICollection collection = this.SourceCollection as ICollection;
                if (collection != null && collection.SyncRoot != null)
                {
                    return collection.SyncRoot;
                }
                else
                {
                    return (object)this.SourceCollection;
                }
            }
        }

        internal int Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }

        protected bool IsRefreshDeferred
        {
            get
            {
                return this.deferLevel > 0;
            }
        }

        protected bool IsCurrentInSync
        {
            get
            {
                if (this.IsCurrentInView)
                {
                    return this.GetItemAt(this.CurrentPosition) == this.CurrentItem;
                }
                else
                {
                    return this.CurrentItem == null;
                }
            }
        }

        private bool IsCurrentInView
        {
            get
            {
                this.VerifyRefreshNotDeferred();
                if (0 <= this.CurrentPosition)
                {
                    return this.CurrentPosition < this.Count;
                }
                else
                {
                    return false;
                }
            }
        }

        public abstract bool Contains(object item);

        public virtual void Refresh()
        {
            SCM.IEditableCollectionView editableCollectionView = this as SCM.IEditableCollectionView;
            if (editableCollectionView != null && (editableCollectionView.IsAddingNew || editableCollectionView.IsEditingItem))
            {
                throw new InvalidOperationException("CollectionView_Member NotAllowed During Refresh");
            }
            else
            {
                this.RefreshInternal();
            }
        }

        public virtual IDisposable DeferRefresh()
        {
            SCM.IEditableCollectionView editableCollectionView = this as SCM.IEditableCollectionView;
            if (editableCollectionView != null && (editableCollectionView.IsAddingNew || editableCollectionView.IsEditingItem))
            {
                throw new InvalidOperationException("CollectionView_Member NotAllowed During DeferRefresh");
            }
            else
            {
                ++this.deferLevel;
                return (IDisposable)new CollectionView.DeferHelper(this);
            }
        }

        public virtual bool MoveCurrentToFirst()
        {
            this.VerifyRefreshNotDeferred();
            return this.MoveCurrentToPosition(0);
        }

        public virtual bool MoveCurrentToLast()
        {
            this.VerifyRefreshNotDeferred();
            return this.MoveCurrentToPosition(this.Count - 1);
        }

        public virtual bool MoveCurrentToNext()
        {
            this.VerifyRefreshNotDeferred();
            if (this.CurrentPosition < this.Count)
            {
                return this.MoveCurrentToPosition(this.CurrentPosition + 1);
            }
            else
            {
                return false;
            }
        }

        public virtual bool MoveCurrentToPrevious()
        {
            this.VerifyRefreshNotDeferred();
            if (this.CurrentPosition >= 0)
            {
                return this.MoveCurrentToPosition(this.CurrentPosition - 1);
            }
            else
            {
                return false;
            }
        }

        public virtual bool MoveCurrentTo(object item)
        {
            this.VerifyRefreshNotDeferred();
            if (object.Equals(this.CurrentItem, item) && (item != null || this.IsCurrentInView))
            {
                return this.IsCurrentInView;
            }
            int position = -1;
            SCM.IEditableCollectionView editableCollectionView = this as SCM.IEditableCollectionView;
            if ((editableCollectionView != null && editableCollectionView.IsAddingNew && object.Equals(item, editableCollectionView.CurrentAddItem)) || item == null || this.PassesFilter(item))
            {
                position = this.IndexOf(item);
            }
            return this.MoveCurrentToPosition(position);
        }

        public abstract bool MoveCurrentToPosition(int position);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public abstract bool PassesFilter(object item);

        public abstract int IndexOf(object item);

        public abstract object GetItemAt(int index);

        internal Type GetItemType(bool useRepresentativeItem)
        {
            foreach (Type type in this.SourceCollection.GetType().GetInterfaces())
            {
                if (type.Name == CollectionView.IEnumerableT)
                {
                    Type[] genericArguments = type.GetGenericArguments();
                    if (genericArguments.Length == 1)
                    {
                        return genericArguments[0];
                    }
                }
            }
            if (useRepresentativeItem)
            {
                object representativeItem = this.GetRepresentativeItem();
                if (representativeItem != null)
                {
                    return representativeItem.GetType();
                }
            }
            return (Type)null;
        }

        internal object GetRepresentativeItem()
        {
            if (this.IsEmpty)
            {
                return (object)null;
            }
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (current != null)
                {
                    return current;
                }
            }
            return (object)null;
        }

        internal void RefreshInternal()
        {
            this.RefreshOverride();
            this.SetFlag(CollectionView.CollectionViewFlags.NeedsRefresh, false);
        }

        internal void VerifyRefreshNotDeferred()
        {
            if (this.IsRefreshDeferred)
            {
                throw new InvalidOperationException("CollectionView_NoAccessWhileChangesAreDeferred");
            }
        }

        protected virtual void OnPropertyChanged(SCM.PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }
            this.PropertyChanged((object)this, e);
        }

        protected abstract void RefreshOverride();

        protected abstract IEnumerator GetEnumerator();

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            ++this.timestamp;
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged((object)this, args);
            }
            if (args.Action != NotifyCollectionChangedAction.Replace)
            {
                this.OnPropertyChanged("Count");
            }
            bool isEmpty = this.IsEmpty;
            if (isEmpty == this.CheckFlag(CollectionView.CollectionViewFlags.CachedIsEmpty))
            {
                return;
            }
            this.SetFlag(CollectionView.CollectionViewFlags.CachedIsEmpty, isEmpty);
            this.OnPropertyChanged("IsEmpty");
        }

        protected void SetCurrent(object newItem, int newPosition)
        {
            int count = newItem != null ? 0 : (this.IsEmpty ? 0 : this.Count);
            this.SetCurrent(newItem, newPosition, count);
        }

        protected void SetCurrent(object newItem, int newPosition, int count)
        {
            if (newItem != null)
            {
                this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, false);
                this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, false);
            }
            else if (count == 0)
            {
                this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, true);
                this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, true);
                newPosition = -1;
            }
            else
            {
                this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentBeforeFirst, newPosition < 0);
                this.SetFlag(CollectionView.CollectionViewFlags.IsCurrentAfterLast, newPosition >= count);
            }
            this.currentItem = newItem;
            this.currentPosition = newPosition;
        }

        protected bool OKToChangeCurrent()
        {
            SCM.CurrentChangingEventArgs args = new SCM.CurrentChangingEventArgs();
            this.OnCurrentChanging(args);
            return !args.Cancel;
        }

        protected void OnCurrentChanging()
        {
            this.currentPosition = -1;
            this.OnCurrentChanging(CollectionView.uncancelableCurrentChangingEventArgs);
        }

        protected virtual void OnCurrentChanging(SCM.CurrentChangingEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (this.CurrentChangedMonitor.Busy)
            {
                if (!args.IsCancelable)
                {
                    return;
                }
                args.Cancel = true;
            }
            else
            {
                if (this.CurrentChanging == null)
                {
                    return;
                }
                this.CurrentChanging((object)this, args);
            }
        }

        protected virtual void OnCurrentChanged()
        {
            if (this.CurrentChanged == null || !this.CurrentChangedMonitor.Enter())
            {
                return;
            }
            using (this.CurrentChangedMonitor)
            {
                this.CurrentChanged((object)this, EventArgs.Empty);
            }
        }

        protected abstract void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args);

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (!this.CheckFlag(CollectionView.CollectionViewFlags.ShouldProcessCollectionChanged))
            {
                return;
            }
            this.ProcessCollectionChanged(args);
        }

        protected void RefreshOrDefer()
        {
            if (this.IsRefreshDeferred)
            {
                this.SetFlag(CollectionView.CollectionViewFlags.NeedsRefresh, true);
            }
            else
            {
                this.RefreshInternal();
            }
        }

        private bool CheckFlag(CollectionView.CollectionViewFlags flags)
        {
            return (this.flags & flags) != (CollectionView.CollectionViewFlags)0;
        }

        private void SetFlag(CollectionView.CollectionViewFlags flags, bool value)
        {
            if (value)
            {
                this.flags = this.flags | flags;
            }
            else
            {
                this.flags = this.flags & ~flags;
            }
        }

        private void EndDefer()
        {
            --this.deferLevel;
            if (this.deferLevel != 0 || !this.CheckFlag(CollectionView.CollectionViewFlags.NeedsRefresh))
            {
                return;
            }
            this.Refresh();
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new SCM.PropertyChangedEventArgs(propertyName));
        }

        [StructLayout(LayoutKind.Auto, CharSet = CharSet.Unicode)]
        internal class PlaceholderAwareEnumerator : IEnumerator
        {
            private CollectionView collectionView;
            private IEnumerator baseEnumerator;
            private CollectionView.PlaceholderAwareEnumerator.Position position;
            private object newItem;
            private int timestamp;

            public PlaceholderAwareEnumerator(CollectionView collectionView, IEnumerator baseEnumerator, object newItem)
            {
                this.collectionView = collectionView;
                this.timestamp = collectionView.Timestamp;
                this.baseEnumerator = baseEnumerator;
                this.newItem = newItem;
            }

            private enum Position
            {
                BeforeNewItem,
                OnNewItem,
                AfterNewItem,
            }

            public object Current
            {
                get
                {
                    if (this.position != CollectionView.PlaceholderAwareEnumerator.Position.OnNewItem)
                    {
                        return this.baseEnumerator.Current;
                    }
                    else
                    {
                        return this.newItem;
                    }
                }
            }

            public bool MoveNext()
            {
                if (this.timestamp != this.collectionView.Timestamp)
                {
                    throw new InvalidOperationException("CollectionView_EnumeratorVersionChanged");
                }
                if (this.position == CollectionView.PlaceholderAwareEnumerator.Position.BeforeNewItem)
                {
                    if (!this.baseEnumerator.MoveNext() || (this.newItem != CollectionView.NoNewItem && this.baseEnumerator.Current == this.newItem && !this.baseEnumerator.MoveNext()))
                    {
                        if (this.newItem == CollectionView.NoNewItem)
                        {
                            return false;
                        }
                        this.position = CollectionView.PlaceholderAwareEnumerator.Position.OnNewItem;
                    }
                    return true;
                }
                else
                {
                    this.position = CollectionView.PlaceholderAwareEnumerator.Position.AfterNewItem;
                    if (!this.baseEnumerator.MoveNext())
                    {
                        return false;
                    }
                    if (this.newItem != CollectionView.NoNewItem && this.baseEnumerator.Current == this.newItem)
                    {
                        return this.baseEnumerator.MoveNext();
                    }
                    return true;
                }
            }

            public void Reset()
            {
                this.position = CollectionView.PlaceholderAwareEnumerator.Position.BeforeNewItem;
                this.baseEnumerator.Reset();
            }
        }

        [StructLayout(LayoutKind.Auto, CharSet = CharSet.Unicode)]
        internal class SimpleMonitor : IDisposable
        {
            private bool entered;

            public bool Busy
            {
                get
                {
                    return this.entered;
                }
            }

            public bool Enter()
            {
                if (this.entered)
                {
                    return false;
                }

                this.entered = true;
                return true;
            }

            public void Dispose()
            {
                this.entered = false;
                GC.SuppressFinalize((object)this);
            }
        }

        [StructLayout(LayoutKind.Auto, CharSet = CharSet.Unicode)]
        private class DeferHelper : IDisposable
        {
            private CollectionView collectionView;

            public DeferHelper(CollectionView collectionView)
            {
                this.collectionView = collectionView;
            }

            public void Dispose()
            {
                if (this.collectionView != null)
                {
                    this.collectionView.EndDefer();
                    this.collectionView = (CollectionView)null;
                }
                GC.SuppressFinalize((object)this);
            }
        }
    }
}