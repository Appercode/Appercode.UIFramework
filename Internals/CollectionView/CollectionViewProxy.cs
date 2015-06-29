using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using SCM = System.ComponentModel;

namespace Appercode.UI.Internals.CollectionView
{
    internal class CollectionViewProxy : ICollectionView, IEnumerable, INotifyCollectionChanged, SCM.INotifyPropertyChanged, SCM.IEditableCollectionView, IViewLifetime
    {
        private ICollectionView view;

        internal CollectionViewProxy(ICollectionView view)
        {
            this.view = view;
            view.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
            view.CurrentChanging += new SCM.CurrentChangingEventHandler(this.OnCurrentChanging);
            view.CurrentChanged += new EventHandler(this.OnCurrentChanged);
            SCM.INotifyPropertyChanged notifyPropertyChanged = view as SCM.INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
            {
                notifyPropertyChanged.PropertyChanged += new SCM.PropertyChangedEventHandler(this.OnPropertyChanged);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event EventHandler CurrentChanged;

        public event SCM.CurrentChangingEventHandler CurrentChanging;

        public event SCM.PropertyChangedEventHandler PropertyChanged;

        public bool CanFilter
        {
            get
            {
                return this.ProxiedView.CanFilter;
            }
        }

        public bool CanGroup
        {
            get
            {
                return this.ProxiedView.CanGroup;
            }
        }

        public bool CanSort
        {
            get
            {
                return this.ProxiedView.CanSort;
            }
        }

        public CultureInfo Culture
        {
            get
            {
                return this.ProxiedView.Culture;
            }
            set
            {
                this.ProxiedView.Culture = value;
            }
        }

        public object CurrentItem
        {
            get
            {
                return this.ProxiedView.CurrentItem;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return this.ProxiedView.CurrentPosition;
            }
        }

        public Predicate<object> Filter
        {
            get
            {
                return this.ProxiedView.Filter;
            }
            set
            {
                this.ProxiedView.Filter = value;
            }
        }

        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this.ProxiedView.GroupDescriptions;
            }
        }

        public ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return this.ProxiedView.Groups;
            }
        }

        public bool IsCurrentAfterLast
        {
            get
            {
                return this.ProxiedView.IsCurrentAfterLast;
            }
        }

        public bool IsCurrentBeforeFirst
        {
            get
            {
                return this.ProxiedView.IsCurrentBeforeFirst;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.ProxiedView.IsEmpty;
            }
        }

        public ICollectionView ProxiedView
        {
            get
            {
                return this.view;
            }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                return this.ProxiedView.SortDescriptions;
            }
        }

        public IEnumerable SourceCollection
        {
            get
            {
                return this.ProxiedView.SourceCollection;
            }
        }

        bool SCM.IEditableCollectionView.CanAddNew
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return false;
                }
                return proxiedView.CanAddNew;
            }
        }

        bool System.ComponentModel.IEditableCollectionView.CanCancelEdit
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return false;
                }
                return proxiedView.CanCancelEdit;
            }
        }

        bool System.ComponentModel.IEditableCollectionView.CanRemove
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return false;
                }
                return proxiedView.CanRemove;
            }
        }

        object SCM.IEditableCollectionView.CurrentAddItem
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return null;
                }
                return proxiedView.CurrentAddItem;
            }
        }

        object SCM.IEditableCollectionView.CurrentEditItem
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return null;
                }
                return proxiedView.CurrentEditItem;
            }
        }

        bool SCM.IEditableCollectionView.IsAddingNew
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return false;
                }
                return proxiedView.IsAddingNew;
            }
        }

        bool SCM.IEditableCollectionView.IsEditingItem
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return false;
                }
                return proxiedView.IsEditingItem;
            }
        }

        SCM.NewItemPlaceholderPosition SCM.IEditableCollectionView.NewItemPlaceholderPosition
        {
            get
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    return SCM.NewItemPlaceholderPosition.None;
                }
                return proxiedView.NewItemPlaceholderPosition;
            }
            set
            {
                SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
                if (proxiedView == null)
                {
                    throw new InvalidOperationException("NewItemPlaceholderPosition - MemberNotAllowedForView");
                }
                proxiedView.NewItemPlaceholderPosition = value;
            }
        }

        public object ViewManagerData
        {
            get;
            set;
        }

        public bool Contains(object item)
        {
            return this.ProxiedView.Contains(item);
        }

        public IDisposable DeferRefresh()
        {
            return this.ProxiedView.DeferRefresh();
        }

        public IEnumerator GetEnumerator()
        {
            return this.ProxiedView.GetEnumerator();
        }

        public bool MoveCurrentTo(object item)
        {
            return this.ProxiedView.MoveCurrentTo(item);
        }

        public bool MoveCurrentToFirst()
        {
            return this.ProxiedView.MoveCurrentToFirst();
        }

        public bool MoveCurrentToLast()
        {
            return this.ProxiedView.MoveCurrentToLast();
        }

        public bool MoveCurrentToNext()
        {
            return this.ProxiedView.MoveCurrentToNext();
        }

        public bool MoveCurrentToPosition(int position)
        {
            return this.ProxiedView.MoveCurrentToPosition(position);
        }

        public bool MoveCurrentToPrevious()
        {
            return this.ProxiedView.MoveCurrentToPrevious();
        }

        public void Refresh()
        {
            this.ProxiedView.Refresh();
        }

        object SCM.IEditableCollectionView.AddNew()
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("AddNew is not allowed for view");
            }
            return proxiedView.AddNew();
        }

        void SCM.IEditableCollectionView.CancelEdit()
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("CancelEdit is not allowed for view");
            }
            proxiedView.CancelEdit();
        }

        void SCM.IEditableCollectionView.CancelNew()
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("CancelNew is not allowed for view");
            }
            proxiedView.CancelNew();
        }

        void SCM.IEditableCollectionView.CommitEdit()
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("CommitEdit is not allowed for view");
            }
            proxiedView.CommitEdit();
        }

        void SCM.IEditableCollectionView.CommitNew()
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("CommitNew is not allowed for view");
            }
            proxiedView.CommitNew();
        }

        void SCM.IEditableCollectionView.EditItem(object item)
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("EditItem is not allowed for view");
            }
            proxiedView.EditItem(item);
        }

        void SCM.IEditableCollectionView.Remove(object item)
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("Remove is not allowed for view");
            }
            proxiedView.Remove(item);
        }

        void SCM.IEditableCollectionView.RemoveAt(int index)
        {
            SCM.IEditableCollectionView proxiedView = this.ProxiedView as SCM.IEditableCollectionView;
            if (proxiedView == null)
            {
                throw new InvalidOperationException("RemoveAt is not allowed for view");
            }
            proxiedView.RemoveAt(index);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = this.CollectionChanged;
            if (notifyCollectionChangedEventHandler != null)
            {
                notifyCollectionChangedEventHandler.Invoke(this, args);
            }
        }

        private void OnCurrentChanged(object sender, EventArgs args)
        {
            EventHandler eventHandler = this.CurrentChanged;
            if (eventHandler != null)
            {
                eventHandler(this, args);
            }
        }

        private void OnCurrentChanging(object sender, SCM.CurrentChangingEventArgs args)
        {
            SCM.CurrentChangingEventHandler currentChangingEventHandler = this.CurrentChanging;
            if (currentChangingEventHandler != null)
            {
                currentChangingEventHandler(this, args);
            }
        }

        private void OnPropertyChanged(object sender, SCM.PropertyChangedEventArgs args)
        {
            SCM.PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
            if (propertyChangedEventHandler != null)
            {
                propertyChangedEventHandler(this, args);
            }
        }
    }
}