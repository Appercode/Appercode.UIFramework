using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Appercode.UI.Data
{
    public abstract class CollectionViewGroup : INotifyPropertyChanged
    {
        private object name;

        private ObservableCollection<object> itemsRW;

        private ReadOnlyObservableCollection<object> itemsRO;

        private int itemCount;

        protected CollectionViewGroup(object name)
        {
            this.name = name;
            this.itemsRW = new ObservableCollection<object>();
            this.itemsRO = new ReadOnlyObservableCollection<object>(this.itemsRW);
        }

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.PropertyChanged += value;
            }
            remove
            {
                this.PropertyChanged -= value;
            }
        }

        public abstract bool IsBottomLevel
        {
            get;
        }

        public int ItemCount
        {
            get
            {
                return this.itemCount;
            }
        }

        public ReadOnlyObservableCollection<object> Items
        {
            get
            {
                return this.itemsRO;
            }
        }

        public object Name
        {
            get
            {
                return this.name;
            }
        }

        protected int ProtectedItemCount
        {
            get
            {
                return this.itemCount;
            }
            set
            {
                this.itemCount = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ItemCount"));
            }
        }

        protected ObservableCollection<object> ProtectedItems
        {
            get
            {
                return this.itemsRW;
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
    }
}
