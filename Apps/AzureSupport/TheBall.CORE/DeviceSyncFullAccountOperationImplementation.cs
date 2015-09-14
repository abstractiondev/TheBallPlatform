using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.Support.VirtualStorage;

namespace TheBall.CORE
{
    public class DeviceSyncFullAccountOperationImplementation
    {
        public static ContentSyncRequest GetTarget_SyncRequest(Stream inputStream)
        {
            using (GZipStream compressedStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                return RemoteSyncSupport.GetSyncRequestFromStream(compressedStream);
            }
        }

        public static TBAccount GetTarget_AccountOwner()
        {
            TBAccount account  = InformationContext.CurrentOwner as TBAccount;
            if(account == null)
                throw new InvalidOperationException("Operation is only supported for accountOwner being CurrentOwner of context");
            return account;
        }

        public static IContainerOwner[] GetTarget_GroupOwners(TBAccount account)
        {
            return account.GroupRoleCollection.CollectionContent
                .Where(role => TBCollaboratorRole.IsRoleStatusValidMember(role.RoleStatus))
                .Select(role => new VirtualOwner("grp", role.GroupID)).Cast<IContainerOwner>().ToArray();
        }

        private class BlobContent
        {
            public IContainerOwner Owner { get; set; }
            public string Folder { get; set; }
            public string RelativeName { get; set; }
            public string FullName { get; set; }
            public long Length { get; set; }
            public string ContentMD5 { get; set; }
        }

        public static ContentSyncResponse GetTarget_SyncResponse(ContentSyncRequest syncRequest, IContainerOwner accountOwner, IContainerOwner[] groupOwners)
        {
            IContainerOwner[] owners = new IContainerOwner[groupOwners.Length];
            owners[0] = accountOwner;
            groupOwners.CopyTo(owners, 1);
            var requestedFolders = syncRequest.RequestedFolders;
            var currentData = getOwnerRequestedContentData(owners, requestedFolders);
            if (currentData.Count == 0)
                return new ContentSyncResponse
                {
                    Contents = new ContentSyncResponse.ContentData[0]
                };
            var ownerGroupedLQ = currentData.GroupBy(item => item.Owner);
            var ownerFolderGroupedLQ = ownerGroupedLQ.Select(grp => new
            {
                Owner = grp.Key,
                FolderContent = grp.GroupBy(ownerGrp => ownerGrp.Folder)
            });

            var currentOwnerFolderHashes = ownerFolderGroupedLQ.SelectMany(grp =>
            {
                var ownerPrefix = grp.Owner.IsAccountContainer() ? "account" : grp.Owner.GetOwnerPrefix();
                var ownerFolderContent = grp.FolderContent.Select(folderGrp =>
                {
                    var folderContent =
                        folderGrp.OrderBy(item => item.ContentMD5).ThenBy(item => item.RelativeName).ToArray();
                    var fullMd5Hash = getFolderContentFullMd5Hash(folderContent);
                    return new
                    {
                        OwnerPrefix = ownerPrefix,
                        Folder = folderGrp.Key,
                        Content = folderContent,
                        FullMD5Hash = fullMd5Hash
                    };
                });
                return ownerFolderContent;
            }).ToArray();

            var requestOwnerFolderHashes = syncRequest.ContentOwners.SelectMany(ownerItem =>
            {
                var ownerPrefix = ownerItem.OwnerPrefix;
                var ownerHashedContent = ownerItem.ContentFolders.Select(folder => new
                {
                    OwnerPrefix = ownerPrefix,
                    Folder = folder.Name,
                    FullMD5Hash = folder.FullMD5Hash
                });
                return ownerHashedContent;
            }).ToArray();

            // MD5 grouped list to be able to remove unchanged folders entirely
            var responseContentDict =
                currentData.OrderBy(item => item.ContentMD5)
                    .ThenBy(item => item.RelativeName)
                    .GroupBy(item => item.ContentMD5)
                    .ToDictionary(grp => grp.Key, grp => grp.ToArray());

            var skippableFolders = currentOwnerFolderHashes
                .Where(current => requestOwnerFolderHashes
                    .Any(req =>
                        req.OwnerPrefix == current.OwnerPrefix && req.Folder == current.Folder &&
                        req.FullMD5Hash == current.FullMD5Hash)).ToArray();

            var toSkipTransferDict = syncRequest.ContentMD5s.Where(item =>
            {
                bool toSkipTransfer = responseContentDict.ContainsKey(item);
                return toSkipTransfer;
            }).ToDictionary(item => item);
            var toDeleteArray = syncRequest.ContentMD5s.Where(item => !toSkipTransferDict.ContainsKey(item)).ToArray();

            var removableNonChangedKeys = responseContentDict.Keys.Where(key =>
            {
                var removable = responseContentDict[key];
                bool removableWhenAllBelongToSkippable = removable.All(item =>
                {
                    var ownerPrefix = item.Owner.GetOwnerPrefix();
                    return
                        skippableFolders.Any(
                            skippable => skippable.OwnerPrefix == ownerPrefix && skippable.Folder == item.Folder);
                });
                return removableWhenAllBelongToSkippable;
            }).ToArray();

            foreach (var removableKey in removableNonChangedKeys)
                responseContentDict.Remove(removableKey);

            ContentSyncResponse syncResponse = new ContentSyncResponse
            {
                Contents = responseContentDict.Keys.Select(md5Key =>
                {
                    bool toSkip = toSkipTransferDict.ContainsKey(md5Key);
                    var data = responseContentDict[md5Key];
                    var fullNames = data.Select(item => item.FullName).ToArray();
                    var contentLength = toSkip ? -1 : data.First().Length;
                    var content = new ContentSyncResponse.ContentData
                    {
                        ContentMD5 = md5Key,
                        ContentLength = contentLength,
                        FullNames = fullNames
                    };
                    return content;
                }).Concat(toDeleteArray.Select(md5Item => new ContentSyncResponse.ContentData
                {
                    ContentMD5 = md5Item,
                    FullNames = null,
                    ContentLength = 0,
                })).ToArray()
            };
            if (syncResponse.Contents.Length == 0)
                syncResponse.Contents = null;
            return syncResponse;
        }

