using System;
using System.IO;
using System.Threading.Tasks;
using TheBall.Core.Storage;
using TheBall.Core.StorageCore;

namespace TheBall.Core
{
    public class UpdateContainerOwnerTemplatesImplementation
    {
        public static IContainerOwner GetTarget_TargetOwner(string ownerRootLocation)
        {
            return VirtualOwner.FigureOwner(ownerRootLocation);
        }


        public static async Task ExecuteMethod_ValidateTargetIsAccountOrGroupAsync(IContainerOwner targetOwner)
        {
            if (targetOwner.IsAccountContainer())
            {
                var accountID = targetOwner.LocationPrefix;
                var account = await ObjectStorage.RetrieveFromSystemOwner<Account>(accountID);
                if (account == null)
                    throw new InvalidDataException("Account not existing with ID: " + accountID);
            } else if (targetOwner.IsGroupContainer())
            {
                var groupID = targetOwner.LocationPrefix;
                var group = await ObjectStorage.RetrieveFromSystemOwner<Group>(groupID);
                if(group == null)
                    throw new InvalidDataException("Group not existing with ID: " + groupID);
            } else
                throw new InvalidDataException("Target must be either account or group");
        }

        public static string GetTarget_TemplateTargetLocation(string templateName, IContainerOwner targetOwner)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var targetLocation = storageService.GetOwnerContentLocation(targetOwner, templateName);
            return targetLocation;
        }

        public static async Task ExecuteMethod_SyncTemplateContentAsync(string templateSourceLocation, string templateTargetLocation)
        {

            //await WorkerSupport.WebContentSyncA(templateSourceLocation, templateTargetLocation);
        }

        public static string GetTarget_SystemTemplateSource(IContainerOwner targetOwner)
        {
            if (targetOwner.IsAccountContainer())
                return "account";
            return "group";
        }

        public static string GetTarget_TemplateSourceLocation(string templateName, string systemTemplateSource)
        {
            var sourceLocation = SystemSupport.SystemOwner.CombinePathForOwner(systemTemplateSource, templateName);
            return sourceLocation;
        }

        public static void ExecuteMethod_ValidateTemplateName(string templateName)
        {
            if (SystemSupport.IsValidTemplateName(templateName))
                return;
            throw new ArgumentException("Invalid template name: " + templateName, nameof(templateName));
        }

    }
}