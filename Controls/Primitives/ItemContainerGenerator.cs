using System.Collections.Generic;
using System.Windows;

namespace Appercode.UI.Controls.Primitives
{
    public class ItemContainerGenerator 
    {
        protected readonly ItemCollection collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemContainerGenerator" /> class.
        /// </summary>
        /// <param name="containerFactory">A <see cref="FrameworkElementFactory" /> that generates a container for each item.</param>
        /// <param name="collection">Collection of items that should be put into containers.</param>
        internal ItemContainerGenerator(FrameworkElementFactory containerFactory, ItemCollection collection)
        {
            this.ContainerFactory = containerFactory;
            this.collection = collection;
        }

        /// <summary>
        /// A <see cref="FrameworkElementFactory" /> that generates a container for each item.
        /// </summary>
        public FrameworkElementFactory ContainerFactory { get; }

        /// <summary>
        /// List of items that should be put into containers.
        /// </summary>
        public IReadOnlyList<object> Items
        {
            get { return this.collection; }
        }

        /// <summary>
        /// Collection of indexes of currently selected items.
        /// </summary>
        public ICollection<int> SelectedIndexes { get; } = new HashSet<int>();

        /// <summary>
        /// Returns the container for the item at the specified <paramref name="index" /> within the <see cref="Items" /> collection.
        /// </summary>
        /// <param name="index">An index of item in the <see cref="Items" /> collection.</param>
        /// <returns>A container filled with the item at the specified <paramref name="index" />.</returns>
        public DependencyObject ContainerFromIndex(int index)
        {
            return this.LinkContainerToItem(index, this.ContainerFactory.InstantiateUnoptimizedTree());
        }

        /// <summary>
        /// Fills provided <paramref name="container" /> with item at <paramref name="index" />
        /// or create and fills a new one if <paramref name="container" /> is null.
        /// </summary>
        /// <param name="index">An index of item in <see cref="Items" /> collection.</param>
        /// <param name="container">A container that should be filled with the item at <paramref name="index" />, or null.</param>
        /// <returns>A container filled with the item at <paramref name="index" />.</returns>
        internal DependencyObject Reuse(int index, DependencyObject container)
        {
            if (container == null)
            {
                return this.ContainerFromIndex(index);
            }

            return this.LinkContainerToItem(index, container);
        }

        internal virtual DependencyObject LinkContainerToItem(int index, DependencyObject container)
        {
            container.SetValue(UIElement.DataContextProperty, this.collection[index]);

            var lbi = container as ListBoxItem;
            if (lbi != null)
            {
                lbi.IsSelected = this.SelectedIndexes.Contains(index);
            }

            return container;
        }
    }
}