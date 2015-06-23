using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace Appercode.UI.Controls
{
    public abstract class PresentationFrameworkCollection<T> : DependencyObject, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        private List<T> internalCollection = new List<T>();

        public PresentationFrameworkCollection()
        {
        }

        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged = delegate { };

        public int Count
        {
            get
            {
                return this.internalCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot { get; private set; }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                return this.internalCollection[index];
            }
            set
            {
                var oldValue = this.internalCollection[index];
                this.internalCollection[index] = value;
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index));
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this.internalCollection[index];
            }
            set
            {
                if (value != null && !(value is T))
                {
                    throw new ArgumentException("InvalidArgument", "value");
                }
                this.internalCollection[index] = (T)value;
            }
        }

        public int IndexOf(T item)
        {
            return this.internalCollection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.internalCollection.Insert(index, item);
            this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            this.internalCollection.InsertRange(index, items);
            this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index));
        }

        public void RemoveAt(int index)
        {
            var item = this.internalCollection[index];
            this.internalCollection.RemoveAt(index);
            this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        public void Add(T item)
        {
            this.internalCollection.Add(item);
            this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, this.internalCollection.Count - 1));
        }

        public void AddRange(IEnumerable<T> items)
        {
            var index = this.internalCollection.Count;
            this.internalCollection.AddRange(items);
            this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index));
        }

        public void Clear()
        {
            if (this.internalCollection.Count > 0)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                this.internalCollection.Clear();
            }
        }

        public bool Contains(T item)
        {
            return this.internalCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.internalCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var index = this.internalCollection.IndexOf(item);
            
            if (index != -1)
            {
                this.internalCollection.RemoveAt(index);
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.internalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Add(object value)
        {
            if (value != null && !(value is T))
            {
                throw new ArgumentException("InvalidArgument", "value");
            }

            this.Add((T)value);
            return this.Count - 1;
        }

        public bool Contains(object value)
        {
            return this.internalCollection.IndexOf((T)value) != -1;
        }

        public int IndexOf(object value)
        {
            if (value != null && !(value is T))
            {
                return -1;
            }

            return this.internalCollection.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            this.Insert(index, (T)value);
        }

        public void Remove(object value)
        {
            if (value != null && !(value is T))
            {
                throw new ArgumentException("InvalidArgument", "value");
            }

            this.Remove((T)value);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        internal void Sort(IComparer<T> comparer)
        {
            this.internalCollection.Sort(comparer);
        }

        internal void Sort(Comparison<T> comparison)
        {
            this.internalCollection.Sort(comparison);
        }

        public class CollectionChangedEventArgs : EventArgs
        {
            public CollectionChangedEventArgs()
            {
                this.Added = new List<T>();
                this.Removed = new List<T>();
            }

            public List<T> Added { get; private set; }
            public List<T> Removed { get; private set; }
        }
    }
}