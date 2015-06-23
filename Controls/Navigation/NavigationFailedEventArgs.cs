using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls.Navigation
{
    public sealed class NavigationFailedEventArgs : EventArgs
    {
        internal NavigationFailedEventArgs(Uri uri, Exception error)
        {
            this.Uri = uri;
            this.Exception = error;
        }

        internal NavigationFailedEventArgs(Type sourcePageType, Exception error)
        {
            this.SourcePageType = sourcePageType;
            this.Exception = error;
        }

        /// <summary>
        /// Gets or sets related URI
        /// </summary>
        public Uri Uri { get; internal set; }

        public Exception Exception
        {
            get;
            private set;
        }

        public bool Handled
        {
            get;
            set;
        }

        public Type SourcePageType
        {
            get;
            private set;
        }
    }
}
