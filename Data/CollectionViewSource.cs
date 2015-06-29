using Appercode.UI.Internals.CollectionView;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Appercode.UI.Data
{
    public class CollectionViewSource : DependencyObject, System.ComponentModel.ISupportInitialize
    {
        public static readonly DependencyProperty ViewProperty;

        public static readonly DependencyProperty SourceProperty;

        private int version;

        private int deferLevel;

        private SortDescriptionCollection sort;

        private ObservableCollection<GroupDescription> groupBy;

        private bool isInitializing;

        private bool isViewInitialized;

        private FilterEventHandler filterHandlers;

        private CollectionViewSource.FilterStub filterStub;

        private CultureInfo culture;

        static CollectionViewSource()
        {
            CollectionViewSource.ViewProperty = DependencyProperty.RegisterReadOnly("View", typeof(ICollectionView), typeof(CollectionViewSource), new PropertyMetadata(null)).DependencyProperty;
            CollectionViewSource.SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(CollectionViewSource), new PropertyMetadata(null, new PropertyChangedCallback(CollectionViewSource.OnSourceChanged)));
        }

        public CollectionViewSource()
        {
            this.sort = new SortDescriptionCollection();
            this.sort.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
            this.groupBy = new ObservableCollection<GroupDescription>();
            this.groupBy.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnForwardedCollectionChanged);
        }

        public event FilterEventHandler Filter
        {
            add
            {
                FilterEventHandler filterEventHandler = this.filterHandlers;
                if (filterEventHandler == null)
                {
                    filterEventHandler = value;
                }
                else
                {
                    filterEventHandler += value;
                }
                this.filterHandlers = filterEventHandler;
                this.OnForwardedPropertyChanged();
            }
            remove
            {
                FilterEventHandler filterEventHandler = this.filterHandlers;
                if (filterEventHandler != null)
                {
                    filterEventHandler -= value;
                    if (filterEventHandler != null)
                    {
                        this.filterHandlers = filterEventHandler;
                    }
                    else
                    {
                        this.filterHandlers = null;
                    }
                }
                this.OnForwardedPropertyChanged();
            }
        }

        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
            set
            {
                this.culture = value;
                this.OnForwardedPropertyChanged();
            }
        }

        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return this.groupBy;
            }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                return this.sort;
            }
        }

        public object Source
        {
            get
            {
                return this.GetValue(CollectionViewSource.SourceProperty);
            }
            set
            {
                this.SetValue(CollectionViewSource.SourceProperty, value);
            }
        }

        public ICollectionView View
        {
            get
            {
                return CollectionViewSource.GetOriginalView(this.CollectionView);
            }
        }

        internal ICollectionView CollectionView
        {
            get
            {
                ICollectionView value = (ICollectionView)this.GetValue(CollectionViewSource.ViewProperty);
                if (value != null && !this.isViewInitialized)
                {
                    ViewRecord viewRecord = ViewManager.Current.GetViewRecord(this.Source, this);
                    value.MoveCurrentToFirst();
                    viewRecord.IsInitialized = true;
                    this.isViewInitialized = true;
                }
                return value;
            }
        }

        private Predicate<object> FilterWrapper
        {
            get
            {
                if (this.filterStub == null)
                {
                    this.filterStub = new CollectionViewSource.FilterStub(this);
                }
                return this.filterStub.FilterWrapper;
            }
        }

        public IDisposable DeferRefresh()
        {
            return new CollectionViewSource.DeferHelper(this);
        }

        void System.ComponentModel.ISupportInitialize.BeginInit()
        {
            this.isInitializing = true;
        }

        void System.ComponentModel.ISupportInitialize.EndInit()
        {
            this.isInitializing = false;
            this.EnsureView();
        }

        protected virtual void OnCollectionViewTypeChanged(Type oldCollectionViewType, Type newCollectionViewType)
        {
        }

        protected virtual void OnSourceChanged(object oldSource, object newSource)
        {
        }

        private static ICollectionView GetOriginalView(ICollectionView view)
        {
            for (CollectionViewProxy i = view as CollectionViewProxy; i != null; i = view as CollectionViewProxy)
            {
                view = i.ProxiedView;
            }
            return view;
        }

        private static bool IsSourceValid(object o)
        {
            if (o != null && !(o is IEnumerable))
            {
                return false;
            }
            return !(o is ICollectionView);
        }

        private static bool IsValidSourceForView(object o)
        {
            if (o == null)
            {
                return true;
            }
            return o is IEnumerable;
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CollectionViewSource collectionViewSource = (CollectionViewSource)d;
            if (!CollectionViewSource.IsSourceValid(e.NewValue))
            {
                throw new ArgumentException("ViewManager: UnsupportedSourceType");
            }
            collectionViewSource.OnSourceChanged(e.OldValue, e.NewValue);
            collectionViewSource.EnsureView();
        }

        private void ApplyPropertiesToView(ICollectionView view)
        {
            int num;
            int count;
            Predicate<object> filterWrapper;
            if (view == null || this.deferLevel > 0)
            {
                return;
            }
            using (IDisposable disposable = view.DeferRefresh())
            {
                if (this.Culture != null)
                {
                    view.Culture = this.Culture;
                }
                if (view.CanSort)
                {
                    view.SortDescriptions.Clear();
                    num = 0;
                    count = this.SortDescriptions.Count;
                    while (num < count)
                    {
                        view.SortDescriptions.Add(this.SortDescriptions[num]);
                        num++;
                    }
                }
                else if (this.SortDescriptions.Count > 0)
                {
                    throw new InvalidOperationException("CollectionViewSource CannotShape Sorting");
                }
                if (this.filterHandlers == null)
                {
                    filterWrapper = null;
                }
                else
                {
                    filterWrapper = this.FilterWrapper;
                }
                if (view.CanFilter)
                {
                    view.Filter = filterWrapper;
                }
                else if (filterWrapper != null)
                {
                    throw new InvalidOperationException("CollectionViewSource CannotShape Filtering");
                }
                if (view.CanGroup)
                {
                    view.GroupDescriptions.Clear();
                    num = 0;
                    count = this.GroupDescriptions.Count;
                    while (num < count)
                    {
                        view.GroupDescriptions.Add(this.GroupDescriptions[num]);
                        num++;
                    }
                }
                else if (this.GroupDescriptions.Count > 0)
                {
                    throw new InvalidOperationException("CollectionViewSource CannotShape Grouping");
                }
            }
        }

        private void BeginDefer()
        {
            CollectionViewSource collectionViewSource = this;
            collectionViewSource.deferLevel = collectionViewSource.deferLevel + 1;
        }

        private void EndDefer()
        {
            CollectionViewSource collectionViewSource = this;
            int num = collectionViewSource.deferLevel - 1;
            int num1 = num;
            collectionViewSource.deferLevel = num;
            if (num1 == 0)
            {
                this.EnsureView();
            }
        }

        private void EnsureView()
        {
            this.EnsureView(this.Source);
        }

        private void EnsureView(object source)
        {
            ICollectionView view;
            if (this.isInitializing || this.deferLevel > 0)
            {
                return;
            }
            if (source == null)
            {
                view = null;
            }
            else
            {
                ViewRecord viewRecord = ViewManager.Current.GetViewRecord(source, this);
                view = viewRecord.View;
                this.isViewInitialized = viewRecord.IsInitialized;
                if (this.version != viewRecord.Version)
                {
                    this.ApplyPropertiesToView(view);
                    viewRecord.Version = this.version;
                }
            }
            this.SetValue(CollectionViewSource.ViewProperty, view);
        }

        private void OnForwardedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnForwardedPropertyChanged();
        }

        private void OnForwardedPropertyChanged()
        {
            CollectionViewSource collectionViewSource = this;
            collectionViewSource.version = collectionViewSource.version + 1;
            this.ApplyPropertiesToView(this.View);
        }

        private bool WrapFilter(object item)
        {
            FilterEventArgs filterEventArg = new FilterEventArgs(item);
            FilterEventHandler filterEventHandler = this.filterHandlers;
            if (filterEventHandler != null)
            {
                filterEventHandler(this, filterEventArg);
            }
            return filterEventArg.Accepted;
        }

        private class DeferHelper : IDisposable
        {
            private CollectionViewSource target;

            public DeferHelper(CollectionViewSource target)
            {
                this.target = target;
                this.target.BeginDefer();
            }

            public void Dispose()
            {
                if (this.target != null)
                {
                    CollectionViewSource collectionViewSource = this.target;
                    this.target = null;
                    collectionViewSource.EndDefer();
                }
                GC.SuppressFinalize(this);
            }
        }

        private class FilterStub
        {
            private WeakReference parent;

            private Predicate<object> filterWrapper;

            public FilterStub(CollectionViewSource parent)
            {
                this.parent = new WeakReference(parent);
                this.filterWrapper = new Predicate<object>(this.WrapFilter);
            }

            public Predicate<object> FilterWrapper
            {
                get
                {
                    return this.filterWrapper;
                }
            }

            private bool WrapFilter(object item)
            {
                CollectionViewSource target = (CollectionViewSource)this.parent.Target;
                if (target == null)
                {
                    return true;
                }
                return target.WrapFilter(item);
            }
        }
    }
}
