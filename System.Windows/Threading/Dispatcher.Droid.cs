using Android.OS;
using System.Threading;

namespace System.Windows.Threading
{
    public sealed partial class Dispatcher
    {
        /// <summary>
        /// Invoke in UI thread.
        /// </summary>
        public void BeginInvoke(Action a)
        {
            var h = new Handler(Looper.MainLooper);
            h.Post(a);
        }

        /// <summary>
        /// Invoke in UI thread.
        /// </summary>
        public void BeginInvoke(Delegate d, params object[] args)
        {
            var h = new Handler(Looper.MainLooper);
            h.Post(() => d.DynamicInvoke(args));
        }

        /// <summary>
        /// Is current thread the UI thread.
        /// </summary>
        public bool CheckAccess()
        {
            return SynchronizationContext.Current != null;
        }

        private static object InvokeInternal(Delegate d, object[] args)
        {
            throw new NotImplementedException("Synchronous invocation from a non-UI thread is not implemented.");
        }
    }
}