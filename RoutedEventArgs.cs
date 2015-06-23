using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI
{
    /// <summary>
    /// Represents the method that will handle various routed events that do not have specific event data beyond the data that is common for all routed events
    /// </summary>
    /// <param name="sender">The object where the event handler is attached</param>
    /// <param name="e">The event data</param>
    public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);

    /// <summary>
    /// Contains state information and event data associated with a routed event
    /// </summary>
    public class RoutedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the RoutedEventArgs class
        /// </summary>
        public RoutedEventArgs()
        {
        }

        /// <summary>
        /// Gets the original reporting source as determined by pure hit testing, before any possible Source adjustment by a parent class
        /// </summary>
        public object OriginalSource
        {
            get;
            set;
        }
    }
}