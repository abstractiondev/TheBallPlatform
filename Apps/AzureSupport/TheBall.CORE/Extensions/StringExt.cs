using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBall.CORE;

namespace AzureSupport.TheBall.CORE
{
    public delegate void ExecuteForTarget(
        IContainerOwner owner, string semanticDomain, string objectType, string objectID, string fullPath);

    public static class StringExt
    {
        public static bool ExecuteForObject(this string fullObjectLocation, ExecuteForTarget executeAction)
        {
            if (fullObjectLocation == null)
                return false;
            string[] locationParts = fullObjectLocation.Split('\\', '/');
            if (locationParts.Length != 5)
                return false;
            var owner = VirtualOwner.FigureOwner(fullObjectLocation);
            string semanticDomain = locationParts[2];
            string objectType = locationParts[3];
            string objectID = locationParts[4];
            string fullPath = fullObjectLocation;
            executeAction(owner, semanticDomain, objectType, objectID, fullPath);
            return true;
        }

    }
}
