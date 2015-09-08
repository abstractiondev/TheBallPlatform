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
            var webViewClient = new TBWebViewClient(this, customDataRetriever);
            webView.SetWebViewClient(webViewClient);
            webView.LoadUrl("file:///android_asset/CoreUI/index.html");
            webView.AddJavascriptInterface(TBJSBridge, "TBJS2MobileBridge");
            return webView;
        }

        private Tuple<string, string, Stream> customDataRetriever(string datakey)
        {
            switch (datakey)
            {
                case "ConnectionHosts.json":
                    byte[] data;
                    using (var stream = new MemoryStream())
                    {
                        JSONSupport.SerializeToJSONStream(new
                        {
                            email = "test.email@from.json",
                            hosts = new[]
                            {
                                new {displayName = "test.theball.me", value = "test.theball.me"},
                                new {displayName = "localhost", value = "localhost"},
                            }
                        }, stream);
                        data = stream.ToArray();
                    }
                    var dataStream = new MemoryStream(data);
                    return new Tuple<string, string, Stream>("application/json", "utf-8", dataStream);
            }
            return null;
        }

        private void ReportException(Exception obj)
        {
            if (Insights.IsInitialized)
                Insights.Report(obj);
        }
    }
}

