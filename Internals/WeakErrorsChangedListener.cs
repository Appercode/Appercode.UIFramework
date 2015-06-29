using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal class WeakErrorsChangedListener
    {
        private WeakReference weakErrorsChangedListener;

        private INotifyDataErrorInfo notifyDataErrorInfo;

        private bool notifyChild;

        internal WeakErrorsChangedListener(INotifyDataErrorInfo notifyDataErrorInfo, bool notifyChild, IErrorsChangedListener errorsChangedListener)
        {
            this.notifyDataErrorInfo = notifyDataErrorInfo;
            this.notifyChild = notifyChild;
            notifyDataErrorInfo.ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(this.ErrorsChangedCallback);
            this.weakErrorsChangedListener = new WeakReference(errorsChangedListener);
        }

        internal void Disconnect()
        {
            if (this.notifyDataErrorInfo == null)
            {
                return;
            }
            this.notifyDataErrorInfo.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(this.ErrorsChangedCallback);
            this.notifyDataErrorInfo = null;
            this.weakErrorsChangedListener = null;
        }

        private void ErrorsChangedCallback(object sender, DataErrorsChangedEventArgs args)
        {
            if (this.weakErrorsChangedListener == null)
            {
                return;
            }
            IErrorsChangedListener target = this.weakErrorsChangedListener.Target as IErrorsChangedListener;
            if (target == null)
            {
                this.Disconnect();
                return;
            }
            target.OnErrorsChanged(sender, this.notifyChild, args);
        }
    }
}
