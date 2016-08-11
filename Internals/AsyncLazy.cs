using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Appercode.UI.Internals
{
    internal class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory)
            : base(taskFactory) { }

        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
    }
}