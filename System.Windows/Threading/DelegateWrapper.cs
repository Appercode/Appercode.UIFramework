namespace System.Windows.Threading
{
    internal class DelegateWrapper
    {
        private readonly Delegate target;
        private readonly object[] args;
        private object result;

        public DelegateWrapper(Delegate target, object[] args)
        {
            this.target = target;
            this.args = args;
        }

        public object Result => this.result;

        public virtual void Invoke()
        {
            this.result = this.target.DynamicInvoke(this.args);
        }
    }
}
