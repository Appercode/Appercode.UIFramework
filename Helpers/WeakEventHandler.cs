using System;
using System.Diagnostics;
using System.Reflection;

namespace Appercode.Helpers
{

    /// <summary>
    /// Event handler week refeenced to target
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    [DebuggerNonUserCode]
    public sealed class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly WeakReference targetReference;
        private readonly MethodInfo method;


        /// <summary>
        /// Conctructor for WeakEventHandler class
        /// </summary>
        /// <param name="callback">event callback</param>
        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            this.method = callback.Method;
            this.targetReference = new WeakReference(callback.Target);
        }

        /// <summary>
        /// Hendler, if target not null invaces callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerNonUserCode]
        public void Handler(object sender, TEventArgs e)
        {
            var target = this.targetReference.Target;
            if (target != null)
            {
                var callback = (Action<object, TEventArgs>)Delegate.CreateDelegate(typeof(Action<object, TEventArgs>), target, this.method, true);
                if (callback != null)
                {
                    callback(sender, e);
                }
            }
        }
    }
}