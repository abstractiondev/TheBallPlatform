using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TheBall;
using TheBall.Admin.INT;
using TheBall.CORE;

namespace WebCoreLayer
{
    public class InformationContextMiddleware
    {
        private readonly RequestDelegate _next;

        public InformationContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            InformationContext.InitializeToLogicalContext(null, SystemSupport.SystemOwner,
                context.Request.Host.Host, null, true);
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                // TODO: Proper error management later on
                var error = ex.ToString();
                Console.WriteLine(error);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(error);
            }
            finally
            {
                await InformationContext.ProcessAndClearCurrentIfAvailableAsync();
            }
        }

    }

    public class InformationContextAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public InformationContextAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var user = context.User?.Identity as ClaimsIdentity;
                if (user?.AuthenticationType == "theball")
                {
                    var claims = user.Claims.ToArray();
                    var userName = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
                    var email = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
                    var accountID = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
                    var owner = VirtualOwner.FigureOwner("acc/" + accountID);
                    InformationContext.AuthenticateContextOwner(owner);
                    InformationContext.Current.Account =
                        new CoreAccountData(accountID, userName, email);
                }
                await _next.Invoke(context);
            }
            finally
            {
            }
        }

    }

}