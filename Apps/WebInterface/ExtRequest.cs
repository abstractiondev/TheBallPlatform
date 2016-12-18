using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP;
using AzureSupport;
using TheBall;
using TheBall.CORE;

namespace WebInterface
{
    public static class ExtRequest
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

        static ExtRequest()
        {
            AuthGroupPrefixLen = AuthGroupPrefix.Length;
            AuthPersonalPrefixLen = AuthPersonalPrefix.Length;
            AboutPrefixLen = AboutPrefix.Length;
            //AuthAccountPrefixLen = AuthAccountPrefix.Length;
            AuthPrefixLen = AuthPrefix.Length;
            GuidIDLen = Guid.Empty.ToString().Length;
        }


        public static async Task<GroupMembership> RequireAndRetrieveGroupAccessRole(this HttpRequest request, string requestPath = null)
        {
            var context = request.RequestContext;
            string groupID = request.GetGroupID(requestPath);
            var userPrincipal = request.RequestContext.HttpContext.User;
            var tbIdentity = userPrincipal.Identity as TheBallIdentity;
            string accountID = tbIdentity?.AccountID;
            if(accountID == null)
                throw new SecurityException("No AccountID known for group access");
            var groupMembershipID = GroupMembership.GetIDFromAccountAndGroup(accountID, groupID);
            var groupMembership = await ObjectStorage.RetrieveFromSystemOwner<GroupMembership>(groupMembershipID);
            if (groupMembership == null)
                throw new SecurityException("No access to requested group");
            return groupMembership;
        }

        public static bool IsGroupRequest(this HttpRequest request, string requestPath = null)
        {
            return isGroupRequest(requestPath ?? request.Path);
        }

        public static bool IsShortcutRequest(this HttpRequest request)
        {
            return isShortcutRequest(request.Path) && request.UrlReferrer != null;
        }

        public static bool IsAboutRequest(this HttpRequest request, string requestPath = null)
        {
            return (requestPath ?? request.Path).StartsWith(AboutPrefix);
        }

        public static bool IsPersonalRequest(this HttpRequest request, string requestPath = null)
        {
            return isPersonalRequest(requestPath ?? request.Path);
        }

        private static bool isShortcutRequest(string path)
        {
            return path.StartsWith("/styles/") || path.StartsWith("/scripts/") || path.StartsWith("/app/");
        }


        private static bool isGroupRequest(string path)
        {
            return path.StartsWith(AuthGroupPrefix);
        }

        private static bool isPersonalRequest(string path)
        {
            return path.StartsWith(AuthPersonalPrefix);
        }

        public static string GetOwnerContentPath(this HttpRequest request, string requestPath = null)
        {
            if(request.IsGroupRequest(requestPath))
                return (requestPath ?? request.Path).Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            else if (request.IsPersonalRequest(requestPath))
                return (requestPath ?? request.Path).Substring(AuthPersonalPrefixLen);
            else if (request.IsAboutRequest(requestPath))
                return (requestPath ?? request.Path).Substring(AboutPrefixLen);
            throw new InvalidDataException("Owner content path not recognized properly: " + (requestPath ?? request.Path));
        }

        public static string GetReferrerOwnerContentPath(this HttpRequest request)
        {
            var urlReferrer = request.UrlReferrer;
            string referrerPath = urlReferrer != null && urlReferrer.Host == request.Url.Host ? urlReferrer.AbsolutePath : "";
            if (String.IsNullOrEmpty(referrerPath))
                return String.Empty;
            if(isGroupRequest(referrerPath))
                return referrerPath.Substring(AuthGroupPrefixLen + GuidIDLen + 1);
            else if (isPersonalRequest(referrerPath))
                return referrerPath.Substring(AuthPersonalPrefixLen);
            throw new InvalidDataException("Owner content path not recognized properly: " + referrerPath);
        }


        public static string GetGroupID(this HttpRequest request, string requestPath = null)
        {
            if(request.IsGroupRequest(requestPath) == false)
                throw new InvalidOperationException("Request is not group request");
            var usePath = requestPath ?? request.Path;
            return usePath.Substring(AuthGroupPrefixLen, GuidIDLen);
        }

        public static IContainerOwner GetGroupAsOwner(this HttpRequest request)
        {
            string groupID = request.GetGroupID();
            return new VirtualOwner("grp", groupID);            
        }


    }
}