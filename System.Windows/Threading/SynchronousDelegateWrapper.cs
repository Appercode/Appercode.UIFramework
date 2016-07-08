using System.Threading;

namespace System.Windows.Threading
{
    internal class SynchronousDelegateWrapper : DelegateWrapper, IDisposable
    {
        private readonly AutoResetEvent waitHandle;

        public SynchronousDelegateWrapper(Delegate target, object[] args)
            : base(target, args)
        {
            this.waitHandle = new AutoResetEvent(false);
        }

        public void Wait()
        {
            this.waitHandle.WaitOne();
        }

        public sealed override void Invoke()
        {
            base.Invoke();
            this.waitHandle.Set();
        }

        public void Dispose()
        {
            this.waitHandle.Dispose();
        }
    }
}
