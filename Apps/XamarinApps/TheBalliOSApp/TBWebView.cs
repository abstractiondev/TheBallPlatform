using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TheBalliOSApp
{
    public class TBWebView : UIWebView
    {

        public TBWebView(CGRect frame) : base(frame)
        {
            OperationProtocol.CurrentWebView = this;
        }

        public override void LoadRequest(NSUrlRequest r)
        {
            if (r.HttpMethod == "POST")
            {
                int i = 0;
            }
            else
            {
                base.LoadRequest(r);
            }
        }
    }
}
