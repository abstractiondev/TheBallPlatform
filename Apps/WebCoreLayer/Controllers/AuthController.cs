using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace WebCoreLayer.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AuthController : Controller
    {
        private const string AuthPersonalPrefix = "/auth/account/";
        private const string AuthGroupPrefix = "/auth/grp/";
        //private const string AuthAccountPrefix = "/auth/acc/";
        private const string AuthPrefix = "/auth/";
        private const string AboutPrefix = "/about/";
        private static int AuthGroupPrefixLen;
        private static int AuthPersonalPrefixLen;
        private static int AboutPrefixLen;
        //private static int AuthAccountPrefixLen;
        private static int AuthProcPrefixLen;
        private static int AuthPrefixLen;
        private static int GuidIDLen;


        private static ConcurrentDictionary<string, Tuple<Regex, string>[]> EnvironmentPatternsDict =
            new ConcurrentDictionary<string, Tuple<Regex, string>[]>();

        [HttpGet]
        public async Task Account(string path)
        {
            //Response.StatusCode = 200;
            //await Response.WriteAsync($"Account (path: {path}): {Request.Path}");
            await HandlePersonalRequest(path ?? "");
            //string ifNoneMatch = Request.Headers["If-None-Match"];
            //Console.WriteLine($"Handled ({Response.StatusCode} etag {ifNoneMatch}): {Request.Path}");
        }


        [HttpGet]
        public async Task Group(string groupId, string path)
        {
            //Response.StatusCode = 200;
            //await Response.WriteAsync($"Group {groupId} (path: {path}): {Request.Path}");
            await HandleGroupRequest(groupId, path ?? "");
        }

        private async Task HandleGroupRequest(string groupId, string contentPath)
        {
            var context = HttpContext;
            var accountID = InformationContext.CurrentAccount.AccountID;
            var groupMembership = await RequireAndRetrieveGroupAccessRole(accountID, groupId);
            var groupOwner = VirtualOwner.FigureOwner("grp/" + groupMembership.Group);
            InformationContext.AuthenticateContextOwner(groupOwner);
            InformationContext.Current.CurrentGroupRole = groupMembership.Role;
            await HandleOwnerRequest(groupOwner, context, contentPath, groupMembership.Role);
        }

        public async Task<GroupMembership> RequireAndRetrieveGroupAccessRole(string accountID, string groupID)
        {
            var groupMembershipID = GroupMembership.GetIDFromAccountAndGroup(accountID, groupID);
            var groupMembership = await ObjectStorage.RetrieveFromSystemOwner<GroupMembership>(groupMembershipID);
            if (groupMembership == null)
                throw new SecurityException("No access to requested group");
            return groupMembership;
        }

        private async Task HandlePersonalRequest(string contentPath)
        {
            var context = HttpContext;
            var principal = context.User;
            var accountID = principal.Identity.Name;
            var accountOwner = VirtualOwner.FigureOwner("acc/" + accountID);
            InformationContext.AuthenticateContextOwner(accountOwner);
            await HandleOwnerRequest(accountOwner, context, contentPath, TBCollaboratorRole.CollaboratorRoleValue);
        }

        private async Task HandleOwnerRequest(IContainerOwner containerOwner, HttpContext context, string contentPath, string role)
        {
            bool isUploadRequest = contentPath.StartsWith("upload/") == true;
            if (context.Request.Method == "POST")
            {
                // Do first post, and then get to the same URL
                if (TBCollaboratorRole.HasCollaboratorRights(role) == false)
                    throw new SecurityException("Role '" + role + "' is not authorized to do changing POST requests to web interface");
                bool isOperationRequest = contentPath.StartsWith("op/");

                if (isOperationRequest || isUploadRequest)
                {
                    try
                    {
                        if (isOperationRequest)
                            await HandleOwnerOperationRequestWithUrlPath(containerOwner, context, contentPath.Substring(3));
                        if (isUploadRequest)
                            await HandleOwnerUploadRequestWithUrlPath(containerOwner, context, contentPath.Substring(3));
                    }
                    catch (Exception ex)
                    {
                        var response = HttpContext.Response;
                        response.Headers.Append("Content-Encoding", Encoding.UTF8.ToString());
                        await response.WriteAsync(String.Format("{{ \"ErrorType\": \"{0}\", \"ErrorText\": {1} }}", ex.GetType().Name, WebSupport.EncodeJsString(ex.Message)));
                        response.ContentType = "application/json";
                        response.StatusCode = 500;
                        ErrorSupport.ReportException(ex);
                        return;
                    }
                }
                else
                {
                    bool redirectAfter = await HandleOwnerPostRequest(containerOwner, context, contentPath);
                    if (redirectAfter)
                        context.Response.Redirect(context.Request.GetEncodedUrl(), false);
                }
                return;
            }
            if (isUploadRequest)
            {
                await HandleOwnerUploadRequestWithUrlPath(containerOwner, context, contentPath);
            }
            else
                await HandleOwnerGetRequest(containerOwner, context, contentPath);
        }

        private async Task HandleOwnerUploadRequestWithUrlPath(IContainerOwner containerOwner, HttpContext context, string uploadRequestPath)
        {
            string uploadName = uploadRequestPath.Split('/')[0];
            await HandleOwnerUploadRequest(containerOwner, context, uploadName);
        }

        private async Task HandleOwnerUploadRequest(IContainerOwner containerOwner, HttpContext context, string uploadName)
        {
            var request = context.Request;
            bool isGet = request.Method == "GET";
            var sourceColl = isGet ? 
                request.Query.ToDictionary(item => item.Key, item => item.Value.ToString()) : 
                request.Form.ToDictionary(item => item.Key, item => item.Value.ToString());
            var resumableChunkNumber = int.Parse(sourceColl["resumableChunkNumber"]);
            var resumableChunkSize = int.Parse(sourceColl["resumableChunkSize"]);
            var resumableCurrentChunkSize = int.Parse(sourceColl["resumableCurrentChunkSize"]);
            var resumableTotalSize = long.Parse(sourceColl["resumableTotalSize"]);
            var resumableType = sourceColl["resumableType"];
            var resumableIdentifier = sourceColl["resumableIdentifier"];
            var resumableFilename = sourceColl["resumableFilename"];
            var resumableRelativePath = sourceColl["resumableRelativePath"];
            var resumableTotalChunks = sourceColl["resumableTotalChunks"];

            var blobPath = Path.Combine("TheBall.Interface", "InterfaceData", "Upload", resumableFilename)
                .Replace(@"\", "/");
            var blob = StorageSupport.GetOwnerBlobReference(blobPath);
            bool isExisting = await blob.ExistsAsync();
            const string ResumableTotalLength = "ResumableTotalLength";

            long currLength = 0;
            bool isSameData = false;

            if (isExisting)
            {
                await blob.FetchAttributesAsync();
                currLength = blob.Properties.Length;
                var completedTotalLength = blob.Metadata.ContainsKey(ResumableTotalLength)
                    ? long.Parse(blob.Metadata[ResumableTotalLength])
                    : currLength;
                isSameData = completedTotalLength == resumableTotalSize;
            }
            if (!isSameData)
            {
                currLength = 0;
            }

            var currentBeginPosition = (resumableChunkNumber - 1) * resumableChunkSize;
            var currentEndPosition = currentBeginPosition + resumableCurrentChunkSize;
            if (isGet)
            {
                if (!isSameData)
                {
                    context.EndResponseWithStatusCode(404);
                    return;
                }
                if (currentEndPosition <= currLength)
                {
                    context.EndResponseWithStatusCode(200);
                    return;
                }
                context.EndResponseWithStatusCode(404);
                return;
            }
            else // POST
            {
                if (resumableCurrentChunkSize == 0)
                    return;
                var currChunkFile = request.Form.Files[0];
                var uploadedContentStream = currChunkFile.OpenReadStream();
                var currContentLength = currChunkFile.Length;
                const int blobBlockSize = 4 * 1024 * 1024;
                if (resumableChunkSize != blobBlockSize)
                    throw new NotSupportedException("Resumable block size only supported for fixed: " + blobBlockSize);
                int written = 0;
                var blockList = new List<string>();
                var buffer = new byte[blobBlockSize];
                do
                {
                    var blockID = getBlockId(resumableChunkNumber);
                    blockList.Add(blockID);
                    var bytesToProcess = (int) Math.Min(currContentLength - written, blobBlockSize);
                    await uploadedContentStream.ReadAsync(buffer, 0, bytesToProcess);
                    using (var memoryStream = new MemoryStream(buffer, 0, bytesToProcess, false))
                    {
                        await blob.PutBlockAsync(blockID, memoryStream, null);
                    }
                    written += bytesToProcess;
                } while (written < currContentLength);
                var priorList = Enumerable.Range(1, resumableChunkNumber - 1).Select(getBlockId).ToArray();
                await blob.PutBlockListAsync(priorList.Concat(blockList), null, new BlobRequestOptions
                {
                    StoreBlobContentMD5 = true
                }, null);
                var currentTotalUploadedLength = currLength + written;
                bool isIncomplete = currentTotalUploadedLength < resumableTotalSize;
                if (isIncomplete)
                {
                    if (blob.Metadata.ContainsKey(ResumableTotalLength))
                        blob.Metadata[ResumableTotalLength] = resumableTotalSize.ToString();
                    else
                        blob.Metadata.Add(ResumableTotalLength, resumableTotalSize.ToString());
                    await blob.SetMetadataAsync();
                }
                else
                {
                    var originalFilename = Path.GetFileName(blobPath);
                    var completePrefix = "Uploaded_"; //DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmss") + "_";
                    var prefixedFileName = completePrefix + originalFilename;
                    var finalBlobPath = blobPath.Replace(originalFilename, prefixedFileName);
                    var finalBlob = StorageSupport.GetOwnerBlobReference(finalBlobPath);
                    using (var copyingStream = await blob.OpenReadAsync())
                        await finalBlob.UploadFromStreamAsync(copyingStream);
                    await blob.DeleteAsync();
                    await context.Response.WriteAsync(prefixedFileName);
                }
            }
        }

        private static string getBlockId(int resumableChunkNumber)
        {
            var blockID = Convert.ToBase64String(Encoding.Default.GetBytes(resumableChunkNumber.ToString("D6")));
            return blockID;
        }

        private async Task HandleOwnerOperationRequestWithUrlPath(IContainerOwner containerOwner, HttpContext context, string operationRequestPath)
        {
            string operationName = operationRequestPath.Split('/')[0];
            await HandleOwnerOperationRequest(containerOwner, context, operationName);
        }

        private async Task HandleOwnerOperationRequest(IContainerOwner containerOwner, HttpContext context,
            string operationName)
        {
            var operationAssembly = typeof(OperationSupport).Assembly;
            Type operationType = operationAssembly.GetType(operationName) ??
                                 OperationSupport.GetLegacyMappedType(operationName);
            if (operationType == null)
            {
                context.EndResponseWithStatusCode(404);
                return;
            }
            operationName = operationType.FullName;
            var request = context.Request;
            var patternItems = EnvironmentPatternsDict.GetOrAdd(InformationContext.Current.InstanceName, instanceName =>
            {
                var environments = InstanceConfig.Current.environments;
                var result = environments.Select(expObj =>
                {
                    dynamic dyn = expObj;
                    string urlPattern = dyn.urlPattern;
                    var regex = new Regex(urlPattern, RegexOptions.Compiled);
                    return new Tuple<Regex, string>(regex, dyn.name);
                }).ToArray();
                return result;
            });
            var pathAndQuery = request.GetEncodedPathAndQuery();
            var environmentName = patternItems.FirstOrDefault(item => item.Item1.IsMatch(pathAndQuery))?.Item2;
            var operationData = OperationSupport.GetHttpOperationDataFromRequest(request,
                InformationContext.CurrentAccount.AccountID, containerOwner.GetOwnerPrefix(), operationName,
                String.Empty, environmentName);
            string operationID = await OperationSupport.QueueHttpOperationAsync(operationData);
            //OperationSupport.ExecuteHttpOperation(operationData);
            //string operationID = "0";
            var response = context.Response;
            response.ContentType = "application/json";
            await response.WriteAsync(String.Format("{{ \"OperationID\": \"{0}\" }}", operationID));
            context.EndResponseWithStatusCode(202);
            //EndResponseWithStatusCode(context, 200);
        }

        private void validateThatOwnerPostComesFromSameReferrer(HttpContext context)
        {
            if (!InstanceConfig.Current.SkipReferrerValidation)
            {
                var request = context.Request;
                var requestUrl = new Uri(request.GetEncodedUrl());
                var referrerValue = request.Headers["Referer"].ToString();
                var referrerUrl = String.IsNullOrEmpty(referrerValue) ? null : new Uri(referrerValue);
                if (referrerUrl == null || requestUrl != referrerUrl)
                    throw new SecurityException("UrlReferrer mismatch or missing - potential cause is (un)intentionally malicious web template.");
            }
        }


        private async Task<bool> HandleOwnerPostRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            validateThatOwnerPostComesFromSameReferrer(context);
            HttpRequest request = context.Request;
            var form = request.Form;

            bool isAjaxDataRequest = request.ContentType.StartsWith("application/json"); // form.Get("AjaxObjectInfo") != null;
            if (isAjaxDataRequest)
            {
                // Various data deserialization tests - options need to be properly set
                // strong type radically faster 151ms over 25sec with flexible type - something ill
                //throw new NotSupportedException("Not supported as-is, implementation for serialization available, not finished");
                //                var stream = request.GetBufferedInputStream();
                //              var dataX = JSONSupport.GetObjectFromStream<List<ParentToChildren> >(stream);
                //                var streamReader = new StreamReader(request.GetBufferedInputStream());
                //                string data = streamReader.ReadToEnd();
                //                var jsonData = JSONSupport.GetJsonFromStream(data);
                //                HandlerOwnerAjaxDataPOST(containerOwner, request);
                //SetCategoryHierarchyAndOrderInNodeSummary.Execute();
                string operationName = request.Query["operation"];

                await HandleOwnerOperationRequest(containerOwner, context, operationName);
                return false;
            }

            bool isClientTemplateRequest = !String.IsNullOrEmpty(form["ContentSourceInfo"]) ||
                                           !String.IsNullOrEmpty(form["ExecuteOperation"]) ||
                                           !String.IsNullOrEmpty(form["ExecuteAdminOperation"]);
            if (isClientTemplateRequest)
            {
                string operationName = "TheBall.Interface.ExecuteLegacyHttpPostRequest";
                await HandleOwnerOperationRequest(containerOwner, context, operationName);
                /*
                HandleOwnerClientTemplatePOST(containerOwner, request);
                bool isPaymentsGroup = containerOwner.ContainerName == "grp" &&
                                       containerOwner.LocationPrefix == InstanceConfig.Current.PaymentsGroupID;
                if (isPaymentsGroup && false)
                {
                    SQLiteSyncOwnerData(containerOwner);
                }*/

                return false;
            }

            throw new NotSupportedException("Old legacy update no longer supported");
        }

        public static async Task HandleOwnerGetRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            var request = context.Request;
            bool isAnonRequest = containerOwner == SystemSupport.AnonymousOwner;
            if (String.IsNullOrEmpty(contentPath) == false && contentPath.EndsWith("/") == false && !isAnonRequest)
                validateThatOwnerGetComesFromSameReferer(containerOwner, context.Request, contentPath);
            bool filesystemOverrideEnabled = InstanceConfig.Current.EnableFilesystemOverride;
            bool fileSystemHandled = await HandleFileSystemGetRequest(containerOwner, context, contentPath);
            if (fileSystemHandled)
                return;
            if (String.IsNullOrEmpty(contentPath) || contentPath.EndsWith("/"))
            {
                CloudBlockBlob redirectBlob = StorageSupport.GetOwnerBlobReference(containerOwner, contentPath +
                                                                      InfraSharedConfig.Current.RedirectFromFolderFileName);
                string redirectToUrl = null;
                try
                {
                    redirectToUrl = await redirectBlob.DownloadTextAsync();
                }
                catch
                {

                }
                if (redirectToUrl == null)
                {
                    if (containerOwner.IsAccountContainer())
                        redirectToUrl = InstanceConfig.Current.AccountDefaultRedirect;
                    else
                        redirectToUrl = InstanceConfig.Current.GroupDefaultRedirect;
                }
                context.Response.Redirect(redirectToUrl, false);
                return;
            }
            if (contentPath.Contains("/MediaContent/"))
            {
                int lastIndexOfSlash = contentPath.LastIndexOf('/');
                var strippedPath = contentPath.Substring(0, lastIndexOfSlash);
                int lastIndexOfMediaContent = strippedPath.LastIndexOf("/MediaContent/");
                if (lastIndexOfMediaContent > 0) // Still found MediaContent after stripping the last slash
                    contentPath = strippedPath;
            }
            CloudBlob blob = StorageSupport.GetOwnerBlobReference(containerOwner, contentPath);
            var response = context.Response;
            // Read blob content to response.
            response.Clear();
            try
            {
                await HandleBlobRequestWithCacheSupport(context, blob, response);
            }
            catch (StorageException scEx)
            {
                if (scEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound ||
                    scEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.BadRequest)
                {
                    if (blob.Name.EndsWith(".json") &&
                        scEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                    {
                        response.StatusCode = (int)HttpStatusCode.NoContent;
                    }
                    else
                    {
                        response.StatusCode = scEx.RequestInformation.HttpStatusCode;
                        await response.WriteAsync("Blob not found or bad request: " + blob.Name + " (original path: " + request.Path + ")");
                    }
                }
                else
                {
                    response.StatusCode = (int)scEx.RequestInformation.HttpStatusCode;
                    await response.WriteAsync("Errorcode: " + scEx.RequestInformation.ExtendedErrorInformation.ErrorCode.ToString() + Environment.NewLine);
                    await response.WriteAsync(scEx.ToString());
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                string errMsg = ex.ToString();
                await response.WriteAsync(errMsg);
            }
        }

        private static async Task HandleBlobRequestWithCacheSupport(HttpContext context, CloudBlob blob, HttpResponse response)
        {
            // Set the cache request properties as IIS will include them regardless for now
            // even when we wouldn't want them on 304 response...
            //var request = context.Request;
            //request.
            var maxAge = CacheSupport.GetExtensionBasedMaxAge(Path.GetExtension(blob.Name));
            var headers = response.GetTypedHeaders();
            var cacheControl = new CacheControlHeaderValue();
            cacheControl.MaxAge = maxAge;
            cacheControl.Private = true;
            headers.CacheControl = cacheControl;
            var request = context.Request;
            await blob.FetchAttributesAsync();
            string ifNoneMatch = request.Headers["If-None-Match"];
            string ifModifiedSince = request.Headers["If-Modified-Since"];
            if (ifNoneMatch != null)
            {
                // Replace Nginx reverse-proxy messed up weakened etag marker
                ifNoneMatch = ifNoneMatch.StartsWith("W/\"") ? ifNoneMatch.Substring(2) : ifNoneMatch;
                if (ifNoneMatch == blob.Properties.ETag)
                {
                    response.ContentLength = 0;
                    response.StatusCode = (int)HttpStatusCode.NotModified;
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
                        response.ContentLength = 0;
                        response.StatusCode = (int) HttpStatusCode.NotModified;
                        return;
                    }
                }
            }
            var fileName = blob.Name.Contains("/MediaContent/") ?
                request.Path.ToString() : blob.Name;
            response.StatusCode = (int) HttpStatusCode.OK;
            response.ContentType = StorageSupport.GetMimeType(fileName);
            headers.ETag = EntityTagHeaderValue.Parse(blob.Properties.ETag);
            headers.LastModified = blob.Properties.LastModified.GetValueOrDefault().UtcDateTime;
            await blob.DownloadToStreamAsync(response.Body);
        }

        private static void validateThatOwnerGetComesFromSameReferer(IContainerOwner containerOwner, HttpRequest request, string contentPath)
        {
            bool isGroupRequest = containerOwner.IsGroupContainer();
            string requestGroupID = isGroupRequest ? containerOwner.LocationPrefix : null;
            bool isAccountRequest = !isGroupRequest;
            var referrerValue = request.Headers["Referer"].ToString();
            var referrerUrl = String.IsNullOrEmpty(referrerValue) ? null : new Uri(referrerValue);
            var requestUrl = new Uri(request.GetEncodedUrl());
            string[] groupTemplates = InstanceConfig.Current.DefaultGroupTemplateList;
            string[] accountTemplates = InstanceConfig.Current.DefaultAccountTemplateList;
            var refererPath = referrerUrl != null && referrerUrl.Host == requestUrl.Host ? referrerUrl.AbsolutePath : "";
            bool refererIsAccount = refererPath.StartsWith("/auth/account/");
            bool refererIsGroup = refererPath.StartsWith("/auth/grp/");

            if (String.IsNullOrEmpty(InstanceConfig.Current.AllowDirectServingRegexp) == false)
            {
                if (Regex.IsMatch(contentPath, InstanceConfig.Current.AllowDirectServingRegexp, RegexOptions.Compiled))
                    return;
            }

            if (isGroupRequest)
            {
                bool defaultMatch = groupTemplates.Any(contentPath.StartsWith);
                if (defaultMatch && (refererIsAccount == false && refererIsGroup == false))
                    return;
            }
            else
            {
                bool defaultMatch = accountTemplates.Any(contentPath.StartsWith);
                if (defaultMatch && (refererIsAccount == false && refererIsGroup == false))
                    return;
            }
            if (referrerUrl == null)
            {
                if (contentPath.StartsWith("customui_") || contentPath.StartsWith("DEV_") ||
                    contentPath.StartsWith("webview/") || contentPath.StartsWith("wwwsite/") ||
                    contentPath.EndsWith(".html"))
                    return;
                throw new SecurityException(
                    "Url referer required for non-default template requests, that target other than customui_ folder");
            }
            if (refererIsAccount && isAccountRequest)
                return;
            if (refererPath.StartsWith("/about/"))
                return;
            if (refererIsAccount == false && refererIsGroup == false) // referer is neither account nor group from this platform
            {
                if (contentPath.EndsWith("/") || contentPath.EndsWith(".html"))
                    return;
                throw new SecurityException("Url referring outside the platform is not allowed except for .html files");
            }
            // At this point we have referer either account or group, accept any plain html request
            if (contentPath.EndsWith(".html"))
                return;
            string refererOwnerPath = GetReferrerOwnerContentPath(request);
            // Accept account and group referers of default templates
            if (refererIsAccount && accountTemplates.Any(refererOwnerPath.StartsWith))
                return;
            if (refererIsGroup && groupTemplates.Any(refererOwnerPath.StartsWith))
                return;
            // Custom referers
            if (refererIsAccount)
            {
                throw new SecurityException("Non-default account referer accessing non-account content");
            }
            else // Referer is group
            {
                if (isAccountRequest)
                    throw new SecurityException("Non-default group referer accessing account content");
                string refererGroupID = GetGroupID(request);
                if (refererGroupID != requestGroupID)
                    throw new SecurityException("Non-default group referer accessing other group content");
            }
        }

        private static async Task<bool> HandleFileSystemGetRequest(IContainerOwner containerOwner, HttpContext context, string contentPath)
        {
            var cfg = InstanceConfig.Current;
            if (!cfg.EnableFilesystemOverride)
                return false;
            var overrideContext = containerOwner?.ContainerName ?? "anon";

            InstanceConfig.OverrideReplacement replacements;
            if (!cfg.FileSystemOverrides.TryGetValue(overrideContext, out replacements))
                return false;

            var replacementItem = replacements.Overrides?.FirstOrDefault(item => contentPath.StartsWith(item[0]));
            if (replacementItem == null)
                return false;
            var pattern = replacementItem[0];
            var replacement = replacementItem[1];
            var fileName = contentPath.Replace(pattern, replacement);
            var response = context.Response;

            if (System.IO.File.Exists(fileName))
            {
                var lastModified = System.IO.File.GetLastWriteTimeUtc(fileName);
                lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerSecond)); ;
                //response.Headers.Add("ETag", blob.Properties.ETag);
                //response.Headers.Add("Last-Modified", blob.Properties.LastModifiedUtc.ToString("R"));
                string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
                if (ifModifiedSince != null)
                {
                    DateTime ifModifiedSinceValue;
                    if (DateTime.TryParse(ifModifiedSince, out ifModifiedSinceValue))
                    {
                        ifModifiedSinceValue = ifModifiedSinceValue.ToUniversalTime();
                        if (lastModified <= ifModifiedSinceValue)
                        {
                            response.ContentLength = 0;
                            response.Headers.Clear();
                            response.StatusCode = 304;
                            return true;
                        }
                    }
                }
                var headers = response.GetTypedHeaders();
                var cacheControl = new CacheControlHeaderValue();
                cacheControl.MaxAge = TimeSpan.FromMinutes(0);
                cacheControl.Private = true;
                headers.CacheControl = cacheControl;
                headers.LastModified = lastModified;
                response.ContentType = StorageSupport.GetMimeType(fileName);
                using (var fileStream = System.IO.File.OpenRead(fileName))
                    await fileStream.CopyToAsync(response.Body);
            }
            else
            {
                response.StatusCode = 404;
            }
            return true;
        }

        public static string GetReferrerOwnerContentPath(HttpRequest request)
        {
            var referrerValue = request.Headers["Referer"].ToString();
            var referrerUrl = String.IsNullOrEmpty(referrerValue) ? null : new Uri(referrerValue);
            var requestUrl = new Uri(request.GetEncodedUrl());
            string referrerPath = referrerUrl != null && referrerUrl.Host == requestUrl.Host ? referrerUrl.AbsolutePath : "";
            if (String.IsNullOrEmpty(referrerPath))
                return String.Empty;
            if (isGroupRequest(referrerPath))
                return referrerPath.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            else if (isPersonalRequest(referrerPath))
                return referrerPath.Substring(AuthPersonalPrefixLen);
            throw new InvalidDataException("Owner content path not recognized properly: " + referrerPath);
        }

        private static bool isGroupRequest(string path)
        {
            return path.StartsWith(AuthGroupPrefix);
        }

        private static bool isPersonalRequest(string path)
        {
            return path.StartsWith(AuthPersonalPrefix);
        }

        public static string GetGroupID(HttpRequest request)
        {
            if (isGroupRequest(request.Path) == false)
                throw new InvalidOperationException("Request is not group request");
            var usePath = request.Path.ToString();
            return usePath.Substring(AuthGroupPrefixLen, GuidIDLen);
        }

    }
}