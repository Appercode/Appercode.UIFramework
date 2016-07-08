using System.Collections.Generic;
using System.Security;
using System.Threading;

namespace System.Windows.Threading
{
    public sealed partial class Dispatcher
    {
        private static Dispatcher mainDispatcher;
        private static object globalLock = new object();
        private static List<WeakReference> dispatchers = new List<WeakReference>();
        private static WeakReference possibleDispatcher = new WeakReference(null);
        private Thread dispatcherThread;

        [SecurityCritical]
        [SecuritySafeCritical]
        private Dispatcher()
        {
            this.dispatcherThread = Thread.CurrentThread;
            lock (Dispatcher.globalLock)
            {
                Dispatcher.dispatchers.Add(new WeakReference(this));
            }
        }

        /// <summary>Gets the <see cref="T:System.Windows.Threading.Dispatcher" /> for the thread currently executing and creates a new <see cref="T:System.Windows.Threading.Dispatcher" /> if one is not already associated with the thread. </summary>
        /// <returns>The dispatcher associated with the current thread.</returns>
        public static Dispatcher CurrentDispatcher
        {
            get
            {
                return Dispatcher.FromThread(Thread.CurrentThread) ?? new Dispatcher();
            }
        }

        /// <summary>Gets the thread this <see cref="T:System.Windows.Threading.Dispatcher" /> is associated with.</summary>
        /// <returns>The thread.</returns>
        public Thread Thread
        {
            get
            {
                return this.dispatcherThread;
            }
        }

        internal static Dispatcher MainDispatcher
        {
            get
            {
                if (Dispatcher.mainDispatcher == null)
                {
                    Dispatcher.mainDispatcher = new Dispatcher();
                }
                return Dispatcher.mainDispatcher;
            }
        }

        /// <summary>Gets the <see cref="T:System.Windows.Threading.Dispatcher" /> for the specified thread. </summary>
        /// <returns>The dispatcher for <paramref name="thread" />.</returns>
        /// <param name="thread">The thread to obtain the <see cref="T:System.Windows.Threading.Dispatcher" /> from.</param>
        public static Dispatcher FromThread(Thread thread)
        {
            Dispatcher dispatcher;
            lock (Dispatcher.globalLock)
            {
                Dispatcher target = null;
                if (thread != null)
                {
                    target = Dispatcher.possibleDispatcher.Target as Dispatcher;
                    if (target == null || target.Thread != thread)
                    {
                        target = null;
                        for (int i = 0; i < Dispatcher.dispatchers.Count; i++)
                        {
                            Dispatcher target1 = Dispatcher.dispatchers[i].Target as Dispatcher;
                            if (target1 == null)
                            {
                                Dispatcher.dispatchers.RemoveAt(i);
                                i--;
                            }
                            else if (target1.Thread == thread)
                            {
                                target = target1;
                            }
                        }
                        if (target != null)
                        {
                            Dispatcher.possibleDispatcher.Target = target;
                        }
                    }
                }
                dispatcher = target;
            }
            return dispatcher;
        }

        /// <summary>
        /// Executes the specified <see cref="Action" /> delegate asynchronously on the UI thread.
        /// </summary>
        /// <param name="a">An <see cref="Action" /> delegate to execute.</param>
        public void BeginInvoke(Action a)
        {
            this.NativeBeginInvoke(a);
        }

        /// <summary>
        /// Executes the specified delegate with the specified arguments asynchronously on the UI thread.
        /// </summary>
        /// <param name="d">A delegate to execute.</param>
        /// <param name="args">Arguments to pass to the delegate.</param>
        public void BeginInvoke(Delegate d, params object[] args)
        {
            this.BeginInvoke(new DelegateWrapper(d, args).Invoke);
        }

        /// <summary>
        /// Checks if current thread equals to the UI thread.
        /// </summary>
        public bool CheckAccess()
        {
            return SynchronizationContext.Current != null;
        }

        /// <summary>
        /// Executes the specified delegate with the specified arguments synchronously on the UI thread.
        /// <para /> If current thread is the UI thread, the delegate will be executed immediately, otherwise execution will be queued on the UI thread.
        /// </summary>
        /// <param name="d">A delegate to execute.</param>
        /// <param name="args">Arguments to pass to the delegate.</param>
        /// <returns>The value returned from the delegate or null if the delegate has no return value.</returns>
        public object Invoke(Delegate d, params object[] args)
        {
            if (this.CheckAccess())
            {
                return d.DynamicInvoke(args);
            }
            else
            {
                object result = null;
                this.NativeInvoke(ref result, d, args);
                return result;
            }
        }

        public void VerifyAccess()
        {
            if (!this.CheckAccess())
            {
                throw new UnauthorizedAccessException("Invalid cross-thread access.");
            }
        }

        partial void NativeBeginInvoke(Action a);

        partial void NativeInvoke(ref object result, Delegate d, params object[] args);
    }
}