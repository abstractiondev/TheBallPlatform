using System;
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
        public FileSystemSyncHandler(string syncRootFolder, string[] ownerSyncedFolders)
        {
            SyncRootFolder = syncRootFolder;
            SyncRequest = VirtualFS.Current.CreateFullSyncRequest(syncRootFolder,
                ownerSyncedFolders);
            RequestStreamHandler = handleRequestStream;
            ResponseStreamHandler = stream => handleResponseStream(stream).Wait();
        }

        private void handleRequestStream(Stream stream)
        {
            using (GZipStream compressedStream = new GZipStream(stream, CompressionLevel.Fastest))
            {
                RemoteSyncSupport.PutSyncRequestToStream(compressedStream, SyncRequest);
            }
        }

        private async Task handleResponseStream(Stream stream)
        {
            using (GZipStream compressedStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                var syncResponse = RemoteSyncSupport.GetSyncResponseFromStream(stream);
                SyncResponse = syncResponse;
                var contentToExpect = syncResponse.Contents.Where(content => content.ResponseContentType == ResponseContentType.IncludedInTransfer).ToArray();
                foreach (var content in contentToExpect)
                    await streamToFile(content, compressedStream);
                var contentToDelete = syncResponse.Contents.Where(content => content.ResponseContentType == ResponseContentType.Deleted).ToArray();
                foreach (var content in contentToDelete)
                    deleteContent(content);
                var contentToRefresh =
                    syncResponse.Contents.Where(
                        content => content.ResponseContentType == ResponseContentType.NameDataRefresh).ToArray();
                foreach (var content in contentToRefresh)
                    refreshContentNameData(content, SyncRootFolder);
            }
        }

        private static void refreshContentNameData(ContentSyncResponse.ContentData content, string syncRootFolder)
        {
            VirtualFS.Current.UpdateContentNameData(content.ContentMD5, content.FullNames.Select(name => Path.Combine(syncRootFolder, name)).ToArray());
        }

        private void deleteContent(ContentSyncResponse.ContentData content)
        {
            VirtualFS.Current.RemoveLocalContentByMD5(content.ContentMD5);
        }

        private async Task streamToFile(ContentSyncResponse.ContentData content, GZipStream compressedStream)
        {
            using (var outStream = await VirtualFS.Current.GetLocalTargetStreamForWrite(content.ContentMD5))
            {
                await compressedStream.CopyBytesAsync(outStream, content.ContentLength);
            }
        }


        public Action<Stream> RequestStreamHandler { get; set; }
        public Action<Stream> ResponseStreamHandler { get; set; }
    }
}