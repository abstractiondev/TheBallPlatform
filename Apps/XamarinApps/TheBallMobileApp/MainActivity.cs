using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using TheBall.Support.DeviceClient;
using Xamarin;

namespace TheBallMobileApp
{
    [Activity(Label = "The Ball Offline App", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {

        private WebView cWebView;
        private TBJS2OP TBJSBridge;

        protected override void OnCreate(Bundle bundle)
        {
            //Insights.Initialize("", ApplicationContext);
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            if (cWebView == null)
            {
                cWebView = hookToWebView(FindViewById<WebView>(Resource.Id.webView));
            }
            
        }

        private WebView hookToWebView(WebView webView)
        {
            var settings = webView.Settings;
            settings.JavaScriptEnabled = true;
            TBJSBridge = new TBJS2OP(this);
            TBJSBridge.RegisterOperation(TheBallHostManager.RegisterConnectionOperation);
            var webViewClient = new TBWebViewClient(this);
            webView.SetWebViewClient(webViewClient);
            webView.LoadUrl("file:///android_asset/CoreUI/index.html");
            return webView;
        }

        private void ReportException(Exception obj)
        {
            if (Insights.IsInitialized)
                Insights.Report(obj);
        }
    }
}

