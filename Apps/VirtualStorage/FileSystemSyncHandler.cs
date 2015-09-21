using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace TheBall.Support.VirtualStorage
{
    public class FileSystemSyncHandler : ISyncStreamHandler
    {
        public ContentSyncRequest SyncRequest { get; private set; }
        public ContentSyncResponse SyncResponse { get; private set; }
        public string SyncRootFolder { get; private set; }

        public static async Task<FileSystemSyncHandler> CreateFileSystemSyncHandler(string syncRootFolder, string[] ownerSyncedFolders, Func<byte[], byte[]> md5HashComputer)
        {
            var result = new FileSystemSyncHandler();
            result.SyncRootFolder = syncRootFolder;
            result.SyncRequest = await VirtualFS.Current.CreateFullSyncRequest(syncRootFolder,
                ownerSyncedFolders, md5HashComputer);
            result.RequestStreamHandler = result.handleRequestStream;
            result.ResponseStreamHandler = result.handleResponseStream;
            return result;
        }

        private async Task handleRequestStream(Stream stream)
        {
            using (GZipStream compressedStream = new GZipStream(stream, CompressionLevel.Fastest))
            {
                RemoteSyncSupport.PutSyncRequestToStream(compressedStream, SyncRequest);
            }
        }

        private async Task handleResponseStream(Stream stream)
        {
            using (GZipStream compressedStream = new GZipStream(stream, CompressionMode.Decompress, true))
            {
                try
                {
                    await VirtualFS.Current.SetPendingSaves(true);
                    var syncResponse = RemoteSyncSupport.GetSyncResponseFromStream(compressedStream);
                    SyncResponse = syncResponse;
                    var contentToExpect =
                        syncResponse.Contents.Where(
                            content => content.ResponseContentType == ResponseContentType.IncludedInTransfer).ToArray();
                    foreach (var content in contentToExpect)
                        await streamToFile(content, compressedStream, SyncRootFolder);
                    var contentToDelete =
                        syncResponse.Contents.Where(
                            content => content.ResponseContentType == ResponseContentType.Deleted).ToArray();
                    foreach (var content in contentToDelete)
                        await deleteContent(content);
                    var contentToRefresh =
                        syncResponse.Contents.Where(
                            content => content.ResponseContentType == ResponseContentType.NameDataRefresh).ToArray();
                    foreach (var content in contentToRefresh)
                        await refreshContentNameData(content, SyncRootFolder);
                }
                finally
                {
                    await VirtualFS.Current.SetPendingSaves(false);
                }

            }
        }

        private static async Task refreshContentNameData(ContentSyncResponse.ContentData content, string syncRootFolder)
        {
            var fullNames = content.FullNames.Select(name => Path.Combine(syncRootFolder, name)).ToArray();
            await VirtualFS.Current.UpdateContentNameData(content.ContentMD5, fullNames);
        }

        private static async Task deleteContent(ContentSyncResponse.ContentData content)
        {
            await VirtualFS.Current.RemoveLocalContentByMD5(content.ContentMD5);
            Debug.WriteLine("Deleted content: {0}", content.ContentMD5);
        }

        private static async Task streamToFile(ContentSyncResponse.ContentData content, Stream stream, string syncRootFolder)
        {
            var contentMd5 = content.ContentMD5;
            using (var outStream = await VirtualFS.Current.GetLocalTargetStreamForWrite(contentMd5))
            {
                await stream.CopyBytesAsync(outStream, content.ContentLength);
                var fullNames = content.FullNames.Select(name => Path.Combine(syncRootFolder, name)).ToArray();
                await VirtualFS.Current.UpdateContentNameData(contentMd5, fullNames, true, content.ContentLength);
                Debug.WriteLine("Wrote {0} bytes of file(s): {1}", content.ContentLength, String.Join(", ", fullNames));
            }
        }


        public Func<Stream, Task> RequestStreamHandler { get; set; }
        public Func<Stream, Task> ResponseStreamHandler { get; set; }
    }
}