using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Appercode.UI.Internals.CollectionView
{
    internal class ViewManager
    {
        private static ViewManager currentViewManager;

        private Dictionary<WeakRefKey, WeakReference> objectsToViewTables = new Dictionary<WeakRefKey, WeakReference>();

        public ViewManager()
        {
        }

        public static ViewManager Current
        {
            get
            {
                if (ViewManager.currentViewManager == null)
                {
                    ViewManager.currentViewManager = new ViewManager();
                }
                return ViewManager.currentViewManager;
            }
        }

        private ViewTable this[object o]
        {
            get
            {
                WeakRefKey weakRefKey = new WeakRefKey(o);
                WeakReference weakReference = null;
                ViewTable target = null;
                if (this.objectsToViewTables.TryGetValue(weakRefKey, out weakReference))
                {
                    target = (ViewTable)weakReference.Target;
                    if (target == null)
                    {
                        this.objectsToViewTables.Remove(weakRefKey);
                    }
                }
                return target;
            }
            set
            {
                WeakRefKey weakRefKey = new WeakRefKey(o);
                this.objectsToViewTables[weakRefKey] = new WeakReference(value);
            }
        }

        internal ViewRecord GetViewRecord(object collection, Appercode.UI.Data.CollectionViewSource cvs)
        {
            ViewTable item = this[collection];
            ViewRecord viewRecord = null;
            if (item != null)
            {
                viewRecord = item[cvs];
                if (viewRecord != null)
                {
                    return viewRecord;
                }
            }
            return this.CreateAndCacheNewView(collection, cvs);
        }

        private void Cleanup()
        {
            List<WeakRefKey> weakRefKeys = new List<WeakRefKey>();
            foreach (WeakRefKey key in this.objectsToViewTables.Keys)
            {
                if (key.Target != null)
                {
                    continue;
                }
                weakRefKeys.Add(key);
            }
            foreach (WeakRefKey weakRefKey in weakRefKeys)
            {
                this.objectsToViewTables.Remove(weakRefKey);
            }
        }

        private ViewRecord CreateAndCacheNewView(object collection, Appercode.UI.Data.CollectionViewSource cvs)
        {
            ICollectionView collectionViews = this.CreateNewView(collection);
            ViewTable viewTable = this.EnsureViewTableForCollection(collection);
            ViewRecord viewRecord = new ViewRecord(collectionViews);
            viewTable[cvs] = viewRecord;
            ((IViewLifetime)collectionViews).ViewManagerData = viewTable;
            this.Cleanup();
            return viewRecord;
        }

        private ICollectionView CreateNewView(object collection)
        {
            ICollectionViewFactory collectionViewFactory = collection as ICollectionViewFactory;
            if (collectionViewFactory != null)
            {
                ICollectionView collectionViews = collectionViewFactory.CreateView();
                if (collectionViews == null)
                {
                    throw new InvalidOperationException("Unsupported Null Collection View");
                }
                if (collectionViews is IViewLifetime)
                {
                    return collectionViews;
                }
                return new CollectionViewProxy(collectionViews);
            }
            IList lists = collection as IList;
            if (lists != null)
            {
                return new ListCollectionView(lists);
            }
            IEnumerable enumerable = collection as IEnumerable;
            if (enumerable == null)
            {
                throw new InvalidOperationException("Collection View Unsupported Source Type");
            }
            return new EnumerableCollectionView(enumerable);
        }

        private ViewTable EnsureViewTableForCollection(object collection)
        {
            ViewTable item = this[collection];
            if (item == null)
            {
                item = new ViewTable();
                this[collection] = item;
            }
            return item;
        }
    }
}
