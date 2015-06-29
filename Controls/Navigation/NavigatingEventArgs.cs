#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#endregion //Using directives

namespace Appercode.UI.Controls.Navigation
{
    /// <summary>
    /// Represents an event arguments object for navigating events
    /// </summary>
    public class NavigatingEventArgs : CancelEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes the event arguments object
        /// </summary>
        public NavigatingEventArgs()
        {
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets related URI
        /// </summary>
        public Uri Uri { get; internal set; }

        #endregion // Properties
    }
}