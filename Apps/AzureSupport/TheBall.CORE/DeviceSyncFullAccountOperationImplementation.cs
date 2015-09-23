using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
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
            IContainerOwner[] owners = new IContainerOwner[groupOwners.Length + 1];
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
                MD5 md5 = MD5.Create();
                var ownerFolderContent = grp.FolderContent.Select(folderGrp =>
                {
                    var folderContent =
                        folderGrp.OrderBy(item => item.ContentMD5).ThenBy(item => item.RelativeName).ToArray();
                    var fullMd5Hash =
                        RemoteSyncSupport.GetFolderMD5Hash(
                            folderContent.Select(src => new RemoteSyncSupport.FolderContent
                            {
                                ContentMD5 = src.ContentMD5,
                                RelativeName = src.RelativeName
                            }), md5.ComputeHash);
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
                    if (ownerPrefix.StartsWith("acc"))
                        ownerPrefix = "account";
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
                syncResponse.IsUnchanged = true;
            return syncResponse;
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
            /*
            using (GZipStream compressedStream = new GZipStream(outputStream, CompressionLevel.Fastest, true))
            {
                writeResponseToStream(syncResponse, compressedStream);
            }*/
            writeResponseToStream(syncResponse, outputStream);
        }

        private static void writeResponseToStream(ContentSyncResponse syncResponse, Stream stream)
        {
            RemoteSyncSupport.PutSyncResponseToStream(stream, syncResponse);
            if (syncResponse.Contents == null)
                return;
            var random = new Random();
            long outputTotal = 0;
            foreach (
                var transferItem in
                    syncResponse.Contents.Where(content => content.ResponseContentType == ResponseContentType.IncludedInTransfer)
                )
            {
                if (transferItem.ContentLength > 0)
                {
                    var variantCount = transferItem.FullNames.Length;
                    var pick = random.Next(0, variantCount - 1);
                    var blobName = transferItem.FullNames[pick];
                    var blob = StorageSupport.CurrActiveContainer.GetBlobReference(blobName);
                    outputTotal += transferItem.ContentLength;
                    blob.DownloadToStream(stream);
                }
                Debug.WriteLine("Wrote {0} bytes of {1}", transferItem.ContentLength, transferItem.ContentMD5);
            }
            Debug.WriteLine("Wrote total bytes: {0}", outputTotal);
        }
    }
}