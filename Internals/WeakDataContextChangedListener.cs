using Appercode.UI.Controls;
using System;

namespace Appercode.UI.Internals
{
    internal class WeakDataContextChangedListener
    {
        private WeakReference<IDataContextChangedListener> weakListener;
        private UIElement mentor;

        internal WeakDataContextChangedListener(UIElement mentor, IDataContextChangedListener listener)
        {
            this.mentor = mentor;
            this.mentor.DataContextChanged += this.MentorDataContextChanged;
            this.weakListener = new WeakReference<IDataContextChangedListener>(listener);
        }

        internal void Disconnect()
        {
            if (this.mentor != null)
            {
                this.mentor.DataContextChanged -= this.MentorDataContextChanged;
                this.mentor = null;
                this.weakListener = null;
            }
        }

        private void MentorDataContextChanged(object sender, DataContextChangedEventArgs e)
        {
            if (this.weakListener != null)
            {
                IDataContextChangedListener target;
                if (this.weakListener.TryGetTarget(out target))
                {
                    target.OnDataContextChanged(sender, e);
                }
                else
                {
                    this.Disconnect();
                }
            }
        }
    }
}
