using System;

namespace Appercode.UI.Internals
{
    internal class ResourcesChangedEventArgs : EventArgs
    {
        private ResourcesChangeInfo info;

        internal ResourcesChangedEventArgs(ResourcesChangeInfo info)
        {
            this.info = info;
        }

        internal ResourcesChangeInfo Info
        {
            get
            {
                return this.info;
            }
        }
    }
}
