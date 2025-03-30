using System;

#if WINUI
using Microsoft.UI.Xaml.Controls;
#elif WPF
using Microsoft.Web.WebView2.Wpf;
#endif

namespace HybridWebView
{
    /// <summary>
    /// Allows configuring the underlying web view after it has been initialized.
    /// </summary>
    internal class HybridWebViewInitializedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="WebView2Control"/> instance that was initialized.
        /// </summary>
        public WebView2 WebView { get; internal set; }
    }
}