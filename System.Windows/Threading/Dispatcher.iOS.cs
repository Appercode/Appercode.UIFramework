using Foundation;

namespace System.Windows.Threading
{
    public partial class Dispatcher
    {
        private static readonly NSObject invoker = new NSObject();

        partial void NativeBeginInvoke(Action a)
        {
            invoker.BeginInvokeOnMainThread(a);
        }

        partial void NativeInvoke(ref object result, Delegate d, params object[] args)
        {
            var wrapper = new DelegateWrapper(d, args);
            invoker.InvokeOnMainThread(wrapper.Invoke);
            result = wrapper.Result;
        }
    }
}
