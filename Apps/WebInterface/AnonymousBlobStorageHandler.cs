using System;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Security;
using AaltoGlobalImpact.OIP;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace WebInterface
{
    public class AnonymousBlobStorageHandler : IHttpHandler
    {
        private static string CloudStorageRootUrl
        {
            get { return String.Format("http://{0}.blob.core.windows.net/", SecureConfig.Current.AzureAccountName); }

        }

        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            ProcessAnonymousRequest(request, response);
        }

        private void ProcessAnonymousRequest(HttpRequest request, HttpResponse response)
        {
            CloudBlobClient publicClient = new CloudBlobClient(new Uri(CloudStorageRootUrl));
            string blobPath = GetBlobPath(request);
            if (blobPath.Contains("/MediaContent/"))
            {
                int lastIndexOfSlash = blobPath.LastIndexOf('/');
                var strippedPath = blobPath.Substring(0, lastIndexOfSlash);
                int lastIndexOfMediaContent = strippedPath.LastIndexOf("/MediaContent/");
                if (lastIndexOfMediaContent > 0) // Still found MediaContent after stripping the last slash
                    blobPath = strippedPath;
            }
            if (blobPath.EndsWith("/"))
            {
                string redirectBlobPath = blobPath + "RedirectFromFolder.red";
                CloudBlockBlob redirectBlob = (CloudBlockBlob) publicClient.GetBlobReferenceFromServer(new Uri(redirectBlobPath));
                string redirectToUrl = null;
                try
                {
                    redirectToUrl = redirectBlob.DownloadText();
                }
                catch
                {

                }
                if (redirectToUrl != null)
                {
                    response.Redirect(redirectToUrl, true);
                    return;
                }
            }
            CloudBlockBlob blob;
            try
            {
                blob = (CloudBlockBlob) publicClient.GetBlobReferenceFromServer(new Uri(blobPath));
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Invalid Uri base: " + blobPath, ex);
            }
            response.Clear();
            try
            {
                HandlePublicBlobRequestWithCacheSupport(HttpContext.Current, blob, response);
                return;
                blob.FetchAttributes();
                response.ContentType = StorageSupport.GetMimeType(blob.Name);
                //response.Cache.SetETag(blob.Properties.ETag);
                response.AddHeader("ETag", blob.Properties.ETag);
                response.Cache.SetMaxAge(TimeSpan.FromMinutes(0));
                response.Cache.SetLastModified(blob.Properties.LastModified.GetValueOrDefault().UtcDateTime);
                response.Cache.SetCacheability(HttpCacheability.Private);
                string ifModifiedSince = request.Headers["If-Modified-Since"];
                if (ifModifiedSince != null)
                {
                    DateTime ifModifiedSinceValue;
                    if (DateTime.TryParse(ifModifiedSince, out ifModifiedSinceValue))
                    {
                        ifModifiedSinceValue = ifModifiedSinceValue.ToUniversalTime();
                        if (blob.Properties.LastModified <= ifModifiedSinceValue)
                        {
                            response.StatusCode = 304;
                            return;
                        }
                    }
                }
                response.ContentType = StorageSupport.GetMimeType(blob.Name);
                blob.DownloadToStream(response.OutputStream);
            } catch(StorageException scEx)
            {
                if (scEx.RequestInformation.HttpStatusCode == (int) HttpStatusCode.NotFound || 
                    scEx.RequestInformation.HttpStatusCode == (int) HttpStatusCode.BadRequest)
                {
                    response.Write("Blob not found or bad request: " + blob.Name + " (original path: " + request.Path + ")");
                    response.StatusCode = scEx.RequestInformation.HttpStatusCode;
                }
                else
                {
                    response.Write("Errorcode: " + scEx.RequestInformation.ExtendedErrorInformation.ErrorCode.ToString() + Environment.NewLine);
                    response.Write(scEx.ToString());
                    response.StatusCode = (int) scEx.RequestInformation.HttpStatusCode;
                }
            } finally
            {
                response.End();
            }
        }

        private static string GetBlobPath(HttpRequest request)
        {
            string hostName = request.Url.DnsSafeHost;
            string containerName = hostName.Replace('.', '-');
            string currServingFolder = "";
            try
            {
                // "/2013-03-20_08-27-28";
                CloudBlobClient publicClient = new CloudBlobClient(new Uri(CloudStorageRootUrl));
                string currServingPath = containerName + "/" + RenderWebSupport.CurrentToServeFileName;
                var currBlob = (CloudBlockBlob) publicClient.GetBlobReferenceFromServer(new Uri(currServingPath));
                string currServingData = currBlob.DownloadText();
                string[] currServeArr = currServingData.Split(':');
                string currActiveFolder = currServeArr[0];
                var currOwner = VirtualOwner.FigureOwner(currServeArr[1]);
                currServingFolder = "/" + currActiveFolder;
            }
            catch
            {
                
            }
            return containerName + currServingFolder + request.Path;
        }

        private static void HandlePublicBlobRequestWithCacheSupport(HttpContext context, CloudBlob blob, HttpResponse response)
        {
            // Set the cache request properties as IIS will include them regardless for now
            // even when we wouldn't want them on 304 response...
            response.Cache.SetMaxAge(TimeSpan.FromMinutes(0));
            response.Cache.SetCacheability(HttpCacheability.Private);
            var request = context.Request;
            blob.FetchAttributes();
            string ifNoneMatch = request.Headers["If-None-Match"];
            string ifModifiedSince = request.Headers["If-Modified-Since"];
            if (ifNoneMatch != null)
            {
                if (ifNoneMatch == blob.Properties.ETag)
                {
                    response.ClearContent();
                    response.StatusCode = 304;
                    return;
                }
            }
            else if (ifModifiedSince != null)
            {
                DateTime ifModifiedSinceValue;
                if (DateTime.TryParse(ifModifiedSince, out ifModifiedSinceValue))
                {
                    ifModifiedSinceValue = ifModifiedSinceValue.ToUniversalTime();
                    if (blob.Properties.LastModified <= ifModifiedSinceValue)
                    {
                        response.ClearContent();
                        response.StatusCode = 304;
                        return;
                    }
                }
            }
            var fileName = blob.Name.Contains("/MediaContent/") ?
                request.Path : blob.Name;
            response.ContentType = StorageSupport.GetMimeType(fileName);
            //response.Cache.SetETag(blob.Properties.ETag);
            response.Headers.Add("ETag", blob.Properties.ETag);
            response.Cache.SetLastModified(blob.Properties.LastModified.GetValueOrDefault().UtcDateTime);
            blob.DownloadToStream(response.OutputStream);
        }


        #endregion
    }
}
