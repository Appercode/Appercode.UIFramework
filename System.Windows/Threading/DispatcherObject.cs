using System.Threading;
namespace System.Windows.Threading
{
    public abstract partial class DispatcherObject
    {
        private Dispatcher dispatcher;

        /// <summary>Gets the <see cref="T:System.Windows.Threading.Dispatcher" /> this <see cref="T:System.Windows.Threading.DispatcherObject" /> is associated with. </summary>
        /// <returns>The dispatcher.</returns>
        public Dispatcher Dispatcher
        {
            get
            {
                return this.dispatcher;
            }
        }

        /// <summary>Determines whether the calling thread has access to this <see cref="T:System.Windows.Threading.DispatcherObject" />.</summary>
        /// <returns>true if the calling thread has access to this object; otherwise, false.</returns>
        public bool CheckAccess()
        {
            bool flag = true;
            Dispatcher dispatcher = this.dispatcher;
            if (dispatcher != null)
            {
                flag = dispatcher.CheckAccess();
            }
            return flag;
        }

        internal void DetachFromDispatcher()
        {
            this.dispatcher = null;
        }
    }
}
