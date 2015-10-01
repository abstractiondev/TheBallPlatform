using Foundation;

namespace TheBalliOSApp
{
    public class OperationProtocol : NSUrlProtocol
    {
        [Export("canInitWithRequest:")]
        public static bool canInitWithRequest(NSUrlRequest request)
        {
            if (request.Url.Path != null)
            {
                bool isOperation = request.Url.Path.Contains("/op/");
                return isOperation;
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

        public override void StartLoading()
        {
            var request = Request;
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