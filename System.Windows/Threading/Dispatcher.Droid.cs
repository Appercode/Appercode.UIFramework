using Android.OS;

namespace System.Windows.Threading
{
    public sealed partial class Dispatcher
    {
        private static readonly Lazy<Handler> Handler = new Lazy<Handler>(CreateHandler);

        private static Handler CreateHandler()
        {
            return new Handler(Looper.MainLooper);
        }

        partial void NativeBeginInvoke(Action a)
        {
            Handler.Value.Post(a);
        }

        partial void NativeInvoke(ref object result, Delegate d, params object[] args)
        {
            using (var wrapper = new SynchronousDelegateWrapper(d, args))
            {
                Handler.Value.Post(wrapper.Invoke);
                wrapper.Wait();
                result = wrapper.Result;
            }
        }
    }
}