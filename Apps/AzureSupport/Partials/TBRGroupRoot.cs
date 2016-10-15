using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    partial class TBRGroupRoot
    {
        public static string[] GetAllGroupIDs()
        {
            string blobPath = "AaltoGlobalImpact.OIP/TBRGroupRoot/";
            string searchPath = StorageSupport.CurrActiveContainer.Name + "/" + blobPath;
            int substringLen = blobPath.Length;
            var blobList = StorageSupport.CurrBlobClient.ListBlobs(searchPath, true).OfType<CloudBlob>();
            return blobList.Select(blob => blob.Name.Substring(substringLen)).ToArray();
        }

        public static TBRGroupRoot CreateNewWithGroup()
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.CreateDefault();
            groupRoot.Group.ID = groupRoot.ID;
            return groupRoot;
        }

        public static TBRGroupRoot CreateLegacyNewWithGroup(string legacyID)
        {
            TBRGroupRoot groupRoot = TBRGroupRoot.CreateDefault();
            groupRoot.ID = legacyID;
            groupRoot.UpdateRelativeLocationFromID();
            groupRoot.Group.ID = groupRoot.ID;
            return groupRoot;
        }

    }
}