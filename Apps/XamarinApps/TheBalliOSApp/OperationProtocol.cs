using System;
using System.IO;
using Foundation;

namespace TheBalliOSApp
{
    public class OperationProtocol : NSUrlProtocol
    {
        [Export("canInitWithRequest:")]
        public static bool canInitWithRequest(NSUrlRequest request)
        {
            var urlPath = request.Url.Path;
            if (urlPath != null)
            {
                bool isOperation = urlPath.Contains("/op/");
                if(isOperation)
                    return true;
                var absolutePath = request.Url.AbsoluteString;
                bool isData = absolutePath.StartsWith(DataPrefix);
                if (isData)
                    return true;
            }
            return false;
        }

        [Export("canonicalRequestForRequest:")]
        public static new NSUrlRequest GetCanonicalRequest(NSUrlRequest forRequest)
        {
            return forRequest;
        }

        [Export("initWithRequest:cachedResponse:client:")]
        public OperationProtocol(NSUrlRequest request, NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client)
            : base (request, cachedResponse, client)
        {
        }

        private const string DataPrefix = "file:///data/";
        private readonly DataRetriever CustomDataRetriever = key => null;
        public delegate Tuple<string, string, Stream> DataRetriever(string dataKey);


        public override void StartLoading()
        {
            var request = Request;
            var url = request.Url.AbsoluteString;
            if (url.StartsWith(DataPrefix))
            {
                string dataKey = url.Substring(DataPrefix.Length);
                var customData = CustomDataRetriever(dataKey);
                if (customData != null)
                {
                    using (var response = new NSUrlResponse(request.Url, customData.Item1, -1, customData.Item2))
                    {
                        Client.ReceivedResponse(this, response, NSUrlCacheStoragePolicy.NotAllowed);
                        this.InvokeOnMainThread(() =>
                        {
                            using (var data = NSData.FromStream(customData.Item3))
                            {
                                Client.DataLoaded(this, data);
                            }
                            Client.FinishedLoading(this);
                        });
                    }
                    return;
                }
                var fixedUrl = url.Replace(DataPrefix, "Content/CoreUI/data/");
                var fileName = Path.Combine(NSBundle.MainBundle.BundlePath, fixedUrl);
                if (!File.Exists(fileName))
                    return;
                using (var response = new NSUrlResponse(request.Url, "application/json", -1, "utf-8"))
                {
                    Client.ReceivedResponse(this, response, NSUrlCacheStoragePolicy.NotAllowed);
                    this.InvokeOnMainThread(() =>
                    {
                        using (var sourceStream = File.OpenRead(fileName))
                        {
                            using (var data = NSData.FromStream(sourceStream))
                                Client.DataLoaded(this, data);
                            Client.FinishedLoading(this);
                        }

                    });
                }
                return;
            }

            var client = Client;
            /*
            var value = Request.Url.Path.Substring(1);
            using (var image = Render(value))
            {
                using (var response = new NSUrlResponse(Request.Url, "image/jpeg", -1, null))
                {
                    Client.ReceivedResponse(this, response, NSUrlCacheStoragePolicy.NotAllowed);
                    this.InvokeOnMainThread(delegate {
                        using (var data = image.AsJPEG())
                        {
                            Client.DataLoaded(this, data);
                        }
                        Client.FinishedLoading(this);
                    });
                }
            }*/
        }

        public override void StopLoading()
        {
        }

    }
}