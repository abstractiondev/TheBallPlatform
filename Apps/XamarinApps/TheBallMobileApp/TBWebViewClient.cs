using Android.App;
using Android.Webkit;

namespace TheBallMobileApp
{
    public class TBWebViewClient : WebViewClient
    {
        private readonly Activity ParentActivity;

        public TBWebViewClient(Activity parentActivity)
        {
            ParentActivity = parentActivity;
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, string url)
        {
            if (url.StartsWith("file:///auth/"))
            {
                string fixedUrl = url.Replace("file:///auth/", "file:///android_asset/tb/");
                if (fixedUrl.EndsWith("/"))
                {
                    fixedUrl += "<redirect address to add>";
                    view.StopLoading();
                    view.LoadUrl(fixedUrl);
                    return null;
                }
                WebResourceResponse intercept = new WebResourceResponse("text/html", "utf-8",
                    ParentActivity.Assets.Open(fixedUrl.Replace("file:///android_asset/", "")));
                return intercept;
            }
            var baseResult = base.ShouldInterceptRequest(view, url);
            return baseResult;
        }

    }
}