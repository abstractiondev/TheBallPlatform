using System;
using System.IO;
using System.Security;
using System.Web;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using TheBall.CORE;

namespace WebInterface
{
    public static class ExtRequest
    {
        private const string AuthPersonalPrefix = "/auth/account/";
        private const string AuthGroupPrefix = "/auth/grp/";
        private const string AuthAccountPrefix = "/auth/acc/";
        private const string AuthPrefix = "/auth/";
        private const string AboutPrefix = "/about/";
        private static int AuthGroupPrefixLen;
        private static int AuthPersonalPrefixLen;
        private static int AuthAccountPrefixLen;
        private static int AuthProcPrefixLen;
        private static int AuthPrefixLen;
        private static int GuidIDLen;

        static ExtRequest()
        {
            AuthGroupPrefixLen = AuthGroupPrefix.Length;
            AuthPersonalPrefixLen = AuthPersonalPrefix.Length;
            AuthAccountPrefixLen = AuthAccountPrefix.Length;
            AuthPrefixLen = AuthPrefix.Length;
            GuidIDLen = Guid.Empty.ToString().Length;
        }


        public static TBRLoginGroupRoot RequireAndRetrieveGroupAccessRole(this HttpRequest request)
        {
            var context = request.RequestContext;
            string groupID = request.GetGroupID();
            string loginUrl = WebSupport.GetLoginUrl(request.RequestContext.HttpContext.User);
            string loginRootID = TBLoginInfo.GetLoginIDFromLoginURL(loginUrl);
            string loginGroupID = TBRLoginGroupRoot.GetLoginGroupID(groupID, loginRootID);
            TBRLoginGroupRoot loginGroupRoot = TBRLoginGroupRoot.RetrieveFromDefaultLocation(loginGroupID);
            if (loginGroupRoot == null)
                throw new SecurityException("No access to requested group");
            return loginGroupRoot;
        }

        public static bool IsGroupRequest(this HttpRequest request)
        {
            return isGroupRequest(request.Path);
        }

        public static bool IsAboutRequest(this HttpRequest request)
        {
            return request.Path.StartsWith(AboutPrefix);
        }

        public static bool IsPersonalRequest(this HttpRequest request)
        {
            return isPersonalRequest(request.Path);
        }

        public static bool IsAccountRequest(this HttpRequest request)
        {
            return isAccountRequest(request.Path);
        }

        private static bool isGroupRequest(string path)
        {
            return path.StartsWith(AuthGroupPrefix);
        }

        private static bool isAccountRequest(string path)
        {
            return path.StartsWith(AuthAccountPrefix);
        }

        private static bool isPersonalRequest(string path)
        {
            return path.StartsWith(AuthPersonalPrefix);
        }

        public static string GetOwnerContentPath(this HttpRequest request)
        {
            if(request.IsGroupRequest())
                return request.Path.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            else if (request.IsAccountRequest())
                return request.Path.Substring(AuthAccountPrefixLen + 1 + GuidIDLen + 1);
            else if (request.IsPersonalRequest())
                return request.Path.Substring(AuthPersonalPrefixLen);
            throw new InvalidDataException("Owner content path not recognized properly: " + request.Path);
        }

        public static string GetReferrerOwnerContentPath(this HttpRequest request)
        {
            var urlReferrer = request.UrlReferrer;
            string referrerPath = urlReferrer != null && urlReferrer.Host == request.Url.Host ? urlReferrer.AbsolutePath : "";
            if (String.IsNullOrEmpty(referrerPath))
                return String.Empty;
            if(isGroupRequest(referrerPath))
                return referrerPath.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            else if (isAccountRequest(referrerPath))
                return referrerPath.Substring(AuthAccountPrefixLen + 1 + GuidIDLen + 1);
            else if (isPersonalRequest(referrerPath))
                return referrerPath.Substring(AuthPersonalPrefixLen);
            throw new InvalidDataException("Owner content path not recognized properly: " + referrerPath);
        }


        public static string GetGroupID(this HttpRequest request)
        {
            if(request.IsGroupRequest() == false)
                throw new InvalidOperationException("Request is not group request");
            return request.Path.Substring(AuthGroupPrefixLen, GuidIDLen);
        }

        public static IContainerOwner GetGroupAsOwner(this HttpRequest request)
        {
            string groupID = request.GetGroupID();
            return new VirtualOwner("grp", groupID);            
        }


    }
}