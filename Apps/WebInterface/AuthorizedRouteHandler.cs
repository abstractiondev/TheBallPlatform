using System.Web;
using System.Web.DynamicData;
using TheBall;
using TheBall.CORE;

namespace WebInterface
{
    public class AuthorizedRouteHandler : DynamicDataRouteHandler
    {
        protected override string GetCustomPageVirtualPath(MetaTable table, string viewName)
        {
            return base.GetCustomPageVirtualPath(table, viewName);
        }

        protected override string GetScaffoldPageVirtualPath(MetaTable table, string viewName)
        {
            return base.GetScaffoldPageVirtualPath(table, viewName);
        }

        public override IHttpHandler CreateHandler(DynamicDataRoute route, MetaTable table, string action)
        {
            var requestPath = HttpContext.Current.Request.Path;
            var currentOwner = VirtualOwner.FigureOwner(requestPath.Replace("/auth/", ""));
            InformationContext.Current.Owner = currentOwner;
            return base.CreateHandler(route, table, action);
        }
    }
}