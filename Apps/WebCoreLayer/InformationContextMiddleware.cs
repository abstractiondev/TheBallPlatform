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
            await _next.Invoke(context);
            await InformationContext.ProcessAndClearCurrentIfAvailableAsync();
        }

    }
}