using Appercode.UI.Controls.Primitives;
using System;
using System.Collections.Specialized;
using System.Windows;

namespace Appercode.UI.Controls
{
    public abstract class VirtualizingPanel : Panel, IVirtualizingPanel
    {
        protected ScrollViewer scrollOwner;

        public ItemContainerGenerator Generator { get; set; }

        public virtual bool HasNativeScroll
        {
            get
            {
                return false;
            }
        }

        public ScrollViewer ScrollOwner
        {
            get
            {
                return this.scrollOwner;
            }
            set
            {
                if (this.scrollOwner != null)
                {
                    this.scrollOwner.ScrollChanged -= this.Value_ScrollChanged;
                }
                if (value != null)
                {
                    value.ScrollChanged += this.Value_ScrollChanged;
                }
                this.scrollOwner = value;
            }
        }

        public virtual void ItemsUpdated(NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual bool SetIsSelectedOnRealizedItem(int index, bool value)
        {
            throw new NotImplementedException();
        }

        public virtual void SetPadding(Thickness padding)
        {
        }

        protected virtual void Value_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
        }
    }
}