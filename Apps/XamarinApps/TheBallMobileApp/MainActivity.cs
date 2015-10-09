using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using TheBall.Support.DeviceClient;
using Xamarin;

namespace TheBallMobileApp
{
    [Activity(Label = "The Ball Offline App", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {

        private WebView cWebView;
        private TBJS2OP TBJSBridge;

        protected async override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        protected async override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }

        protected async override void OnResume()
        {
            base.OnResume();
        }

        protected async override void OnPause()
        {
            base.OnPause();
        }

        protected async override void OnCreate(Bundle bundle)
        {
            //Insights.Initialize("", ApplicationContext);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //string connToSync = "members.onlinetaekwondo.net";
            if (cWebView == null)
            {
                cWebView = hookToWebView(FindViewById<WebView>(Resource.Id.webView));
            }
            
        }

        private WebView hookToWebView(WebView webView)
        {
            string bridgeName = "TBJS2MobileBridge";
            string startupUrl = "file:///android_asset/CoreUI/index.html";
            var settings = webView.Settings;
            settings.JavaScriptEnabled = true;
            settings.AllowFileAccessFromFileURLs = true;
            TheBallHostManager hostManager = new TheBallHostManager();
            var webViewClient = new TBWebViewClient(this, hostManager, webView);
            TBJSBridge = new TBJS2OP(this);
            TBJSBridge.RegisterOperation(hostManager.GoToConnectionOperation);
            TBJSBridge.RegisterOperation(hostManager.CreateConnectionOperation);
            TBJSBridge.RegisterOperation(hostManager.DeleteConnectionOperation);
            webView.SetWebViewClient(webViewClient);
            webView.LoadUrl(startupUrl);
            webView.AddJavascriptInterface(TBJSBridge, bridgeName);
            CookieManager.SetAcceptFileSchemeCookies(true);
            CookieManager.Instance.SetCookie("file://", "TheBall_EMAIL=theballdemo@gmail.com");
            return webView;
        }

        private void ReportException(Exception obj)
        {
            if (Insights.IsInitialized)
                Insights.Report(obj);
        }
    }
}

