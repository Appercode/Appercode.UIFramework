using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Appercode.UI.Internals.CollectionView
{
    internal class EnumerableCollectionView : Appercode.UI.Data.CollectionView
    {
        private ListCollectionView view;

        private ObservableCollection<object> snapshot;

        private IEnumerator trackingEnumerator;

        private int ignoreEventsLevel;

        private bool pollForChanges;

        internal EnumerableCollectionView(IEnumerable source)
            : base(source)
        {
            this.snapshot = new ObservableCollection<object>();
            this.LoadSnapshotCore(source);
            if (this.snapshot.Count <= 0)
            {
                this.SetCurrent(null, -1, 0);
            }
            else
            {
                this.SetCurrent(this.snapshot[0], 0, 1);
            }
            this.pollForChanges = !(source is INotifyCollectionChanged);
            this.view = new ListCollectionView(this.snapshot);
            this.view.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnViewChanged);
            this.view.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyChanged);
            this.view.CurrentChanging += new CurrentChangingEventHandler(this.OnCurrentChanging);
            this.view.CurrentChanged += new EventHandler(this.OnCurrentChanged);
        }

        public override bool CanFilter
        {
            get
            {
                return this.view.CanFilter;
            }
        }

        public override bool CanGroup
        {
            get
            {
                return this.view.CanGroup;
            }
        }

        public override bool CanSort
        {
            get
            {
                return this.view.CanSort;
            }
        }

        public override int Count
        {
            get
            {
                this.EnsureSnapshot();
                return this.view.Count;
            }
        }

        public override CultureInfo Culture
        {
            get
            {
                return this.view.Culture;
            }
            set
            {
                this.view.Culture = value;
            }
        }

        public override object CurrentItem
        {
            get
            {
                return this.view.CurrentItem;
            }
        }

        public override int CurrentPosition
        {
            get
            {
                return this.view.CurrentPosition;
            }
        }

        public override Predicate<object> Filter
        {
            get
            {
                return this.view.Filter;
            }
            set
            {
                this.view.Filter = value;
            }
        }

        public override ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this.view.GroupDescriptions;
            }
        }

        public override ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return this.view.Groups;
            }
        }

        public override bool IsCurrentAfterLast
        {
            get
            {
                return this.view.IsCurrentAfterLast;
            }
        }

        public override bool IsCurrentBeforeFirst
        {
            get
            {
                return this.view.IsCurrentBeforeFirst;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                this.EnsureSnapshot();
                if (this.view == null)
                {
                    return true;
                }
                return this.view.IsEmpty;
            }
        }

        public override bool NeedsRefresh
        {
            get
            {
                return this.view.NeedsRefresh;
            }
        }

        public override SortDescriptionCollection SortDescriptions
        {
            get
            {
                return this.view.SortDescriptions;
            }
        }

        public override bool Contains(object item)
        {
            this.EnsureSnapshot();
            return this.view.Contains(item);
        }

        public override IDisposable DeferRefresh()
        {
            return this.view.DeferRefresh();
        }

        public override object GetItemAt(int index)
        {
            this.EnsureSnapshot();
            return this.view.GetItemAt(index);
        }

        public override int IndexOf(object item)
        {
            this.EnsureSnapshot();
            return this.view.IndexOf(item);
        }

        public override bool MoveCurrentTo(object item)
        {
            return this.view.MoveCurrentTo(item);
        }

        public override bool MoveCurrentToFirst()
        {
            return this.view.MoveCurrentToFirst();
        }

        public override bool MoveCurrentToLast()
        {
            return this.view.MoveCurrentToLast();
        }

        public override bool MoveCurrentToNext()
        {
            return this.view.MoveCurrentToNext();
        }

        public override bool MoveCurrentToPosition(int position)
        {
            return this.view.MoveCurrentToPosition(position);
        }

        public override bool MoveCurrentToPrevious()
        {
            return this.view.MoveCurrentToPrevious();
        }

        public override bool PassesFilter(object item)
        {
            if (!this.view.CanFilter || this.view.Filter == null)
            {
                return true;
            }
            return this.view.Filter(item);
        }

        protected override void ProcessCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (args.NewStartingIndex < 0 || this.snapshot.Count <= args.NewStartingIndex)
                        {
                            for (int i = 0; i < args.NewItems.Count; i++)
                            {
                                this.snapshot.Add(args.NewItems[i]);
                            }
                            return;
                        }
                        for (int j = args.NewItems.Count - 1; j >= 0; j--)
                        {
                            this.snapshot.Insert(args.NewStartingIndex, args.NewItems[j]);
                        }
                        return;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (args.OldStartingIndex < 0)
                        {
                            throw new InvalidOperationException("EnumerableCollectionView_AddedItemNotFound");
                        }
                        int count = args.OldItems.Count - 1;
                        int oldStartingIndex = args.OldStartingIndex + count;
                        while (count >= 0)
                        {
                            if (!object.Equals(args.OldItems[count], this.snapshot[oldStartingIndex]))
                            {
                                throw new InvalidOperationException("ListCollectionView_AddedItemNotAtIndex");
                            }
                            this.snapshot.RemoveAt(oldStartingIndex);
                            count--;
                            oldStartingIndex--;
                        }
                        return;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        int num = args.NewItems.Count - 1;
                        int newStartingIndex = args.NewStartingIndex + num;
                        while (num >= 0)
                        {
                            if (!object.Equals(args.OldItems[num], this.snapshot[newStartingIndex]))
                            {
                                throw new InvalidOperationException("ListCollectionView_AddedItemNotAtIndex");
                            }
                            this.snapshot[newStartingIndex] = args.NewItems[num];
                            num--;
                            newStartingIndex--;
                        }
                        return;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        return;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.LoadSnapshot(this.SourceCollection);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        protected override void RefreshOverride()
        {
            this.LoadSnapshot(this.SourceCollection);
        }

        protected override IEnumerator GetEnumerator()
        {
            this.EnsureSnapshot();
            return ((IEnumerable)this.view).GetEnumerator();
        }

        private void OnCurrentChanged(object sender, EventArgs args)
        {
            if (this.ignoreEventsLevel != 0)
            {
                return;
            }
            this.OnCurrentChanged();
        }

        private void OnCurrentChanging(object sender, CurrentChangingEventArgs args)
        {
            if (this.ignoreEventsLevel != 0)
            {
                return;
            }
            this.OnCurrentChanging(args);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (this.ignoreEventsLevel != 0)
            {
                return;
            }
            string propertyName = args.PropertyName;
            string str = propertyName;
            if (propertyName != null)
            {
                switch (str)
                {
                    case "CanAddNew":
                    case "CanCancelEdit":
                    case "CanRemove":
                    case "CurrentAddItem":
                    case "CurrentEditItem":
                    case "IsAddingNew":
                    case "IsEditingItem":
                        {
                            return;
                        }
                }
            }
            this.OnPropertyChanged(args);
        }

        private void OnViewChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (this.ignoreEventsLevel != 0)
            {
                return;
            }
            this.OnCollectionChanged(args);
        }

        private void BeginIgnoreEvents()
        {
            EnumerableCollectionView enumerableCollectionView = this;
            enumerableCollectionView.ignoreEventsLevel = enumerableCollectionView.ignoreEventsLevel + 1;
        }

        private void EndIgnoreEvents()
        {
            EnumerableCollectionView enumerableCollectionView = this;
            enumerableCollectionView.ignoreEventsLevel = enumerableCollectionView.ignoreEventsLevel - 1;
        }

        private void EnsureSnapshot()
        {
            if (this.pollForChanges)
            {
                try
                {
                    this.trackingEnumerator.MoveNext();
                }
                catch (InvalidOperationException e)
                {
                    Debug.WriteLine(e.ToString());
                    this.LoadSnapshotCore(this.SourceCollection);
                }
            }
        }

        private IDisposable IgnoreViewEvents()
        {
            return new EnumerableCollectionView.IgnoreViewEventsHelper(this);
        }

        private void LoadSnapshot(IEnumerable source)
        {
            base.OnCurrentChanging();
            object currentItem = this.CurrentItem;
            int currentPosition = this.CurrentPosition;
            bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
            bool isCurrentAfterLast = this.IsCurrentAfterLast;
            this.LoadSnapshotCore(source);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnCurrentChanged();
            if (this.IsCurrentAfterLast != isCurrentAfterLast)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("IsCurrentAfterLast"));
            }
            if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("IsCurrentBeforeFirst"));
            }
            if (currentPosition != this.CurrentPosition)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPosition"));
            }
            if (currentItem != this.CurrentItem)
            {
                this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentItem"));
            }
        }

        private void LoadSnapshotCore(IEnumerable source)
        {
            this.trackingEnumerator = source.GetEnumerator();
            using (IDisposable disposable = this.IgnoreViewEvents())
            {
                this.snapshot.Clear();
                while (this.trackingEnumerator.MoveNext())
                {
                    this.snapshot.Add(this.trackingEnumerator.Current);
                }
            }
        }

        private class IgnoreViewEventsHelper : IDisposable
        {
            private EnumerableCollectionView parent;

            public IgnoreViewEventsHelper(EnumerableCollectionView parent)
            {
                this.parent = parent;
                this.parent.BeginIgnoreEvents();
            }

            public void Dispose()
            {
                if (this.parent != null)
                {
                    this.parent.EndIgnoreEvents();
                    this.parent = null;
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}
