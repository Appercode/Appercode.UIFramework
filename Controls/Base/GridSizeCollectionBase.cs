#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion //Using directives

namespace Appercode.UI.Controls.Base
{
    /// <summary>
    /// Represents a base class for Grid Row/Column definition collections
    /// </summary>
    /// <typeparam name="TItem">Collection item type</typeparam>
    public abstract class GridSizeCollectionBase<TItem> : IList<TItem>, IList
        where TItem : DefinitionBase
    {
        #region Fields

        /// <summary>
        /// Holds internal items list
        /// </summary>
        private IList<TItem> internalList = new List<TItem>();

        /// <summary>
        /// Holds parent grid
        /// </summary>
        private Grid owner;

        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Initializes the collection
        /// </summary>
        /// <param name="owner"></param>
        protected GridSizeCollectionBase(Grid owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            this.owner = owner;
        }

        #endregion //Constructors

        #region Interfaces implementation

        /// <summary>
        /// Gets the number of elements contained in the collection
        /// </summary>
        public int Count
        {
            get { return this.internalList.Count; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection
        /// </summary>
        public object SyncRoot
        {
            get { return ((IList)this.internalList).SyncRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((IList)this.internalList).IsSynchronized; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return ((IList)this.internalList).IsFixedSize; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (TItem)value; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TItem this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
                this.OnObjectAttached(value);
                this.OnModified();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds a new item to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(TItem item)
        {
            this.internalList.Add(item);
            this.OnObjectAttached(item);
            this.OnModified();
        }

        /// <summary>
        /// Adds a new item to the collection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(object value)
        {
            var res = ((IList)this.internalList).Add((TItem)value);
            this.OnObjectAttached((TItem)value);
            this.OnModified();
            return res;
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            return value is TItem && this.Contains((TItem)value);
        }

        /// <summary>
        /// Removes all items from the collection
        /// </summary>
        public void Clear()
        {
            this.internalList.Clear();
            this.OnModified();
        }

        /// <summary>
        /// etermines the index of a specific item in the collection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object value)
        {
            return this.IndexOf((TItem)value);
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, object value)
        {
            this.Insert(index, (TItem)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value)
        {
            this.Remove((TItem)value);
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TItem item)
        {
            return this.internalList.Contains(item);
        }

        /// <summary>
        ///  Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <returns></returns>
        public bool Remove(TItem item)
        {
            var res = this.internalList.Remove(item);
            this.OnModified();
            return res;
        }

        /// <summary>
        /// Copies the elements of the collection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            ((IList)this.internalList).CopyTo(array, index);
        }

        /// <summary>
        /// Determines the index of a specific item in the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TItem item)
        {
            return this.internalList.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, TItem item)
        {
            this.internalList.Insert(index, item);
            this.OnObjectAttached(item);
            this.OnModified();
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
            this.OnModified();
        }

        #endregion // Interfaces implementation

        #region Protected virtual methods

        /// <summary>
        /// Invoked when collection is modified
        /// Invalidates parent grid
        /// </summary>
        protected virtual void OnModified()
        {
            this.owner.Invalidate();
        }

        /// <summary>
        /// Invoked when new item attached to collection
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnObjectAttached(TItem item)
        {
            item.ParentGrid = this.owner;
        }

        #endregion //Protected virtual methods
    }
}