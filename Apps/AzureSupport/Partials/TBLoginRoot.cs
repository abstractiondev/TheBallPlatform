using System;
//using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TheBall;
using TheBall.Core;
using TheBall.Core.InstanceSupport;

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