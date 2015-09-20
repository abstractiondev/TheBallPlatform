using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;

namespace TheBalliOSApp
{
    public class WebViewController : UIViewController
    {
        public WebViewController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        private UIWebView webView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view
            Title = "WebView";
            View.BackgroundColor = UIColor.Green;

            webView = new UIWebView(View.Bounds);
            webView.BackgroundColor = UIColor.Blue;
            View.AddSubview(webView);

            //string url = "http://xamarin.com";
            //webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
            webView.LoadHtmlString("<html><h1>Is anybody t</h1></html>", NSBundle.MainBundle.BundleUrl);
        }
    }
}