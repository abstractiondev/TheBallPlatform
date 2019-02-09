using System;
using System.Web;
using OIP = AaltoGlobalImpact.OIP;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TheBall.CORE
{
#if obsolete    
    public class CreateGroupWithTemplatesImplementation
    {
        public static string ExecuteMethod_ExecuteCreateGroup(string groupName, string accountId)
        {
            var parameters = new OIP.CreateGroupParameters()
                {
                    AccountID = accountId,
                    GroupName = groupName
                };
            var result = OIP.CreateGroup.Execute(parameters);
            return result.GroupID;
        }

        [Obsolete("Should have async alternative", true)]
        public static void ExecuteMethod_CopyGroupTemplates(string templateNameList, string executeCreateGroupOutput)
        {
            string[] templates = templateNameList.Split(',');
            foreach(var templateName in templates)
                RenderWebSupport.RefreshGroupTemplate(executeCreateGroupOutput, templateName);
        }

        public static void ExecuteMethod_RedirectToGivenUrl(string redirectUrlAfterCreation, string executeCreateGroupOutput)
        {
            if (string.IsNullOrEmpty(redirectUrlAfterCreation) == false)
            {
                string redirectTarget = "/auth/grp/" + executeCreateGroupOutput + "/" + redirectUrlAfterCreation;
                HttpContext.Current.Response.Redirect(redirectTarget, true);
            }
        }

        public static IContainerOwner GetTarget_GroupAsOwner(string executeCreateGroupOutput)
        {
            return VirtualOwner.FigureOwner("grp/" + executeCreateGroupOutput);
        }

        public static SetOwnerWebRedirectParameters SetDefaultRedirect_GetParameters(string groupDefaultRedirect, IContainerOwner groupAsOwner)
        {
            return new SetOwnerWebRedirectParameters {Owner = groupAsOwner, RedirectPath = groupDefaultRedirect};
        }

        public static void ExecuteMethod_InitializeGroupWithDefaultObjects(IContainerOwner groupAsOwner)
        {
            // Initialize nodesummarycontainer
            NodeSummaryContainer nodeSummaryContainer = NodeSummaryContainer.CreateDefault();
            nodeSummaryContainer.SetLocationAsOwnerContent(groupAsOwner, "default");
            nodeSummaryContainer.StoreInformation(groupAsOwner, true);
        }
    }
#endif
}