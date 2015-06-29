using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class WeakCollectionChangedListener
    {
        private WeakReference weakListener;

        private INotifyCollectionChanged source;

        private WeakCollectionChangedListener(INotifyCollectionChanged source, ICollectionChangedListener listener)
        {
            this.source = source;
            this.source.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SourceCollectionChanged);
            this.weakListener = new WeakReference(listener);
        }

        internal static WeakCollectionChangedListener CreateIfNecessary(object source, ICollectionChangedListener listener)
        {
            if (!(source is INotifyCollectionChanged))
            {
                return null;
            }
            return new WeakCollectionChangedListener(source as INotifyCollectionChanged, listener);
        }

        internal void Disconnect()
        {
            if (this.source == null)
            {
                return;
            }
            this.source.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.SourceCollectionChanged);
            this.source = null;
            this.weakListener = null;
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.weakListener == null)
            {
                return;
            }
            ICollectionChangedListener target = this.weakListener.Target as ICollectionChangedListener;
            if (target == null)
            {
                this.Disconnect();
                return;
            }
            target.OnCollectionChanged(sender, e);
        }
    }
}
