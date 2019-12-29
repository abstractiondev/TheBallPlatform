using System;
using System.Linq;
using System.Threading.Tasks;
using TheBall.Core.StorageCore;

namespace TheBall.Core
{
    public class GetOwnerSemanticDomainsImplementation
    {
        public static async Task<string[]> GetTarget_OwnerDomainsAsync(bool skipSystemDomains)
        {
            var storageService = CoreServices.GetCurrent<IStorageService>();
            var domainsToSkip = skipSystemDomains ? SystemSupport.ReservedDomainNames : new string[0];
            var owner = InformationContext.CurrentOwner;
            var blobListing = await storageService.GetLocationFoldersA(owner, "");
            var domainNames =
                blobListing.Select(fullName => fullName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last())
                    .Where(name => name.Count(ch => ch == '.') == 1 && domainsToSkip.Contains(name) == false)
                    .ToArray();
            return domainNames;
        }

        public static GetOwnerSemanticDomainsReturnValue Get_ReturnValue(string[] ownerDomains)
        {
            return new GetOwnerSemanticDomainsReturnValue {OwnerSemanticDomains = ownerDomains};
        }
    }
}