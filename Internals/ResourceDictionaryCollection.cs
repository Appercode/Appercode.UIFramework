using System;
using System.Collections.ObjectModel;
using Appercode.UI.StylesAndResources;

namespace Appercode.UI.Internals
{
    internal class ResourceDictionaryCollection : ObservableCollection<ResourceDictionary>
    {
        private ResourceDictionary owner;

        internal ResourceDictionaryCollection(ResourceDictionary owner)
        {
            this.owner = owner;
        }

        protected override void ClearItems()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.owner.RemoveParentOwners(base[i]);
            }
            base.ClearItems();
        }

        protected override void InsertItem(int index, ResourceDictionary item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, ResourceDictionary item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }
    }
}
