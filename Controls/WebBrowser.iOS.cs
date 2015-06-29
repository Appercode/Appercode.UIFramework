using Appercode.UI.Controls.Navigation;
using Foundation;
using System;
using System.Linq;
using System.Net;
using System.Text;
using UIKit;

namespace Appercode.UI.Controls
{
    public sealed partial class WebBrowser
    {
        #region Constants

        private const string WindowExternalNotifyScriptDefinition = @"if (window && !window.external)
{
    window.external = {
        notify: function(data)
        {
            window.location.href = ""external://notify?data="" + data;
        }
    };
}";

        private const string WindowExternalNotifyScheme = "external";

        private const string WindowExternalNotifyUrlPrefix = "external://notify?data=";

        #endregion //Constants

        #region Fields

        /// <summary>
        /// Holds Uri that currently started to load
        /// </summary>
        private Uri lastLoaded;

        #endregion //Fields

        #region Properties

        /// <summary>
        /// Gets navive control
        /// </summary>
        internal UIWebView NativeWebView
        {
            get { return (UIWebView)this.NativeUIElement; }
        }

        #endregion //Properties

        #region Platform-specific implementations

        /// <summary>
        /// Causes the WebBrowser to navigate back a page in the browsing history.
        /// </summary>
        public void GoBack()
        {
            this.NativeWebView.GoBack();
        }

        /// <summary>
        /// Causes the WebBrowser to navigate forward a page in the browsing history.
        /// </summary>
        public void GoForward()
        {
            this.NativeWebView.GoForward();
        }

        /// <summary>
        /// Executes a scripting function defined in the currently loaded document.
        /// </summary>
        /// <param name="scriptName">The scripting function to execute.</param>
        /// <returns>Returns the value returned by the scripting function.</returns>
        public object InvokeScript(string scriptName)
        {
            return this.NativeWebView.EvaluateJavascript(scriptName);
        }

