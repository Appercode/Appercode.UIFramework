using System;

namespace Appercode.UI.Data
{
    public class FilterEventArgs : EventArgs
    {
        private object item;

        private bool accepted;

        internal FilterEventArgs(object item)
        {
            this.item = item;
            this.accepted = true;
        }

        public bool Accepted
        {
            get
            {
                return this.accepted;
            }
            set
            {
                this.accepted = value;
            }
        }

        public object Item
        {
            get
            {
                return this.item;
            }
        }
    }
}