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
        private string ConnectionRootFolder;
        private readonly WebView View;

        internal TBWebViewClient(Activity parentActivity, TheBallHostManager hostManager, WebView view)
        {
            ParentActivity = parentActivity;
            CustomDataRetriever = hostManager.CustomDataRetriever;
            //ConnectionRootFolder = connectionRoot;
            View = view;
            hostManager.LoadUIHandler = (path, root) =>
            {
                ConnectionRootFolder = root;
                View.Post(() =>
                {
                    View.LoadUrl(path);
                });
            };
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, string url)
        {
            if (url.StartsWith("file:///auth/"))
            {
                int indexOfParams = url.IndexOf("?");
                if (indexOfParams > 0)
                    url = url.Substring(0, indexOfParams);
                //string fixedUrl = url.Replace("file:///auth/", "file:///android_asset/tb/");
                string fixedUrl = url.Replace("file:///auth", ConnectionRootFolder);
                if (fixedUrl.EndsWith("/"))
                {
                    //fixedUrl += "<redirect address to add>";
                    fixedUrl = url + "cpanel/html/cpanel.html";
                    view.Post(() =>
                    {
                        view.StopLoading();
                        view.LoadUrl(fixedUrl);
                    });
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
                //if (!File.Exists(fixedUrl))
                //    return null;
                var mimeType = TheBallHostManager.GetMimeType(fixedUrl);
                WebResourceResponse intercept = new WebResourceResponse(mimeType, "utf-8",
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