using System;
using System.Windows;

namespace Appercode.UI.Internals.PathParser
{
    internal class WeakDependencyPropertyChangedListener
    {
        private WeakReference weakDependencyPropertyListener;

        private DependencyObject source;

        private WeakDependencyPropertyChangedListener(DependencyObject source, bool isCoreProperty, DependencyPropertyListener dependencyPropertyListener)
        {
            this.source = source;
            this.source.DPChanged += this.SourcePropertyChanged;
            this.weakDependencyPropertyListener = new WeakReference(dependencyPropertyListener);
        }

        internal static WeakDependencyPropertyChangedListener CreateIfNecessary(DependencyObject source, bool isCoreProperty, DependencyPropertyListener dependencyPropertyListener)
        {
            return new WeakDependencyPropertyChangedListener(source, isCoreProperty, dependencyPropertyListener);
        }

        internal void Disconnect()
        {
            if (this.source == null)
            {
                return;
            }
            this.source.DPChanged -= this.SourcePropertyChanged;
            this.source = null;
            this.weakDependencyPropertyListener = null;
        }

        private void SourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.weakDependencyPropertyListener == null)
            {
                return;
            }
            DependencyPropertyListener target = this.weakDependencyPropertyListener.Target as DependencyPropertyListener;
            if (target == null)
            {
                this.Disconnect();
                return;
            }
            target.SourcePropertyChanged(sender, e.Property);
        }
    }
}
