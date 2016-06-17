#region Using directives
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Appercode.UI.Controls.NativeControl.Wrapers;
using Appercode.UI.Controls.Navigation;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
#endregion //Using directives

namespace Appercode.UI.Controls
{   
    public sealed partial class WebBrowser
    {
        /// <summary>
        /// Overrides NativeInit() method
        /// Does the initialization of native UI element
        /// </summary>
        protected internal override void NativeInit()
        {
            if (this.Parent != null && this.Context != null)
            {
                if (this.NativeUIElement == null)
                {
                    var webView = new WrappedWebView(this.Context);
                    this.WebViewClient = new WrappingWebViewClient(this);
                    webView.SetWebViewClient(this.WebViewClient);
                    webView.SetWebChromeClient(new MyWebChromeClient());

                    webView.AddJavascriptInterface(new ExternalJsInterface(this), "external");

                    webView.Download += WebView_Download;
                    webView.Picture += WebView_Picture;

                    this.NativeUIElement = webView;
                    if (this.Source != null)
                    {
                        this.Navigate(this.Source); //TODO: Check this
                    }
                }

                SetIsScriptEnabledNative(this.IsScriptEnabled);
                SetIsGeolocationEnabledNative(this.IsGeolocationEnabled);

                base.NativeInit();
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the native WebView control
        /// </summary>
        private WrappedWebView NativeWebView
        {
            get { return (WrappedWebView)this.NativeUIElement; }
        }

        /// <summary>
        /// Gets native Android WebViewClient listener object
        /// </summary>
        private WrappingWebViewClient WebViewClient { get; set; }

        #endregion //Properties

        #region Platform-specific implementations

        /// <summary>
        /// Sets script enabled/disabled state
        /// </summary>
        /// <param name="isEnabled"></param>
        partial void SetIsScriptEnabledNative(bool isEnabled)
        {
            if (this.NativeWebView != null && this.NativeWebView.Settings != null)
            {
                this.NativeWebView.Settings.JavaScriptEnabled = isEnabled;
            }
        }

        /// <summary>
        /// Sets geolocation enabled/disabled state
        /// </summary>
        /// <param name="isEnabled"></param>
        partial void SetIsGeolocationEnabledNative(bool isEnabled)
        {
            if (this.NativeWebView != null && this.NativeWebView.Settings != null)
            {
                this.NativeWebView.Settings.SetGeolocationEnabled(isEnabled);
            }
        }

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
        /// Causes the WebBrowser to navigate back a page in browsing history.
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
        /// <remarks>
        /// Script function will be executed even if IsScriptEnabled == false
        /// </remarks>
        /// </summary>
        /// <param name="scriptName">The scripting function to execute.</param>
        /// <returns>Returns the value returned by the scripting function.</returns>
        public object InvokeScript(string scriptName)
        {
            var jsEnabled = this.NativeWebView.Settings.JavaScriptEnabled;
            if (!this.NativeWebView.Settings.JavaScriptEnabled)
            {
                this.NativeWebView.Settings.JavaScriptEnabled = true; //TODO: Do we need this? Check how WP8 contol works in this case
            }

            try
            {
                //To invoke JS synchronously, we need to call this.NativeWebView.mWebViewCore.mBrowserFrame.stringByEvaluatingJavaScriptFromString(scriptName);

                IntPtr webViewClass = JNIEnv.FindClass("android/webkit/WebView");
                IntPtr mWebViewCoreField = JNIEnv.GetFieldID(webViewClass, "mWebViewCore", "Landroid/webkit/WebViewCore;");
                IntPtr webViewCoreInstance = JNIEnv.GetObjectField(this.NativeWebView.Handle, mWebViewCoreField);

                IntPtr webViewCoreClass = JNIEnv.FindClass("android/webkit/WebViewCore");
                IntPtr mBrowserFrameField = JNIEnv.GetFieldID(webViewCoreClass, "mBrowserFrame", "Landroid/webkit/BrowserFrame;");
                IntPtr browserFrameInstance = JNIEnv.GetObjectField(webViewCoreInstance, mBrowserFrameField);

                IntPtr browserFrameClass = JNIEnv.FindClass("android/webkit/BrowserFrame");
                IntPtr stringByEvaluatingJavaScriptFromStringMethod = JNIEnv.GetMethodID(browserFrameClass, "stringByEvaluatingJavaScriptFromString", "(Ljava/lang/String;)Ljava/lang/String;");
                IntPtr resRef = JNIEnv.CallObjectMethod(browserFrameInstance, stringByEvaluatingJavaScriptFromStringMethod, new JValue(new Java.Lang.String(scriptName)));

                var res = JNIEnv.GetString(resRef, JniHandleOwnership.TransferLocalRef);

                return res;
            }
            finally
            {
                if (!this.NativeWebView.Settings.JavaScriptEnabled != jsEnabled)
                {
                    this.NativeWebView.Settings.JavaScriptEnabled = jsEnabled;
                }
            }
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

            return InvokeScript(sb.ToString());
        }

        /// <summary>
        /// Initiates a navigate request to the provided URI.
        /// </summary>
        /// <param name="uri">The URI to navigate to.</param>
        public void Navigate(Uri uri)
        {
            //this.Source = uri; // TODO: Do we need this?
                                 // If yes, preven recursive call of Navigate(Uri) method
            if (this.NativeUIElement != null)
            {
                this.NativeWebView.LoadUrl(UriToUrlString(uri));
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
            string url = UriToUrlString(uri);

            if (!string.IsNullOrEmpty(additionalHeaders))
            {
                if (postData != null && postData.Length != 0)
                {
                    this.WebViewClient.EnqueuePostData(url, postData, ParceAdditionalHeaders(additionalHeaders));
                    this.NativeWebView.LoadUrl(url);
                }
                else
                {
                    this.NativeWebView.LoadUrl(url, ParceAdditionalHeaders(additionalHeaders));
                }
            }
            else
            {
                if (postData != null && postData.Length != 0)
                {
                    this.NativeWebView.PostUrl(url, postData);
                }
                else
                {
                    this.NativeWebView.LoadUrl(url);
                }
            }
        }

        /// <summary>
        /// Injects an HTML string into the web browser control for rendering.
        /// </summary>
        /// <param name="html">The HTML to display in the browser.</param>
        public void NavigateToString(string html)
        {
            String mime = "text/html";
            String encoding = "utf-8"; //TODO: Check this

            this.NativeWebView.LoadDataWithBaseURL(
                this.Base,
                html,
                mime,
                encoding,
                null);
        }

        //
        // Summary:
        //     This API is not intended to be used directly from your code.
        public override void OnApplyTemplate()
        {
            //TODO: Do we need something else here?
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Returns a string containing the HTML content of the webpage.
        /// </summary>
        /// <returns></returns>
        public string SaveToString()
        {
            var res = InvokeScript("document.documentElement.outerHTML") as string;
            return res;
        }

        #endregion //Platform-specific implementations

        #region Private methods

        /// <summary>
        /// Translates specified Uri object to URL string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private string UriToUrlString(Uri uri)
        {
            var res = uri.ToString();
            return res;
        }

        /// <summary>
        /// Parces HTML headers string to key-value dictionary
        /// </summary>
        /// <param name="additionalHeaders"></param>
        /// <returns></returns>
        private static IDictionary<string, string> ParceAdditionalHeaders(string additionalHeaders)
        {
            if (string.IsNullOrEmpty(additionalHeaders)) return null;

            var pairList = from line in additionalHeaders.Split('\n')
                           where !string.IsNullOrEmpty(line)
                           let colonPos = line.IndexOf(':')
                           where colonPos >= 0
                           let key = line.Remove(colonPos)
                           let value = line.Substring(colonPos + 1)
                           select new {key, value};

            return pairList.ToDictionary(x => x.key, x => x.value);
        }

        /// <summary>
        /// Refreshes CanGoBack and CanGoForward properties
        /// </summary>
        private void RefreshNavigationHistoryProperties()
        {
            this.CanGoBack = this.NativeWebView.CanGoBack();
            this.CanGoForward = this.NativeWebView.CanGoForward();
        }

        #endregion //Private methods

        #region Events

        private void WebView_Picture(object sender, WebView.PictureEventArgs pictureEventArgs)
        {
            //TODO: Do we need to handle this somehow?
        }

        private void WebView_Download(object sender, DownloadEventArgs downloadEventArgs)
        {
            //TODO: Do we need to handle this somehow?
        }

        #endregion //Events

        #region Nested types

        /// <summary>
        /// Represents an implementation for callback interface used by the Android WebView control
        /// </summary>
        private class MyWebChromeClient : WebChromeClient
        {
            /// <summary>
            /// Called when page adds something to console output, e.g. when a JavaScript error occured
            /// </summary>
            /// <param name="consoleMessage"></param>
            /// <returns></returns>
            public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
            {
                var ln = consoleMessage.LineNumber();
                var msg = consoleMessage.Message();
                var sid = consoleMessage.SourceId();

                //NOTE: We can rise NavigationFailed event here with JS error, but Microsoft control doesn't work that way

                return base.OnConsoleMessage(consoleMessage);
            }
        }

        /// <summary>
        /// Represents a container for arguments specified in WebBrowser.Navigate(Uri, byte[], string) method
        /// When all these arguments are not ampty
        /// <remarks>
        /// Original Android WebView control doesn't have method for Posting data with custom http headers, so
        /// we need to manually perform this behavior
        /// </remarks>
        /// </summary>
        private class NavigationData
        {
            /// <summary>
            /// Initializes the data object
            /// </summary>
            /// <param name="url">Target url string</param>
            /// <param name="postData">The posted form data.</param>
            /// <param name="additionalHeaders">The additional HTTP headers.</param>
            public NavigationData(string url, byte[] postData, IDictionary<string, string> additionalHeaders)
            {
                this.Url = url;
                this.PostData = postData;
                this.AdditionalHeaders = additionalHeaders;
            }

            /*
            public NavigationData(string url, bool cancelLoading)
            {
                this.Url = url;
                this.Canceled = cancelLoading;
            }

            public bool Canceled { get; set; }
            */

            /// <summary>
            /// Gets the target url string
            /// </summary>
            public string Url { get; private set; }
            
            /// <summary>
            /// Gets the posted form data.
            /// </summary>
            public byte[] PostData { get; private set; }

            /// <summary>
            /// Gets the additional HTTP headers.
            /// </summary>
            public IDictionary<string, string> AdditionalHeaders { get; private set; }
        }

        /// <summary>
        /// Represents an implementation of WebViewClient callback interface
        /// </summary>
        private class WrappingWebViewClient : WebViewClient
        {
            #region Fields

            /// <summary>
            /// Holds parent WebBrowser control
            /// </summary>
            private WebBrowser _Parent;

            /// <summary>
            /// Holds enqueued navigation arguments
            /// </summary>
            private IList<NavigationData> _EnqueuedNavigationDataList = new List<NavigationData>();

            #endregion //Fields

            #region Constructors

            /// <summary>
            /// Initializes the callbacks listener
            /// </summary>
            /// <param name="parent"></param>
            public WrappingWebViewClient(WebBrowser parent)
            {
                _Parent = parent;
            }

            #endregion //Constructors

            #region Public methods

            /// <summary>
            /// Enqueues args for next post request with additional http header
            /// </summary>
            /// <param name="url"></param>
            /// <param name="postData"></param>
            /// <param name="additionalHeaders"></param>
            public void EnqueuePostData(string url, byte[] postData, IDictionary<string, string> additionalHeaders)
            {
                lock (_EnqueuedNavigationDataList)
                {
                    _EnqueuedNavigationDataList.Add(new NavigationData(url, postData, additionalHeaders));
                }
            }

            /// <summary>
            /// Enqueues specified navigation data
            /// </summary>
            private void EnqueueNavigationData(NavigationData navigationData)
            {
                lock (_EnqueuedNavigationDataList)
                {
                    _EnqueuedNavigationDataList.Add(navigationData);
                }
            }

            /// <summary>
            /// Tries to get a post request args object for specified url
            /// If related args object not found will return null
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            private NavigationData GetNavigationData(string url)
            {
                lock (_EnqueuedNavigationDataList)
                {
                    var res = _EnqueuedNavigationDataList.FirstOrDefault(x => x.Url == url);
                    _EnqueuedNavigationDataList.Remove(res);
                    return res;
                }
            }

            #endregion //Public methods

            #region Overriden methods

            public override void DoUpdateVisitedHistory(WebView view, string url, bool isReload)
            {
                base.DoUpdateVisitedHistory(view, url, isReload);

                _Parent.RefreshNavigationHistoryProperties();
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                var thid = System.Threading.Thread.CurrentThread.ManagedThreadId;

                var nea = new NavigatingEventArgs();
                nea.Uri = new Uri(url);
                _Parent.OnNavigating(nea);

                if (nea.Cancel)
                {
                    _Parent.NativeWebView.StopLoading();
                    //EnqueueNavigationData(new NavigationData(url, true));
                }
                else
                {
                    base.OnPageStarted(view, url, favicon);
                }
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);

                //NOTE: Original Mictosoft WebBrowser control sets only Uri property
                // other are always set to default:
                //  NavigationMode === New;
                //  IsNavigationInitiator === true;
                var e = new NavigationEventArgs(new Uri(url), null);

                //TODO: Actually in this case event rising sequence will be different from original Microsoft control, so we need to make it closer to original control behavior
                _Parent.OnNavigated(e);
                _Parent.OnLoadCompleted(e);
            }

            public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
            {
                base.OnReceivedError(view, errorCode, description, failingUrl);

                var e = new NavigationFailedEventArgs(
                    new Uri(failingUrl),
                    new InvalidOperationException(description) // Do we need to set this exception? WP8 WebView always set it to null
                    );
                _Parent.OnNavigationFailed(e);

                if (e.Handled)
                {
                    //TODO: Don't render error page
                }
            }

            /*
            public override void OnFormResubmission(WebView view, Android.OS.Message dontResend, Android.OS.Message resend)
            {
                base.OnFormResubmission(view, dontResend, resend);
            }

            public override void OnLoadResource(WebView view, string url)
            {
                base.OnLoadResource(view, url);
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }

            public override void OnReceivedHttpAuthRequest(WebView view, HttpAuthHandler handler, string host, string realm)
            {
                base.OnReceivedHttpAuthRequest(view, handler, host, realm);
            }

            public override void OnReceivedLoginRequest(WebView view, string realm, string account, string args)
            {
                base.OnReceivedLoginRequest(view, realm, account, args);
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, Android.Net.Http.SslError error)
            {
                base.OnReceivedSslError(view, handler, error);
            }

            public override void OnScaleChanged(WebView view, float oldScale, float newScale)
            {
                base.OnScaleChanged(view, oldScale, newScale);
            }

            public override void OnTooManyRedirects(WebView view, Android.OS.Message cancelMsg, Android.OS.Message continueMsg)
            {
                base.OnTooManyRedirects(view, cancelMsg, continueMsg);
            }

            public override void OnUnhandledKeyEvent(WebView view, KeyEvent e)
            {
                base.OnUnhandledKeyEvent(view, e);
            }
            
            public override bool ShouldOverrideKeyEvent(WebView view, KeyEvent e)
            {
                return base.ShouldOverrideKeyEvent(view, e);
            }

            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                return base.ShouldOverrideUrlLoading(view, url);
            }
            */

            private WebResourceResponse GetCancelWebResourceResponse()
            {
                return new WebResourceResponse("", "", new MemoryStream());
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                var url = request.Url.ToString();
                var postData = this.GetNavigationData(url);
                if (postData?.PostData != null)
                {
                    try
                    {
                        var webRequest = WebRequest.Create(url);
                        // TODO: What should we set here?
                        // request.ContentType = "text/plain"; 
                        webRequest.Method = "POST";
                        if (postData.AdditionalHeaders != null)
                        {
                            foreach (var additionalHeader in postData.AdditionalHeaders)
                            {
                                webRequest.Headers.Add(additionalHeader.Key, additionalHeader.Value);
                            }
                        }

                        using (var reqestStream = webRequest.GetRequestStream())
                        {
                            reqestStream.Write(postData.PostData, 0, postData.PostData.Length);
                            reqestStream.Flush();
                        }

                        // TODO: Don't we need using directive here?
                        var response = webRequest.GetResponse() as HttpWebResponse;

                        // TODO: do we need to rise NavigationFailed event if response.StatusCode != HttpStatusCode.OK?
                        return new WebResourceResponse(response.ContentType, response.ContentEncoding, response.GetResponseStream());
                    }
                    catch (InvalidOperationException ioex)
                    {
                        // TODO: Do we need to set this exception? WP8 WebView always set it to null
                        var args = new NavigationFailedEventArgs(new Uri(url), ioex);
                        _Parent.OnNavigationFailed(args);
                        if (args.Handled)
                        {
                            return GetCancelWebResourceResponse();
                        }
                    }
                }

                return null;
            }

            #endregion //Overriden methods
        }

        /// <summary>
        /// Represents a custom JavaScript interface
        /// <remarks>
        /// We need this to support WebBrowser.ScriptNotify event
        /// </remarks>
        /// </summary>
        public class ExternalJsInterface : Java.Lang.Object
        {
            private WebBrowser _WebBrowser;

            public ExternalJsInterface(WebBrowser webBrowser)
            {
                _WebBrowser = webBrowser;
            }

            [Export("notify")]
            public void Notify(string input)
            {
                _WebBrowser.OnScriptNotify(new NotifyEventArgs()
                                               {
                                                   Value = input
                                               });
            }
        }

        /// <summary>
        /// Wraps original Android WebView control
        /// </summary>
        public class WrappedWebView : WebView, ITapableView, IJavaFinalizable
        {
            #region Fields

            /// <summary>
            /// Holds tap detector object instance
            /// </summary>
            private TapDetector _TapDetector;

            #endregion //Fields

            #region Constructors

            /// <summary>
            /// Initializes the wrapped web view
            /// </summary>
            /// <param name="javaReference"></param>
            /// <param name="transfer"></param>
            protected WrappedWebView(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
                _TapDetector = new TapDetector(this);
            }

            /// <summary>
            /// Initializes the wrapped web view
            /// </summary>
            /// <param name="context"></param>
            public WrappedWebView(Context context)
                : base(context)
            {
                _TapDetector = new TapDetector(this);
            }

            /// <summary>
            /// Initializes the wrapped web view
            /// </summary>
            /// <param name="context"></param>
            /// <param name="attrs"></param>
            public WrappedWebView(Context context, IAttributeSet attrs)
                : base(context, attrs)
            {
                _TapDetector = new TapDetector(this);
            }

            /// <summary>
            /// Initializes the wrapped web view
            /// </summary>
            /// <param name="context"></param>
            /// <param name="attrs"></param>
            /// <param name="defStyle"></param>
            public WrappedWebView(Context context, IAttributeSet attrs, int defStyle)
                : base(context, attrs, defStyle)
            {
                _TapDetector = new TapDetector(this);
            }

            /// <summary>
            /// Initializes the wrapped web view
            /// </summary>
            /// <param name="context"></param>
            /// <param name="attrs"></param>
            /// <param name="defStyle"></param>
            /// <param name="privateBrowsing"></param>
            public WrappedWebView(Context context, IAttributeSet attrs, int defStyle, bool privateBrowsing)
                : base(context, attrs, defStyle, privateBrowsing)
            {
                _TapDetector = new TapDetector(this);
            }

            #endregion //Constructors

            #region Interfaces implementations

            public override bool OnTouchEvent(MotionEvent e)
            {
                _TapDetector.Detect(e);
                return base.OnTouchEvent(e);
            }

            public event EventHandler NativeTap;
            public event EventHandler JavaFinalized;

            public void WrapedNativeRaiseTap()
            {
                if (this.NativeTap != null)
                {
                    this.NativeTap(this, null);
                }
            }

            protected override void JavaFinalize()
            {
                if (this.JavaFinalized != null)
                {
                    this.JavaFinalized(null, null);
                }
                base.JavaFinalize();
            }

            #endregion //Interfaces implementations
        }

        #endregion //Nested types
    }
}
