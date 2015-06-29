using Appercode.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class WeakDataContextChangedListener
    {
        private WeakReference weakListener;

        private UIElement mentor;

        private WeakDataContextChangedListener(UIElement source, IDataContextChangedListener listener)
        {
            this.mentor = source;
            this.mentor.DataContextChanged += new DataContextChangedEventHandler(this.MentorDataContextChanged);
            this.weakListener = new WeakReference(listener);
        }

        internal static WeakDataContextChangedListener CreateIfNecessary(object source, IDataContextChangedListener listener)
        {
            if (!(source is UIElement))
            {
                return null;
            }
            return new WeakDataContextChangedListener(source as UIElement, listener);
        }

        internal void Disconnect()
        {
            if (this.mentor == null)
            {
                return;
            }
            this.mentor.DataContextChanged -= new DataContextChangedEventHandler(this.MentorDataContextChanged);
            this.mentor = null;
            this.weakListener = null;
        }

        private void MentorDataContextChanged(object sender, DataContextChangedEventArgs e)
        {
            if (this.weakListener == null)
            {
                return;
            }
            IDataContextChangedListener target = this.weakListener.Target as IDataContextChangedListener;
            if (target == null)
            {
                this.Disconnect();
                return;
            }
            target.OnDataContextChanged(sender, e);
        }
    }
}
