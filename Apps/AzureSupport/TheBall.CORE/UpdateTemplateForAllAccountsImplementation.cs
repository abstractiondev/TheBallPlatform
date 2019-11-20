using System.Linq;
using System.Threading.Tasks;

namespace TheBall.Core
{
    public class UpdateTemplateForAllAccountsImplementation
    {
        public static async Task<string[]> GetTarget_AccountLocationsAsync()
        {
            var groupIDs = await ObjectStorage.ListOwnerObjectIDs<Account>(SystemSupport.SystemOwner);
            var locations = groupIDs.Select(id => "acc/" + id).ToArray();
            return locations;
        }

        public static async Task ExecuteMethod_CallUpdateOwnerTemplatesAsync(string templateName, string[] accountLocations)
        {
            var operationTasks = accountLocations.Select(ownerLocation =>
            {
                var execTask = UpdateContainerOwnerTemplates.ExecuteAsync(new UpdateContainerOwnerTemplatesParameters
                {
                    OwnerRootLocation = ownerLocation,
                    TemplateName = templateName
                });
                return execTask;
            }).ToArray();
            await Task.WhenAll(operationTasks);
        }

    }
}