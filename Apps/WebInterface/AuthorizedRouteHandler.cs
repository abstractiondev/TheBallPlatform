using System;
using System.Security;
using System.Web;
using System.Web.DynamicData;
using AaltoGlobalImpact.OIP;
using TheBall;
using TheBall.CORE;

namespace WebInterface
{
    public class AuthorizedRouteHandler : DynamicDataRouteHandler
    {
        public override IHttpHandler CreateHandler(DynamicDataRoute route, MetaTable table, string action)
        {
            var request = HttpContext.Current.Request;
            if(!request.IsGroupRequest())
                throw new NotSupportedException("Route handling only supported for groups");
            var accessRoleTask = request.RequireAndRetrieveGroupAccessRole();
            accessRoleTask.Wait();
            var accessRole = accessRoleTask.Result;
            if(!TBCollaboratorRole.HasModeratorRights(accessRole.Role))
                throw new SecurityException("Moderator rights required to perform dynamic data fetch");
            InformationContext.Current.Owner = accessRole;
            InformationContext.Current.CurrentGroupRole = accessRole.Role;
            return base.CreateHandler(route, table, action);
        }
    }
}