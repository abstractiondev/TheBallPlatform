using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;
using TheBall.CORE.Storage;

namespace TheBall.CORE
{
    public class UpdateTemplateForAllGroupsImplementation
    {
        public static async Task<string[]> GetTarget_GroupLocationsAsync()
        {
            var groupIDs = await ObjectStorage.ListOwnerObjectIDs<Group>(SystemSupport.SystemOwner);
            var locations = groupIDs.Select(id => "grp/" + id).ToArray();
            return locations;
        }

        public static async Task ExecuteMethod_CallUpdateOwnerTemplatesAsync(string templateName, string[] groupLocations)
        {
            var operationTasks = groupLocations.Select(ownerLocation =>
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