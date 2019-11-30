using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheBall.Core.Storage;

namespace TheBall.Core
{
    public class ExportOwnerContentToZipImplementation
    {
        public static async Task<string[]> GetTarget_IncludedFoldersAsync(IContainerOwner owner, string packageRootFolder)
        {
            var folders = await BlobStorage.GetOwnerFoldersA(owner, packageRootFolder);
            var directories = folders.Select(item => item.FolderName).ToArray();
            directories = SystemSupport.FilterAwayReservedFolders(directories);
            return directories;
        }

        public static ContentPackage PackageOwnerContentToZip_GetOutput(PackageOwnerContentReturnValue operationReturnValue, IContainerOwner owner, string packageRootFolder, string[] includedFolders)
        {
            return operationReturnValue.ContentPackage;
        }

        public static PackageOwnerContentParameters PackageOwnerContentToZip_GetParameters(IContainerOwner owner, string packageRootFolder, string[] includedFolders)
        {
            return new PackageOwnerContentParameters
                {
                    Owner = owner,
                    PackageName = "Full export",
                    Description = "Full export done by ExportOwnerContentToZip",
                    PackageType = "FULLEXPORT",
                    IncludedFolders = includedFolders,
                    PackageRootFolder = packageRootFolder
                };
        }

        public static ExportOwnerContentToZipReturnValue Get_ReturnValue(ContentPackage packageOwnerContentToZipOutput)
        {
            return new ExportOwnerContentToZipReturnValue {ContentPackageID = packageOwnerContentToZipOutput.ID};
        }
    }
}

