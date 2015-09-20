using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TheBalliOSApp
{
    public partial class RootViewController : UIViewController
    {
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
            //webView.Frame = CGRect.FromLTRB(0, 0, this.View.Frame.Size.Width, this.View.Frame.Size.Height);
            //webView.BackgroundColor = UIColor.Cyan;
            //webView.frame = CGRectMake(0, 0, self.view.frame.size.width, self.view.frame.size.height);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            //webView.LoadHtmlString("<html><h1>Are you still there?</h1></html>", NSBundle.MainBundle.BundleUrl);
            webView.LoadRequest(new NSUrlRequest(new NSUrl("http://yle.fi", false)));
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
    }
}