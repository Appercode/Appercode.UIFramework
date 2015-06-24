using Foundation;
using System.Threading;

namespace System.Windows.Threading
{
    public partial class Dispatcher
    {
        private static readonly NSObject invoker = new NSObject();

        /// <summary>
        /// Invoke in UI thread.
        /// </summary>
        public void BeginInvoke(Action a)
        {
            invoker.BeginInvokeOnMainThread(a.Invoke);
        }

        /// <summary>
        /// Invoke in UI thread.
        /// </summary>
        public void BeginInvoke(Delegate d, params object[] args)
        {
            invoker.BeginInvokeOnMainThread(() => d.DynamicInvoke(args));
        }

        /// <summary>
        /// Is current thread the UI thread.
        /// </summary>
        public bool CheckAccess()
        {
            return SynchronizationContext.Current != null;
        }
    }
}
