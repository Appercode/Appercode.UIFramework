using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Appercode.UI.Controls.Primitives
{
    public abstract class Selector : ItemsControl // , ISupportInitialize
    {
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
            DependencyProperty.Register("IsSynchronizedWithCurrentItem", typeof(Nullable<bool>), typeof(Selector), new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Selector), new PropertyMetadata(-1, (d, e) =>
            {
                if ((int)e.NewValue < -1 || (int)e.NewValue >= ((Selector)d).Items.Count)
                {
                    throw new ArgumentOutOfRangeException("Selector.SelectedIndex");
                }
                ((Selector)d).ApplySelectedIndex((int)e.NewValue, (int)e.OldValue);
            }));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Selector), new PropertyMetadata(null, (d, e) =>
                {
                    ((Selector)d).ApplySelectedItem(e.NewValue);
                }));

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(Selector), new PropertyMetadata(null, (d, e) =>
                {
                    ((Selector)d).ApplySelectedValue(e.NewValue);
                }));

        protected internal bool IsSelectionActive;

        public Selector()
        {
        }

        public event SelectionChangedEventHandler SelectionChanged;

        [TypeConverterAttribute(typeof(NullableBoolConverter))]
        public bool? IsSynchronizedWithCurrentItem
        {
            get { return (bool?)this.GetValue(IsSynchronizedWithCurrentItemProperty); }
            set { this.SetValue(IsSynchronizedWithCurrentItemProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)this.GetValue(SelectedIndexProperty); }
            set { this.SetValue(SelectedIndexProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        public object SelectedValue
        {
            get { return (object)this.GetValue(SelectedValueProperty); }
            set { this.SetValue(SelectedValueProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected virtual void ApplySelectedIndex(int newSelectedIndex, int oldSelectedIndex)
        {
            if (this.IsSelectionActive)
            {
                return;
            }

            IList removedItems;
            List<object> addedItems = new List<object>();

            removedItems = this.GetSelectedItems();
            addedItems.Add(this.Items[newSelectedIndex]);

            this.IsSelectionActive = true;

            this.UnselectAllItems();

            object obj;
            if (newSelectedIndex >= 0)
            {
                obj = this.Items[newSelectedIndex];
                this.SelectItem(newSelectedIndex);
            }
            else
            {
                obj = null;
            }

            this.SelectedItem = obj;
            this.SelectedValue = obj; 
            
            this.OnSelectionChanged(removedItems, addedItems);

            this.IsSelectionActive = false;
        }

        protected virtual void ApplySelectedItem(object selectedItem)
        {
            if (this.IsSelectionActive)
            {
                return;
            }

            int index = this.Items.IndexOf(selectedItem);

            if (index != -1 && selectedItem != null)
            {
                this.IsSelectionActive = true;

                IList removedItems;
                List<object> addedItems = new List<object>();

                removedItems = this.GetSelectedItems();
                addedItems.Add(selectedItem);

                this.UnselectAllItems();
                this.SelectItem(index);

                this.SelectedItem = selectedItem;
                this.SelectedValue = selectedItem;
                this.SelectedIndex = index;

                this.OnSelectionChanged(removedItems, addedItems);

                this.IsSelectionActive = false;
            }
        }

        protected virtual void ApplySelectedValue(object selectedValue)
        {
            if (this.IsSelectionActive)
            {
                return;
            }

            int index = this.Items.IndexOf(selectedValue);

            if (index == -1)
            {
                if (selectedValue != null)
                {
                    this.IsSelectionActive = true;

                    IList removedItems;
                    List<object> addedItems = null;

                    removedItems = this.GetSelectedItems();

                    this.UnselectAllItems();
                    this.SelectItem(index);

                    this.UnselectAllItems();
                    this.SelectedValue = null;
                    this.SelectedItem = null;
                    this.SelectedIndex = -1;

                    this.OnSelectionChanged(removedItems, addedItems);

                    this.IsSelectionActive = false;
                }
            }
            else
            {
                this.IsSelectionActive = true;

                IList removedItems;
                List<object> addedItems = new List<object>();

                removedItems = this.GetSelectedItems();
                addedItems.Add(selectedValue);

                this.UnselectAllItems();
                this.SelectItem(index);

                this.SelectedValue = selectedValue;
                this.SelectedIndex = index;
                this.SelectedItem = this.Items[index];

                this.OnSelectionChanged(removedItems, addedItems);

                this.IsSelectionActive = false;
            }
        }

        protected virtual void OnSelectionChanged(IList removedItems, IList addedItems)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, new SelectionChangedEventArgs(removedItems, addedItems));
            }
        }

        protected virtual void UnselectAllItems()
        {
        }

        /// <summary>
        /// Selects Item at <paramref name="index"/>
        /// </summary>
        /// <param name="index">index of Item to select</param>
        /// <returns>true if Item was selected otherwise false</returns>
        protected virtual bool SelectItem(int index)
        {
            return false;
        }

        protected virtual IList GetSelectedItems()
        {
            List<object> list = new List<object>();

            list.Add(this.Items[this.SelectedIndex]);

            return list;
        }
    }
}