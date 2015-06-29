using System;

namespace System.Windows.Threading
{
    public partial class DispatcherObject
    {
        public DispatcherObject()
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void VerifyAccess()
        {
        }
    }
}
