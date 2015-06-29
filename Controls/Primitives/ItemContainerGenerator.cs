using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Appercode.UI.Controls.Primitives
{
    public class ItemContainerGenerator 
    {
        private readonly List<int> selectedIndexes = new List<int>();

        /// <summary>
        /// Creates instanse of ItemContainerGenerator
        /// </summary>
        /// <param name="containerFactory">Factory that generates containers for items</param>
        /// <param name="collection">Collection os items that shold be put into containers</param>
        public ItemContainerGenerator(FrameworkElementFactory containerFactory, IList collection = null)
        {
            this.ContainerFactory = containerFactory;
            this.Collection = collection;
        }

        /// <summary>
        /// Factory that generates containers for items
        /// </summary>
        public FrameworkElementFactory ContainerFactory { get; private set; }

        /// <summary>
        /// Collection os items that shold be put into containers
        /// </summary>
        public IList Collection
        {
            get;
            set;
        }

        /// <summary>
        /// List of currently selected indexes
        /// </summary>
        public List<int> SelectedIndexes
        {
            get
            {
                return this.selectedIndexes;
            }
        }

        /// <summary>
        /// Generate container for item at <paramref name="index"/>
        /// </summary>
        /// <param name="index">index of item in <see cref="Collection"/></param>
        /// <returns>Container(UIElement) filled with item at <paramref name="index"/></returns>
        public virtual DependencyObject Generate(int index)
        {
            return this.FillElementWithData(index, (UIElement)this.ContainerFactory.InstantiateUnoptimizedTree());
        }

        /// <summary>
        /// Fills provided <paramref name="elementToReuse"/> with item at <paramref name="index"/> or create and fills new one if <paramref name="elementToReuse"/> is null
        /// </summary>
        /// <param name="index">index of item in <see cref="Collection"/></param>
        /// <param name="elementToReuse">Container(UIElement) to fill with item at <paramref name="index"/>, or null to create new container</param>
        /// <returns>Container(UIElement) filled with item at <paramref name="index"/></returns>
        public UIElement Reuse(int index, UIElement elementToReuse)
        {
            if (elementToReuse == null)
            {
                return (UIElement)this.Generate(index);
            }

            return (UIElement)this.FillElementWithData(index, elementToReuse);
        }

        protected virtual DependencyObject FillElementWithData(int index, UIElement element)
        {
            element.DataContext = this.Collection[index];

            var lbi = element as ListBoxItem;
            if (lbi != null)
            {
                lbi.IsSelected = this.SelectedIndexes.Contains(index);
            }

            return element;
        }
    }
}