        private static string getFolderContentFullMd5Hash(BlobContent[] folderContent)
        {
            string fullMd5Hash;
            using (var memStream = new MemoryStream())
            {
                foreach (var contentItem in folderContent)
                {
                    var md5ItemData = Convert.FromBase64String(contentItem.ContentMD5);
                    var nameData = Encoding.UTF8.GetBytes(contentItem.RelativeName);
                    memStream.Write(md5ItemData, 0, md5ItemData.Length);
                    memStream.Write(nameData, 0, nameData.Length);
                }
                var dataToHash = memStream.ToArray();
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(dataToHash);
                fullMd5Hash = Convert.ToBase64String(hash);
            }
            return fullMd5Hash;
        }

        private static List<BlobContent> getOwnerRequestedContentData(IContainerOwner[] owners, string[] requestedFolders)
        {
            List<BlobContent> contentData = new List<BlobContent>();
            foreach (var owner in owners)
            {
                foreach (var requestedFolder in requestedFolders)
                {
                    var ownerBlobs = owner.GetOwnerBlobListing(requestedFolder, true).Cast<CloudBlockBlob>();
                    var contents = ownerBlobs.Select(blob => new BlobContent
                    {
                        Owner = owner,
                        FullName = blob.Name,
                        RelativeName = StorageSupport.RemoveOwnerPrefixIfExists(blob.Name),
                        Length = blob.Properties.Length,
                        ContentMD5 = blob.Properties.ContentMD5,
                        Folder = requestedFolder
                    });
                    contentData.AddRange(contents);
                }
            }
            return contentData;
        }

        public static void ExecuteMethod_WriteResponseToStream(Stream outputStream, ContentSyncResponse syncResponse)
        {
            using (GZipStream compressedStream = new GZipStream(outputStream, CompressionLevel.Fastest))
            {
                RemoteSyncSupport.PutSyncResponseToStream(compressedStream, syncResponse);
                if (syncResponse.Contents == null)
                    return;
                var random = new Random();
                foreach (var transferItem in syncResponse.Contents.Where(content => content.IncludedInTransfer))
                {
                    var variantCount = transferItem.FullNames.Length;
                    var pick = random.Next(0, variantCount - 1);
                    var blobName = transferItem.FullNames[pick];
                    var blob = StorageSupport.CurrActiveContainer.GetBlobReference(blobName);
                    blob.DownloadToStream(compressedStream);
                }
            }
        }
    }
}