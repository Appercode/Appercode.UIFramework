using System;

namespace System.Windows
{
    internal class ReadOnlyFrameworkPropertyMetadata : PropertyMetadata
    {
        private GetReadOnlyValueCallback getValueCallback;

        public ReadOnlyFrameworkPropertyMetadata(object defaultValue, GetReadOnlyValueCallback getValueCallback)
            : base(defaultValue)
        {
            this.getValueCallback = getValueCallback;
        }

        internal override GetReadOnlyValueCallback GetReadOnlyValueCallback
        {
            get { return this.getValueCallback; }
        }
    }
}