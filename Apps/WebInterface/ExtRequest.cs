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
            return request.Path.StartsWith(AuthGroupPrefix);
        }

        public static bool IsAboutRequest(this HttpRequest request)
        {
            return request.Path.StartsWith(AboutPrefix);
        }

        public static bool IsPersonalRequest(this HttpRequest request)
        {
            return request.Path.StartsWith(AuthPersonalPrefix);
        }

        public static bool IsAccountRequest(this HttpRequest request)
        {
            return request.Path.StartsWith(AuthAccountPrefix);
        }

        public static string GetOwnerContentPath(this HttpRequest request)
        {
            if(request.IsGroupRequest())
                return request.Path.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            else if (request.IsAccountRequest())
                return request.Path.Substring(AuthAccountPrefixLen + 1 + GuidIDLen + 1);
            else if (request.IsPersonalRequest())
                return request.Path.Substring(AuthPersonalPrefixLen);
            throw new InvalidDataException("Owner content cath not recognized properly: " + request.Path);
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