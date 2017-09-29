using System;
//using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using TheBall;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRLoginRoot
    {
        public static async Task<TBRLoginRoot> GetOrCreateLoginRootWithAccount(HttpContext httpContext, string loginUrl,
            bool isAccountRequest, string currentDomainName)
        {
            throw new NotSupportedException();
        }

    }
}