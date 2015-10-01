using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using TheBall.Support.DeviceClient;
using UIKit;

namespace TheBalliOSApp
{
    public partial class RootViewController : UIViewController
    {
        private TBWebView webView;

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public RootViewController(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //await SyncFullAccount();

            // Perform any additional setup after loading the view, typically from a nib.
            //webView.LoadHtmlString("<html><h1>Are you still there?</h1></html>", NSBundle.MainBundle.BundleUrl);
            //webView.LoadRequest(new NSUrlRequest(new NSUrl("http://yle.fi", false)));

            var frame = new CGRect(0, 20, 768, 1004);
            webView = new TBWebView(frame);

            webView.LoadError += WebView_LoadError;
            webView.LoadFinished += WebView_LoadFinished;
            webView.LoadStarted += WebView_LoadStarted;
            string fileName = "Content/CoreUI/index.html"; 
            string localHtmlUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);
            webView.LoadRequest(new NSUrlRequest(new NSUrl(localHtmlUrl, false)));
            webView.ScalesPageToFit = false;

            webView.ShouldStartLoad += ShouldStartLoad;

            View.Add(webView);

        }

        private bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (request.Body != null && request.Body.Length > 0)
            {
                var bodyData = request.Body.ToArray();
                var bodyString = Encoding.UTF8.GetString(bodyData);
            }
            if (request.HttpMethod == "POST")
            {
                return true;
            }
            return true;
        }

        private void WebView_LoadStarted(object sender, EventArgs e)
        {
            var i = 0;
        }

        private void WebView_LoadFinished(object sender, EventArgs e)
        {
            var i = 0;
        }

        private void WebView_LoadError(object sender, UIWebErrorArgs e)
        {
            var i = 0;
            var error = e.Error;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion

        private async Task SyncFullAccount()
        {
            await ClientExecute.ExecuteWithSettingsAsync(async settings =>
            {
                var testConnName = "tstConnY";
                const string testHostName = "home.theball.me";
                var testConn = settings.Connections.FirstOrDefault(conn => conn.Name == testConnName);
                if (testConn == null)
                {
                    ClientExecute.CreateConnection(testHostName, "kalle.launiala@gmail.com", testConnName);
                }
                else
                {
                    var connRoot = getConnectionRootFolder(testConn.HostName);
                    ClientExecute.SetStaging(testConn.Name, connRoot,
                        "AaltoGlobalImpact.OIP,TheBall.Interface,cpanel,webview");

                    await ClientExecute.StageOperation(testConn.Name, false, false, false, true, true);
                    Debug.WriteLine("Done syncing without errors...");
                }
            }, exception =>
            {
                Debug.WriteLine("Conn error: " + exception.ToString());
            });

        }

        private static string getConnectionRootFolder(string hostName)
        {
            var logicalRootPath = "/TheBall.Data";
            var tbRoot = "FSRoot";
            string rootFolder = Path.Combine(logicalRootPath, tbRoot, hostName);
            return rootFolder;
        }


    }
}