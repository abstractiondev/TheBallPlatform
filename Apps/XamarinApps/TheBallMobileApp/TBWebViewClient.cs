using System;
using System.IO;
using Android.App;
using Android.Webkit;
using Javax.Security.Auth;

namespace TheBallMobileApp
{
    public class TBWebViewClient : WebViewClient
    {
        public delegate Tuple<string, string, Stream> DataRetriever(string dataKey);

        private readonly Activity ParentActivity;
        private readonly DataRetriever CustomDataRetriever;
        private const string DataPrefix = "file:///data/";
        private readonly string ConnectionRootFolder;

        public TBWebViewClient(Activity parentActivity, DataRetriever customDataRetriever, string connectionRoot)
        {
            ParentActivity = parentActivity;
            CustomDataRetriever = customDataRetriever;
            ConnectionRootFolder = connectionRoot;
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, string url)
        {
            if (url.StartsWith("file:///auth/"))
            {
                //string fixedUrl = url.Replace("file:///auth/", "file:///android_asset/tb/");
                string fixedUrl = url.Replace("file:///auth", ConnectionRootFolder);
                if (fixedUrl.EndsWith("/"))
                {
                    fixedUrl += "<redirect address to add>";
                    view.StopLoading();
                    view.LoadUrl(fixedUrl);
                    return null;
                }
                var responseTask = TheBallHostManager.GetWebResponseContent(fixedUrl);
                responseTask.Wait();
                var response = responseTask.Result;
                if (response == null)
                    return null;
                WebResourceResponse intercept = new WebResourceResponse(response.Item1, response.Item2, response.Item3);
                return intercept;
            }
            if (url.StartsWith(DataPrefix))
            {
                string dataKey = url.Substring(DataPrefix.Length);
                var customData = CustomDataRetriever(dataKey);
                if (customData != null)
                {
                    WebResourceResponse response = new WebResourceResponse(customData.Item1, customData.Item2, customData.Item3);
                    return response;
                }
                var fixedUrl = url.Replace(DataPrefix, "CoreUI/data/");
                if (!File.Exists(fixedUrl))
                    return null;
                WebResourceResponse intercept = new WebResourceResponse("application/json", "utf-8",
                    ParentActivity.Assets.Open(fixedUrl));
                return intercept;
            }
            var baseResult = base.ShouldInterceptRequest(view, url);
            return baseResult;
        }

        public override void OnLoadResource(WebView view, string url)
        {
            base.OnLoadResource(view, url);
        }

    }
}