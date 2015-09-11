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
            TheBallHostManager.SetDeviceClientHooks();

            if (cWebView == null)
            {
                string connToSync = "members.onlinetaekwondo.net";
                //string connToSync = "beta.diosphere.org";
                Connection primaryConnection = null;
                ClientExecute.ExecuteWithSettings(settings =>
                {
                    primaryConnection = settings.Connections.FirstOrDefault(conn => conn.HostName == connToSync);
                }, ReportException);

                string startupUrl = null;
                string connectionRoot = null;
                if (primaryConnection != null)
                {
                    connectionRoot = getConnectionRootFolder(primaryConnection.HostName);
                    startupUrl = getStartupUrlFromRoot(connectionRoot);
                }
                startupUrl = null;
                connectionRoot = null;
                cWebView = hookToWebView(FindViewById<WebView>(Resource.Id.webView), startupUrl, connectionRoot);
                bool updateOnStart = true;
                if (updateOnStart)
                {
                    ClientExecute.ExecuteWithSettings(settings =>
                    {
                        foreach (var connection in settings.Connections.Where(con => con.HostName == connToSync))
                        {
                            var connRoot = getConnectionRootFolder(connection.HostName);
                            var connName = connection.Name;
                            ClientExecute.SetStaging(connName, connRoot,
                                "AaltoGlobalImpact.OIP,TheBall.Interface,cpanel,webview");
                            ClientExecute.StageOperation(connName, false, false, false, true);
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                        }
                    }, ReportException);
                }
            }
            
        }

        private static string getStartupUrlFromRoot(string rootFolder)
        {
            string urlPath = Path.Combine(rootFolder, "account", "cpanel", "html", "account.html");
            return File.Exists(urlPath) ? "file://" + urlPath : null;
        }

        private static string getConnectionRootFolder(string hostName)
        {
            var localPersonalPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var tbRoot = "TB";
            string rootFolder = Path.Combine(localPersonalPath, tbRoot, hostName);
            return rootFolder;
        }

        private WebView hookToWebView(WebView webView, string startupUrl, string connectionRoot)
        {
            string bridgeName = "TBJS2MobileBridge";
            if (startupUrl == null)
                startupUrl = "file:///android_asset/CoreUI/index.html";
            var settings = webView.Settings;
            settings.JavaScriptEnabled = true;
            TBJSBridge = new TBJS2OP(this);
            TBJSBridge.RegisterOperation(TheBallHostManager.CreateConnectionOperation);
            TBJSBridge.RegisterOperation(TheBallHostManager.DeleteConnectionOperation);
            var webViewClient = new TBWebViewClient(this, TheBallHostManager.CustomDataRetriever, connectionRoot);
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

