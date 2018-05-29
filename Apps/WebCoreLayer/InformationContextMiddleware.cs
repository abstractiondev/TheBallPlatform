using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TheBall;
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
                var error = ex.ToString();
                await context.Response.WriteAsync(error);
            }
            finally
            {
                await InformationContext.ProcessAndClearCurrentIfAvailableAsync();
            }
        }

    }
}