using Appercode.UI.Controls.Navigation;
using System;
using System.ComponentModel;
using System.Windows;

namespace Appercode.UI.Controls
{
    /// <summary>
    /// Represents the method that will handle the WebBrowser.LoadCompleted event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void LoadCompletedEventHandler(object sender, NavigationEventArgs e);

    /// <summary>
    /// Allows HTML rendering and navigation functionality to be embedded in an application.
    /// </summary>
    public sealed partial class WebBrowser : WebBrowserBase, ISupportInitialize
    {
        #region Dependency properties definitions

        /// <summary>
        /// Identifies the Base dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseProperty =
                    DependencyProperty.Register(
                        "Base",
                        typeof(string),
                        typeof(WebBrowser),
                        new PropertyMetadata("", (d, e) => ((WebBrowser)d).SetBaseNative((string)e.NewValue)));

        /// <summary>
        /// Identifies the CanGoBack dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty =
                    DependencyProperty.Register(
                        "CanGoBack",
                        typeof(bool),
                        typeof(WebBrowser),
                        new PropertyMetadata(false));

        /// <summary>
        /// Identifies the CanGoForward dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty =
                    DependencyProperty.Register(
                        "CanGoForward",
                        typeof(bool),
                        typeof(WebBrowser),
                        new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsGeolocationEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGeolocationEnabledProperty =
                    DependencyProperty.Register(
                        "IsGeolocationEnabled",
                        typeof(bool),
                        typeof(WebBrowser),
                        new PropertyMetadata(true, (d, e) => ((WebBrowser)d).SetIsGeolocationEnabledNative((bool)e.NewValue)));

        /// <summary>
        /// Identifies the IsScriptEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsScriptEnabledProperty =
                    DependencyProperty.Register(
                        "IsScriptEnabled",
                        typeof(bool),
                        typeof(WebBrowser),
                        new PropertyMetadata(false, (d, e) => ((WebBrowser)d).SetIsScriptEnabledNative((bool)e.NewValue)));

        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
                    DependencyProperty.Register(
                        "Source",
                        typeof(Uri),
                        typeof(WebBrowser),
                        new PropertyMetadata(null, (d, e) => ((WebBrowser)d).SetSourceNative((Uri)e.NewValue)));

        #endregion //Dependency properties definitions

        #region Constructors

        /// <summary>
        /// Initializes the control
        /// </summary>
        public WebBrowser()
        {
        }

        #endregion //Constructors

        #region Events

        /// <summary>
        /// Occurs after the WebBrowser control has loaded content.
        /// </summary>
        public event LoadCompletedEventHandler LoadCompleted;

        /// <summary>
        /// Occurs after the WebBrowser control successfully navigates.
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigated;

        /// <summary>
        /// Occurs when the WebBrowser control is navigating, including from a redirect.
        /// </summary>
        public event EventHandler<NavigatingEventArgs> Navigating;

        /// <summary>
        /// Occurs after the WebBrowser control fails to navigate.
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed;

        /// <summary>
        /// Occurs when JavaScript calls the window.external.notify(&lt;data&gt;) method.
        /// </summary>
        public event EventHandler<NotifyEventArgs> ScriptNotify;

        #endregion //Events

        #region Properties

        /// <summary>
        /// Sets the base directory within isolated storage that is used to resolve relative
        /// references within the WebBrowser control.
        /// </summary>
        /// <returns>
        /// Returns System.String.
        /// </returns>
        [Category("Common Properties")]
        public string Base
        {
            get
            {
                return (string)this.GetValue(WebBrowser.BaseProperty);
            }
            set
            {
                this.SetValue(WebBrowser.BaseProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the WebBrowser
        /// can navigate back a page in the browsing history.
        /// </summary>
        /// <returns>
        /// true if the WebBrowser can navigate back; otherwise,
        /// false. The default is true.
        /// </returns>
        [Category("Common Properties")]
        public bool CanGoBack
        {
            get
            {
                return (bool)this.GetValue(WebBrowser.CanGoBackProperty);
            }
            internal set
            {
                this.SetValue(WebBrowser.CanGoBackProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the WebBrowser
        /// can navigate forward a page in the browsing history.
        /// </summary>
        /// <returns>
        /// true if the WebBrowser can navigate forward; otherwise,
        /// false. The default is true.
        /// </returns>
        [Category("Common Properties")]
        public bool CanGoForward
        {
            get
            {
                return (bool)this.GetValue(WebBrowser.CanGoForwardProperty);
            }
            internal set
            {
                this.SetValue(WebBrowser.CanGoForwardProperty, value);
            }
        }

        /// <summary>
        /// Determines whether a website that is hosted in the WebBrowser
        /// control can use location services on the device.
        /// </summary>
        /// <returns>
        /// Returns System.Boolean. true if the website that is hosted in the WebBrowser
        /// control can use location services on the device; otherwise, false. The default
        /// is false.
        /// </returns>
        [Category("Common Properties")]
        public bool IsGeolocationEnabled
        {
            get
            {
                return (bool)this.GetValue(WebBrowser.IsGeolocationEnabledProperty);
            }
            set
            {
                this.SetValue(WebBrowser.IsGeolocationEnabledProperty, value);
            }
        }

        /// <summary>
        /// Enables or disables scripting. This applies to the next document that is
        /// navigated to, not the current document. This property is false by default.
        /// Set this property to true to enable scripting, or false to disable scripting.
        /// </summary>
        /// <returns>
        /// Returns System.Boolean. true if scripting is enabled; otherwise, false.
        /// </returns>
        [Category("Common Properties")]
        public bool IsScriptEnabled
        {
            get
            {
                return (bool)this.GetValue(WebBrowser.IsScriptEnabledProperty);
            }
            set
            {
                this.SetValue(WebBrowser.IsScriptEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the URI location of the WebBrowser
        /// control.
        /// </summary>
        /// <returns>
        /// The URI source of the HTML content to display in the WebBrowser
        /// control.
        /// </returns>
        [Category("Common Properties")]
        public Uri Source
        {
            get
            {
                return (Uri)this.GetValue(WebBrowser.SourceProperty);
            }
            set
            {
                this.SetValue(WebBrowser.SourceProperty, value);
            }
        }

        #endregion //Properties

        #region Public methods

        #endregion //Public methods

        #region ISupportInitialize interface implementation

        /// <summary>
        /// Implements ISupportInitialize.BeginInit() method
        /// Does nothing for now
        /// </summary>
        public void BeginInit()
        {
            // TODO: Do we nned to do here?
        }

        /// <summary>
        /// Implements ISupportInitialize.EndInit() method
        /// Does nothing for now
        /// </summary>
        public void EndInit()
        {
            // TODO: Do we nned to do here?
        }

        #endregion // ISupportInitialize interface implementation

        #region Overriden methods

        #endregion //Overriden methods

        #region Internal methods

        /// <summary>
        /// Rises ScriptNotify event
        /// </summary>
        /// <param name="e">Events arguments object to use</param>
        internal void OnScriptNotify(NotifyEventArgs e)
        {
            if (this.ScriptNotify != null)
            {
                this.ScriptNotify(this, e);
            }
        }

        /// <summary>
        /// Rises LoadCompleted event
        /// </summary>
        /// <param name="e">Events arguments object to use</param>
        internal void OnLoadCompleted(NavigationEventArgs e)
        {
            if (this.LoadCompleted != null)
            {
                // As I understand in WP8
                //    NavigationEventArgs.NavigationMode === New
                //    NavigationEventArgs.Content === null
                //    NavigationEventArgs.IsNavigationInitiator === true
                // for eny LoadCompleted calls
                this.LoadCompleted(this, e);
            }
        }

        /// <summary>
        /// Rises Navigated event
        /// </summary>
        /// <param name="e">Events arguments object to use</param>
        internal void OnNavigated(NavigationEventArgs e)
        {
            if (this.Navigated != null)
            {
                // As I understand in WP8
                //    NavigationEventArgs.NavigationMode === New
                //    NavigationEventArgs.Content === null
                //    NavigationEventArgs.IsNavigationInitiator === true
                // for eny LoadCompleted calls
                this.Navigated(this, e);
            }
        }

        /// <summary>
        /// Rises Navigating event
        /// </summary>
        /// <param name="e">Events arguments object to use</param>
        internal void OnNavigating(NavigatingEventArgs e)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, e);
            }
        }

        /// <summary>
        /// Rises NavigationFailed event
        /// </summary>
        /// <param name="e">Events arguments object to use</param>
        internal void OnNavigationFailed(NavigationFailedEventArgs e)
        {
            if (this.NavigationFailed != null)
            {
                this.NavigationFailed(this, e);
            }
        }

        #endregion //Internal methods

        #region Private methods

        /// <summary>
        /// Escapes string special characters to apply for JavaScript invocation
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string EscapeJsSpecialSymbols(string str)
        {
            return str
                .Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\v", "\\v")
                .Replace("\"", "\\\"");
        }

        #region Platform-specific partial methods definitions

        /// <summary>
        /// When implemented on target platform, sets base state
        /// </summary>
        /// <param name="base"></param>
        partial void SetBaseNative(string @base);

        /// <summary>
        /// When implemented on target platform, sets script enabled/disabled state
        /// </summary>
        /// <param name="isEnabled"></param>
        partial void SetIsScriptEnabledNative(bool isEnabled);

        /// <summary>
        /// When implemented on target platform, sets geolocation enabled/disabled state
        /// </summary>
        /// <param name="isEnabled"></param>
        partial void SetIsGeolocationEnabledNative(bool isEnabled);

        /// <summary>
        /// When implemented on target platform, sets source Uri
        /// </summary>
        partial void SetSourceNative(Uri source);

        #endregion //Platform-specific partial methods definitions

        #endregion //Private methods
    }
}
