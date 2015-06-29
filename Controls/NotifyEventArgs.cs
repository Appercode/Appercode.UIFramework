using System;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Provides data for the WebBrowser control WebBrowser.ScriptNotify event.
    /// </summary>
    public class NotifyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets argument data passed from JavaScript.
        /// </summary>
        public string Value { get; internal set; }
    }
}
