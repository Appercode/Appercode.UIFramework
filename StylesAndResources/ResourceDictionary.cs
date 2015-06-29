using Appercode.UI.Controls;
using Appercode.UI.Internals;
using Appercode.UI.Markup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Appercode.UI.StylesAndResources
{
    /// <summary>Provides a hash table / dictionary implementation that contains resources used by components and other elements of a Appercode application. </summary>
    public partial class ResourceDictionary : IDictionary, ICollection, IEnumerable, INameScope, ISupportInitialize
    {
        private Dictionary<object, object> baseDictionary;

        private List<UIElement> ownerFEs;

        private List<WeakReference> deferredResourceReferences;

        private ObservableCollection<ResourceDictionary> mergedDictionaries;

        private ResourceDictionary.PrivateFlags flags;

        private DependencyObject inheritanceContext;

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.ResourceDictionary" /> class. </summary>
        public ResourceDictionary()
        {
            this.baseDictionary = new Dictionary<object, object>();
        }

        private enum PrivateFlags : byte
        {
            IsInitialized = 1,
            IsInitializePending = 2,
            IsReadOnly = 4,
            IsThemeDictionary = 8,
            HasImplicitStyles = 16,
            CanBeAccessedAcrossThreads = 32
        }

        /// <summary>Gets the number of entries in the base <see cref="T:System.Windows.ResourceDictionary" />. </summary>
        /// <returns>The current number of entries in the base dictionary.</returns>
        public int Count
        {
            get
            {
                return this.baseDictionary.Count;
            }
        }

        /// <summary>Gets whether this <see cref="T:System.Windows.ResourceDictionary" /> is fixed-size. </summary>
        /// <returns>true if the hash table is fixed-size; otherwise, false.</returns>
        public bool IsFixedSize
        {
            get
            {
                return ((IDictionary)this.baseDictionary).IsFixedSize;
            }
        }

        /// <summary>Gets whether this <see cref="T:System.Windows.ResourceDictionary" /> is read-only. </summary>
        /// <returns>true if the hash table is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return this.ReadPrivateFlag(ResourceDictionary.PrivateFlags.IsReadOnly);
            }
            set
            {
                this.WritePrivateFlag(ResourceDictionary.PrivateFlags.IsReadOnly, value);
                if (value)
                {
                    this.SealValues();
                }
                if (this.mergedDictionaries != null)
                {
                    for (int i = 0; i < this.mergedDictionaries.Count; i++)
                    {
                        this.mergedDictionaries[i].IsReadOnly = value;
                    }
                }
            }
        }

        /// <summary>Gets a collection of all keys contained in this <see cref="T:System.Windows.ResourceDictionary" />. </summary>
        /// <returns>The collection of all keys.</returns>
        public ICollection Keys
        {
            get
            {
                object[] objArray = new object[this.Count];
                this.baseDictionary.Keys.CopyTo(objArray, 0);
                return objArray;
            }
        }

        /// <summary>Gets a collection of the <see cref="T:System.Windows.ResourceDictionary" /> dictionaries that constitute the various resource dictionaries in the merged dictionaries.</summary>
        /// <returns>The collection of merged dictionaries.</returns>
        public Collection<ResourceDictionary> MergedDictionaries
        {
            get
            {
                if (this.mergedDictionaries == null)
                {
                    this.mergedDictionaries = new ResourceDictionaryCollection(this);
                    this.mergedDictionaries.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnMergedDictionariesChanged);
                }
                return this.mergedDictionaries;
            }
        }

        /// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.IsSynchronized" />.</summary>
        /// <returns>true if access to <see cref="T:System.Windows.ResourceDictionary" /> is synchronized (thread safe); otherwise, false. </returns>
        bool System.Collections.ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)this.baseDictionary).IsSynchronized;
            }
        }

        /// <summary>For a description of this member, see <see cref="P:System.Collections.ICollection.SyncRoot" />.</summary>
        /// <returns>An object that can be used to synchronize access to <see cref="T:System.Windows.ResourceDictionary" />. </returns>
        object System.Collections.ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)this.baseDictionary).SyncRoot;
            }
        }

        /// <summary> Gets a collection of all values associated with keys contained in this <see cref="T:System.Windows.ResourceDictionary" />. </summary>
        /// <returns>The collection of all values.</returns>
        public ICollection Values
        {
            get
            {
                return new ResourceDictionary.ResourceValuesCollection(this);
            }
        }

        internal bool CanBeAccessedAcrossThreads
        {
            get
            {
                return this.ReadPrivateFlag(ResourceDictionary.PrivateFlags.CanBeAccessedAcrossThreads);
            }
            set
            {
                this.WritePrivateFlag(ResourceDictionary.PrivateFlags.CanBeAccessedAcrossThreads, value);
            }
        }

        internal List<WeakReference> DeferredResourceReferences
        {
            get
            {
                return this.deferredResourceReferences;
            }
        }

        internal bool HasImplicitStyles
        {
            get
            {
                return this.ReadPrivateFlag(ResourceDictionary.PrivateFlags.HasImplicitStyles);
            }
            set
            {
                this.WritePrivateFlag(ResourceDictionary.PrivateFlags.HasImplicitStyles, value);
            }
        }

        private bool IsInitialized
        {
            get
            {
                return this.ReadPrivateFlag(ResourceDictionary.PrivateFlags.IsInitialized);
            }
            set
            {
                this.WritePrivateFlag(ResourceDictionary.PrivateFlags.IsInitialized, value);
            }
        }

        private bool IsInitializePending
        {
            get
            {
                return this.ReadPrivateFlag(ResourceDictionary.PrivateFlags.IsInitializePending);
            }
            set
            {
                this.WritePrivateFlag(ResourceDictionary.PrivateFlags.IsInitializePending, value);
            }
        }

        private bool IsThemeDictionary
        {
            get
            {
                return this.ReadPrivateFlag(ResourceDictionary.PrivateFlags.IsThemeDictionary);
            }
            set
            {
                this.WritePrivateFlag(ResourceDictionary.PrivateFlags.IsThemeDictionary, value);
            }
        }

        /// <summary> Gets or sets the value associated with the given key. </summary>
        /// <returns>Value of the key.</returns>
        /// <param name="key">The desired key to get or set.</param>
        public object this[object key]
        {
            get
            {
                bool flag;
                return this.GetValue(key, out flag);
            }
            set
            {
                this.SealValue(value);
                if (!this.CanBeAccessedAcrossThreads)
                {
                    this.SetValueWithoutLock(key, value);
                }
                else
                {
                    lock (((ICollection)this).SyncRoot)
                    {
                        this.SetValueWithoutLock(key, value);
                    }
                }
            }
        }

        /// <summary>Adds a resource by key to this <see cref="T:System.Windows.ResourceDictionary" />. </summary>
        /// <param name="key">The name of the key to add.</param>
        /// <param name="value">The value of the resource to add.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Windows.ResourceDictionary" /> is locked or read-only.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Hashtable" />. </exception>
        public void Add(object key, object value)
        {
            this.SealValue(value);
            if (!this.CanBeAccessedAcrossThreads)
            {
                this.AddWithoutLock(key, value);
            }
            else
            {
                lock (((ICollection)this).SyncRoot)
                {
                    this.AddWithoutLock(key, value);
                }
            }
        }

        /// <summary>Begins the initialization phase for this <see cref="T:System.Windows.ResourceDictionary" />. </summary>
        /// <exception cref="T:System.InvalidOperationException">Called <see cref="M:System.Windows.ResourceDictionary.BeginInit" /> more than once before <see cref="M:System.Windows.ResourceDictionary.EndInit" /> was called.</exception>
        public void BeginInit()
        {
            if (this.IsInitializePending)
            {
                throw new InvalidOperationException("NestedBeginInitNotSupported");
            }
            this.IsInitializePending = true;
            this.IsInitialized = false;
        }

        /// <summary>Clears all keys (and values) in the base <see cref="T:System.Windows.ResourceDictionary" />. This does not clear any merged dictionary items.</summary>
        public void Clear()
        {
            if (!this.CanBeAccessedAcrossThreads)
            {
                this.ClearWithoutLock();
            }
            else
            {
                lock (((ICollection)this).SyncRoot)
                {
                    this.ClearWithoutLock();
                }
            }
        }

        /// <summary>Copies the <see cref="T:System.Windows.ResourceDictionary" /> elements to a one-dimensional <see cref="T:System.Collections.DictionaryEntry" /> at the specified index. </summary>
        /// <param name="array">The one-dimensional array that is the destination of the <see cref="T:System.Collections.DictionaryEntry" /> objects copied from the <see cref="T:System.Windows.ResourceDictionary" /> instance. The array must have zero-based indexing. </param>
        /// <param name="arrayIndex">The zero-based index of <paramref name="array" /> where copying begins.</param>
        public void CopyTo(DictionaryEntry[] array, int arrayIndex)
        {
            if (!this.CanBeAccessedAcrossThreads)
            {
                this.CopyToWithoutLock(array, arrayIndex);
            }
            else
            {
                lock (((ICollection)this).SyncRoot)
                {
                    this.CopyToWithoutLock(array, arrayIndex);
                }
            }
        }

        /// <summary>Determines whether the <see cref="T:System.Windows.ResourceDictionary" /> contains an element with the specified key. </summary>
        /// <returns>true if <see cref="T:System.Windows.ResourceDictionary" /> contains a key-value pair with the specified key; otherwise, false.</returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Windows.ResourceDictionary" />.</param>
        public bool Contains(object key)
        {
            bool flag = this.baseDictionary.ContainsKey(key);
            if (this.mergedDictionaries != null)
            {
                for (int i = this.MergedDictionaries.Count - 1; i > -1 && !flag; i--)
                {
                    ResourceDictionary item = this.MergedDictionaries[i];
                    if (item != null)
                    {
                        flag = item.Contains(key);
                    }
                }
            }
            return flag;
        }

        /// <summary>Ends the initialization phase, and invalidates the previous tree such that all changes made to keys during the initialization phase can be accounted for. </summary>
        public void EndInit()
        {
            if (!this.IsInitializePending)
            {
                throw new InvalidOperationException("EndInitWithoutBeginInitNotSupported");
            }
            this.IsInitializePending = false;
            this.IsInitialized = true;
            this.NotifyOwners(new ResourcesChangeInfo(null, this));
        }

        /// <summary>Not supported by this Dictionary implementation. </summary>
        /// <returns>Always returns null.</returns>
        /// <param name="name">See Remarks.</param>
        public object FindName(string name)
        {
            return null;
        }

        /// <summary>Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> that can be used to iterate through the <see cref="T:System.Windows.ResourceDictionary" />. </summary>
        /// <returns>A specialized enumerator for the <see cref="T:System.Windows.ResourceDictionary" />.</returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return new ResourceDictionary.ResourceDictionaryEnumerator(this);
        }

        /// <summary>Not supported by this Dictionary implementation. </summary>
        /// <param name="name">See Remarks.</param>
        /// <param name="scopedElement">See Remarks.</param>
        /// <exception cref="T:System.NotSupportedException">In all cases when this method is called.</exception>
        public void RegisterName(string name, object scopedElement)
        {
            throw new NotSupportedException("NamesNotSupportedInsideResourceDictionary");
        }

        /// <summary>Removes the entry with the specified key from the base dictionary. </summary>
        /// <param name="key">Key of the entry to remove.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Windows.ResourceDictionary" /> is locked or read-only.</exception>
        public void Remove(object key)
        {
            if (!this.CanBeAccessedAcrossThreads)
            {
                this.RemoveWithoutLock(key);
            }
            else
            {
                lock (((ICollection)this).SyncRoot)
                {
                    this.RemoveWithoutLock(key);
                }
            }
        }

        /// <summary>For a description of this member, see <see cref="M:System.Collections.ICollection.CopyTo(System.Array,System.Int32)" />.</summary>
        /// <param name="array">A zero-based <see cref="T:System.Array" /> that receives the copied items from the <see cref="T:System.Windows.Markup.Localizer.BamlLocalizationDictionary" />.</param>
        /// <param name="arrayIndex">The first position in the specified <see cref="T:System.Array" /> to receive the copied contents.</param>
        void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
        {
            this.CopyTo(array as DictionaryEntry[], arrayIndex);
        }

        /// <summary>For a description of this member, see <see cref="M:System.Collections.IEnumerable.GetEnumerator" />.</summary>
        /// <returns>An <see cref="T:System.Collections." /><see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IDictionary)this).GetEnumerator();
        }

        /// <summary>Not supported by this Dictionary implementation.</summary>
        /// <param name="name">See Remarks.</param>
        public void UnregisterName(string name)
        {
        }

        internal void AddOwner(DispatcherObject owner)
        {
            if (this.inheritanceContext == null)
            {
                this.inheritanceContext = owner as DependencyObject;
                if (this.inheritanceContext == null)
                {
                    this.inheritanceContext = new DependencyObject();
                    this.inheritanceContext.DetachFromDispatcher();
                }
                else
                {
                    this.AddInheritanceContextToValues();
                }
            }
            UIElement frameworkElement = owner as UIElement;
            if (frameworkElement != null)
            {
                if (this.ownerFEs == null)
                {
                    this.ownerFEs = new List<UIElement>(1);
                }
                else if (this.ownerFEs.Contains(frameworkElement) && this.ContainsCycle(this))
                {
                    throw new InvalidOperationException("ResourceDictionaryInvalidMergedDictionary");
                }
                if (this.HasImplicitStyles)
                {
                    ////frameworkElement.ShouldLookupImplicitStyles = true;
                }
                this.ownerFEs.Add(frameworkElement);
            }
            this.AddOwnerToAllMergedDictionaries(owner);
            this.TryInitialize();
        }

        internal bool ContainsOwner(DispatcherObject owner)
        {
            UIElement frameworkElement = owner as UIElement;
            if (frameworkElement != null)
            {
                if (this.ownerFEs == null)
                {
                    return false;
                }
                return this.ownerFEs.Contains(frameworkElement);
            }
            return false;            
        }

        internal object FetchResource(object resourceKey, bool allowDeferredResourceReference, bool mustReturnDeferredResourceReference, out bool canCache)
        {
            DeferredResourceReference deferredThemeResourceReference;
            if (!allowDeferredResourceReference || !mustReturnDeferredResourceReference || !this.Contains(resourceKey))
            {
                return this.GetValue(resourceKey, out canCache);
            }
            canCache = false;
            
            deferredThemeResourceReference = new DeferredResourceReference(this, resourceKey);
            
            if (this.deferredResourceReferences == null)
            {
                this.deferredResourceReferences = new List<WeakReference>();
            }
            this.deferredResourceReferences.Add(new WeakReference(deferredThemeResourceReference));
            return deferredThemeResourceReference;
        }

        internal object GetValue(object key, out bool canCache)
        {
            object valueWithoutLock;
            if (!this.CanBeAccessedAcrossThreads)
            {
                return this.GetValueWithoutLock(key, out canCache);
            }
            lock (((ICollection)this).SyncRoot)
            {
                valueWithoutLock = this.GetValueWithoutLock(key, out canCache);
            }
            return valueWithoutLock;
        }

        internal Type GetValueType(object key, out bool found)
        {
            found = false;
            Type type = null;
            object item = this.baseDictionary[key];
            if (item != null)
            {
                found = true;
                type = item.GetType();                
            }
            else if (this.mergedDictionaries != null)
            {
                for (int i = this.MergedDictionaries.Count - 1; i > -1; i--)
                {
                    ResourceDictionary resourceDictionaries = this.MergedDictionaries[i];
                    if (resourceDictionaries != null)
                    {
                        type = resourceDictionaries.GetValueType(key, out found);
                        if (found)
                        {
                            break;
                        }
                    }
                }
            }
            return type;
        }

        internal void RemoveOwner(DispatcherObject owner)
        {
            UIElement frameworkElement = owner as UIElement;
            if (frameworkElement != null && this.ownerFEs != null)
            {
                this.ownerFEs.Remove(frameworkElement);
                if (this.ownerFEs.Count == 0)
                {
                    this.ownerFEs = null;
                }
            }
            if (owner == this.inheritanceContext)
            {
                this.RemoveInheritanceContextFromValues();
                this.inheritanceContext = null;
            }
            this.RemoveOwnerFromAllMergedDictionaries(owner);
        }

        internal void RemoveParentOwners(ResourceDictionary mergedDictionary)
        {
            if (this.ownerFEs != null)
            {
                for (int i = 0; i < this.ownerFEs.Count; i++)
                {
                    mergedDictionary.RemoveOwner(this.ownerFEs[i]);
                }
            }
        }

        private void AddInheritanceContext(object value)
        {
            /*
            //if (this.inheritanceContext.ProvideSelfAsInheritanceContext(value, VisualBrush.VisualProperty))
            //{
            //    DependencyObject dependencyObject = value as DependencyObject;
            //    if (dependencyObject != null)
            //    {
            //        dependencyObject.IsInheritanceContextSealed = true;
            //    }
            //}
            */ 
        }

        private void AddInheritanceContextToValues()
        {
            foreach (object value in this.baseDictionary.Values)
            {
                this.AddInheritanceContext(value);
            }
        }

        private void AddOwnerToAllMergedDictionaries(DispatcherObject owner)
        {
            if (this.mergedDictionaries != null)
            {
                for (int i = 0; i < this.mergedDictionaries.Count; i++)
                {
                    this.mergedDictionaries[i].AddOwner(owner);
                }
            }
        }

        private void AddWithoutLock(object key, object value)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("ResourceDictionaryIsReadOnly");
            }
            this.baseDictionary.Add(key, value);
            this.UpdateHasImplicitStyles(key);
            this.NotifyOwners(new ResourcesChangeInfo(key));
        }

        private void ClearWithoutLock()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("ResourceDictionaryIsReadOnly");
            }
            if (this.Count > 0)
            {
                this.ValidateDeferredResourceReferences(null);
                this.RemoveInheritanceContextFromValues();
                this.baseDictionary.Clear();
                this.NotifyOwners(ResourcesChangeInfo.CatastrophicDictionaryChangeInfo);
            }
        }

        private bool ContainsCycle(ResourceDictionary origin)
        {
            for (int i = 0; i < this.MergedDictionaries.Count; i++)
            {
                ResourceDictionary item = this.MergedDictionaries[i];
                if (item == origin || item.ContainsCycle(origin))
                {
                    return true;
                }
            }
            return false;
        }

        private void CopyToWithoutLock(DictionaryEntry[] array, int arrayIndex)
        {
            bool flag;
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            ((ICollection)this.baseDictionary).CopyTo(array, arrayIndex);
            int num = arrayIndex + this.Count;
            for (int i = arrayIndex; i < num; i++)
            {
                DictionaryEntry dictionaryEntry = array[i];
                object value = dictionaryEntry.Value;
                if (this.RealizeDeferContent(dictionaryEntry.Key, ref value, out flag))
                {
                    dictionaryEntry.Value = value;
                }
            }
        }

        private object GetValueWithoutLock(object key, out bool canCache)
        {
            object item;
            if (this.baseDictionary.TryGetValue(key, out item) == false)
            {
                canCache = true;
                if (this.mergedDictionaries != null)
                {
                    for (int i = this.MergedDictionaries.Count - 1; i > -1; i--)
                    {
                        ResourceDictionary resourceDictionaries = this.MergedDictionaries[i];
                        if (resourceDictionaries != null)
                        {
                            item = resourceDictionaries.GetValue(key, out canCache);
                            if (item != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                this.RealizeDeferContent(key, ref item, out canCache);
            }
            return item;
        }

        private void NotifyOwners(ResourcesChangeInfo info)
        {
            bool isInitialized = this.IsInitialized;
            bool flag = !info.IsResourceAddOperation ? false : this.HasImplicitStyles;
            if (isInitialized || flag)
            {
                if (this.ownerFEs != null)
                {
                    for (int i = 0; i < this.ownerFEs.Count; i++)
                    {
                        if (flag)
                        {
                            this.ownerFEs[i].ShouldLookupImplicitStyles = true;
                        }
                        if (isInitialized)
                        {
                            throw new NotImplementedException();
                            ////TreeWalkHelper.InvalidateOnResourcesChange(this.ownerFEs[i], null, info);
                        }
                    }
                }
            }
        }

        private void OnMergedDictionariesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResourceDictionary item;
            ResourcesChangeInfo catastrophicDictionaryChangeInfo;
            bool flag;
            List<ResourceDictionary> resourceDictionaries = null;
            List<ResourceDictionary> resourceDictionaries1 = null;
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                catastrophicDictionaryChangeInfo = ResourcesChangeInfo.CatastrophicDictionaryChangeInfo;
            }
            else
            {
                if (e.NewItems == null || e.NewItems.Count <= 0)
                {
                    flag = e.OldItems == null ? false : e.OldItems.Count > 0;
                }
                else
                {
                    flag = true;
                }
                System.Diagnostics.Debug.Assert(flag, "The NotifyCollectionChanged event fired when no dictionaries were added or removed");
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
                {
                    resourceDictionaries = new List<ResourceDictionary>(e.OldItems.Count);
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        item = (ResourceDictionary)e.OldItems[i];
                        resourceDictionaries.Add(item);
                        this.RemoveParentOwners(item);
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
                {
                    resourceDictionaries1 = new List<ResourceDictionary>(e.NewItems.Count);
                    for (int j = 0; j < e.NewItems.Count; j++)
                    {
                        item = (ResourceDictionary)e.NewItems[j];
                        resourceDictionaries1.Add(item);
                        if (!this.HasImplicitStyles && item.HasImplicitStyles)
                        {
                            this.HasImplicitStyles = true;
                        }
                        this.PropagateParentOwners(item);
                    }
                }
                catastrophicDictionaryChangeInfo = new ResourcesChangeInfo(resourceDictionaries, resourceDictionaries1, false, false, null);
            }
            this.NotifyOwners(catastrophicDictionaryChangeInfo);
        }

        private void PropagateParentOwners(ResourceDictionary mergedDictionary)
        {
            if (this.ownerFEs != null)
            {
                if (mergedDictionary.ownerFEs == null)
                {
                    mergedDictionary.ownerFEs = new List<UIElement>(this.ownerFEs.Count);
                }
                for (int i = 0; i < this.ownerFEs.Count; i++)
                {
                    mergedDictionary.AddOwner(this.ownerFEs[i]);
                }
            }
        }

        private bool ReadPrivateFlag(ResourceDictionary.PrivateFlags bit)
        {
            return (byte)(this.flags & bit) != 0;
        }

        private bool RealizeDeferContent(ref object value)
        {
            bool flag;
            return this.RealizeDeferContent(null, ref value, out flag);
        }

        private bool RealizeDeferContent(object key, ref object value, out bool canCache)
        {
            canCache = false;
            return false;            
        }

        private void RemoveInheritanceContext(object value)
        {
            throw new NotImplementedException();
        }

        private void RemoveInheritanceContextFromValues()
        {
            foreach (object value in this.baseDictionary.Values)
            {
                this.RemoveInheritanceContext(value);
            }
        }

        private void RemoveOwnerFromAllMergedDictionaries(DispatcherObject owner)
        {
            if (this.mergedDictionaries != null)
            {
                for (int i = 0; i < this.mergedDictionaries.Count; i++)
                {
                    this.mergedDictionaries[i].RemoveOwner(owner);
                }
            }
        }

        private void RemoveWithoutLock(object key)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("ResourceDictionaryIsReadOnly");
            }
            this.ValidateDeferredResourceReferences(key);
            this.RemoveInheritanceContext(this.baseDictionary[key]);
            this.baseDictionary.Remove(key);
            this.NotifyOwners(new ResourcesChangeInfo(key));
        }

        private void SealValue(object value)
        {
            if (this.inheritanceContext != null)
            {
                this.AddInheritanceContext(value);
            }
            if (this.IsReadOnly)
            {
                StyleHelper.SealIfSealable(value);
            }
        }

        private void SealValues()
        {
            foreach (object value in this.baseDictionary.Values)
            {
                this.SealValue(value);
            }
        }

        private void SetValueWithoutLock(object key, object value)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("ResourceDictionaryIsReadOnly");
            }
            if (!this.baseDictionary.ContainsKey(key) || this.baseDictionary[key] != value)
            {
                this.ValidateDeferredResourceReferences(key);
                this.baseDictionary[key] = value;
                this.UpdateHasImplicitStyles(key);
                this.NotifyOwners(new ResourcesChangeInfo(key));
            }
        }

        private void TryInitialize()
        {
            if (!this.IsInitializePending && !this.IsInitialized)
            {
                this.IsInitialized = true;
            }
        }

        private void UpdateHasImplicitStyles(object key)
        {
            if (!this.HasImplicitStyles && key is Type)
            {
                this.HasImplicitStyles = true;
            }
        }

        private void ValidateDeferredResourceReferences(object resourceKey)
        {
            if (this.deferredResourceReferences != null)
            {
                IEnumerator enumerator = this.deferredResourceReferences.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DeferredResourceReference current = enumerator.Current as DeferredResourceReference;
                    if (current == null || (resourceKey != null && !object.Equals(resourceKey, current.Key)))
                    {
                        continue;
                    }
                    current.GetValue(BaseValueSourceInternal.Unknown);
                }
            }
        }

        private void WritePrivateFlag(ResourceDictionary.PrivateFlags bit, bool value)
        {
            if (value)
            {
                ResourceDictionary resourceDictionaries = this;
                resourceDictionaries.flags = (ResourceDictionary.PrivateFlags)((byte)(resourceDictionaries.flags | bit));
                return;
            }
            ResourceDictionary resourceDictionaries1 = this;
            resourceDictionaries1.flags = (ResourceDictionary.PrivateFlags)((byte)((byte)resourceDictionaries1.flags & (byte)(~bit)));
        }

        private class ResourceDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
        {
            private ResourceDictionary owner;

            private IEnumerator keysEnumerator;

            internal ResourceDictionaryEnumerator(ResourceDictionary owner)
            {
                this.owner = owner;
                this.keysEnumerator = this.owner.Keys.GetEnumerator();
            }

            DictionaryEntry System.Collections.IDictionaryEnumerator.Entry
            {
                get
                {
                    object current = this.keysEnumerator.Current;
                    return new DictionaryEntry(current, this.owner[current]);
                }
            }

            object System.Collections.IDictionaryEnumerator.Key
            {
                get
                {
                    return this.keysEnumerator.Current;
                }
            }

            object System.Collections.IDictionaryEnumerator.Value
            {
                get
                {
                    return this.owner[this.keysEnumerator.Current];
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return ((IDictionaryEnumerator)this).Entry;
                }
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                return this.keysEnumerator.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                this.keysEnumerator.Reset();
            }
        }

        private class ResourceValuesCollection : ICollection, IEnumerable
        {
            private ResourceDictionary owner;

            internal ResourceValuesCollection(ResourceDictionary owner)
            {
                this.owner = owner;
            }

            int System.Collections.ICollection.Count
            {
                get
                {
                    return this.owner.Count;
                }
            }

            bool System.Collections.ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object System.Collections.ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            void System.Collections.ICollection.CopyTo(Array array, int index)
            {
                foreach (object key in this.owner.Keys)
                {
                    int num = index;
                    index = num + 1;
                    array.SetValue(this.owner[key], num);
                }
            }

            IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new ResourceDictionary.ResourceValuesEnumerator(this.owner);
            }
        }

        private class ResourceValuesEnumerator : IEnumerator
        {
            private ResourceDictionary owner;

            private IEnumerator keysEnumerator;

            internal ResourceValuesEnumerator(ResourceDictionary owner)
            {
                this.owner = owner;
                this.keysEnumerator = this.owner.Keys.GetEnumerator();
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return this.owner[this.keysEnumerator.Current];
                }
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                return this.keysEnumerator.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                this.keysEnumerator.Reset();
            }
        }
    }
}