        /// <summary>
        /// Executes a scripting function defined in the currently loaded document, and passes the function an array of string parameters.
        /// </summary>
        /// <param name="scriptName">The scripting function to execute.</param>
        /// <param name="args">A variable number of strings to pass to the function as parameters.</param>
        /// <returns>The value returned by the scripting function.</returns>
        public object InvokeScript(string scriptName, params string[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}(", scriptName);

            foreach (var arg in args)
            {
                if (arg == null)
                {
                    sb.Append("null");
                }
                else
                {
                    sb.AppendFormat("\"{0}\"", EscapeJsSpecialSymbols(arg));
                }
            }

            sb.Append(")");

            return this.InvokeScript(sb.ToString());
        }

        /// <summary>
        /// Initiates a navigate request to the provided URI.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        public void Navigate(Uri uri)
        {
            this.Source = uri;
            if (this.NativeUIElement != null)
            {
                this.NativeWebView.LoadRequest(new NSUrlRequest(UriToNsUrl(uri)));
            }
        }

        /// <summary>
        /// Initiates a navigate request to the provided URI. This method allows customizable options for including posted form data and HTTP headers.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        /// <param name="postData">The posted form data.</param>
        /// <param name="additionalHeaders">The additional HTTP headers.</param>
        public void Navigate(Uri uri, byte[] postData, string additionalHeaders)
        {
            var request = new NSMutableUrlRequest(UriToNsUrl(uri));
            if (postData != null && postData.Length != 0)
            {
                request.Body = NSData.FromArray(postData);
            }
            if (!string.IsNullOrEmpty(additionalHeaders))
            {
                request.Headers = ParceAdditionalHeaders(additionalHeaders);
            }

            this.NativeWebView.LoadRequest(request);
        }

        /// <summary>
        /// Injects an HTML string into the web browser control for rendering.
        /// </summary>
        /// <param name="html">The HTML to display in the browser.</param>
        public void NavigateToString(string html)
        {
            string mime = "text/html";
            string encoding = "utf-8"; // TODO: Is it right?

            this.NativeWebView.LoadData(
                NSData.FromString(html),
                mime,
                encoding,
                new NSUrl(string.IsNullOrEmpty(this.Base) ? "about:blank" : this.Base));
        }

        /// <summary>
        /// This API is not intended to be used directly from your code.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // TODO: Do we need something else here?
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Returns a string containing the HTML content of the webpage.
        /// </summary>
        /// <returns></returns>
        public string SaveToString()
        {
            var res = this.InvokeScript("document.documentElement.outerHTML") as string;
            return res;
        }

        #endregion //Platform-specific implementations

        #region Overriden methods

        protected internal override void NativeInit()
        {
            if (this.Parent != null)
            {
                if (this.NativeUIElement == null)
                {
                    this.NativeUIElement = new UIWebView();

                    this.NativeWebView.LoadError += this.NativeWebView_LoadError;
                    this.NativeWebView.LoadFinished += this.NativeWebView_LoadFinished;
                    this.NativeWebView.LoadStarted += this.NativeWebView_LoadStarted;
                }

                this.NativeWebView.ShouldStartLoad = this.NativeWebView_ShouldStartLoad;

                if (this.Source != null)
                {
                    this.Navigate(this.Source);
                }

                // LoadError;
                // public event EventHandler LoadFinished;
                // public event EventHandler LoadStarted;

                //// TODO: Init

                base.NativeInit();
            }
        }

        #endregion //Overriden methods

        #region Helpers

        /// <summary>
        /// Creates a NSUrl object from specified Uri object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static NSUrl UriToNsUrl(Uri uri)
        {
            return new NSUrl(uri.OriginalString); // TODO: Check this
        }

        /// <summary>
        /// Parces HTML headers string to a NSDictionary object
        /// </summary>
        /// <param name="additionalHeaders"></param>
        /// <returns></returns>
        private static NSDictionary ParceAdditionalHeaders(string additionalHeaders)
        {
            if (string.IsNullOrEmpty(additionalHeaders))
            {
                return null;
            }

            var pairList = from line in additionalHeaders.Split('\n')
                           where !string.IsNullOrEmpty(line)
                           let colonPos = line.IndexOf(':')
                           where colonPos >= 0
                           let key = line.Remove(colonPos)
                           let value = line.Substring(colonPos + 1)
                           select new { key, value };

            var res = new NSMutableDictionary();

            foreach (var pair in pairList)
            {
                res.SetValueForKey(new NSString(pair.key), new NSString(pair.value));
            }

            return res;
        }

        #endregion //Helpers

        #region Private methods

        /// <summary>
        /// Sets source Uri
        /// </summary>
        partial void SetSourceNative(Uri source)
        {
            if (this.NativeUIElement != null)
            {
                this.Navigate(this.Source);
            }
        }

        /// <summary>
        /// Refreshes CanGoBack and CanGoForward properties
        /// </summary>
        private void RefreshNavigationHistoryProperties()
        {
            this.CanGoBack = this.NativeWebView.CanGoBack;
            this.CanGoForward = this.NativeWebView.CanGoForward;
        }

        private void DefineWindowExternalNotifyScriptFunction()
        {
            this.InvokeScript(WebBrowser.WindowExternalNotifyScriptDefinition);
        }

        #endregion //Private methods

        #region Event handlers

        private void NativeWebView_LoadError(object sender, UIWebErrorArgs uiWebErrorArgs)
        {
            // TODO: Do we need here to call: RefreshNavigationHistoryProperties (); ??
            this.OnNavigationFailed(
                new NavigationFailedEventArgs(
                    this.lastLoaded,
                    uiWebErrorArgs.Error == null
                        ? null
                        : new InvalidOperationException(uiWebErrorArgs.Error.Description)));
        }

        private void NativeWebView_LoadFinished(object sender, EventArgs e)
        {
            this.DefineWindowExternalNotifyScriptFunction();
            this.RefreshNavigationHistoryProperties();

            var e1 = new NavigationEventArgs(this.lastLoaded, null);
            this.OnNavigated(e1);
            this.OnLoadCompleted(e1);
        }

        private void NativeWebView_LoadStarted(object sender, EventArgs e)
        {
        }

        private bool NativeWebView_ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (request.Url.Scheme == WindowExternalNotifyScheme)
            {
                var data = request.Url.AbsoluteString.Substring(WindowExternalNotifyUrlPrefix.Length);

                data = WebUtility.UrlDecode(data);

                this.OnScriptNotify(
                    new NotifyEventArgs()
                    {
                        Value = data
                    });

                return false;
            }

            this.lastLoaded = new Uri(request.Url.AbsoluteString); // TODO: Check this
            var e = new NavigatingEventArgs()
            {
                Uri = this.lastLoaded
            };

            this.OnNavigating(e);

            return !e.Cancel;
        }
        #endregion // Event handlers
    }
}
