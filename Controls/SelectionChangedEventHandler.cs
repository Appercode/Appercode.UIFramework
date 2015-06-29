using System;
using System.Collections;

namespace Appercode.UI.Controls
{
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    public class SelectionChangedEventArgs : RoutedEventArgs
    {
        private object[] addedItems;

        private object[] removedItems;

        public SelectionChangedEventArgs(IList removedItems, IList addedItems)
        {
            if (removedItems == null)
            {
                throw new ArgumentNullException("removedItems");
            }
            if (addedItems == null)
            {
                throw new ArgumentNullException("addedItems");
            }
            this.removedItems = new object[removedItems.Count];
            removedItems.CopyTo(this.removedItems, 0);
            this.addedItems = new object[addedItems.Count];
            addedItems.CopyTo(this.addedItems, 0);
        }

        public IList AddedItems
        {
            get
            {
                return this.addedItems;
            }
        }

        public IList RemovedItems
        {
            get
            {
                return this.removedItems;
            }
        }
    }
}