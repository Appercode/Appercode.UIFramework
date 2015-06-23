using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.PathParser
{
    internal class WeakPropertyChangedListener
    {
        private WeakReference weakclrPropertyListener;

        private INotifyPropertyChanged notifyPropertyChanged;

        private WeakPropertyChangedListener(INotifyPropertyChanged notify, ISourcePropertyChanged sourcePropertyChanged)
        {
            this.notifyPropertyChanged = notify;
            notify.PropertyChanged += new PropertyChangedEventHandler(this.PropertyChangedCallback);
            this.weakclrPropertyListener = new WeakReference(sourcePropertyChanged);
        }

        internal object Source
        {
            get
            {
                return this.notifyPropertyChanged;
            }
        }

        internal static WeakPropertyChangedListener CreateIfNecessary(object source, ISourcePropertyChanged sourcePropertyChanged)
        {
            INotifyPropertyChanged notifyPropertyChanged = source as INotifyPropertyChanged;
            if (notifyPropertyChanged == null)
            {
                return null;
            }
            return new WeakPropertyChangedListener(notifyPropertyChanged, sourcePropertyChanged);
        }

        internal void Disconnect()
        {
            if (this.notifyPropertyChanged == null)
            {
                return;
            }
            this.notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(this.PropertyChangedCallback);
            this.notifyPropertyChanged = null;
            this.weakclrPropertyListener = null;
        }

        private void PropertyChangedCallback(object sender, PropertyChangedEventArgs args)
        {
            if (this.weakclrPropertyListener == null)
            {
                return;
            }
            ISourcePropertyChanged target = this.weakclrPropertyListener.Target as ISourcePropertyChanged;
            if (target == null)
            {
                this.Disconnect();
                return;
            }
            target.SourcePropertyChanged(sender, args);
        }
    }
}
